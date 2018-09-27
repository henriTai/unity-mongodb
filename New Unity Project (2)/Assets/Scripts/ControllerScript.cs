using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerScript : MonoBehaviour {

    public List<Text> Scores;
    public List<Text> Names;
    public Text bestScoreText;
    public Text bestPlayerText;
    public List<ListEntry> highScores;
    public InputField nameInput;
    public InputField scoreInput;
    public Button submitButton;
    public int highScore = 0;

    public bool connectedToDatabase;
    public string playerName = "";
    public int playerScore = 0;

	// Use this for initialization
	void Start () {
        highScores = new List<ListEntry>();
        ListEntry e = new ListEntry { _name = "", _score = 0 };
        for (int i=0; i<10; i++)
        {
            highScores.Add(e);
        }
        submitButton.onClick.AddListener(SubmitOnClick);
	}


    // Update is called once per frame
    void Update () {
		
	}

    private void SubmitOnClick()
    {
        int newScore;
        if (int.TryParse(scoreInput.text, out newScore))
        {
            if (newScore > highScore)
            {
                highScore = Mathf.Clamp(newScore, 0, 999999);
                bestScoreText.text = "" + highScore;
                if (nameInput.text != null && !nameInput.text.Equals(""))
                {
                    bestPlayerText.text = nameInput.text;
                }
                else
                {
                    bestPlayerText.text = "Unknown";
                }

            }
        }
    }

    public void UpdateHighScores(List<ListEntry> entries)
    {
        for (int i = 0; i < 10; i++)
        {
            if (entries[i]._name !="" || entries[i]._score != 0)
            {
                Names[i].text = entries[i]._name;
                Scores[i].text = ScoreToString(entries[i]._score);
            }
        }
    }

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
}
