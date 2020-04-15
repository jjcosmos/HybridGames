using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TurnStringInput : MonoBehaviour
{
    // Start is called before the first frame update
    TMP_InputField tmpInput;
    [SerializeField] InputManager myInput;
    void Awake()
    {
        tmpInput = GetComponent<TMP_InputField>();
    }

    public void OnSubmit()
    {
        if(Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey(KeyCode.Return))
        {
            //string newString = Utility.EncodeInt32(Utility.BASE32, Int32.Parse("1"+input.text));
            string newString = tmpInput.text;
            //Debug.LogError("Done editing");
            string decoded = Utility.DecodeInt32(Utility.BASE32, newString).ToString();
            Debug.Log(decoded);
            myInput.ProcessInput(decoded);
        }
        
    }
}
