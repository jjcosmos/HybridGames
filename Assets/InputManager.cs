using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InputManager : MonoBehaviour
{
    // Temporary, server will replace this code
    [SerializeField] Players myPlayers;
    bool allPlayersSubmitted;

    [SerializeField] Slider PlayerSlider;
    [SerializeField] Slider DirectionInput;
    [SerializeField] Slider ActionTypeInput;
    [SerializeField] Button LockMoveButton;
    [SerializeField] Button SubmitButton;


    int player;
    int dir;
    int act;
    bool editable = false;
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
        UpdateSubmitButton();
    }

    public void OnDirectionSliderValueChanged()
    {
        dir = (int)DirectionInput.value;
    }

    public void OnActionSliderValueChanged()
    {
        act = (int)ActionTypeInput.value;
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
        Debug.Log($"Turn complete for {player}. Turn consists of\n {myPlayers.RespectivePlayerTurns[player-1].GetMovesetAsString()}");
        Debugger.instance.Push($"Turn complete for {player}. Turn consists of\n {myPlayers.RespectivePlayerTurns[player - 1].GetMovesetAsString()}");
    }

    private void UpdateSubmitButton()
    {
        if(myPlayers.RespectivePlayerTurns[player-1].IsComplete())
        {
            SubmitButton.interactable = true;
        }
        else
        {
            SubmitButton.interactable = false;
        }
    }

    
}
