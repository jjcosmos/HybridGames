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

    public PlayerMoveSet()
    {
        Directions = new List<int>();
        ActionTypes = new List<int>();
    }

    //action types: 0 is move, 1 is stop, 2 is attack
    public bool AddMove(int direction, int actionType)
    {
        if(Directions.Count >= 3)
        {
            Debug.LogWarning("Moves are full for this turn");
            return false;
        }


        if(actionType==ATTACK && ActionTypes.Count >=2)
        {
            Debug.LogWarning("You cannot add an attack action with 1 action space left");
            return false;
        }
        else if(actionType == STOP && !hasUsedStop)
        {
            Directions.Add(direction);
            ActionTypes.Add(actionType);
            hasUsedStop = true;
            return true;
        }
        else if(actionType == MOVE)
        {
            Directions.Add(direction);
            ActionTypes.Add(actionType);
            return true;
        }
        return false;
        
    }
    
    public Vector2 GetActionAt(int index)
    {
        return new Vector2(Directions[index], ActionTypes[index]);
    }
}
