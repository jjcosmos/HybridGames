using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
        Debug.LogError("Done editing");
        myInput.ProcessInput(input.text);
    }
}
