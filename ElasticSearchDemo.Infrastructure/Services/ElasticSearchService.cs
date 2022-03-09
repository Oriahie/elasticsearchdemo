using Elasticsearch.Net;
using ElasticSearchDemo.Core;
using ElasticSearchDemo.Core.Entities;
using ElasticSearchDemo.Core.Interfaces.Service;
using ElasticSearchDemo.Infrastructure.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Nest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ElasticSearchDemo.Infrastructure.Services
{
    public class ElasticSearchService : IElasticSearchService
    {
        private readonly ElasticClient _elasticClient;
        private readonly IWebHostEnvironment _env;
        public ElasticSearchService(ElasticClient elasticClient, IWebHostEnvironment env)
        {
            _elasticClient = elasticClient;
            _env = env;
        }

        public async Task<(List<Management>, List<Property>)> SearchFile(string searchParam, List<string> market = default, int pageSize = 25)
        {
            var man = await _elasticClient.SearchAsync<Management>(s => s.Query(q => q
                                                                     .Match(m => m
                                                                     .Field(f => f.mgmt.market)
                                                                     .Query(string.Join(',', market))) && q
                                                                     .MultiMatch(c => c
                                                                     .Query(searchParam)
                                                                     .Analyzer("standard")
                                                                     .Fields(ff => ff
                                                                     .Field(c => c.mgmt.name)
                                                                     .Field(c => c.mgmt.state))
                                                                     .Type(TextQueryType.PhrasePrefix)))
                                                                     .From(0)
                                                                     .Size(pageSize));


            var prop = await _elasticClient.SearchAsync<Property>(s => s.Query(q => q
                                                                    .Match(m => m
                                                                    .Field(f => f.property.market)
                                                                    .Query(string.Join(',', market))))
                                                                    .Query(b => b
                                                                    .MultiMatch(c => c
                                                                    .Query(searchParam)
                                                                    .Analyzer("standard")
                                                                    .Fields(ff => ff
                                                                    .Field(c => c.property.city)
                                                                    .Field(c => c.property.formerName)
                                                                    .Field(c => c.property.name)
                                                                    .Field(c => c.property.streetAddress))
                                                                    .Type(TextQueryType.PhrasePrefix)))
                                                                    .From(0)
                                                                    .Size(pageSize));

            return (man.Documents.ToList(),prop.Documents.ToList());
        }

        public async Task<bool> UploadFile(IFormFile file, RecordType Type)
        {
            #region save file to temporary folder

            var webRoot = _env.WebRootPath;
            var thePath = $"{webRoot}//tempfiles";
            var filePath = Path.Combine(thePath, file.FileName);
            var extension = Path.GetExtension(file.FileName);
            if (!IsValidExtension(extension))
                throw new Exception($"File extension ({extension}) is not allowed");

            if (!Directory.Exists(thePath))
                Directory.CreateDirectory(thePath);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
                fileStream.Close();
            }
            #endregion

            var res = false;
            try
            {
                res = true;
                string text = File.ReadAllText(filePath);
                switch (Type)
                {
                    case RecordType.PROPERTY:
                        {
                            var indexName = Constants.PropertyIndexName;
                            var data = JsonSerializer.Deserialize<List<Property>>(text);

                            //send to a background process
                            await Task.Factory.StartNew(async () =>
                            {
                                foreach (var item in data)
                                {
                                    await _elasticClient.IndexAsync(item, x => x.Index(indexName));
                                }
                            });
                        }
                        break;
                    case RecordType.MANAGEMENT:
                        {
                            var indexName = Constants.ManagementIndexName;
                            var data = JsonSerializer.Deserialize<List<Management>>(text);

                            //send to a background process
                            await Task.Factory.StartNew(async () =>
                            {
                                foreach (var item in data)
                                {
                                    await _elasticClient.IndexAsync(item, x => x.Index(indexName));
                                }
                            });
                        }
                        break;
                    default:
                        break;
                }

            }
            catch { res = false; }
            finally
            {
                File.Delete(filePath);
            }
            return res;
        }


        public static List<List<T>> ChunkBy<T>(List<T> source, int chunksize = 500)
        {
            return source.Select((x, i) => new { Index = i, Value = x })
                         .GroupBy(x => x.Index / chunksize)
                         .Select(x => x.Select(v => v.Value).ToList())
                         .ToList();
        }

        private bool IsValidExtension(string extension)
        {
            extension = extension.ToLower();
            return extension switch
            {
                ".json" => true,
                _ => false,
            };
        }
    }
}
