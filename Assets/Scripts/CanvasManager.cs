using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CanvasManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] CanvasGroup JoinScreen;
    [SerializeField] CanvasGroup ControlsScreen;
    private float increment = .05f;
    private void Start()
    {
        JoinScreen.gameObject.SetActive(true);
        ControlsScreen.gameObject.SetActive(false);
        ControlsScreen.alpha = 0;
        JoinScreen.alpha = 1;
        
    }

    public void Switch()
    {
        StartCoroutine(SwitchToControls());
    }
    public IEnumerator SwitchToControls()
    {
        ControlsScreen.gameObject.SetActive(true);
        while (ControlsScreen.alpha < 1)
        {
            ControlsScreen.alpha += increment;
            JoinScreen.alpha -= increment;
            yield return new WaitForSeconds(.05f);
        }
        JoinScreen.gameObject.SetActive(false);
    }
}