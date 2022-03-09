using Elasticsearch.Net;
using ElasticSearchDemo.Core;
using ElasticSearchDemo.Core.Entities;
using ElasticSearchDemo.Core.Interfaces.Service;
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
            var man = await _elasticClient.SearchAsync<Management>(s => s.Query(q=>q
                                                                     .Match(m => m
                                                                     .Field(f => f.mgmt.market)
                                                                     .Query(string.Join(',', market))))
                                                                     .Query(b => b
                                                                     .MultiMatch(c => c
                                                                     .Query(searchParam)
                                                                     .Analyzer("standard")
                                                                     .Fields(ff => ff
                                                                     .Field(c => c.mgmt.name)
                                                                     .Field(c => c.mgmt.state))
                                                                     .Type(TextQueryType.BestFields)))
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
                                                                    .Type(TextQueryType.BestFields)))
                                                                    .From(0)
                                                                    .Size(pageSize));


            

            //.Index("mgmt")
            //.Query(q => q
            //.CombinedFields(,)
            //.QueryString(t => t
            //.Query(searchParam)))
            //.From((pageIndex - 1) * pageSize)
            //.Size(pageSize));

            //.Query(q=>q.Match(f=>f.Field(x=>x.mgmt.name).Query(searchParam))).From(0).Size(25));;
            //.Query(q => q
            //.Term(t => t
            //.Field(f => f.mgmt.market).Value(market)))
            //.Query(q => q
            //.Match(m => m
            //.Field(f => f.mgmt.name)
            //.Query(searchParam)
            //)));; ;
            //.From(0)
            //.Size(pageSize)); ; ;



            




            return (man.Documents.ToList(),prop.Documents.ToList());
        }

        public async Task<bool> UploadFile(IFormFile file, RecordType Type)
        {
            #region save file to temporary folder
            var generatedName = Guid.NewGuid().ToString();
            var webRoot = _env.WebRootPath;
            var thePath = $"{webRoot}//tempfiles";
            var filePath = Path.Combine(thePath, file.FileName);
            var extension = Path.GetExtension(file.FileName);
            if (!IsValidExtension(extension))
                throw new Exception($"File extension ({extension}) is not allowed");

            // var filePath = Path.Combine(path, name);
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
                string text = File.ReadAllText(filePath);
                switch (Type)
                {
                    case RecordType.PROPERTY:
                        {
                            var indexName = "property";
                            var data = JsonSerializer.Deserialize<List<Property>>(text);

                            var chunkedData = ChunkBy(data);

                            foreach (var item in chunkedData)
                            {
                                var dd = await _elasticClient.IndexAsync(item,x=>x.Index(indexName));
                                var ddd = await _elasticClient.IndexManyAsync(item, indexName);

                                //var bulkRes = await _elasticClient.BulkAsync(b =>
                                //                    b.Index(indexName)
                                //                    .IndexMany(item));
                            }

                            Console.WriteLine("before inserting");

                            var bulkAll = _elasticClient.BulkAll(data, b => b
                                     .Index(indexName)
                                     .BackOffRetries(2)
                                     .BackOffTime("30s")
                                     .MaxDegreeOfParallelism(4)
                                     .Size(1000));
                            bulkAll.Wait(TimeSpan.FromMinutes(30), _ => { Console.WriteLine("data indexed"); });
                            await _elasticClient.Indices.PutAliasAsync(indexName, indexName);
                        }
                        break;
                    case RecordType.MANAGEMENT:
                        {
                            var indexName = "mgmttest";
                            var data = JsonSerializer.Deserialize<List<Management>>(text);

                            //var chunkedData = ChunkBy(data,5);

                            foreach (var item in data)
                            {
                                await _elasticClient.IndexAsync(item, x => x.Index(indexName));
                                
                                
                                
                                ////var dad = _elasticClient.inde<Management>(item.Firs);

                                //var ddd = await _elasticClient.IndexManyAsync(item, indexName);

                                ////var bulkRes = await _elasticClient.BulkAsync(b =>
                                ////                    b.Index(indexName)
                                ////                    .IndexMany(item));
                            }

                            Console.WriteLine("before inserting");

                            var bulkAll = _elasticClient.BulkAll(data, b => b
                                     .Index(indexName)
                                     .BackOffRetries(2)
                                     .BackOffTime("30s")
                                     .MaxDegreeOfParallelism(4)
                                     .Size(1000));
                            bulkAll.Wait(TimeSpan.FromMinutes(30), _ => { Console.WriteLine("data indexed"); });
                            await _elasticClient.Indices.PutAliasAsync(indexName, indexName);

                        }
                        break;
                    default:
                        break;
                }
                
                //foreach (var item in data)
                //{
                //    var d = await _elasticClient.IndexAsync(item, x => x.Index(indexName));
                //}

                //var bulkResponse = await _elasticClient.IndexAsync(payload,x=>x.Index(indexName));


                //var ddd = _elasticClient.BulkAll(data, b => b.Index("bulktest")).Wait(TimeSpan.FromMinutes(15), next =>
                //{
                //    // do something e.g. write number of pages to console
                //});
                //var bulkResponse = _elasticClient.LowLevel.Bulk<BulkResponse>(
                //    text, new BulkRequestParameters
                //    {
                //        RequestConfiguration = new RequestConfiguration
                //        {
                //            RequestTimeout = TimeSpan.FromMinutes(3)
                //        }
                //    });
                //res = bulkResponse.IsValid;
            }
            catch (Exception ex)
            {

            }
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
