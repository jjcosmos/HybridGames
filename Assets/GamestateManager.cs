﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamestateManager : MonoBehaviour
{
    // Start is called before the first frame update
    public enum TileType { blank, point, player, invalid };
    public enum ResultType { moved, selfEliminated, eliminatedOther, collectedPoint, cached, stalemated }
    public int bounds = 5;
    int[,] Gameboard;
    readonly int MOVE = 0;
    readonly int STOP = 1;
    readonly int ATTACK = 2;
    readonly int STUN = 3;
    readonly int WIN_BONUS = 3;

    int player1score;
    int player1inventory;
    int player1cache;

    int player2score;
    int player2inventory;
    int player2cache;
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
                if (Gameboard[i, j] == player * 10)
                {
                    return new Vector2(i, j);
                }
            }
        }
        return new Vector2(-1, -1);
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

    public bool EditValueAt(Vector2 position, int newValue)
    {
        int _i = (int)position.x;
        int _j = (int)position.y;
        try
        {
            Gameboard[_i, _j] = newValue;
            return true;
        }
        catch (System.Exception)
        {
            Debugger.instance.Push($"Failed to edit value at {position}");
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
                if (Gameboard[i, j] < 10)
                {
                    temp += "  ";
                }
            }
            temp += "\n";
        }
        return temp;
    }

    private bool IsBoardClearable()
    {
        bool temp = true;
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                
                if (Gameboard[i, j] == 1 || Gameboard[i,j] == 2)
                {
                    temp = false;
                    return temp;
                }
            }
            
        }
        return temp;
    }

    public ResultType TryMovePlayers(Players myPlayers, int turn)
    {
        int playerIndex = 1;
        int otherplayerIndex = 2;
        PlayerMoveSet player1 = myPlayers.RespectivePlayerTurns[0];
        PlayerMoveSet player2 = myPlayers.RespectivePlayerTurns[1];

        
        int myDirection = (int)player1.GetActionAt(turn).x;
        int p1ActionType = (int)player1.GetActionAt(turn).y;
        Vector2 p1MovetoPosition = player1.GetPositionOfMove(this, turn, GetPlayerLocation(playerIndex));
        Vector2 p1CurrentPosition = GetPlayerLocation(playerIndex);

        Vector2 p2CurrentPosition = GetPlayerLocation(otherplayerIndex);
        Vector2 p2MoveToPosition = player2.GetPositionOfMove(this, turn, p2CurrentPosition);

        
        //my player attacking space moved (or stopped) into
        bool condit1 =
            (p1MovetoPosition == player2.GetPositionOfMove(this, turn, GetPlayerLocation(otherplayerIndex)) &&
            (player2.GetActionAt(turn).y == MOVE || player2.GetActionAt(turn).y == STOP) &&
            p1ActionType == ATTACK);

        //my player moving or stopping with other attacking into
        bool condit1_b =
            (p1MovetoPosition == player2.GetPositionOfMove(this, turn, GetPlayerLocation(otherplayerIndex)) &&
            (player2.GetActionAt(turn).y == ATTACK) &&
            (p1ActionType == MOVE ||p1ActionType== STOP));

        //my player attacking space attacked into
        bool condit2 =
            (p1MovetoPosition == player2.GetPositionOfMove(this, turn, GetPlayerLocation(otherplayerIndex)) &&
            player2.GetActionAt(turn).y == 2 &&
            p1ActionType == 2);

        //both players attempt to occupy a point tile
        bool condit3 =
            (p1MovetoPosition == player2.GetPositionOfMove(this, turn, GetPlayerLocation(otherplayerIndex)) &&
            GetTypeAtIndex(p1MovetoPosition) == TileType.point);

        //my player moving into a space moved into
        bool condit4 =
            (p1MovetoPosition == player2.GetPositionOfMove(this, turn, GetPlayerLocation(otherplayerIndex)) &&
            p1ActionType == 0 &&
            player2.GetActionAt(turn).y == 0);

        //my player attacking space another player is in WHILE the other player is moving into MY space 
        bool condit5 =
            (p1MovetoPosition == GetPlayerLocation(playerIndex) &&
            player2.GetPositionOfMove(this, turn, GetPlayerLocation(otherplayerIndex)) == p1CurrentPosition &&
            p1ActionType == ATTACK &&
            player2.GetActionAt(turn).y == MOVE);

        //b version of one above
        bool condit5_b =
            (p1MovetoPosition == GetPlayerLocation(playerIndex) &&
            player2.GetPositionOfMove(this, turn, GetPlayerLocation(otherplayerIndex)) == p1CurrentPosition &&
            p1ActionType == MOVE &&
            player2.GetActionAt(turn).y == ATTACK);

        //my player attacking space another player is in WHILE the other player is ATTACKING into MY space 
        bool condit6 =
            (p1MovetoPosition == GetPlayerLocation(playerIndex) &&
            player2.GetPositionOfMove(this, turn, GetPlayerLocation(otherplayerIndex)) == p1CurrentPosition &&
            p1ActionType == ATTACK &&
            player2.GetActionAt(turn).y == ATTACK);

        //my player moving into a space another player is in while they are attacking my space OBSOLETE
        bool condit7 =
            (p1MovetoPosition == GetPlayerLocation(playerIndex) &&
            player2.GetPositionOfMove(this, turn, GetPlayerLocation(otherplayerIndex)) == p1CurrentPosition &&
            p1ActionType == MOVE &&
            player2.GetActionAt(turn).y == ATTACK);

        //my player moving into a point square while not nulled (not eliminated or stalemated) -- Do close to last
        bool condit8 =
            (GetTypeAtIndex(p1MovetoPosition) == TileType.point &&
            !(player1.isTurnNulled || player1.isEliminated));

        //my player moving into cache while not nulled
        bool condit9 =
            ((p1MovetoPosition) == player1.cachePosition &&
            !(player1.isTurnNulled || player1.isEliminated));

        //my player moving into an empty uncontested space
        bool condit10 =
            ((GetTypeAtIndex(p1MovetoPosition)) == TileType.blank &&
            player2.GetPositionOfMove(this, turn, GetPlayerLocation(otherplayerIndex)) != p1MovetoPosition);

        //my player moving into an empty uncontested space
        bool condit10_b =
            ((GetTypeAtIndex(p2MoveToPosition)) == TileType.blank &&
            player2.GetPositionOfMove(this, turn, GetPlayerLocation(otherplayerIndex)) != p1MovetoPosition);

        bool player1Cache = (p1MovetoPosition == player1.cachePosition);
        bool player2Cache = (p2MoveToPosition == player2.cachePosition);

        

        if (condit1)
        {
            player2.isTurnNulled = true;
            player2.isEliminated = true;
            // Add points to current player
            // Eliminate other player and end round
            player1cache += WIN_BONUS;
            return ResultType.eliminatedOther;
        }
        else if (condit1_b)
        {
            player1.isTurnNulled = true;
            player1.isEliminated = true;
            // Add points to other player
            // Eliminate  player and ends round
            player2cache += WIN_BONUS;
            return ResultType.selfEliminated;
        }
        else if (condit2 && condit3)// both players attack a space with a point on it
        {
            EditValueAt(p1MovetoPosition, 0);
            player1.isTurnNulled = true;
            player2.isTurnNulled = true;
            return ResultType.stalemated;
        }
        else if (condit2 && !condit3)
        {
            player1.isTurnNulled = true;
            player2.isTurnNulled = true;
            return ResultType.stalemated;
        }
        else if (condit4 && condit3)
        {
            EditValueAt(p1MovetoPosition, 0);
            player1.isTurnNulled = true;
            player2.isTurnNulled = true;
            return ResultType.stalemated;
        }
        else if (condit4 && !condit3)
        {
            player1.isTurnNulled = true;
            player2.isTurnNulled = true;
            return ResultType.stalemated;
        }
        else if (condit5)
        {
            player2.isTurnNulled = true;
            player2.isEliminated = true;
            // Add points to current player
            // Eliminate other player and end round
            player1cache += WIN_BONUS;
            return ResultType.eliminatedOther;
        }
        else if (condit5_b)
        {
            player1.isTurnNulled = true;
            player1.isEliminated = true;
            // Add points to other player
            // Eliminate my player and end round
            player2cache += WIN_BONUS;
            return ResultType.selfEliminated;
        }
        else if (condit6)
        {
            player1.isTurnNulled = true;
            player2.isTurnNulled = true;
            return ResultType.stalemated;
        }

        //START INDEP EVENTS
        if (condit10)
        {
            if (GetTypeAtIndex(p1MovetoPosition) == TileType.point)
            {
                //add value of movetoposition to current Player's points
                player1inventory += Gameboard[(int)p1MovetoPosition.x, (int)p1MovetoPosition.y];
            }
            else if (player1Cache)
            {
                //player 1 caches points
                player1cache += player1inventory;
                player1inventory = 0;

                if (IsBoardClearable())
                {
                    //end round and count cache
                }
            }
            int playerValue = Gameboard[(int)p1CurrentPosition.x, (int)p1CurrentPosition.y];
            EditValueAt((int)p1MovetoPosition.x, (int)p1MovetoPosition.y, playerValue);
            EditValueAt((int)p1CurrentPosition.x, (int)p1CurrentPosition.y, 0);
        }
        if (condit10_b)
        {
            if (GetTypeAtIndex(p1MovetoPosition) == TileType.point)
            {
                //add value of movetoposition to current Player's points
                player2inventory += Gameboard[(int)p2MoveToPosition.x, (int)p2MoveToPosition.y];
            }
            else if (player2Cache)
            {
                //player 2 caches points
                player2cache += player2inventory;
                player2inventory = 0;

                if (IsBoardClearable())
                {
                    //end round and count cache
                }
            }
            int playerValue = Gameboard[(int)p2CurrentPosition.x, (int)p2CurrentPosition.y];
            EditValueAt((int)p2MoveToPosition.x, (int)p2MoveToPosition.y, playerValue);
            EditValueAt((int)p2CurrentPosition.x, (int)p2CurrentPosition.y, 0);
        }
        return ResultType.moved;
        //END INDEP EVENTS






        return ResultType.moved; //PH
    }
}
