using Application.Models;
using Application.Exceptions;
using Domain;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Linq;

namespace Application.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        private string CollectionName;
        private IMongoClient Client;
        private IMongoDatabase Database;

        public BaseRepository(IOptions<DatabaseSettings> dbSettingOptions)
        {
            var dbSettings = dbSettingOptions.Value;
            Client = new MongoClient(dbSettings.ConnectionString);
            Database = Client.GetDatabase(dbSettings.DatabaseName);
        }

        internal void SetCollection(string collectionName)
        {
            if (string.IsNullOrEmpty(collectionName.Trim()))
                throw new NullDbCollectionNameException();

            CollectionName = collectionName;
        }

        internal IMongoCollection<T> GetCollection()
        {
            if (string.IsNullOrEmpty(CollectionName))
                throw new NotAssignedDbConnectionNameException();

            var collection = Database.GetCollection<T>(CollectionName);

            if (collection == null)
            {
                Database.CreateCollection(CollectionName);
                collection = Database.GetCollection<T>(CollectionName);
            }

            return collection;
        }

        public async Task DeleteAsync(Expression<Func<T, bool>> expression)
        {
            var collection = GetCollection();

            await collection.DeleteOneAsync(expression);
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> expression)
        {
            var collection = GetCollection();

            var count = await collection.CountAsync(expression);

            return count > 0;
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> expression = null)
        {

            var collection = GetCollection();

            var cursor = await collection.FindAsync(expression);

            return await cursor.FirstOrDefaultAsync();
        }

        public async Task InsertAsync(T entity)
        {
            var collection = GetCollection();

            await collection.InsertOneAsync(entity);
        }

        public async Task<IEnumerable<T>> ListAsync(Expression<Func<T, bool>> expression = null)
        {
            var collection = GetCollection();

            var cursor = await collection.FindAsync(expression);

            return await cursor.ToListAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            var collection = GetCollection();

            var result = await collection.ReplaceOneAsync(x => x.Id == entity.Id, entity);
        }

        public async Task<BulkWriteResultModel> BulkWriteAsync(IEnumerable<T> inserts = null, IEnumerable<T> updates = null, IEnumerable<T> deletes = null)
        {
            var collection = GetCollection();

            var recordCount = 0;
            if (inserts != null) recordCount += inserts.Count();
            if (updates != null) recordCount += updates.Count();
            if (deletes != null) recordCount += deletes.Count();

            if (recordCount == 0)
            {
                return new BulkWriteResultModel();
            }

            var writeModels = new List<WriteModel<T>>();

            if (inserts != null)
                foreach (var item in inserts)
                {
                    writeModels.Add(new InsertOneModel<T>(item));
                }

            if (updates != null)
                foreach (var item in updates)
                {
                    var filterDefinition = Builders<T>.Filter.Where(x => x.Id == item.Id);

                    writeModels.Add(new ReplaceOneModel<T>(filterDefinition, item));
                }

            if (deletes != null)
                foreach (var item in deletes)
                {
                    var filterDefinition = Builders<T>.Filter.Where(x => x.Id == item.Id);

                    writeModels.Add(new DeleteOneModel<T>(filterDefinition));
                }

            var result = await collection.BulkWriteAsync(writeModels);

            return new BulkWriteResultModel { Inserted = result.InsertedCount, Updated = result.ModifiedCount, Deleted = result.DeletedCount };
        }
    }
}
