using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class Door : MonoBehaviour
{
    [SerializeField]
    private bool _isOpen = false;
    public bool IsOpen { get => _isOpen; }
    [SerializeField]
    private AnimationClip _doorOpenAnim;
    [SerializeField]
    private AnimationClip _doorCloseAnim;
    private Animation _animation;

    private void Awake()
    {
        _animation = GetComponent<Animation>();
    }

    public void Open()
    {
        _isOpen = true;
        _animation.Play(_doorOpenAnim.name);
    }

    public void Close()
    {
        _isOpen = false;
        _animation.Play(_doorCloseAnim.name);
    }

    public void Interact()
    {
        if (_isOpen)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    public enum Mark
    {
        Front,
        Back,
    }
}
