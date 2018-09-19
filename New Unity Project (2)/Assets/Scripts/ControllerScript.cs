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
    public int[] highScores = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    public InputField nameInput;
    public InputField scoreInput;
    public Button submitButton;
    public int highScore = 0;

    public bool connectedToDatabase;
    public string playerName = "";
    public int playerScore = 0;

	// Use this for initialization
	void Start () {
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
}
