using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    private TextMeshProUGUI scoreText;
    [SerializeField] GamestateManager gameState;

    [Header("Player 1")]
    [SerializeField] TextMeshProUGUI player1Inventory;
    [SerializeField] TextMeshProUGUI player1Cache;
    [SerializeField] TextMeshProUGUI player1Score;

    [Header("Player 2")]
    [SerializeField] TextMeshProUGUI player2Inventory;
    [SerializeField] TextMeshProUGUI player2Cache;
    [SerializeField] TextMeshProUGUI player2Score;
    // Start is called before the first frame update
    void Start()
    {
        //scoreText = GetComponent<TextMeshProUGUI>();
    }

    // yeah I know this kinda sucks
    void FixedUpdate()
    {
        player1Inventory.text = "INV: " + gameState.player1inventory.ToString();
        player1Cache.text = "CACHE: " + gameState.player1cache.ToString();
        player1Score.text = "SCORE: " + gameState.player1score.ToString();

        player2Inventory.text = "INV: " + gameState.player2inventory.ToString();
        player2Cache.text = "CACHE: " + gameState.player2cache.ToString();
        player2Score.text = "SCORE: " + gameState.player2score.ToString();


        //scoreText.text = $"P1-    I:{gameState.player1inventory}  C:{gameState.player1cache}  S:{gameState.player1score} \nP2-    I:{gameState.player2inventory}  C:{gameState.player2cache}  S:{gameState.player2score}";
    }
}
