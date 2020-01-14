using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository
{
    public class MongoCRUD
    {
        public static async Task Init(IMongoDatabase _db)
        {
            var users = _db.GetCollection<UserModel>("Users");
            var options = new CreateIndexOptions() { Unique = true };
           
            var usersBuilder = Builders<UserModel>.IndexKeys;
            
            var indexModel = new CreateIndexModel<UserModel>(usersBuilder.Ascending(x => x.Email), options);
            
            await users.Indexes.CreateOneAsync(indexModel);

        }

        public static async Task<bool> CollectionExistsAsync(string collectionName, IMongoDatabase db)
        {
            var filter = new BsonDocument("name", collectionName);

            var collections = await db.ListCollectionsAsync(new ListCollectionsOptions { Filter = filter });

            return await collections.AnyAsync();
        }

        public static async Task Insert<T>(string table, T record, IMongoDatabase _db)
        {
            var collection = _db.GetCollection<T>(table);
            await collection.InsertOneAsync(record);
        }

        public static async Task<List<T>> LoadAll<T>(string table, IMongoDatabase _db)
        {
            var collection = _db.GetCollection<T>(table);
            var result = await collection.FindAsync(new BsonDocument());

            return await result.ToListAsync();
        }

        public static async Task<List<T>> LoadAllForUser<T>(string table, IMongoDatabase _db, Guid userId)
        {
            var collection = _db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("UserId", userId);

            var result = await collection.FindAsync(filter);

            return await result.ToListAsync();
        }

        public static async Task<T> LoadById<T>(string table, Guid id, IMongoDatabase _db)
        {
            var collection = _db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("Id", id);

            var result = await collection.FindAsync(filter);

            return await result.FirstAsync();
        }

        public static async Task<List<T>> LoadAllNotesForUser<T>(string table, Guid id, IMongoDatabase _db)
        {
            var collection = _db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("UserId", id);

            var result = await collection.FindAsync(filter);

            return await result.ToListAsync();
        }

        public static async Task<T> LoadByEmail<T>(string table, string email, IMongoDatabase _db)
        {
            var collection = _db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("Email", email);

            var result = await collection.FindAsync(filter);

            return await result.FirstAsync();
        }
    }
}
