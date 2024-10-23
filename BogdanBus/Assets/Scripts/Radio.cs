using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Radio : MonoBehaviour, IInteractableObject
{
    [Header("Sounds")]
    [SerializeField] AudioSource _audio;

    private void Awak()
    {
        _audio.Play();
        _audio.Pause();
    }

    public void Interact()
    {
        // ~~~ audio (interact) ~~~
        if (_audio.isPlaying)
            _audio.Pause();
        else
            _audio.UnPause();
    }
}
