using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

public class MDBRepository : MonoBehaviour{

    private MongoCollection<BsonDocument> _collection;
    private MongoDatabase _database;
    private ControllerScript _gameController;

    public MDBRepository (MongoDBClient client, ControllerScript controller)
    {
        _database = client.GetDatabase("unity");
        _collection = _database.GetCollection<BsonDocument>("scores");
        _gameController = controller;
    }


    IEnumerator GetHighScores()
    {
        List<ListEntry> entries = new List<ListEntry>();
        QueryDocument _document = new QueryDocument("key", "value");
        var cursor = _collection.FindAs<BsonDocument>(_document);
        yield return _collection;
        foreach (BsonDocument d in cursor)
        {
            entries.Add(BsonSerializer.Deserialize<ListEntry>(d));
            yield return d;
        }
        _gameController.UpdateHighScores(entries);


        yield return null;
    }
}
