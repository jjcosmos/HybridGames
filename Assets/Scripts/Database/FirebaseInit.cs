using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Firebase;
using Firebase.Extensions;

namespace database
{

    public class FirebaseInit : MonoBehaviour
    {
        public UnityEvent OnFirebaseInit = new UnityEvent();

        // Start is called before the first frame update
        void Start()
        {
            //initializes connection to databse
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                if (task.Exception != null)
                {
                    Debug.LogError($"Failed to initialize connection to firebase: {task.Exception}");
                    return;
                }
                Debug.Log("Connected to " + task.ToString());
                OnFirebaseInit.Invoke(); //if successful invokes an event
        });
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
