using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wipers : MonoBehaviour
{
    private Animator _animator;
    private bool _isWiping = false;

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
        }
        else
        {
            _animator.SetBool("isWiping", false);
        }
    }
}
