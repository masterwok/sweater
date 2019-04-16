using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sweater.Api.Filters;
using Sweater.Api.Services;
using Sweater.Core.Attributes;
using Sweater.Core.Clients;
using Sweater.Core.Clients.Contracts;
using Sweater.Core.Constants;
using Sweater.Core.Extensions;
using Sweater.Core.Indexers.Contracts;
using Sweater.Core.Indexers.Public.Kat;
using Sweater.Core.Indexers.Public.LeetX;
using Sweater.Core.Indexers.Public.Rarbg;
using Sweater.Core.Indexers.Public.ThePirateBay;
using Sweater.Core.Indexers.Public.Zoogle;
using Sweater.Core.Indexers.Public.Zooqle;
using Sweater.Core.Models;
using Sweater.Core.Services.Contracts;

namespace Sweater.Api
{
    public class Startup
    {
        private readonly QueryConfig _queryConfig;

        private readonly IConfigurationSection _indexerConfigSection;

        public Startup(IConfiguration configuration)
        {
            _queryConfig = configuration.GetSection("queryConfig").Get<QueryConfig>();
            _indexerConfigSection = configuration.GetSection("indexers");
        }

        /// <summary>
        /// Configure services for the IoC container.
        /// </summary>
        [SuppressMessage("ReSharper", "RedundantNameQualifier")]
        public void ConfigureServices(IServiceCollection services)
        {
            services
                // CORS must be enabled before MVC
                .AddCors()
                .AddMemoryCache()
                .AddMvc(options =>
                {
                    // Controller filter attribute
                    options.Filters.Add<CatchAllExceptionFilter>();
                    options.Filters.Add<ValidModelStateFilter>();
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Filters
            services.AddScoped<CatchAllExceptionFilter>();

            // Services
            services.AddSingleton(typeof(ILogService<>), typeof(LogService<>));
            services.AddTransient<IIndexerQueryService, CachedIndexerQueryService>();
            services.AddSingleton(_queryConfig);

            // Clients
            services.AddTransient<IHttpClient, HttpClientWrapper>();

            // Indexers
            services.AddTransient<ThePirateBay>();
            services.AddTransient<LeetX>();
            services.AddTransient<Rarbg>();
            services.AddTransient<Kat>();
            services.AddTransient<Zooqle>();

            // Indexer Configurations
            services.AddTransient(serviceProvider => _indexerConfigSection
                .GetSection(Rarbg.ConfigName)
                .Get<Core.Indexers.Public.Rarbg.Models.Settings>());

            services.AddTransient(serviceProvider => _indexerConfigSection
                .GetSection(LeetX.ConfigName)
                .Get<Core.Indexers.Public.LeetX.Models.Settings>());

            services.AddTransient(serviceProvider => _indexerConfigSection
                .GetSection(ThePirateBay.ConfigName)
                .Get<Core.Indexers.Public.ThePirateBay.Models.Settings>());
            
            services.AddTransient(serviceProvider => _indexerConfigSection
                .GetSection(Kat.ConfigName)
                .Get<Core.Indexers.Public.Kat.Models.Settings>());
            
            services.AddTransient(serviceProvider => _indexerConfigSection
                .GetSection(Zooqle.ConfigName)
                .Get<Core.Indexers.Public.Zoogle.Models.Settings>());

            // Indexer factory
            services.AddTransient<Func<Indexer, IIndexer>>(serviceProvider => indexer =>
            {
                try
                {
                    var type = indexer.GetAttribute<TypeAttribute>().Type;

                    return serviceProvider.GetService(type) as IIndexer;
                }
                catch (Exception)
                {
                    throw new KeyNotFoundException($"Indexer is not registered: {indexer}");
                }
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseCors(options => options.AllowAnyOrigin());
            app.UseMvc();
        }
    }
}