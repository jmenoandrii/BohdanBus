using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wipers : MonoBehaviour
{
    private Animator _animator;
    private bool _isWiping = false;

    [Header("Sounds")]
    [SerializeField] AudioSource _audioWiper;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _animator.SetBool("isWiping", false);
    }

    public void Interact()
    {
        _isWiping = !_isWiping;
        if (_isWiping)
        {
            _animator.SetBool("isWiping", true);

            // ~~~ audio (play) ~~~
            _audioWiper.Play();
        }
        else
        {
            _animator.SetBool("isWiping", false);

            // ~~~ audio (stop) ~~~
            _audioWiper.Stop();
        }
    }
}
