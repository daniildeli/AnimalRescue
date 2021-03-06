﻿using AnimalRescue.DataAccess.Mongodb.Interfaces.Repositories;
using AnimalRescue.DataAccess.Mongodb.Models.Tag;
using AnimalRescue.Infrastructure.Utilities;

using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AnimalRescue.DataAccess.Mongodb.Migrations
{
    public static class MigrationConfiguration
    {
        public static string ResourceDataFolder { get; } = "Data";

        public static async Task ConfigureMigrationsAsync(IServiceProvider serviceProvider)
        {
            await SetUpDataBase<IWellKnownTagRepository, WellKnownTag>(
                serviceProvider,
                "tags.json",
                (repo, collection) => repo.WhereByIdAsync(collection),
                (repo, collection) => repo.CreateAsync(collection),
                (x, y) => x.Id == y.Id);
        }

        private static async Task SetUpDataBase<TRepository, TEntity>(
            IServiceProvider serviceProvider,
            string fileName,
            Func<TRepository, List<TEntity>, Task<List<TEntity>>> searchFunc,
            Func<TRepository, List<TEntity>, Task<IEnumerable<TEntity>>> createFunc,
            Func<TEntity, TEntity, bool> compareFunc)

        {
            string content = GetResourceTextFile(fileName);

            if (string.IsNullOrWhiteSpace(content))
            {
                return;
            }

            List<TEntity> collection = JsonConvert.DeserializeObject<List<TEntity>>(content);

            TRepository repository = serviceProvider.GetRequiredService<TRepository>();
            List<TEntity> existingCollection = await searchFunc(repository, collection);
            collection = collection
                .Except(existingCollection, new EntityComparer<TEntity>(compareFunc))
                .ToList();

            if (collection.Count > 0)
            {
                await createFunc(repository, collection);
            }
        }
        private static string GetResourceTextFile(string filename)
        {
            StringBuilder stringBuilder = new StringBuilder();
            try
            {
                using (Stream stream = typeof(MigrationConfiguration).Assembly
                    .GetManifestResourceStream($"{Assembly.GetCallingAssembly().GetName().Name}.{ResourceDataFolder}.{filename}"))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            stringBuilder.Append(line);
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
                return null;
            }

            return stringBuilder.ToString();
        }
    }
}
