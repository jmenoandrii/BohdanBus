using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class Door : MonoBehaviour
{
    [Header("Animations")]
    [SerializeField]
    private AnimationClip _doorOpenAnim;
    [SerializeField]
    private AnimationClip _doorCloseAnim;
    private Animation _animation;

    [Header("Sounds")]
    [SerializeField]
    private AudioSource _audioDoorOpen;
    [SerializeField]
    private AudioSource _audioDoorClose;

    [Header("*** View zone ***")]
    [SerializeField]
    private bool _isOpen = false;
    public bool IsOpen { get => _isOpen; }


    private void Awake()
    {
        _animation = GetComponent<Animation>();
    }

    public void Open()
    {
        _isOpen = true;
        _animation.Play(_doorOpenAnim.name);

        // ~~~ audio (open) ~~~
        if (_audioDoorClose.isPlaying)
            _audioDoorClose.Stop();

        if (!_audioDoorOpen.isPlaying)
            _audioDoorOpen.Play();
    }

    public void Close()
    {
        _isOpen = false;
        _animation.Play(_doorCloseAnim.name);

        // ~~~ audio (close) ~~~
        if (_audioDoorOpen.isPlaying)
            _audioDoorOpen.Stop();

        if (!_audioDoorClose.isPlaying)
            _audioDoorClose.Play();
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
