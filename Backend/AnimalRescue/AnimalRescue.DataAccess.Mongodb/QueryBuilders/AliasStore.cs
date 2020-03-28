﻿using AnimalRescue.DataAccess.Mongodb.Attributes;
using AnimalRescue.Infrastructure.Utilities;

using MongoDB.Bson.Serialization.Attributes;

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AnimalRescue.DataAccess.Mongodb.QueryBuilders
{
    internal class AliasStore  : IAliasStore
    {
        private ConcurrentDictionary<Type, List<Alias>> aliasDictionary;

        public AliasStore()
        {
            int initialCapacity = 101;
            int numProcs = Environment.ProcessorCount;
            int concurrencyLevel = numProcs * 2;

            aliasDictionary = new ConcurrentDictionary<Type, List<Alias>>(concurrencyLevel, initialCapacity);
        }

        public Alias GetAlias<T>(string aliasePropertyName)
        {
            Type type = typeof(T);            

            return GetAlias(type, aliasePropertyName);
        }

        public Alias GetAlias(Type type, string aliasePropertyName)
        {
            if (aliasDictionary.TryGetValue(type, out var currentAlias))
            {
                return currentAlias
                    .FirstOrDefault(alias => IsEqualNames(alias, aliasePropertyName));
            }

            currentAlias = GetAliases(type);

            return currentAlias
                .FirstOrDefault(alias => IsEqualNames(alias, aliasePropertyName));
        }

        private List<Alias> GetAliases(Type type)
        {
            List<Alias> currentAlias = type
                .GetProperties()
                .Select(ConvertToAlias)
                .SelectMany(AliasToListNestedAliases)
                .Where(x => x != null)
                .Distinct(new EntityComparer<Alias>(IsEqual))
                .ToList();

            aliasDictionary.TryAdd(type, currentAlias);

            var collections = currentAlias
                .Where(currentAlias => currentAlias.PropertyType.GetInterface(nameof(ICollection)) != null)
                .Select(currentAlias => currentAlias.PropertyType.GetGenericArguments().Single())
                .Where(t => !t.IsPrimitive && !t.IsValueType && (t.Namespace == null || !t.Namespace.StartsWith("System")))
                .ToList();

            collections.ForEach(x => GetAliases(x));

            return currentAlias;
        }

        private static bool IsEqualNames(Alias alias, string aliasePropertyName)
        {
            return alias.AliasePropertyName
                .Equals(aliasePropertyName, StringComparison.OrdinalIgnoreCase);
        }
        private static bool IsEqual(Alias x, Alias y) =>
            x.PropertyName == y.PropertyName
            && x.AliasePropertyName == y.AliasePropertyName
            && x.DataBasePropertyName == y.DataBasePropertyName;

        private static Alias ConvertToAlias(PropertyInfo propertyInfo)
        {
            var aliasName = propertyInfo.GetCustomAttribute<CouplingPropertyNameAttribute>()?.AliasName;
            var elementName = propertyInfo.GetCustomAttribute<BsonElementAttribute>()?.ElementName;

            if (elementName == null || aliasName == null)
            {
                return null;
            }

            return new Alias
            {
                AliasePropertyName = aliasName,
                DataBasePropertyName = elementName,
                PropertyType = propertyInfo.PropertyType,
                PropertyName = propertyInfo.Name,
            };
        }

        private static List<Alias> AliasToListNestedAliases(Alias alias)
        {
            if (alias == null)
            {
                return new List<Alias>();
            }

            List<Alias> result = new List<Alias> { alias };

            var propertyInfos = alias.PropertyType.GetProperties();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                Alias currentAlias = ConvertToAlias(propertyInfo);

                if (currentAlias == null)
                {
                    continue;
                }

                currentAlias.AliasePropertyName = $"{alias.AliasePropertyName}.{currentAlias.AliasePropertyName}";
                currentAlias.DataBasePropertyName = $"{alias.DataBasePropertyName}.{currentAlias.DataBasePropertyName}";

                result.Add(currentAlias);
                result.AddRange(AliasToListNestedAliases(currentAlias));
            }

            return result;
        } 
    }
}
