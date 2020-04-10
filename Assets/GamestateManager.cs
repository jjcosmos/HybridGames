using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamestateManager : MonoBehaviour
{
    // Start is called before the first frame update
    public enum TileType {blank, point, player, invalid};
    public enum ResultType {moved, selfEliminated, eliminatedOther, collectedPoint, cached, stalemated}
    public int bounds = 5;
    int[,] Gameboard;
    readonly int MOVE = 0;
    readonly int STOP = 1;
    readonly int ATTACK = 2;
    readonly int STUN = 3;
    void Awake()
    {
        //0 is empty, 10 is player 1, 20 is player 2
        //1 is 1 point tile
        //2 is 2 point tile
        //x for x < 10 is and x point tile
        Gameboard = new int[5, 5] 
        { 
            { 0, 0, 20, 0, 0 },
            { 1, 0, 0,  0, 1 },
            { 2, 1, 0,  0, 2 },
            { 1, 0, 0,  0, 1 },
            { 0, 0, 10, 0, 0 } 
        };
    }

    public Vector2 GetPlayerLocation(int player)
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if(Gameboard[i,j] == player * 10)
                {
                    return new Vector2(i, j);
                }
            }
        }
        return new Vector2(-1,-1);
    }

    public TileType GetTypeAtIndex(int _i, int _j)
    {
        int value;
        try
        {
            value = Gameboard[_i, _j];
        }
        catch (System.Exception)
        {
            return TileType.invalid;
        }
        
        if(value == 0)
        {
            return TileType.blank;
        }
        else if(value < 10)
        {
            return TileType.point;
        }
        else
        {
            return TileType.player;
        }
    }
    public TileType GetTypeAtIndex(Vector2 position)
    {
        int value;
        try
        {
            value = Gameboard[(int)position.x, (int)position.y];
        }
        catch (System.Exception)
        {
            return TileType.invalid;
        }

        if (value == 0)
        {
            return TileType.blank;
        }
        else if (value < 10)
        {
            return TileType.point;
        }
        else
        {
            return TileType.player;
        }
    }

    public bool EditValueAt(int _i, int _j, int newValue)
    {
        try
        {
            Gameboard[_i, _j] = newValue;
            return true;
        }
        catch (System.Exception)
        {

            return false;
        }
        
    }

    public int[,] GetGameboard()
    {
        return Gameboard;
    }

    public string GetGameboardAsText()
    {
        string temp = "";
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                temp += Gameboard[i, j] + " ";
                if(Gameboard[i, j] < 10)
                {
                    temp += "  ";
                }
            }
            temp += "\n";
        }
        return temp;
    }

    public ResultType TryMovePlayer(Players myPlayers, int turn)
    {
        int playerIndex=0;
        int otherplayerIndex = 0;
        foreach (PlayerMoveSet playerTurn in myPlayers.RespectivePlayerTurns)
        {
            playerIndex++;
            int myDirection = (int)playerTurn.GetActionAt(turn).x;
            int myActionType = (int)playerTurn.GetActionAt(turn).y;
            Vector2 myMovetoPosition = playerTurn.GetPositionOfMove(this, turn, GetPlayerLocation(playerIndex));
            Vector2 myCurrentPosition = GetPlayerLocation(playerIndex);
            foreach (PlayerMoveSet others in myPlayers.RespectivePlayerTurns)
            {

                otherplayerIndex++;
                //my player attacking space moved (or stopped) into
                bool condit1 = 
                    (myMovetoPosition == others.GetPositionOfMove(this, turn, GetPlayerLocation(otherplayerIndex)) && 
                    (others.GetActionAt(turn).y == 0 || others.GetActionAt(turn).y == 1) && 
                    myActionType == 2);

                //my player attacking space attacked into
                bool condit2 = 
                    (myMovetoPosition == others.GetPositionOfMove(this, turn, GetPlayerLocation(otherplayerIndex)) && 
                    others.GetActionAt(turn).y == 2 && 
                    myActionType == 2);

                //both players attempt to occupy a point tile
                bool condit3 =
                    (myMovetoPosition == others.GetPositionOfMove(this, turn, GetPlayerLocation(otherplayerIndex)) &&
                    GetTypeAtIndex(myMovetoPosition) == TileType.point);

                //my player moving into a space moved into
                bool condit4 =
                    (myMovetoPosition == others.GetPositionOfMove(this, turn, GetPlayerLocation(otherplayerIndex)) &&
                    myActionType == 0 &&
                    others.GetActionAt(turn).y == 0);

                //my player attacking space another player is in WHILE the other player is moving into MY space 
                bool condit5 =
                    (myMovetoPosition == GetPlayerLocation(playerIndex) &&
                    others.GetPositionOfMove(this, turn, GetPlayerLocation(otherplayerIndex)) == myCurrentPosition &&
                    myActionType == ATTACK &&
                    others.GetActionAt(turn).y == MOVE);

                //my player attacking space another player is in WHILE the other player is ATTACKING into MY space 
                bool condit6 =
                    (myMovetoPosition == GetPlayerLocation(playerIndex) &&
                    others.GetPositionOfMove(this, turn, GetPlayerLocation(otherplayerIndex)) == myCurrentPosition &&
                    myActionType == ATTACK &&
                    others.GetActionAt(turn).y == ATTACK);

                //my player moving into a space another player is in while they are attacking my space
                bool condit7 =
                    (myMovetoPosition == GetPlayerLocation(playerIndex) &&
                    others.GetPositionOfMove(this, turn, GetPlayerLocation(otherplayerIndex)) == myCurrentPosition &&
                    myActionType == MOVE &&
                    others.GetActionAt(turn).y == ATTACK);

                //my player moving into a point square while not nulled (not eliminated or stalemated) -- Do close to last
                bool condit8 =
                    (GetTypeAtIndex(myMovetoPosition) == TileType.point &&
                    !(playerTurn.isTurnNulled || playerTurn.isEliminated));

                //my player moving into cache while not nulled
                bool condit9 =
                    ((myMovetoPosition) == playerTurn.cachePosition &&
                    !(playerTurn.isTurnNulled || playerTurn.isEliminated));

                //my player moving into an empty space
                bool condit10 =
                    ((GetTypeAtIndex(myMovetoPosition)) == TileType.blank;








                if (others == playerTurn)
                {
                    break;
                }
                else
                {
                    if(true)
                    {

                    }
                }
            }
        }

        

        return ResultType.moved; //PH
    }
}
