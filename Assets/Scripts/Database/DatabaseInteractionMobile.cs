﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Extensions;
using Firebase;
using Firebase.Database;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace database
{
    public class DatabaseInteractionMobile : MonoBehaviour
    {
        private FirebaseDatabase database;
        private DatabasePlayerMoveInfo player;
        private string gameID;
        public int playerID = 0;
        public UnityEvent OnPlayerIDAssigned;
        [SerializeField] PlayerTextUpdater txtUpdater;

        // Start is called before the first frame update
        void Start()
        {
            //set database to default instance determined by firebaseinit
            database = FirebaseDatabase.DefaultInstance;
            player = new DatabasePlayerMoveInfo();
            Debug.Log("Mobile connected to: " + database.RootReference.ToString());
            OnPlayerIDAssigned.AddListener(txtUpdater.UpdateText);
        }



        //Sets the gameID of the mobile user generated by the Desktop app
        public async Task<bool> EnterGameAsync(string newGameID)
        {
            gameID = newGameID;
            if (await PlayerExists())
            {
                if (await LobbyFull())
                {
                    Debug.Log("Error: Lobby Full");
                    return false;
                }
                playerID = 1;
            }
            await database.GetReference(gameID).Child(playerID.ToString()).SetRawJsonValueAsync(JsonUtility.ToJson(player));
            Debug.Log("Player " + playerID + "Joined session: " + gameID);
            OnPlayerIDAssigned.Invoke();
            return true;
        }

        //Sends a turn to the database for the controlled player (must have a game id set)
        public void SendTurnToDatabase(string move)
        {
            //prevents players from sending moves when not in a game
            if (gameID == string.Empty)
                throw new System.Exception("Cannot send a turn without being connected to a game.");

            Debug.Log("Move: " + move + " recieved from player: " + playerID);
            player.move = move;
            database.GetReference(gameID).Child(playerID.ToString()).SetRawJsonValueAsync(JsonUtility.ToJson(player));
        }

        private async Task<bool> PlayerExists()
        {
            var dataSnapshot = await database.RootReference.Child(gameID).Child("0").GetValueAsync();
            Debug.Log(dataSnapshot);
            return dataSnapshot.Exists;
        }

        private async Task<bool> LobbyFull()
        {
            var dataSnapshot = await database.RootReference.Child(gameID).Child("0").GetValueAsync();
            var dataSnapshot2 = await database.RootReference.Child(gameID).Child("1").GetValueAsync();
            Debug.Log(dataSnapshot);
            return dataSnapshot.Exists && dataSnapshot2.Exists;
        }

        private void OnDestroy()
        {
            database.GetReference(gameID).Child(playerID.ToString()).RemoveValueAsync();
        }
    }
}