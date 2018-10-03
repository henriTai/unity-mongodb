//using System.Collections;
//using System.Net;
//using System.IO;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Networking;

//public static class RestfulRequester{

//    public static IEnumerator GetHighScores()
//    {
//        UnityWebRequest req = UnityWebRequest.Get("http://localhost:5000/api/scores");
//        req.SetRequestHeader("Content-Type", "application/json");
//        req.SetRequestHeader("Unitykey", "unity1234");
//        req.SendWebRequest();
//        Debug.Log(req.isHttpError);
//        Debug.Log(req.isNetworkError);
//        Debug.Log(req.downloadHandler.text);

//        //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("http://localhost:5000/"));
//        //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
//        //StreamReader reader = new StreamReader(response.GetResponseStream());
//        if (req.isDone && !req.isHttpError && !req.isNetworkError)
//        {
//            string jsonResponse = req.downloadHandler.text;
//            Scores info = JsonUtility.FromJson<Scores>(jsonResponse);
//            List<ScoreEntry> entries = new List<ScoreEntry>();
//            foreach (ScoreEntry score in info.scores)
//            {
//                entries.Add(new ScoreEntry { Name = score.Name, Score = score.Score });
//            }
//        }
//        //yield return entries;
//        yield return null;
//    }

//    public static IEnumerator AddHighScore(NewEntry newEntry)
//    {
//        string postString = JsonUtility.ToJson(newEntry);
//        UnityWebRequest req = UnityWebRequest.Post("http://localhost:5000/api/scores", postString);
//        req.SetRequestHeader("Content-Type", "application/json");
//        req.SetRequestHeader("Unitykey", "unity1234");
//        var result = req.SendWebRequest();
//        Debug.Log(req.isHttpError);
//        Debug.Log(req.isNetworkError);
//        Debug.Log(req.downloadHandler.text);

//        //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("http://localhost:5000"));
//        //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
//        //StreamReader reader = new StreamReader(response.GetResponseStream());
//        //string jsonResponse = ;
//        //Players info = JsonUtility.FromJson<Players>(jsonResponse);
//        //List<ListEntry> entries = new List<ListEntry>();
//        //foreach (Player player in info.Plrs)
//        //{
//        //    entries.Add(new ListEntry { _name = player.Name, _score = player.Score });
//        //}
//        //return entries;
//        yield return null;
//    }
//}
