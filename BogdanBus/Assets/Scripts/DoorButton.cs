using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class DoorButton : MonoBehaviour, IInteractableObject
{
    [SerializeField]
    private Door[] _doors;
    private Animation _animation;

    [Header("Sounds")]
    [SerializeField] AudioSource _audioPress;

    private void Awake()
    {
        _animation = GetComponent<Animation>();
    }

    public void Interact()
    {
        if (!_animation.isPlaying)
            _animation.Play();
        
        // ~~~ audio (press) ~~~
        if (_audioPress.isPlaying)
            _audioPress.Stop();
        _audioPress.Play();

        foreach (var door in _doors)
        {
            door.Interact(); 
        }
    }
}
