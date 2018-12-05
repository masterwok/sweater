using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sweater.Api.Filters;
using Sweater.Core.Clients;
using Sweater.Core.Clients.Contracts;
using Sweater.Core.Constants;
using Sweater.Core.Indexers;
using Sweater.Core.Indexers.Contracts;
using Sweater.Core.Indexers.Public.LeetX;
using Sweater.Core.Indexers.Public.Rarbg;
using Sweater.Core.Indexers.Public.ThePirateBay;
using Sweater.Core.Models;
using Sweater.Core.Services;
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

        private static BaseIndexer GetIndexerInstance(IServiceProvider serviceProvider, Indexer indexer)
        {
            switch (indexer)
            {
                case Indexer.ThePirateBay: return serviceProvider.GetService<ThePirateBay>();
                case Indexer.LeetX: return serviceProvider.GetService<LeetX>();
                case Indexer.Rarbg: return serviceProvider.GetService<Rarbg>();
                case Indexer.All: throw new InvalidEnumArgumentException("All indexer has no class to instantiate.");
                default: throw new KeyNotFoundException($"Indexer is not registered: {indexer}");
            }
        }

        /// <summary>
        /// Configure services for the IoC container.
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            // Enable CORS globally
            services.AddCors();

            services
                .AddMvc(options =>
                {
                    // Controller filter attributes
                    options.Filters.Add<CatchAllExceptionFilter>();
                    options.Filters.Add<ValidModelStateFilterAttribute>();
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Filters
            services.AddScoped<CatchAllExceptionFilter>();

            // Services
            services.AddTransient<IIndexerQueryService, IndexerQueryService>();
            services.AddSingleton(_queryConfig);

            // Clients
            services.AddTransient<IHttpClient, HttpClientWrapper>();

            // Indexers
            services.AddTransient<ThePirateBay>();
            services.AddTransient<LeetX>();
            services.AddTransient<Rarbg>();

            // Indexer factory
            services.AddTransient<Func<Indexer, IIndexer>>(serviceProvider => indexer =>
                GetIndexerInstance(serviceProvider, indexer)
                    .Configure(_indexerConfigSection.GetSection(indexer.ToString()))
            );
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