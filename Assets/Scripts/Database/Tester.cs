using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using database;

public class Tester : MonoBehaviour
{
    public string gameID;
    public DatabaseInteractionMobile mobile;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    async void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            await mobile.EnterGameAsync(gameID);
        }

        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            mobile.SendTurnToDatabase("example");
        }
    }
    
    
}
