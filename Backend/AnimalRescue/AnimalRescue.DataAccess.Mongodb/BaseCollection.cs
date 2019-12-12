﻿using AnimalRescue.DataAccess.Mongodb.Configurations;
using AnimalRescue.DataAccess.Mongodb.Interfaces.Collections;
using AnimalRescue.DataAccess.Mongodb.Models;

using AutoMapper;

using MongoDB.Driver;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AnimalRescue.DataAccess.Mongodb
{
    public abstract class BaseCollection<T> : IBaseCollection<T>
        where T : BaseItem
    {
        protected IMongoClient client;
        protected IMongoDatabase database;
        protected IMongoCollection<T> collection;
        protected IMapper mapper;
        public BaseCollection(IMongoClient client, IMongoDbSettings settings, IMapper mapper)     
            : this(client, settings, mapper, typeof(T).Name)
        {
        }

        public BaseCollection(IMongoClient client, IMongoDbSettings settings, IMapper mapper, string collectionName)
        {
            this.client = client;
            database = client.GetDatabase(settings.DatabaseName);
            collection = database.GetCollection<T>(collectionName);
            this.mapper = mapper;
        }

        protected List<Tout> ConvertListTo<Tout>(List<T> items)
        {
            return mapper.Map<List<T>, List<Tout>>(items);
        }
        protected List<Tout> ConvertListTo<Tout>(IAsyncCursor<T> items)
        {
            return mapper.Map<List<T>, List<Tout>>(items.ToList());
        }
        protected T ConvertOneFrom<TIn>(TIn item)
        {
            return mapper.Map<TIn, T>(item);
        }
        protected Tout ConvertOneTo<Tout>(T item)
        {
            return mapper.Map<T, Tout>(item);
        }
        protected Tout ConvertOneTo<Tout>(IAsyncCursor<T> item)
        {
            return ConvertOneTo<Tout>(item.FirstOrDefault());
        }

        public async Task<IAsyncCursor<T>> GetAsync() => await collection.FindAsync(t => true);
        public async Task<IAsyncCursor<T>> GetAsync(string id) => await collection.FindAsync(t => t.Id == id);
        public async Task<T> GetOneByIdAsync(string id) => (await GetAsync(id)).FirstOrDefault();
        public async Task<IAsyncCursor<T>> GetAsync(int currentPage, int pageSize) =>
             await collection.Find(x => true)
            .Skip((currentPage - 1) * pageSize)
            .Limit(pageSize)
            .ToCursorAsync();

        public async Task<IAsyncCursor<T>> GetAsync(Expression<Func<T, bool>> func, int currentPage, int pageSize) =>
             await collection.Find(func)
            .Skip((currentPage - 1) * pageSize)
            .Limit(pageSize)
            .ToCursorAsync();

        public async Task<IAsyncCursor<T>> GetAsync(Expression<Func<T, bool>> func, Expression<Func<T, T>> projection, int currentPage, int pageSize) =>
             await collection.Find(func)
            .Skip((currentPage - 1) * pageSize)
            .Limit(pageSize)
            .Project(projection)
            .ToCursorAsync();

        public async Task UpdateAsync(string id, T instance) => await collection.ReplaceOneAsync(t => t.Id == id, instance);
        public async Task UpdateAsync(T instance) => await collection.ReplaceOneAsync(t => t.Id == instance.Id, instance);
        public async Task RemoveAsync(T instance) => await collection.DeleteOneAsync(t => t.Id == instance.Id);
        public async Task RemoveAsync(string id) => await collection.DeleteOneAsync(t => t.Id == id);
        public async Task<T> CreateAsync(T instance)
        {
            await collection.InsertOneAsync(instance);
            return instance;
        }

        public List<T> Get() => collection.Find(t => true).ToList();
        public T Get(string id) => collection.Find(t => t.Id == id).FirstOrDefault();
        public void Update(string id, T instance) => collection.ReplaceOne(t => t.Id == id, instance);
        public void Remove(T instance) => collection.DeleteOne(t => t.Id == instance.Id);
        public void Remove(string id) => collection.DeleteOne(t => t.Id == id);

        public T Create(T instance)
        {
            collection.InsertOne(instance);
            return instance;
        }
    }
}
