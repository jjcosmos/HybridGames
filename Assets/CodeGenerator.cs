﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using database;
public class CodeGenerator : MonoBehaviour
{

    [SerializeField] List<Action> myActions;
    [SerializeField] TMP_InputField output; //lol
    [SerializeField] DatabaseInteractionMobile dbMobile;
    string myTurn = "";
    public void OnButtonPress()
    {
        int rand = UnityEngine.Random.Range(1, 10);
        myTurn = rand.ToString();
        foreach (var action in myActions)
        {
            Vector2 vec = action.GetDirAct();
            string dir = vec.x.ToString();
            string act = vec.y.ToString();
            if(vec.y == -1)
            {
                act = "";
                dir = "";
            }
            myTurn += act + dir; 
        }
        Debug.Log(myTurn);
        output.text = Utility.EncodeInt32(Utility.BASE32, Int32.Parse(myTurn));

        dbMobile.SendTurnToDatabase(output.text); //need to find out if there is an event when
    }
}
