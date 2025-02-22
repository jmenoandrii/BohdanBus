using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class WiperButton : MonoBehaviour, IInteractableObject
{
    [SerializeField]
    private Wipers _wiper;

    private bool _active = false;
    private Animator _animator;

    [Header("Sounds")]
    [SerializeField] AudioSource _audioButton;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Interact()
    {
        _wiper.Interact();
        _active = !_active;
        _animator.SetBool("Active", _active);

        // ~~~ audio (interact) ~~~
        if (_audioButton.isPlaying)
            _audioButton.Stop();
        _audioButton.Play();
    }
}
