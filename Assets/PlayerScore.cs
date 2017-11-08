using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScore : MonoBehaviour {

    private int Score;
    private int Combo;
    private int Multiplier;
    private const int HIT_SCORE = 100;

	// Use this for initialization
	void Start () {
        Score = 0;
        Combo = 0;
        Multiplier = 1;
	}
	
    public void AddScore()
    {
        Combo++;
        Score += HIT_SCORE * Multiplier;
        //Increase score multiplier on 5th hit
        if (Combo == 5)
        {
            Multiplier = 2;
        }
        //... on 10th hit
        else if (Combo == 10)
        {
            Multiplier = 3;
        }
        //... on 20th hit
        else if (Combo == 20)
        {
            Multiplier = 4;
        }
        //... on 50th hit
        else if (Combo == 50)
        {
            Multiplier = 5;
        }
        //... on 100th hit
        else if (Combo == 100)
        {
            Multiplier = 6;
        }
    }

    public void ResetCombo()
    {
        Combo = 0;
        Multiplier = 1;
    }

    void Update()
    {
        UnityEngine.UI.Text score = GetComponent<UnityEngine.UI.Text>();
        score.text = "Score: " + Score + "\r\nCombo: " + Combo + "\r\nMultiplier: " + Multiplier + "x";
    }
}
