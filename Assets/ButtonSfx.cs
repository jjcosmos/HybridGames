using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSfx : MonoBehaviour
{
    [SerializeField] AudioClip buttonSound;
    public static ButtonSfx bSFX;
    private AudioSource myAudio;
    void Awake()
    {
        bSFX = this;
        myAudio = GetComponent<AudioSource>();
    }

    public void PlayButtonSound()
    {
        myAudio.PlayOneShot(buttonSound);
    }
}
