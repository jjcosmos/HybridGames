using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerMoveSet
{
    [SerializeField] List<int> Directions;
    [SerializeField] List<int> ActionTypes;
    bool hasUsedStop;
    

    readonly int MOVE = 0;
    readonly int STOP = 1;
    readonly int ATTACK = 2;
    readonly int STUN = 3;

    //This looks weird, but is correct
    readonly Vector2 UP = new Vector2(-1, 0);
    readonly Vector2 RIGHT = new Vector2(0, 1);
    readonly Vector2 DOWN = new Vector2(1, 0);
    readonly Vector2 LEFT = new Vector2(0, -1);

    public bool isTurnNulled;
    public bool isEliminated;

    public Vector2 currentPosition;
    public Vector2 cachePosition;

    public PlayerMoveSet()
    {
        Directions = new List<int>();
        ActionTypes = new List<int>();
        hasUsedStop = false;
    }

    public Vector2 GetOffset(int direction)
    {
        switch (direction)
        {
            case 0:
                return currentPosition;
            case 1:
                return UP;
            case 2:
                return RIGHT;
            case 3:
                return DOWN;
            case 4:
                return LEFT;
            default:
                return Vector2.zero;
        }
    }
    public void Reset()
    {
        Directions.Clear();
        ActionTypes.Clear();
        hasUsedStop = false;
        isTurnNulled = false;
        isEliminated = false;
    }

    //action types: 0 is move, 1 is stop, 2 is attack
    public bool AddMove(int direction, int actionType)
    {
        //Debug.Log($"Try adding a move {actionType} in the direction {direction}");
        //Debugger.instance.Push($"Try adding a move {actionType} in the direction {direction}");
        if(Directions.Count >= 3)
        {
            Debugger.instance.Push("Moves are full for this turn");
            return false;
        }


        if (actionType == ATTACK && ActionTypes.Count >= 2)
        {
            Debugger.instance.Push("You cannot add an attack action with 1 action space left");
            return false;
        }
        else if (actionType == ATTACK && ActionTypes.Count < 2)
        {
            Directions.Add(direction);
            ActionTypes.Add(actionType);
            Directions.Add(0);
            ActionTypes.Add(STUN);
            Debugger.instance.Push($"Attack added in direction {direction}");
            return true;
        }
        else if (actionType == STOP && !hasUsedStop)
        {
            Directions.Add(direction);
            ActionTypes.Add(actionType);
            hasUsedStop = true;
            Debugger.instance.Push($"Stop added in direction {direction}");
            return true;
        }
        else if (actionType == MOVE)
        {
            Directions.Add(direction);
            ActionTypes.Add(actionType);
            Debugger.instance.Push($"Move added in direction {direction}");
            return true;
        }
        //Debug.Log($"Bad choice :(  hasStopped is {hasUsedStop}");
        return false;
        
    }
    
    public Vector2 GetActionAt(int index)
    {
        //if(Directions.Exists[index])
        if(Directions.Count -1 < index)
        {
            string combindedString = string.Join(",", ActionTypes);
            Debug.LogError($"Trying to access an invalid index {index} of {combindedString}");
            throw new System.Exception($"Trying to access an invalid index {index} of {combindedString}");
        }
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
                    Debugger.instance.Push($"Moving {board.GetGameboard()[(int)currentPosition.x, (int)currentPosition.y]} in direction UP to {currentPosition + UP}");
                    return currentPosition + UP;
                }
                break;
            case 2:
                if (board.GetTypeAtIndex(currentPosition + RIGHT) != GamestateManager.TileType.invalid)
                {
                    Debugger.instance.Push($"Moving {board.GetGameboard()[(int)currentPosition.x, (int)currentPosition.y]} in direction RIGHT to {currentPosition + RIGHT}");
                    return currentPosition + RIGHT;
                }
                break;
            case 3:
                if (board.GetTypeAtIndex(currentPosition + DOWN) != GamestateManager.TileType.invalid)
                {
                    Debugger.instance.Push($"Moving {board.GetGameboard()[(int)currentPosition.x, (int)currentPosition.y]} in direction DOWN to {currentPosition + DOWN}");
                    return currentPosition + DOWN;
                }
                break;
            case 4:
                if (board.GetTypeAtIndex(currentPosition + LEFT) != GamestateManager.TileType.invalid)
                {
                    Debugger.instance.Push($"Moving {board.GetGameboard()[(int)currentPosition.x, (int)currentPosition.y]} in direction LEFT to {currentPosition + LEFT}");
                    return currentPosition + LEFT;
                }
                break;
            default:
                break;
                
                
        }
        return new Vector2(-1, -1);
    }

    public Vector2 GetPositionOfMove(GamestateManager board, int direction, Vector2 currentPosition, bool onion)
    {
        //   1
        // 4 0 2
        //   3

        int dir = direction;
        switch (dir)
        {
            case 0:
                return currentPosition;
            case 1:
                if (board.GetTypeAtIndex(currentPosition + UP) != GamestateManager.TileType.invalid)
                {
                    Debugger.instance.Push($"Moving {board.GetGameboard()[(int)currentPosition.x, (int)currentPosition.y]} in direction UP to {currentPosition + UP}");
                    return currentPosition + UP;
                }
                break;
            case 2:
                if (board.GetTypeAtIndex(currentPosition + RIGHT) != GamestateManager.TileType.invalid)
                {
                    Debugger.instance.Push($"Moving {board.GetGameboard()[(int)currentPosition.x, (int)currentPosition.y]} in direction RIGHT to {currentPosition + RIGHT}");
                    return currentPosition + RIGHT;
                }
                break;
            case 3:
                if (board.GetTypeAtIndex(currentPosition + DOWN) != GamestateManager.TileType.invalid)
                {
                    Debugger.instance.Push($"Moving {board.GetGameboard()[(int)currentPosition.x, (int)currentPosition.y]} in direction DOWN to {currentPosition + DOWN}");
                    return currentPosition + DOWN;
                }
                break;
            case 4:
                if (board.GetTypeAtIndex(currentPosition + LEFT) != GamestateManager.TileType.invalid)
                {
                    Debugger.instance.Push($"Moving {board.GetGameboard()[(int)currentPosition.x, (int)currentPosition.y]} in direction LEFT to {currentPosition + LEFT}");
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

    public int GetDirectionsCount()
    {
        return Directions.Count;
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
