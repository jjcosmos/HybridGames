using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGraphicsManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] GridManager myGrid;
    [SerializeField] GamestateManager myGamestate;
    [SerializeField] InputManager myInputManager;
    [SerializeField] Transform pointParent;

    [Header("Pieces")]
    [SerializeField] Transform player1;
    [SerializeField] Transform player2;
    [SerializeField] GameObject point1Prefab; 
    [SerializeField] GameObject point2Prefab; 
    void Start()
    {
        myInputManager.OnTurnExecute.AddListener(OnNewTurn);
        Vector2 p1LocOnArray = myGamestate.GetPlayerLocation(1);
        Vector2 p2LocOnArray = myGamestate.GetPlayerLocation(2);
        player1.position = myGrid.GetGridSpaceAt((int)p1LocOnArray.x, (int)p1LocOnArray.y).transform.position;
        player2.position = myGrid.GetGridSpaceAt((int)p2LocOnArray.x, (int)p2LocOnArray.y).transform.position;
        PlacePointTiles();
    }

    private void PlacePointTiles()
    {
        int[,] myGameboard = myGamestate.GetGameboard();
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if(myGameboard[i,j] == 1)
                {
                    Vector3 pointPos = myGrid.GetGridSpaceAt(i, j).transform.position;
                    Instantiate(point1Prefab, pointPos, Quaternion.identity, pointParent);
                }
                if (myGameboard[i, j] == 2)
                {
                    Vector3 pointPos = myGrid.GetGridSpaceAt(i, j).transform.position;
                    Instantiate(point2Prefab, pointPos, Quaternion.identity, pointParent);
                }

            }
        }   
    }

    private void ClearPointTiles()
    {
        foreach (Transform child in pointParent)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    private void TeleportPlayers()
    {
        Vector2 p1LocOnArray = myGamestate.GetPlayerLocation(1);
        Vector2 p2LocOnArray = myGamestate.GetPlayerLocation(2);
        player1.position = myGrid.GetGridSpaceAt((int)p1LocOnArray.x, (int)p1LocOnArray.y).transform.position;
        player2.position = myGrid.GetGridSpaceAt((int)p2LocOnArray.x, (int)p2LocOnArray.y).transform.position;
    }

    public void OnNewTurn()
    {
        ClearPointTiles();
        PlacePointTiles();
        TeleportPlayers();
    }
}
