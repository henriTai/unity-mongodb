using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text;

public class ControllerScript : MonoBehaviour {

    public List<Text> Scores;
    public List<Text> Names;
    public Text bestScoreText;
    public Text bestPlayerText;
    public Scores highScores;
    public InputField nameInput;
    public InputField scoreInput;
    public Button submitButton;
    public Button updateButton;
    public int highScore = 0;
    public bool QueryInProgress = false;
    public string uri = "http://localhost:5000/api/scores/";

    //public bool connectedToDatabase;
    public string playerName = "";
    public int playerScore = 0;

	// Use this for initialization
	void Start () {
        //highScores = new List<ScoreEntry>();
        //highScores = new Scores();
        ScoreEntry e = new ScoreEntry { name = "", score = 0 };
        for (int i=0; i<10; i++)
        {
            highScores.scores.Add(e);
        }
        submitButton.onClick.AddListener(SubmitOnClick);
        updateButton.onClick.AddListener(UpdateHighScores);
    }

    private void Update()
    {
    }

    private void SubmitOnClick()
    {
        int newScore;
        if (int.TryParse(scoreInput.text, out newScore))
        {
            //if (newScore > highScore)
            //{
            //    highScore = Mathf.Clamp(newScore, 0, 999999);
            //    bestScoreText.text = "" + highScore;
            //    if (nameInput.text != null && !nameInput.text.Equals(""))
            //    {
            //        bestPlayerText.text = nameInput.text;
            //    }
            //    else
            //    {
            //        bestPlayerText.text = "Unknown";
            //    }
            //}
            AddToList(new NewEntry { Name = nameInput.text, Score = newScore });
        }
    }

    public void AddToList(NewEntry newEntry)
    {
        StartCoroutine(AddHighScore(newEntry));

        //int whereToPut = -1;

        //for (int i = 0; i < 10; i++)
        //{
        //    if (newEntry.Score > highScores[i].Score)
        //    {
        //        whereToPut = i;
        //        break;
        //    }
        //}

        //if (whereToPut >= 0)
        //{
        //    for (int i = 8; i >= whereToPut; i--)
        //    {
        //        highScores[i+1] = highScores[i];
        //    }
        //    highScores[whereToPut] = newEntry;
        //}
        //UpdateHighScoresOffline();
    }

    public void UpdateHighScores()
    {
        StartCoroutine(GetHighScores());

        //List<ListEntry> entries = (List<ListEntry>)RestfulRequester.GetHighScores();

        //for (int i = 0; i < entries.Count; i++)
        //{
        //    if (entries[i]._name != "" || entries[i]._score != 0)
        //    {
        //        Names[i].text = entries[i]._name;
        //        Scores[i].text = ScoreToString(entries[i]._score);
        //    }
        //}
    }

    //public void UpdateHighScoresOffline()
    //{

    //    for (int i = 0; i < highScores.Count; i++)
    //    {
    //        if (highScores[i]._name != "" || highScores[i]._score != 0)
    //        {
    //            Names[i].text = highScores[i]._name;
    //            Scores[i].text = ScoreToString(highScores[i]._score);
    //        }
    //    }
    //}

    private string ScoreToString (int score)
    {
        string value = score.ToString();
        int digits = value.Length;
        if (digits < 6)
        {
            int zeros = 6 - digits;
            string added = "";
            for (int i=0; i<zeros; i++) { added += "0";}
            value = added + value;
        }
        else if (digits > 6)
        {
            value = "999999";
        }
        return value;
    }

    public IEnumerator GetHighScores()
    {
        UnityWebRequest req = UnityWebRequest.Get(uri);
        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("UnityKey", "unity1234");
        yield return req.SendWebRequest();
        //Debug.Log(req.isHttpError);
        //Debug.Log(req.isNetworkError);
        //Debug.Log(req.downloadHandler.text);

        //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("http://localhost:5000/"));
        //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        //StreamReader reader = new StreamReader(response.GetResponseStream());

        if (req.isDone && !req.isHttpError && !req.isNetworkError)
        {
            string jsonResponse = req.downloadHandler.text;
            Debug.Log("Recieved this: " + jsonResponse);
            Scores info = JsonUtility.FromJson<Scores>("{\"scores\":" + jsonResponse.ToString() + "}");
            //List<ScoreEntry> entries = new List<ScoreEntry>();
            //foreach (ScoreEntry score in info.scores)
            //{
            //    entries.Add(new ScoreEntry { Name = score.Name, Score = score.Score });
            //}

            highScores = info;

            for (int i = 0; i < highScores.scores.Count; i++)
            {
                if (highScores.scores[i].name != "" || highScores.scores[i].score != 0)
                {
                    Names[i].text = highScores.scores[i].name;
                    Scores[i].text = ScoreToString(highScores.scores[i].score);
                }
            }

        }
        //yield return entries;

        yield return null;
    }

    public IEnumerator AddHighScore(NewEntry newEntry)
    {
        WWWForm form = new WWWForm();
        string postString = JsonUtility.ToJson(newEntry);
        byte[] bytes = Encoding.UTF8.GetBytes(postString);
        Debug.Log("Sent this: " + Encoding.UTF8.GetString(bytes));
        form.AddBinaryData("body", bytes);
        UnityWebRequest req = UnityWebRequest.Post(uri, form);
        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("UnityKey", "unity1234");
        req.uploadHandler = new UploadHandlerRaw(bytes);
        req.uploadHandler.contentType = "application/json";

        yield return req.SendWebRequest();
        //Debug.Log(req.isHttpError);
        //Debug.Log(req.isNetworkError);
        //Debug.Log(req.downloadHandler.text);

        if (req.isDone && !req.isNetworkError)
        {
            Debug.Log("Recieved this: " + req.downloadHandler.text);
            //yield return req.downloadHandler.text;

            StartCoroutine(GetHighScores());
        }

        //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("http://localhost:5000"));
        //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        //StreamReader reader = new StreamReader(response.GetResponseStream());
        //string jsonResponse = ;
        //Players info = JsonUtility.FromJson<Players>(jsonResponse);
        //List<ListEntry> entries = new List<ListEntry>();
        //foreach (Player player in info.Plrs)
        //{
        //    entries.Add(new ListEntry { _name = player.Name, _score = player.Score });
        //}
        //return entries;
        yield return null;
    }
}
