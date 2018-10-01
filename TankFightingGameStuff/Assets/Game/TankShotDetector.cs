using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankShotDetector : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnTriggerEnter2D(Collider2D collision)
    {
        
        GameObject.FindGameObjectWithTag("GameController").SendMessage("TankShot", this);
        Destroy(collision.gameObject);
    }
}
