using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerColorSwitch : MonoBehaviour
{
    public Image image;
    public Client client;
    public Sprite player1;
    public Sprite player2;

    private bool preformChange = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (preformChange)
        {  
            if (client.myPlayerID == 1)
            {
                image.sprite = player1;
                image.enabled = true;
                preformChange = false;
            }
            else if (client.myPlayerID == 2)
            {
                image.sprite = player2;
                image.enabled = true;
                preformChange = false;
            }
        }
    }

    public void Restart()
    {
        preformChange = true;
        image.enabled = false;
    }
}
