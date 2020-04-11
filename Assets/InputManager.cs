﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InputManager : MonoBehaviour
{
    // Temporary, server will replace this code
    [SerializeField] Players myPlayers;
    [SerializeField] GamestateManager gameManager;
    bool allPlayersSubmitted;

    [SerializeField] Slider PlayerSlider;
    [SerializeField] Slider DirectionInput;
    [SerializeField] Slider ActionTypeInput;
    [SerializeField] Button LockMoveButton;
    [SerializeField] Button SubmitButton;

    [SerializeField] Button ExecuteTurnsButton;

    [SerializeField] TextMeshProUGUI PlayerIndicator;
    [SerializeField] TextMeshProUGUI DirectionIndicator;
    [SerializeField] TextMeshProUGUI ActionIndicator;


    int player;
    int dir;
    int act;
    bool editable = false;
    int currentTurnI;
    //PlayerMoveSet currentMoves;
    private void Start()
    {
        
        OnPlayerSliderValueChanged();
        OnDirectionSliderValueChanged();
        OnActionSliderValueChanged();
        UpdateSubmitButton();
        //currentMoves = new PlayerMoveSet();
    }

    public void OnPlayerSliderValueChanged()
    {
        player = (int)PlayerSlider.value;
        PlayerIndicator.text = player.ToString();
        UpdateSubmitButton();
    }

    public void OnDirectionSliderValueChanged()
    {
        dir = (int)DirectionInput.value;
        DirectionIndicator.text = dir.ToString();
    }

    public void OnActionSliderValueChanged()
    {
        act = (int)ActionTypeInput.value;
        ActionIndicator.text = act.ToString();
    }

    public void OnLockTurnPressed()
    {
        if (!myPlayers.RespectivePlayerTurns[player-1].IsComplete())
        {
            myPlayers.RespectivePlayerTurns[player-1].AddMove(dir, act);
        }
        else
        {
            Debugger.instance.Push("Turn is complete. Nothing else can be added at the moment.");
        }
        UpdateSubmitButton();
    }

    public void OnSubmitPressed()
    {
        //Debug.Log($"Turn complete for {player}. Turn consists of\n {myPlayers.RespectivePlayerTurns[player-1].GetMovesetAsString()}");
        Debugger.instance.Push($"Turn complete for {player}. Turn consists of\n {myPlayers.RespectivePlayerTurns[player - 1].GetMovesetAsString()}");
    }

    private void UpdateSubmitButton()
    {
        if(myPlayers.RespectivePlayerTurns[player-1].IsComplete())
        {
            SubmitButton.interactable = true;
            Debugger.instance.Push("Turn can be completed");
        }
        else
        {
            SubmitButton.interactable = false;
            Debugger.instance.Push($"Turn can not be completed. Completed turns is {myPlayers.RespectivePlayerTurns[player - 1].GetDirectionsCount()}");
        }

        UpdateExecuteButton();
    }

    public void OnExecutePressed()
    {

        if (currentTurnI < 3)
        {

            GamestateManager.ResultType result = gameManager.TryMovePlayers(myPlayers, currentTurnI);

            if (result == GamestateManager.ResultType.stalemated)
            {
                foreach (PlayerMoveSet player in myPlayers.RespectivePlayerTurns)
                {
                    player.Reset();

                }
                Debugger.instance.Push("Bonk");
                currentTurnI = 0;
                //break;
            }
            else if (result == GamestateManager.ResultType.eliminatedOther || result == GamestateManager.ResultType.selfEliminated)
            {
                //reset round;
                Debugger.instance.Push("Player Eliminated");
                currentTurnI = 0;
                //break;
            }
            Debugger.instance.Push($"Turn {currentTurnI} completed succ cessfully");
        }
        currentTurnI++;
        UpdateExecuteButton();
        UpdateSubmitButton();
    }

    private void UpdateExecuteButton()
    {
        foreach (var item in myPlayers.RespectivePlayerTurns)
        {
            if (!item.IsComplete())
            {
                ExecuteTurnsButton.interactable = false;
                return;
            }
        }
        ExecuteTurnsButton.interactable = true;
    }

    
}
