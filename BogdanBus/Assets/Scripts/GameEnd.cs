using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnd : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] BoardingSystem _busSystem;
    [SerializeField] Trigger _endBusStopTrigger;
    [SerializeField] Animator _endScreenAnimator;
    public static GameEnd Singletone;

    [Header("*** View zone ***")]
    // Counters
    [SerializeField] private int _missedStopCount;
    [SerializeField] private int _peopleCount;
    [SerializeField] private int _monstersCount;

    private void Awake()
    {
        Singletone = this;
    }

    public void AddMissedStop() 
    { 
        _missedStopCount++;
        if (_missedStopCount >= 7)
        {
            YouAreFired();
        }
    }
    public void AddHuman() 
    { 
        _peopleCount++; 
    }
    public void AddMonster() 
    { 
        _monstersCount++; 
    }

    public void PerformeEndBusStop()
    {
        if (_peopleCount > 0 && _monstersCount > 0)
        {
            Death();
        }
        else if (_monstersCount == 0)
        {
            WorkShiftEnd();
        }
        else if (_peopleCount == 0)
        {
            WayToHell();
        }
    }

    private void YouAreFired()
    {
        Debug.Log("END -> YouAreFired");

        Bus bus = _busSystem.GetComponent<Bus>();
        bus.ForceStop();

        _endScreenAnimator.SetBool("youAreFired", true);
    }

    private void WorkShiftEnd()
    {
        Debug.Log("END -> WorkShiftEnd");
    }

    private void WayToHell()
    {
        Debug.Log("END -> WayToHell");
    }

    private void Death()
    {
        Debug.Log("END -> Death");

        Bus bus = _busSystem.GetComponent<Bus>();
        bus.ForceStop();
    }
}
