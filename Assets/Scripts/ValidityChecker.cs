using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ValidityChecker : MonoBehaviour
{
    [SerializeField] CanvasManager manager;
    [Header("Actions")]
    [SerializeField] Action p1Action;
    [SerializeField] Action p2Action;
    [SerializeField] Action p3Action;
    

    private void LateUpdate()
    {
        if (manager.switched && Input.GetButtonUp("Fire1"))
        {
            StartCoroutine(DelayCheck());
            Debug.Log("Checking validity");
        }
    }

    private IEnumerator DelayCheck()
    {
        yield return new WaitForSecondsRealtime(.1f);
        CheckToggleValidity(0);
    }

    public void CheckToggleValidity(int group)
    {
        CheckToggleValidity(p1Action);
        CheckToggleValidity(p2Action);
        CheckToggleValidity(p3Action);
    }

    private void CheckToggleValidity(Action action)
    {
        //move stop attack stun
        Vector2 actDir = action.GetDirAct();
        int actionType = (int)actDir.y;
        //int actionDir = (int)actDir.y;
        //Part 1
        
        if(action == p1Action)
        {
            Debug.Log($"P1 ACTION TYPE IS {actionType}");
            if (actionType == 0)
            {
                if((int)p2Action.GetDirAct().y == 3)
                {
                    Debug.Log("Found stun", p2Action.gameObject);
                    p2Action.ToggleActionTypeByName("Move", true);
                    p2Action.SetGroupsInteractable(true);
                }
            }
            else if(actionType == 1)
            {
                if ((int)p2Action.GetDirAct().y == 3)
                {
                    Debug.Log("Found stun", p2Action.gameObject);
                    p2Action.ToggleActionTypeByName("Move", true);
                    p2Action.SetGroupsInteractable(true);
                }
                SwitchOffStopsExcept(p1Action);
            }
            else if (actionType == 2)
            {
                Debug.Log("Found attack on action 1");
                p2Action.ToggleActionTypeByName("Stun", true);
                p2Action.SetGroupsInteractable(false);
            }
            
        }//part 2
        else if(action == p2Action)
        {
            Debug.Log($"P2 ACTION TYPE IS {actionType}");
            if (actionType == 0)
            {
                if ((int)p3Action.GetDirAct().y == 3)
                {
                    Debug.Log("Found stun", p3Action.gameObject);
                    p3Action.ToggleActionTypeByName("Move", true);
                    p3Action.SetGroupsInteractable(true);
                }
            }
            else if (actionType == 1)
            {
                if ((int)p3Action.GetDirAct().y == 3)
                {
                    Debug.Log("Found stun", p3Action.gameObject);
                    p3Action.ToggleActionTypeByName("Move", true);
                    p3Action.SetGroupsInteractable(true);
                }
                SwitchOffStopsExcept(p2Action);
            }
            else if (actionType == 2)
            {
                Debug.Log("Found attack on action 2");
                p3Action.ToggleActionTypeByName("Stun", true);
                p3Action.SetGroupsInteractable(false);
            }
        }
        else
        {
            Debug.Log($"P3 ACTION TYPE IS {actionType}");
            //part 3
            if (actionType == 1)
            {
                SwitchOffStopsExcept(p3Action);
            }
        }
    }

    private void SwitchOffStopsExcept(Action exempt)
    {

        Debug.Log("Switching off stops");
        if (exempt == p1Action)
        {
            if(p2Action.GetActionType() == 1)
            {
                p2Action.ToggleActionTypeByName("Move", true);
            }
            if (p3Action.GetActionType() == 1)
            {
                p3Action.ToggleActionTypeByName("Move", true);
            }
        }
        else if (exempt == p2Action)
        {
            if (p1Action.GetActionType() == 1)
            {
                p1Action.ToggleActionTypeByName("Move", true);
            }
            if (p3Action.GetActionType() == 1)
            {
                p3Action.ToggleActionTypeByName("Move", true);
            }
        }
        else
        {
            if (p1Action.GetActionType() == 1)
            {
                p1Action.ToggleActionTypeByName("Move", true);
            }
            if (p2Action.GetActionType() == 1)
            {
                p2Action.ToggleActionTypeByName("Move", true);
            }
        }
    }
}
