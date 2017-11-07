using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollisionBehaviour : MonoBehaviour {

    void OnCollisionEnter(Collision col)
    {
        Debug.Log("OnCollisionEnter with " + col.gameObject.name);
        if (col.gameObject.name == "LeftHandObj" || col.gameObject.name == "RightHandObj")
        {
            Destroy(gameObject);
        }
    }
}
