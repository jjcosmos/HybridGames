using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTextUpdater : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] TMPro.TextMeshProUGUI tmp;
    public void UpdateText(int player)
    {
        tmp.text = $"You are Player: {player}";
    }
}
