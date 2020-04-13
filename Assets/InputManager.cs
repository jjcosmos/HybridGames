﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
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
    public int currentTurnI;
    public UnityEvent OnTurnExecute;

    private void Awake()
    {
        OnTurnExecute = new UnityEvent();
    }
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

        bool isVaildType = (act == 1 ) || (dir != 0 && act != 1);
        bool canBeCompleted = (isVaildType && CheckChainValidity());
        if (!myPlayers.RespectivePlayerTurns[player-1].IsComplete() && canBeCompleted)
        {
            if(act != 1)
            {
                myPlayers.RespectivePlayerTurns[player-1].AddMove(dir, act);
            }
            else
            {
                myPlayers.RespectivePlayerTurns[player - 1].AddMove(0, act);
            }
            
        }
        else
        {
            Debugger.instance.Push("Turn is complete. Nothing else can be added at the moment.");
        }
        UpdateSubmitButton();
    }

    private bool CheckChainValidity()
    {
        PlayerMoveSet currPlayer = myPlayers.RespectivePlayerTurns[player - 1];
        Vector2 pos = gameManager.GetPlayerLocation(player);



        if (currPlayer.GetDirectionsCount() == 0)
        {
            pos = currPlayer.GetPositionOfMove(gameManager, dir, pos, true);
            Debugger.instance.Push($"Testing chain 1 position {pos} for validity...");
        }
        else if (currPlayer.GetDirectionsCount() == 1)
        {
            pos = currPlayer.GetPositionOfMove(gameManager, (int)currPlayer.GetActionAt(0).x, pos, true);
            Debugger.instance.Push($"Testing chain 1 position {pos} for validity...");
            pos = currPlayer.GetPositionOfMove(gameManager, dir, pos, true);
            Debugger.instance.Push($"Testing chain 2 position {pos} for validity...");
        }
        else if (currPlayer.GetDirectionsCount() == 2)
        {
            pos = currPlayer.GetPositionOfMove(gameManager, (int)currPlayer.GetActionAt(0).x, pos, true);
            Debugger.instance.Push($"Testing chain 1 position {pos} for validity...");
            pos = currPlayer.GetPositionOfMove(gameManager, (int)currPlayer.GetActionAt(1).x, pos, true);
            Debugger.instance.Push($"Testing chain 2 position {pos} for validity...");
            pos = currPlayer.GetPositionOfMove(gameManager, dir, pos, true);
            Debugger.instance.Push($"Testing chain 3 position {pos} for validity...");
        }

        bool isValid = gameManager.GetTypeAtIndex((int)pos.x, (int)pos.y) != GamestateManager.TileType.invalid;
        return isValid;
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
            OnTurnExecute.Invoke();

            if (result == GamestateManager.ResultType.stalemated)
            {
                ResetAll();
                //break;
            }
            else if (result == GamestateManager.ResultType.eliminatedOther || result == GamestateManager.ResultType.selfEliminated || result == GamestateManager.ResultType.endByCache)
            {
                //reset round;
                Debugger.instance.Push($"Game finished with result type {result}");
                ResetAll();
                //break;
            }
            else
            {
                Debugger.instance.Push($"Turn {currentTurnI} completed successfully");
                currentTurnI++;
            }
            

            if(currentTurnI >= 3)
            {
                ResetAll();
            }
        }
        else
        {
            ResetAll();

        }
        
        UpdateExecuteButton();
        UpdateSubmitButton();
    }

    private void ResetAll()
    {
        foreach (PlayerMoveSet player in myPlayers.RespectivePlayerTurns)
        {
            currentTurnI = 0;
            player.Reset();
        }
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
