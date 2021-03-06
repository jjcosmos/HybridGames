﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System;
using UnityEngine.SceneManagement;
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
    private bool firstTime = true;
    private void Awake()
    {
        firstTime = true;
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
        if (!firstTime)
        {
            ButtonSfx.bSFX.PlayButtonSound();
            
        }
        else
        {
            firstTime = false;
        }
        
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

    public void OnLockTurnPressed()//should be on lock action
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
        ButtonSfx.bSFX.PlayButtonSound();
        if (currentTurnI < 3)
        {

            GamestateManager.ResultType result = gameManager.TryMovePlayers(myPlayers, currentTurnI);
            

            if (result == GamestateManager.ResultType.stalemated)
            {
                NotiText._out.PushText($"Turn Stalemated");
                ResetAll();
                //break;
            }
            else if (result == GamestateManager.ResultType.eliminatedOther || result == GamestateManager.ResultType.selfEliminated || result == GamestateManager.ResultType.endByCache)
            {
                //reset round;
                Debugger.instance.Push($"Game finished with result type {result}");
                ResetAll();
                switch (result)
                {
                    
                    case GamestateManager.ResultType.selfEliminated:
                        NotiText._out.PushText($"Round Over: Player 1 Eliminated");
                        break;
                    case GamestateManager.ResultType.eliminatedOther:
                        NotiText._out.PushText($"Round Over: Player 1 Eliminated");
                        break;
                    case GamestateManager.ResultType.endByCache:
                        NotiText._out.PushText($"Round Over: Last Cache");
                        break;
                    default:
                        break;
                }
                
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
            OnTurnExecute.Invoke();
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
        OnTurnExecute.Invoke();
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

    public void ProcessInput(string numString)
    {
        bool attackFlag = false;
        if(numString.Length != 7 && numString.Length !=5)
        {
            Debugger.instance.Push($"INPUT INVALID. 6 Chars needed. Given {numString}");
            return;
        }
        for (int i = 1; i < numString.Length; i+=2)
        {
            int turnAction = numString[i] - '0';
            int turnDirection = numString[i + 1] - '0';


            

            if(turnAction < 3 && turnDirection < 5)
            {
                dir = turnDirection;
                act = turnAction;
                OnLockTurnPressed();
            }
            else
            {
                Debugger.instance.Push($"INPUT INVALID AT TURN INDEX {i}. Resetting player {player}'s turn.");
                myPlayers.RespectivePlayerTurns[player - 1].Reset();
                //clear action
                return;
            }

            if (act == 2)
            {
                Debugger.instance.Push($"Flagging attack at index {i} of {numString}");
                attackFlag = true;
            }

        }

        if (numString.Length < 6 && !attackFlag)
        {
            Debugger.instance.Push($"INPUT INVALID FOR STUN. Resetting player {player}'s turn.");
            myPlayers.RespectivePlayerTurns[player - 1].Reset();
            return;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Asterisk))
        {
            int currentScene = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentScene);
        }
    }
}
