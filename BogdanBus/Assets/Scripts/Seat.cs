using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seat : MonoBehaviour
{
    [SerializeField]
    private bool _isTaken;

    public bool IsTaken { get => _isTaken; }

    public void Take() { _isTaken = true; }

    public void GiveUp() { _isTaken = false; }
}
