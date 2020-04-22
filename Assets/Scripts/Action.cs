using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Action : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] ToggleGroup DirGroup;
    [SerializeField] ToggleGroup ActGroup;
    public Vector2 GetDirAct()
    {
        Debug.Log(DirGroup.ActiveToggles());
        Debug.Log(ActGroup.ActiveToggles());
        int dir = GetIntFromDirName(DirGroup.ActiveToggles().FirstOrDefault().name);
        int act = GetIntFromActName(ActGroup.ActiveToggles().FirstOrDefault().name);
        Vector2 vec = new Vector2(dir,act);
        return vec;
    }

    private int GetIntFromDirName(string name)
    {
        switch (name)
        {
            case "Up":
                return 1;
            case "Right":
                return 2;
            case "Down":
                return 3;
            case "Left":
                return 4;
            default:
                Debug.LogError($"No can do.");
                return 0;
        }
    }

    private int GetIntFromActName(string name)
    {
        switch (name)
        {
            case "Move":
                return 0;
            case "Stop":
                return 1;
            case "Attack":
                return 2;
            case "Stun":
                return -1;
            default:
                return -1;
        }
    }
}
