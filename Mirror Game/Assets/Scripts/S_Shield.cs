using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Shield : MonoBehaviour {

    [SerializeField] GameObject player;
	// Update is called once per frame
	void Update () {
        //keep the shield centred on the player object
        transform.position = player.transform.position;
        //rotation code for the shield game object to give the shield a forcefield type look
        transform.Rotate(new Vector3(1.0f, 1.0f, 1.0f) * Time.deltaTime * 200);
    }
}
