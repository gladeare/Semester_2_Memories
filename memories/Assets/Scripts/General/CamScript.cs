using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamScript : MonoBehaviour {

    [Range(0, 100)]
    public float cameraStiffness;
    [Range(0, 3)]
    public float aheadMovement;

    GameObject player;
    TestMovement pScript;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        pScript = player.GetComponent<TestMovement>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        //follow the player
        //transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z); //old
        Vector3 goalPos = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
        goalPos += new Vector3(pScript.rb.velocity.x * aheadMovement, pScript.rb.velocity.y * aheadMovement, 0);
        transform.position = Vector3.Lerp(transform.position, goalPos, cameraStiffness / 100);
	}
}
