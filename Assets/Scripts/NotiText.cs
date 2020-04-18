using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class NotiText : MonoBehaviour
{
    // Start is called before the first frame update
    public static NotiText _out;
    private Animator myAnim;
    private TextMeshProUGUI tmp;
    void Awake()
    {
        _out = this;
        myAnim = GetComponent<Animator>();
        tmp = GetComponent<TextMeshProUGUI>();
    }

    public void PushText(string text)
    {
        tmp.text = text;
        myAnim.Play("Fade");
    }

}
