using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;


public class Copy : MonoBehaviour
{
    [SerializeField] TMP_InputField input;
    public void OnClick()
    {
        TextEditor t = new TextEditor();
        t.text = input.text;
        t.SelectAll();
        //t.Copy();
        GUIUtility.systemCopyBuffer = input.text;
    }
}
