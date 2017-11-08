using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {

    private int Health;
	// Use this for initialization
	void Start () {
        Health = 100;
	}
	
	// Update is called once per frame
	void Update () {
        Transform phi = transform.Find("PlayerHealthIndicator");
        if (phi != null)
        {
            phi.localScale = new Vector3(Health / 100.0f * 0.95f, 0.7f, 1f);
            phi.localPosition = new Vector3(-0.475f + 0.475f * Health / 100.0f, 0f, 0f);
        }
	}

    public void ReduceHealth()
    {
        if (Health > 10) Health -= 10;
        else Health = 0;
    }

    public int GetHealth()
    {
        return Health;
    }
}
