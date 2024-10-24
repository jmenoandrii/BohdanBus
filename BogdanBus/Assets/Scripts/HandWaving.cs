using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class HandWaving : MonoBehaviour
{
    [SerializeField]
    private GameObject _hand;
    private Animator _animator;
    private bool _isWaving;

    private void Start()
    {
        _animator = _hand.GetComponent<Animator>();
        _hand.SetActive(false);
        _isWaving = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H) && !_isWaving)
        {
            ShowHand();
        }
    }

    private void ShowHand()
    {
        _isWaving = true;
        _hand.SetActive(true);
        _animator.Play("waving");
        StartCoroutine(HideHandAfterAnimation());
    }

    private IEnumerator HideHandAfterAnimation()
    {
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);

        _hand.SetActive(false);
        _isWaving = false;
    }
}
