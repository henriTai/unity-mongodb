using MongoDB.Driver;

namespace UnityProjectServer.MongoDB
{
    public class MongoDBClient
    {
        private readonly MongoClient _mongoClient;

        public MongoDBClient()
        {
            _mongoClient = new MongoClient("mongodb://localhost:27017");
            //_mongoClient = new MongoClient("mongodb://unitybackend:ZSXTGo3QaUgV6fq9Ra5RweYGSZJL1ihrJk4rxN8N9oXBOL5bwNxjvWabn8mQK1GTpHoxYHpQ2OmjQ7ygfg01Rg==@unitybackend.documents.azure.com:10255/?ssl=true&replicaSet=globaldb");
        }

        public IMongoDatabase GetDatabase(string name)
        {
            return _mongoClient.GetDatabase(name);
        }
    }
}