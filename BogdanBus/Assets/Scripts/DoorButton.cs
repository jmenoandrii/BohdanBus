using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class DoorButton : MonoBehaviour, IInteractableObject
{
    [SerializeField]
    private Door[] _doors;
    private Animation _animation;

    private void Awake()
    {
        _animation = GetComponent<Animation>();
    }

    public void Interact()
    {
        if (!_animation.isPlaying)
            _animation.Play();
            
        foreach (var door in _doors)
        {
            door.Interact(); 
        }
    }
}
