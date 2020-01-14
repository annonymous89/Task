using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdSrv.Infrastructure
{
    public class Init
    {
        public static async Task Initialize()
        {
            var db = new MongoClient().GetDatabase("NotesDB");

            if (!await Repository.MongoCRUD.CollectionExistsAsync("Users", db))
                await Repository.MongoCRUD.Init(db);
            
        }
    }
}
