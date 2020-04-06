using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamestateManager : MonoBehaviour
{
    // Start is called before the first frame update
    public enum TileType {blank, point, player, invalid};
    public int bounds = 5;
    int[,] Gameboard;
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
}
