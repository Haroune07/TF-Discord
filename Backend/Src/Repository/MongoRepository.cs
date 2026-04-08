using Backend.Settings;
using Backend.Src.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Backend.Src.Repository
{
    public class MongoRepository<T> : IRepository<T> where T : IEntity
    {
        private readonly IMongoCollection<T> collection;

        public MongoRepository(IMongoClient client, IOptions<MongoDBSettings> options)
        {
            string collectionName = typeof(T).Name + "s";
            collection = client.GetDatabase(options.Value.DatabaseName).GetCollection<T>(collectionName);
        }

        public Task DeleteAsync(string id)
        {
             return collection.DeleteOneAsync(t => t.Id == id);
        }

        public async Task<List<T>> FindAsync(Expression<Func<T, bool>> filter)
        {
            return (await collection.FindAsync(filter)).ToList();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return (await collection.FindAsync(_ => true)).ToList();
        }

        public async Task<T?> GetByIdAsync(string id)
        {
            return (await collection.FindAsync(t => t.Id == id)).FirstOrDefault();
        }

        public Task InsertAsync(T entity)
        {
            return collection.InsertOneAsync(entity);
        }

        public Task UpdateAsync(string id, T entity)
        {
            return collection.ReplaceOneAsync(t => t.Id == id, entity);
        }
    }
}
