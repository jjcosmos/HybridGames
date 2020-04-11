using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamestateManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] InputManager inputManager;
    public enum TileType { blank, point, player, invalid };
    public enum ResultType { moved, selfEliminated, eliminatedOther, collectedPoint, cached, stalemated, endByCache }
    public int bounds = 5;
    int[,] Gameboard;
    int[,] GameboardCopy;
    readonly int MOVE = 0;
    readonly int STOP = 1;
    readonly int ATTACK = 2;
    readonly int STUN = 3;
    readonly int WIN_BONUS = 3;

    public int player1score;
    public int player1inventory;
    public int player1cache;

    public int player2score;
    public int player2inventory;
    public int player2cache;
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

        GameboardCopy = new int[5, 5]{
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
                if (Gameboard[i, j] == (player * 10))
                {
                    //Debugger.instance.Push($"Found player {player} at {i},{j}");
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
            Debugger.instance.Push($"Returing invalid position at {position}");
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

                if (Gameboard[i, j] == 1 || Gameboard[i, j] == 2)
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
        Debugger.instance.Push($"Player1 starting at {p1CurrentPosition} and moving to {p1MovetoPosition}");

        Vector2 p2CurrentPosition = GetPlayerLocation(otherplayerIndex);
        Vector2 p2MoveToPosition = player2.GetPositionOfMove(this, turn, p2CurrentPosition);
        Debugger.instance.Push($"Player2 starting at {p2CurrentPosition} and moving to {p2MoveToPosition}");


        //my player attacking space moved (or stopped) into
        bool condit1 =
            (p1MovetoPosition == p2MoveToPosition &&
            (player2.GetActionAt(turn).y == MOVE || player2.GetActionAt(turn).y == STOP) &&
            p1ActionType == ATTACK);

        //my player moving or stopping with other attacking into
        bool condit1_b =
            (p1MovetoPosition == p2MoveToPosition &&
            (player2.GetActionAt(turn).y == ATTACK) &&
            (p1ActionType == MOVE || p1ActionType == STOP));

        //my player attacking space attacked into
        bool condit2 =
            (p1MovetoPosition == p2MoveToPosition &&
            player2.GetActionAt(turn).y == 2 &&
            p1ActionType == 2);

        //both players attempt to occupy the same point tile
        bool condit3 =
            (p1MovetoPosition == p2MoveToPosition &&
            GetTypeAtIndex(p1MovetoPosition) == TileType.point);

        //my player moving into a space moved into
        bool condit4 =
            (p1MovetoPosition == p2MoveToPosition &&
            p1ActionType == 0 &&
            player2.GetActionAt(turn).y == 0);

        //my player attacking space another player is in WHILE the other player is moving into MY space 
        bool condit5 =
            (p1MovetoPosition == GetPlayerLocation(otherplayerIndex) &&
            p2MoveToPosition == p1CurrentPosition &&
            p1ActionType == ATTACK &&
            player2.GetActionAt(turn).y == MOVE);

        //b version of one above
        bool condit5_b =
            (p2MoveToPosition == GetPlayerLocation(playerIndex) &&
            p1MovetoPosition == p2CurrentPosition &&
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

        //my player moving into a point square while not nulled (not eliminated or stalemated) -- Do close to last OBSOLETE
        bool condit8 =
            (GetTypeAtIndex(p1MovetoPosition) == TileType.point &&
            !(player1.isTurnNulled || player1.isEliminated));

        //my player moving into cache while not nulled OBSOLETE
        bool condit9 =
            ((p1MovetoPosition) == player1.cachePosition &&
            !(player1.isTurnNulled || player1.isEliminated));

        //my player moving into an empty uncontested space
        bool condit10 =
            (((GetTypeAtIndex(p1MovetoPosition)) == TileType.blank || (GetTypeAtIndex(p1MovetoPosition)) == TileType.point) &&
            player2.GetPositionOfMove(this, turn, GetPlayerLocation(otherplayerIndex)) != p1MovetoPosition);

        //my player moving into an empty uncontested space
        bool condit10_b =
            (((GetTypeAtIndex(p2MoveToPosition)) == TileType.blank || (GetTypeAtIndex(p2MoveToPosition)) == TileType.point) &&
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
            Debugger.instance.Push("condit1");
            ShiftCacheAndReset();
            return ResultType.eliminatedOther;
        }
        else if (condit1_b)
        {
            player1.isTurnNulled = true;
            player1.isEliminated = true;
            // Add points to other player
            // Eliminate  player and ends round
            player2cache += WIN_BONUS;
            Debugger.instance.Push("condit1_b");
            ShiftCacheAndReset();
            return ResultType.selfEliminated;
        }
        else if (condit2 && condit3)// both players attack a space with a point on it
        {
            EditValueAt(p1MovetoPosition, 0);
            player1.isTurnNulled = true;
            player2.isTurnNulled = true;
            Debugger.instance.Push("condit2 and 3");
            return ResultType.stalemated;
        }
        else if (condit2 && !condit3)
        {
            player1.isTurnNulled = true;
            player2.isTurnNulled = true;
            Debugger.instance.Push("condit2 and not 3");
            return ResultType.stalemated;
        }
        else if (condit4 && condit3)
        {
            EditValueAt(p1MovetoPosition, 0);
            player1.isTurnNulled = true;
            player2.isTurnNulled = true;
            Debugger.instance.Push("condit 4 and 3");
            return ResultType.stalemated;
        }
        else if (condit4 && !condit3)
        {
            player1.isTurnNulled = true;
            player2.isTurnNulled = true;
            Debugger.instance.Push($"condit 4 and not 3. Bonk'd at {p1MovetoPosition} {p2MoveToPosition}");
            return ResultType.stalemated;
        }
        else if (condit5)
        {
            player2.isTurnNulled = true;
            player2.isEliminated = true;
            // Add points to current player
            // Eliminate other player and end round
            player1cache += WIN_BONUS;
            Debugger.instance.Push("condit5");
            ShiftCacheAndReset();
            return ResultType.eliminatedOther;
        }
        else if (condit5_b)
        {
            player1.isTurnNulled = true;
            player1.isEliminated = true;
            // Add points to other player
            // Eliminate my player and end round
            player2cache += WIN_BONUS;
            Debugger.instance.Push("condit5_b");
            ShiftCacheAndReset();
            return ResultType.selfEliminated;
        }
        else if (condit6)
        {
            player1.isTurnNulled = true;
            player2.isTurnNulled = true;
            Debugger.instance.Push("Condit6");
            return ResultType.stalemated;
        }

        //START INDEP EVENTS
        ResultType returnType = ResultType.moved;
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
                    returnType = ResultType.endByCache;
                }
            }
            int playerValue = Gameboard[(int)p1CurrentPosition.x, (int)p1CurrentPosition.y];
            EditValueAt((int)p1MovetoPosition.x, (int)p1MovetoPosition.y, playerValue);
            EditValueAt((int)p1CurrentPosition.x, (int)p1CurrentPosition.y, 0);
        }
        if (condit10_b)
        {
            if (GetTypeAtIndex(p2MoveToPosition) == TileType.point)
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
                    returnType = ResultType.endByCache;
                }
            }
            int playerValue = Gameboard[(int)p2CurrentPosition.x, (int)p2CurrentPosition.y];
            EditValueAt((int)p2MoveToPosition.x, (int)p2MoveToPosition.y, playerValue);
            EditValueAt((int)p2CurrentPosition.x, (int)p2CurrentPosition.y, 0);
        }
        return returnType;
        //END INDEP EVENTS

    }

    private void ShiftCacheAndReset()
    {
        Debugger.instance.Push("Shifting cache and resetting");
        player1score += player1cache;
        player2score += player2cache;
        player1cache = 0;
        player2cache = 0;
        player1inventory = 0;
        player2inventory = 0;
        //Gameboard = GameboardCopy;
        Gameboard = GameboardCopy.Clone() as int[,];
    }
}
