using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using WebSocketSharp;
using Newtonsoft;

[System.Serializable]
public class ServerResponse
{
    public string error { get; set; }
    public string data { get; set; }

    public static ServerResponse FromJson(string json)
    {
        var parts = json.Split(',');
        var err = parts[0].Split(':')[1].Split('"')[1];
        var dat = parts[1].Split(':')[1].Split('"')[1];
        return new ServerResponse() { error = err, data = dat };
    }

}

public class MenuController : MonoBehaviour {

    // Use this for initialization
    public string ServerUrl;
    public InputField RoomNameField;
    public List<GameObject> ScrollItems;
    public GameObject ListItem;
    public GameObject ViewPort;

    void Start()
    {
        ScrollItems = new List<GameObject>();

        StartCoroutine(UpdateLobbies());
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void UpdateScroll(List<string> rooms)
    {
        ScrollItems.ForEach(xx => Destroy(xx));

        foreach (var room in rooms)
        {
            Debug.Log("aaaa : " + room);
            var ii = Instantiate(ListItem, new Vector3(10, (rooms.IndexOf(room) * -120) -20, 0), Quaternion.identity, ViewPort.transform);
            ScrollItems.Add(ii);

            ii.GetComponentsInChildren<Text>()[0].text = room;
            ii.GetComponentInChildren<Button>().gameObject.name = room;
            ii.GetComponentInChildren<Button>().onClick.AddListener(delegate {
                Debug.Log("keksza");
                StartCoroutine(ConnectToRoom(room));

            });
            ii.transform.parent = ViewPort.transform.parent;
            ii.GetComponent<RectTransform>().localPosition = new Vector3(10, -20+(rooms.IndexOf(room) * -120));
        }
    }

    IEnumerator UpdateLobbies()
    {
        while (true)
        {
            UnityWebRequest www = UnityWebRequest.Get(ServerUrl + "GetRooms");
            //www.useHttpContinue = false;

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error + " || " + www.downloadHandler.text + " || " + www.responseCode);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                var dd = www.downloadHandler.text;
                var resp = String.Join("",dd.Replace('[', ']').Split(']')[1].Split('"')).Split(',').ToList().Where(x=>x != "").ToList();

                UpdateScroll(resp);
            }

            yield return new WaitForSeconds(2);
        }
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
            var resp = ServerResponse.FromJson(www.downloadHandler.text);

            Debug.Log(resp.error+" !!");
            Debug.Log(resp.data +" !! " );
            if (resp.error =="None")
            {
                PlayerPrefs.SetString("serverId",resp.data);
                SceneManager.LoadScene("game");
            }
            else
            {
                
            }
        }
    }
}
