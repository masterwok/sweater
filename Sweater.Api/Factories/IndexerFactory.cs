using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sweater.Core.Constants;
using Sweater.Core.Indexers.Contracts;
using Sweater.Core.Indexers.Public.LeetX;
using Sweater.Core.Indexers.Public.Rarbg;
using Sweater.Core.Indexers.Public.ThePirateBay;


namespace Sweater.Api.Factories
{
    public static class IndexerFactory
    {
        public static IIndexer Create(
            IServiceProvider serviceProvider
            , IConfigurationSection configuration
            , Indexer indexer
        )
        {
            var indexerConfig = configuration.GetSection(indexer.ToString());

            switch (indexer)
            {
                case Indexer.ThePirateBay:
                    return CreateThePirateBayIndexer(serviceProvider, indexerConfig);
                case Indexer.LeetX:
                    return CreateLeetXIndexer(serviceProvider, indexerConfig);
                case Indexer.Rarbg:
                    return CreateRarbgIndexer(serviceProvider, indexerConfig);
                case Indexer.All: throw new InvalidEnumArgumentException("All indexer has no class to instantiate.");
                default: throw new KeyNotFoundException($"Indexer is not registered: {indexer}");
            }
        }

        private static IIndexer CreateRarbgIndexer(
            IServiceProvider serviceProvider
            , IConfiguration indexerConfig
        )
        {
            var instance = serviceProvider.GetService<Rarbg>();

            instance.Configure(indexerConfig.Get<Core.Indexers.Public.Rarbg.Models.Settings>());

            return instance;
        }

        private static IIndexer CreateLeetXIndexer(IServiceProvider serviceProvider, IConfiguration indexerConfig)
        {
            var instance = serviceProvider.GetService<LeetX>();

            instance.Configure(indexerConfig.Get<Core.Indexers.Public.LeetX.Models.Settings>());

            return instance;
        }

        private static IIndexer CreateThePirateBayIndexer(
            IServiceProvider serviceProvider
            , IConfiguration indexerConfig
        )
        {
            var instance = serviceProvider.GetService<ThePirateBay>();

            instance.Configure(indexerConfig.Get<Core.Indexers.Public.ThePirateBay.Models.Settings>());

            return instance;
        }
    }
}