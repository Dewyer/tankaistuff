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


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void RunCode(string code)
    {
        var linesArr = code.Split('\n');
        var lines = linesArr.ToList().Select(x => x.ToLower().Trim()).ToList();
        var atCharCount = 0;
        var cleanCode = String.Join("",code.Trim().Split('\n'));
        var skipping = false;

        for (int ii = 0; ii < lines.Count(); ii++)
        {
            if (!lines[ii].StartsWith("#"))
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
                            CallFunction(lineLoop[atLineInLoop]);
                        }
                    }

                    skipping = true;
                }
                else if ((!skipping) || lines[Math.Max(0,ii-1)].Contains("}"))
                {
                    skipping = false;
                    CallFunction(lines[ii]);
                }
            }

            atCharCount += lines[ii].Length;
        }
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

    public void Test()
    {
        if (Preparing && !CodeRunning)
        {
            try
            {
                RunCode(CodeInput.text);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }

    public void CallFunction(string line)
    {
        if (line.StartsWith("test"))
        {
            Debug.Log("anyas");
            return;
        }

        throw new Exception("No such function as "+line);
    }

}
