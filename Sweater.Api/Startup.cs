using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sweater.Core.Clients;
using Sweater.Core.Constants;
using Sweater.Core.Indexers.Contracts;
using Sweater.Core.Indexers.Public;
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

        /// <summary>
        /// Configure services for the IoC container.
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Services
            services.AddTransient<IIndexerQueryService, IndexerQueryService>();
            services.AddSingleton(_queryConfig);

            // Clients
            services.AddTransient<IWebClient, WebClientWrapper>();

            // Indexers
            services.AddTransient<ThePirateBayIndexer>();
            services.AddTransient<Func<Indexer, IIndexer>>(serviceProvider => key =>
            {
                var config = _indexerConfigSection.GetSection(key.ToString());

                switch (key)
                {
                    case Indexer.ThePirateBay: return serviceProvider.GetService<ThePirateBayIndexer>().Configure(config);
                    default: throw new KeyNotFoundException($"Indexer is not registered: {key}");
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
            app.UseMvc();
        }
    }
}