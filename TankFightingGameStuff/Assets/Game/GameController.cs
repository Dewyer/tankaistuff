using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    public InputField CodeInput;
    public GameObject CodePanel;
    public bool CodeRunning = false;
    public bool Preparing = true;
    public GameObject TankBlue;
    public GameObject TankRed;
    public GameObject Bullet;
    public float tankSpeed;
    public Queue<Action> CodeActions;
    Dictionary<GameObject, List<GameObject>> ShotsFiredByTank;

    // Use this for initialization
    void Start () {
        CodeActions = new Queue<Action>();
        ShotsFiredByTank = new Dictionary<GameObject, List<GameObject>>();
        ShotsFiredByTank.Add(TankRed, new List<GameObject>());
        ShotsFiredByTank.Add(TankBlue, new List<GameObject>());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void RunCode(string code,GameObject onObject)
    {
        var linesArr = code.Split('\n');
        var lines = linesArr.ToList().Select(x => x.ToLower().Trim()).ToList();
        var atCharCount = 0;
        var cleanCode = String.Join("",code.Trim().Split('\n'));
        var skipping = false;
        CodeActions = new Queue<Action>();
        CodeRunning = true;
        UpdateDevVisible();

        for (int ii = 0; ii < lines.Count(); ii++)
        {
            if (!lines[ii].StartsWith("#") || !String.IsNullOrEmpty(lines[ii]))
            {
                if (lines[ii].StartsWith("ismételd") && lines[ii].Split(' ').Length >= 2)
                {
                    var allIn = GetCodeBlock(cleanCode, atCharCount).Trim();
                    Debug.Log("inloop:"+allIn);
                    var times = int.Parse(lines[ii].Split(' ')[1]);
                    var lineLoop = String.Join(")\n", allIn.Split(')')).Split('\n').Where(x=>x != "").ToList();
                    
                    for (int time = 0; time < times; time++)
                    {
                        for (int atLineInLoop = 0; atLineInLoop < lineLoop.Count; atLineInLoop++)
                        {
                            Debug.LogWarning("loop:"+lineLoop[atLineInLoop]);
                            CallFunction(lineLoop[atLineInLoop],onObject);
                        }
                    }

                    skipping = true;
                }
                else if ((!skipping) || lines[Math.Max(0,ii-1)].Contains("}"))
                {
                    skipping = false;
                    CallFunction(lines[ii],onObject);
                }
            }

            atCharCount += lines[ii].Length;
        }
        if (CodeActions.Count != 0)
            CodeActions.Dequeue()();
    }

    public string GetCodeBlock(string cleanCode,int atCharCount)
    {
        var inText = "";
        var gotOpener = false;
        for (int offset = 0; offset < cleanCode.Length - atCharCount; offset++)
        {
            if (gotOpener)
            {
                if (cleanCode[offset + atCharCount] == '}')
                {
                    break;
                }
                inText += cleanCode[offset + atCharCount];
            }
            else
            {
                if (cleanCode[offset + atCharCount] == '{')
                {
                    gotOpener = true;
                }
            }
        }

        return inText;
    }

    public void UpdateDevVisible()
    {
        if (Preparing && !CodeRunning)
        {
            CodePanel.SetActive(false);
        }
        else
        {
            CodePanel.SetActive(true);
        }

    }

    public void Test()
    {
        if (Preparing && !CodeRunning)
        {
            try
            {
                RunCode(CodeInput.text,TankBlue);
            }
            catch (Exception e)
            {
                CodeRunning = false;
                UpdateDevVisible();
                Debug.LogError(e.Message);
            }
        }
    }

    public void TankShot(object hitO)
    {
        var hit = (BulletHit)hitO;
        if (!ShotsFiredByTank[hit.Tank].Contains(hit.Bullet))
        {
            //Destroy(hit.Bullet);
            Debug.Log(""+hit.Bullet.tag);
        }
    }

    IEnumerator GoForward(int milis, GameObject onObject)
    {
        Debug.Log("nowfor");
        onObject.GetComponent<Rigidbody2D>().velocity = onObject.transform.up * tankSpeed;
        yield return new WaitForSeconds(milis / 1000f);
        onObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        ActionEndCallBack();
    }

    IEnumerator Turn(bool isBal,int milis, GameObject onObject)
    {
        Debug.Log("rot");
        var vel = 5 * (isBal?1:-1);
        var timeelapsed = 0;
        while (timeelapsed <= milis)
        {
            onObject.GetComponent<Rigidbody2D>().MoveRotation(onObject.transform.rotation.eulerAngles.z + vel);
            timeelapsed += 10;
            yield return new WaitForSeconds(10 / 1000f);
        }
        ActionEndCallBack();
    }

    IEnumerator Shoot(GameObject onObject)
    {
        var origin = onObject.GetComponentInChildren<Transform>().position;
        var newBullet = Instantiate(Bullet, origin, onObject.transform.rotation);
        newBullet.GetComponent<Rigidbody2D>().velocity = newBullet.transform.up * tankSpeed * 2;
        ShotsFiredByTank[onObject].Add(newBullet);

        yield return new WaitForSeconds(0.1f);
        ActionEndCallBack();
    }

    public void ActionEndCallBack()
    {
        if (CodeRunning)
        {
            if (CodeActions.Count != 0)
            {
                CodeActions.Dequeue()();
            }
            else
            {
                CodeRunning = false;
                UpdateDevVisible();
            }
        }
    }

    public int GetOneIntArg(string line)
    {
        try
        {
            var ii = int.Parse(line.Replace(')','(').Split('(')[1]);
            return ii;
        }
        catch (Exception ex)
        {
            throw new Exception("Cant parse function arg: " + ex.Message);
        }
        return 0;
    }

    public void CallFunction(string line,GameObject onObject)
    {
        if (line.StartsWith("test"))
        {
            Debug.Log("puk");
            return;
        }
        if (line.StartsWith("előre"))
        {
            var forMilis = GetOneIntArg(line);
            CodeActions.Enqueue(() => { StartCoroutine(GoForward(forMilis, onObject)); });
            return;
        }
        if (line.StartsWith("balra"))
        {
            var forMilis = GetOneIntArg(line);
            CodeActions.Enqueue(() => { StartCoroutine(Turn(true,forMilis, onObject)); });
            return;
        }
        if (line.StartsWith("jobbra"))
        {
            var forMilis = GetOneIntArg(line);
            CodeActions.Enqueue(() => { StartCoroutine(Turn(false, forMilis, onObject)); });
            return;
        }
        if (line.StartsWith("lő") && line.Contains("()"))
        {
            CodeActions.Enqueue(() => { StartCoroutine(Shoot(onObject)); });
            return;
        }

        throw new Exception("No such function as "+line);
    }

}
