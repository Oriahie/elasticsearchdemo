using Elasticsearch.Net;
using ElasticSearchDemo.Core.Entities;
using ElasticSearchDemo.Core.Interfaces.Service;
using ElasticSearchDemo.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearchDemo.Infrastructure.Extensions
{
    public static class AppServicesExtensions
    {
        public static void AddAppServices(this IServiceCollection services)
        {

            var connectionSettings = new ConnectionSettings(new Uri(""))
                                        .EnableDebugMode()
                                        .PrettyJson()
                                        .DefaultMappingFor<Management>(i=>i.IndexName("mgmt"))
                                        .DefaultMappingFor<Property>(i=>i.IndexName("propt"))
                                        .RequestTimeout(TimeSpan.FromMinutes(2));

            var client = new ElasticClient(connectionSettings);

            services.AddTransient<IElasticSearchService, ElasticSearchService>();

            services.AddSingleton(client);
            services.AddHttpClient();
        }


        public static void AddCorsConfig(this IServiceCollection services, IConfiguration configuration)
        {

            string[] corsUrl = configuration.GetSection("AppSettings:CorsUrls").Get<string[]>();
            services.AddCors(Options =>
            {
                Options.AddPolicy("CorsPolicy",
                    builder =>
                    builder
                    .WithOrigins(corsUrl)
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    );
            });
        }




    }
}
