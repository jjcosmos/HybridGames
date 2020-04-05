using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveSet
{
    List<int> Directions;
    List<int> ActionTypes;
    bool hasUsedStop;

    readonly int MOVE = 0;
    readonly int STOP = 1;
    readonly int ATTACK = 2;
    readonly int STUN = 3;

    readonly Vector2 UP = new Vector2(0, 1);
    readonly Vector2 RIGHT = new Vector2(1, 0);
    readonly Vector2 DOWN = new Vector2(-1, 0);
    readonly Vector2 LEFT = new Vector2(-1, 0);

    public PlayerMoveSet()
    {
        Directions = new List<int>();
        ActionTypes = new List<int>();
        hasUsedStop = false;
    }

    //action types: 0 is move, 1 is stop, 2 is attack
    public bool AddMove(int direction, int actionType)
    {
        Debug.Log($"Adding a move {actionType} in the direction {direction}");
        if(Directions.Count >= 3)
        {
            Debug.LogWarning("Moves are full for this turn");
            return false;
        }


        if (actionType == ATTACK && ActionTypes.Count >= 2)
        {
            Debug.LogWarning("You cannot add an attack action with 1 action space left");
            return false;
        }
        else if (actionType == ATTACK && ActionTypes.Count < 2)
        {
            Directions.Add(direction);
            ActionTypes.Add(actionType);
            Directions.Add(0);
            ActionTypes.Add(STUN);
        }
        else if (actionType == STOP && !hasUsedStop)
        {
            Directions.Add(direction);
            ActionTypes.Add(actionType);
            hasUsedStop = true;
            return true;
        }
        else if (actionType == MOVE)
        {
            Directions.Add(direction);
            ActionTypes.Add(actionType);
            return true;
        }
        Debug.Log($"Bad choice :(  hasStopped is {hasUsedStop}");
        return false;
        
    }
    
    public Vector2 GetActionAt(int index)
    {
        return new Vector2(Directions[index], ActionTypes[index]);
    }

    public Vector2 GetPositionOfMove(GamestateManager board, int moveIndex, Vector2 currentPosition)
    {
        //   1
        // 4 0 2
        //   3

        int dir = Directions[moveIndex];
        switch (dir)
        {
            case 0:
                return currentPosition;
            case 1:
                if(board.GetTypeAtIndex(currentPosition + UP) != GamestateManager.TileType.invalid)
                {
                    return currentPosition + UP;
                }
                break;
            case 2:
                if (board.GetTypeAtIndex(currentPosition + RIGHT) != GamestateManager.TileType.invalid)
                {
                    return currentPosition + RIGHT;
                }
                break;
            case 3:
                if (board.GetTypeAtIndex(currentPosition + DOWN) != GamestateManager.TileType.invalid)
                {
                    return currentPosition + DOWN;
                }
                break;
            case 4:
                if (board.GetTypeAtIndex(currentPosition + LEFT) != GamestateManager.TileType.invalid)
                {
                    return currentPosition + LEFT;
                }
                break;
            default:
                break;
                
                
        }
        return new Vector2(-1, -1);
    }

    public bool IsComplete()
    {
        if(Directions.Count > 2)
        {
            return true;
        }
        return false;
    }

    public string GetMovesetAsString()
    {
        string stronk = "";
        for (int i = 0; i < 3; i++)
        {
            stronk += $"Action {ActionTypes[i]} in direction {Directions[i]} \n";
        }
        return stronk;
    }
}
