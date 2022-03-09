using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearchDemo.Infrastructure.Extensions
{
    public static class SwaggerExtension
    {

        public static void UseCustomSwaggerApi(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Elastic Search Demo");
                c.RoutePrefix = "swagger";
            });
        }

        public static void AddSwaggerServices(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                string title = "Elastic Search Demo";
                c.SwaggerDoc("v1", new OpenApiInfo { Title = $"{ title }", Version = "v1" });
            });
        }
    }
}
