using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    private TextMeshProUGUI scoreText;
    [SerializeField] GamestateManager gameState;
    // Start is called before the first frame update
    void Start()
    {
        scoreText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {

        scoreText.text = $"P1-    I:{gameState.player1inventory}  C:{gameState.player1cache}  S:{gameState.player1score} \nP2-    I:{gameState.player2inventory}  C:{gameState.player2cache}  S:{gameState.player2score}";
    }
}
