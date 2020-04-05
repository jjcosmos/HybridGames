using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Players : MonoBehaviour
{
    // Start is called before the first frame update
    int[] playerIPs;
    int numPlayers;
    public List<PlayerMoveSet> RespectivePlayerTurns;

    private void Awake()
    {
        RespectivePlayerTurns = new List<PlayerMoveSet>();
        numPlayers = 2;
        for (int i = 0; i < numPlayers; i++)
        {
            RespectivePlayerTurns.Add(new PlayerMoveSet());
        }
    }

    
}
