using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnd : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] BoardingSystem _busSystem;
    [SerializeField] Animator _endScreenAnimator;
    [SerializeField] EndBus _endBus;
    [SerializeField] GameObject _hellTunnel;
    [SerializeField] Radio _radio;
    public static GameEnd Singletone;

    private bool _isGameEnded;

    public bool IsGameEnd { get => _isGameEnded; }

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

    private void EndHandler()
    {
        _isGameEnded = true;
        _radio.ForceTurnOff();
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
        EndHandler();

        Debug.Log("END -> YouAreFired");

        Bus bus = _busSystem.GetComponent<Bus>();
        bus.ForceStop();

        _endScreenAnimator.SetBool("youAreFired", true);
    }

    private void StartWorkShiftEnd()
    {
        EndHandler();
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

    public void StartWayToHell()
    {
        if (_peopleCount == 0 && _monstersCount >= 0)
        {
            Debug.Log("END -> WayToHell (start)");

            _hellTunnel.SetActive(true);
        }
    }

    public void FinishWayToHell()
    {
        EndHandler();
        Debug.Log("END -> WayToHell (finish)");

        Bus bus = _busSystem.GetComponent<Bus>();
        bus.ForceStop();

        _endScreenAnimator.SetBool("isWayToHell", true);
    }

    private void StartDeath()
    {
        EndHandler();
        Debug.Log("END -> Death (start)");

        Bus bus = _busSystem.GetComponent<Bus>();
        bus.ForceStop();

        _endBus.gameObject.SetActive(true);
        _endBus.StartCrashing();
    }

    public void FinshDeath()
    {
        Debug.Log("END -> Death (finish)");

        _endScreenAnimator.SetBool("isDead", true);
    }
}
