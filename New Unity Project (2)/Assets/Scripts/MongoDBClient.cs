using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MongoDB.Bson;
using MongoDB.Driver;

public class MongoDBClient {

    private readonly MongoClient _mongoClient;

    public MongoDBClient()
    {
        _mongoClient = new MongoClient("Server=localhost:27017");
    }

    public MongoDatabase GetDatabase (string name)
    {
        return _mongoClient.GetServer().GetDatabase(name);
    }

}
