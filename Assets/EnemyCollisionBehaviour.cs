using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollisionBehaviour : MonoBehaviour {

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "LeftHandObj" || col.gameObject.name == "RightHandObj")
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        //Debug.Log(gameObject.name + ": " + transform.position.x + " " + transform.position.y + " " + transform.position.z);
    }
}
