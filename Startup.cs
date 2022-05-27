using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace dynamodb_charp_lambda
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "dynamodb_charp_lambda", Version = "v1" });
            });

            services.AddOptions<SecretsOptions>().BindConfiguration("Secrets");

            var credentials = new BasicAWSCredentials("AKIAR5PTDWH762H7JM53", "LlqN/rCPomIoNLHrPSulPyCli2Ds2xL4ojZ4Se4W");
            var config = new AmazonDynamoDBConfig()
            {
                RegionEndpoint = RegionEndpoint.EUWest2
            };
            var client = new AmazonDynamoDBClient(credentials, config);

            services.AddSingleton <IAmazonDynamoDB>(client);
            services.AddSingleton<IDynamoDBContext, DynamoDBContext>();

            //var builder = new ConfigurationBuilder();
            //builder.AddSecretsManager(configurator: config =>
            //{
            //    config.KeyGenerator = (secret, name) => name.Replace("__", ":");
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "dynamodb_charp_lambda v1"));
            }

            // This should be using the Secrets.cs file if in the development environment or the 
            // AWS Secrets Manager (the key with __ in it) if not.  If this code isn't here, then we should be getting the
            // code from appsettings.json.  It doesn't seem to work howvever - it is always getting the value from 
            // appsettings.json, no matter if this code is here or not
            var builder2 = new ConfigurationBuilder();
            if (env.IsDevelopment())
            {
                builder2.AddUserSecrets<Program>();
            }
            else
            {
                builder2.AddSecretsManager(configurator: config =>
                {
                    config.KeyGenerator = (secret, name) => name.Replace("__", ":");
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            
        }
    }
}
