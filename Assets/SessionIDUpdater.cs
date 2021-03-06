﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using database;
public class SessionIDUpdater : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] DatabaseInteractionDesktop dbInteraction;
    private TMPro.TextMeshProUGUI displayTxt;
    private bool sessionCreated;
    void Awake()
    {
        displayTxt = GetComponent<TMPro.TextMeshProUGUI>();
        //dbInteraction.OnSessionCreated.AddListener(OnInitSession);
        StartCoroutine(IAnimateText());
    }

    public void OnInitSession(string gameID)
    {
        sessionCreated = true;
        StopAllCoroutines();
        Debugger.instance.Push($"Session Created with ID {gameID}");
        Debug.Log("Updating lobby id");
        displayTxt.text = $"Session ID: {gameID}";
    }

    private IEnumerator IAnimateText()
    {
        while (!sessionCreated)
        {
            displayTxt.text = "Creating Session";
            yield return new WaitForSecondsRealtime(.5f);
            displayTxt.text = "Creating Session.";
            yield return new WaitForSecondsRealtime(.5f);
            displayTxt.text = "Creating Session..";
            yield return new WaitForSecondsRealtime(.5f);
            displayTxt.text = "Creating Session...";
            yield return new WaitForSecondsRealtime(.5f);
        }
    }
}
