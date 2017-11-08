using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollisionBehaviour : MonoBehaviour {

    public PlayerHealth playerHealth;
    public PlayerScore playerScore;
    public bool enableMovement;
    private Vector3 initialPosition;
    private float velocity;
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "LeftHandObj" || col.gameObject.name == "RightHandObj")
        {
            Destroy(gameObject);
            playerScore.AddScore();
        }
        if (col.gameObject.name == "Base")
        {
            Destroy(gameObject);
            playerHealth.ReduceHealth();
            playerScore.ResetCombo();
        }
    }

    void Start()
    {
        initialPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        velocity = Random.Range(0.01f, 2f);
    }

    void Update()
    {
        if (enableMovement) transform.Translate(-initialPosition.x * Time.deltaTime * velocity, -initialPosition.y * Time.deltaTime * velocity, 0);
        //Debug.Log(gameObject.name + ": " + transform.position.x + " " + transform.position.y + " " + transform.position.z);
    }
}
