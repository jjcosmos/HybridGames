using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace database
{

    public class DatabaseInteractionDesktop : MonoBehaviour
    {
        private FirebaseDatabase database;
        private System.Random rand = new System.Random();
        private int requests = 0; //total game code requests

        public string gameID;
        public UnityEvent OnSessionCreated;
        [SerializeField] SessionIDUpdater updater;

        // Start is called before the first frame update
        async void Start()
        {
            
            database = FirebaseDatabase.DefaultInstance;
            gameID = await generateGameIDAsync();
            await database.GetReference(gameID).SetValueAsync(0);
            
        }

        private void Awake()
        {
            OnSessionCreated = new UnityEvent();
        }


        Dictionary<int, string> GetTurnFromDatabase()
        {
            Dictionary<int, string> move = new Dictionary<int, string>();
            move.Add(0, database.GetReference(gameID).Child("0").GetValueAsync().ToString());
            move.Add(1, database.GetReference(gameID).Child("1").GetValueAsync().ToString());
            return move;            
        }

        //generates a game id
        private async Task<string> generateGameIDAsync()
        {
            if(requests > 10000)
            {
                throw new System.Exception("Maximum number of active games has been reached");
            }

            string newID = "";

            //generates the gameID
            for (int i = 0; i < 4; i++)
            {
                if (rand.Next(0, 2) == 0)
                {
                    double num = rand.NextDouble();
                    int shift = System.Convert.ToInt32(System.Math.Floor(25 * num));
                    char letter = System.Convert.ToChar(shift + 65);
                    newID += letter;
                }
                else
                {
                    newID += rand.Next(1, 9);
                }
            }

            //checks if game already exists
            if (await GameExists(newID))
            {
                Debug.Log("Generated game code: " + newID + " exists, generating new code.");
                requests++;
                return await generateGameIDAsync();
            }
            Debug.Log("Game code set to: " + newID);
            //OnSessionCreated.Invoke();
            updater.OnInitSession(newID);
            return newID;
        }

        private async Task<bool> GameExists(string id)
        {
            var dataSnapshot = await database.RootReference/*GetReference(gameID)*/.GetValueAsync();
            return dataSnapshot.Equals(id);
        }

        private void OnDestroy()
        {
            //database.GetReference(gameID).RemoveValueAsync();
        }
    }
}
