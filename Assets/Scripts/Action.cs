using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Action : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public ToggleGroup DirGroup;
    [SerializeField] public ToggleGroup ActGroup;
    private CanvasGroup DirCanvas;
    private CanvasGroup ActCanvas;
    public Vector2 GetDirAct()
    {
        //Debug.Log(DirGroup.ActiveToggles());
        //Debug.Log(ActGroup.ActiveToggles());
        int dir = GetIntFromDirName(DirGroup.ActiveToggles().FirstOrDefault().name);
        int act = GetIntFromActName(ActGroup.ActiveToggles().FirstOrDefault().name);
        Vector2 vec = new Vector2(dir,act);
        return vec;
    }

    private void Start()
    {
        DirCanvas = DirGroup.gameObject.AddComponent<CanvasGroup>();
        ActCanvas= ActGroup.gameObject.AddComponent<CanvasGroup>();
    }
    public int GetActionType()
    {
        
        Vector2 vec = GetDirAct();
        Debug.Log($"Returning action type {(int)vec.y}", gameObject);
        return (int)vec.y;
    }
    public void SetGroupsInteractable(bool isInteractable)
    {
        DirCanvas.interactable = isInteractable;
        ActCanvas.interactable = isInteractable;
    }

    public void ToggleActionTypeByName(string name, bool isOn)
    {
        Transform t = ActGroup.transform;
        Debug.Log($"Toggling {name} to {isOn}");
        switch (name)
        {
            case "Move":
                t.transform.GetChild(0).GetComponent<Toggle>().isOn = isOn;
                Debug.Log($"@@@Setting {t.transform.GetChild(0).name} to {isOn}");
                break;
            case "Stop":
                t.transform.GetChild(1).GetComponent<Toggle>().isOn = isOn;
                Debug.Log($"@@@Setting {t.transform.GetChild(1).name} to {isOn}");
                break;
            case "Attack":
                t.transform.GetChild(2).GetComponent<Toggle>().isOn = isOn;
                Debug.Log($"@@@Setting {t.transform.GetChild(2).name} to {isOn}");
                break;
            case "Stun":
                t.transform.GetChild(3).GetComponent<Toggle>().isOn = isOn;
                Debug.Log($"@@@Setting {t.transform.GetChild(3).name} to {isOn}");
                break;
            default:
                break;
        }
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
                return 3;
            default:
                return -1;
        }
    }
}
