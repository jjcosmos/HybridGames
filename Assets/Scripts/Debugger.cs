﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Debugger : MonoBehaviour
{
    // Start is called before the first frame update
    public bool enableDebugging;
    public static Debugger instance;
    TextMeshProUGUI Log;
    [SerializeField] List<string> LogText;
    int maxLines = 15;
    void Awake()
    {
        instance = this;
        Log = GetComponent<TextMeshProUGUI>();
        LogText = new List<string>();
    }

    public void Push(string newText)
    {
        if (!enableDebugging)
        {
            return;
        }
        if(LogText.Count >= maxLines)
        {
            LogText.RemoveAt(0);
            LogText.Add(newText);
        }
        else
        {
            LogText.Add(newText);
        }
        RefreshDebugWindow();
    }

    private void RefreshDebugWindow()
    {
        string temp = "";
        foreach (var item in LogText)
        {
            temp += item + "\n";
        }
        Log.text = temp;
    }
}
