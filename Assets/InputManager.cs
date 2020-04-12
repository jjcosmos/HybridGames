using System.Collections;
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
