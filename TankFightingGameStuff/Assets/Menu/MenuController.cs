using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class MenuController : MonoBehaviour {

	// Use this for initialization

    IEnumerator Start()
    {
        WebSocket w = new WebSocket(new Uri("wss://localhost:44316/gamews"));
        yield return StartCoroutine(w.Connect());
        w.SendString("Hi there");
        int i = 0;
        while (true)
        {
            string reply = w.RecvString();
            if (reply != null)
            {
                Debug.Log("Received: " + reply);
                w.SendString("Hi there" + i++);
            }
            if (w.error != null)
            {
                Debug.LogError("Error: " + w.error);
                break;
            }
            yield return 0;
        }
        w.Close();
    }

    // Update is called once per frame
    void Update () {
		
	}
}
