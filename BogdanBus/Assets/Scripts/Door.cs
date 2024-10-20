using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField]
    public bool _isOpen = false;
    [SerializeField]
    private GameObject _doorMesh;

    public void Open()
    {
        _isOpen = true;
    }

    public void Close()
    {
        _isOpen = false;
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
        _doorMesh.SetActive(!_isOpen);
    }

    public enum DoorMark
    {
        Front,
        Back,
    }
}
