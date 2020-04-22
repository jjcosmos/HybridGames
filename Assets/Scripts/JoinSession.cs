using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using database;
using System.Threading.Tasks;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class JoinSession : MonoBehaviour
{
    [SerializeField] TMP_InputField input;
    [SerializeField] DatabaseInteractionMobile dbInteractionM;
    [SerializeField] CanvasManager cManager;
    // Start is called before the first frame update
    public UnityEvent onSuccess;
    private void Awake()
    {
        onSuccess = new UnityEvent();
        
    }
    // Update is called once per frame
    public async void TryJoin()
    {
        
        bool canJoin = await dbInteractionM.EnterGameAsync(input.text);
        if (canJoin)
        {
            Debug.Log("Joined Session");
            onSuccess.Invoke();
        }
        else
        {
            Debug.Log("Failed to Join Session");
        }
    }

   
    
}
