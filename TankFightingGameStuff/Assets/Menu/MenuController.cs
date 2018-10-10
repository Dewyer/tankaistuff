using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using WebSocketSharp;

public class ServerResponse
{
    public string Error { get; set; }
    public string Data { get; set; }

}

public class MenuController : MonoBehaviour {

    // Use this for initialization
    public string ServerUrl;
    public InputField RoomNameField;

    // Update is called once per frame
    void Update () {
		
	}

    public void ClickNewRoom()
    {
        StartCoroutine(OpenNewRoom(RoomNameField.text));
    }

    IEnumerator OpenNewRoom(string name)
    {

        UnityWebRequest www = UnityWebRequest.Get(ServerUrl + "OpenRoom?name="+name);
        //www.useHttpContinue = false;

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error+" || "+www.downloadHandler.text+" || "+www.responseCode );
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
        }
    }

    IEnumerator ConnectToRoom(string name)
    {

        UnityWebRequest www = UnityWebRequest.Get(ServerUrl + "ConnectToRoom?name=" + name);
        //www.useHttpContinue = false;

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error + " || " + www.downloadHandler.text + " || " + www.responseCode);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
        }
    }
}
