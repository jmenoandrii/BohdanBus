using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnd : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] BoardingSystem _busSystem;
    [SerializeField] Trigger _endBusStopTrigger;
    [SerializeField] Animator _endScreenAnimator;
    [SerializeField] EndBus _endBus;
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

    /*public void PerformeEndBusStop()
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
    }*/

    public void BusStopEnding()
    {
        if (_peopleCount > 0 && _monstersCount > 0)
        {
            StartDeath();
        }
        else if (_monstersCount == 0)
        {
            StartWorkShiftEnd();
        }
    }

    private void YouAreFired()
    {
        Debug.Log("END -> YouAreFired");

        Bus bus = _busSystem.GetComponent<Bus>();
        bus.ForceStop();

        _endScreenAnimator.SetBool("youAreFired", true);
    }

    private void StartWorkShiftEnd()
    {
        Debug.Log("END -> WorkShiftEnd (start)");

        Bus bus = _busSystem.GetComponent<Bus>();
        bus.ForceStop();

        _busSystem.StartForceFreeSalon();
    }

    public void FinishWorkShiftEnd()
    {
        Debug.Log("END -> WorkShiftEnd (finish)");

        _endScreenAnimator.SetBool("isShiftEnd", true);
    }

    private void WayToHell()
    {
        if (_peopleCount == 0)
        {
            Debug.Log("END -> WayToHell");

        }
    }

    private void StartDeath()
    {
        Debug.Log("END -> Death");

        Bus bus = _busSystem.GetComponent<Bus>();
        bus.ForceStop();

        _endBus.gameObject.SetActive(true);
        _endBus.StartCrashing();
    }

    public void FinshDeath()
    {
        _endScreenAnimator.SetBool("isDead", true);
    }
}
