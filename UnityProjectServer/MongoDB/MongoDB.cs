using MongoDB.Driver;

namespace UnityProjectServer.MongoDB
{
    public class MongoDBClient
    {
        private readonly MongoClient _mongoClient;

        public MongoDBClient()
        {
            _mongoClient = new MongoClient("mongodb://localhost:27017");
        }

        public IMongoDatabase GetDatabase(string name)
        {
            return _mongoClient.GetDatabase(name);
        }
    }
}