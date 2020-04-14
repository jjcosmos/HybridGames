using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TurnStringInput : MonoBehaviour
{
    // Start is called before the first frame update
    TMP_InputField input;
    [SerializeField] InputManager myInput;
    void Awake()
    {
        input = GetComponent<TMP_InputField>();
    }

    public void OnSubmit()
    {
        //string newString = Utility.EncodeInt32(Utility.BASE32, Int32.Parse("1"+input.text));
        string newString = input.text;
        //Debug.LogError("Done editing");
        string decoded = Utility.DecodeInt32(Utility.BASE32, newString).ToString();
        Debug.Log(decoded);
        myInput.ProcessInput(decoded);
    }
}
