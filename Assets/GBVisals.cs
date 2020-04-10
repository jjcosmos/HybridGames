using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GBVisals : MonoBehaviour
{
    // Start is called before the first frame update
    TextMeshProUGUI board;
    [SerializeField] GamestateManager gameState;
    void Start()
    {
        board = GetComponent<TextMeshProUGUI>();
        board.text = gameState.GetGameboardAsText();
    }

    
}
