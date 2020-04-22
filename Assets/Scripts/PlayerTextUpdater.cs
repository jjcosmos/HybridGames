using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using database;
public class PlayerTextUpdater : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] DatabaseInteractionMobile dbMobile;
    [SerializeField] TMPro.TextMeshProUGUI tmp;
    public void UpdateText()
    {
        int player = dbMobile.playerID;
        tmp.text = $"You are Player: {player + 1}";
    }
}
