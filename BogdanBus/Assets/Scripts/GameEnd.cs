using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnd : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] BoardingSystem _busSystem;
    [SerializeField] BusStop _endBusStop;
    [SerializeField] DeathZone _deathZone;
    [SerializeField] Animator _endScreenAnimator;

    [Header("*** View zone ***")]
    // Counters
    [SerializeField] private int _missedStopCount;
    [SerializeField] private int _peopleCount;
    [SerializeField] private int _monstersCount;
    // Others
    [SerializeField] private BusStop _busStop;
    [SerializeField] private EndType _endType = EndType.NotDefined;

    private void Update()
    {

        if (_endType != EndType.None)
        {
            CheckEnd();
        }
        if (_endType == EndType.Fired)
            YouAreFired();
        else if (_endType == EndType.Death)
            Death();
        else if (_endType == EndType.People)
            WorkShiftEnd();
        else if (_endType == EndType.Monster)
            WayToHell();
    }

    public void AddMissedStop() { _missedStopCount++; }
    public void AddHuman() { _peopleCount++; }
    public void AddMonster() { _monstersCount++; }

    private void CheckEnd()
    {
        if (_missedStopCount >= 7)
        {
            _endType = EndType.Fired;
        }
        else if (_peopleCount > 0 && _monstersCount > 0)
        {
            _endType = EndType.Death;
        }
        else if (_monstersCount == 0)
        {
            _endType = EndType.People;
        }
        else if (_peopleCount == 0)
        {
            _endType = EndType.Monster;
        }
    }

    private void YouAreFired()
    {
        // when we have come to the next bus stop
        if (_busStop == null)
            return;

        Debug.Log("END -> YouAreFired");

        _busStop.ClearPassenger();

        Bus bus = _busSystem.GetComponent<Bus>();
        bus.ForceStop();

        _endScreenAnimator.SetBool("youAreFired", true);

        _endType = EndType.None;
    }

    private void WorkShiftEnd()
    {
        // when we have come to the end bus stop
        if (_busStop != _endBusStop)
            return;

        Debug.Log("END -> WorkShiftEnd");

        _endType = EndType.None;
    }

    private void WayToHell()
    {
        _busStop = _busSystem.CurrentBusStop;
        if (_busStop != _endBusStop)
            return;

        Debug.Log("END -> WayToHell");

        _endType = EndType.None;
    }

    private void Death()
    {
        // when we have come to the death zone
        if (!_deathZone.IsBus)
            return;

        Debug.Log("END -> Death");

        Bus bus = _busSystem.GetComponent<Bus>();
        bus.ForceStop();

        _endType = EndType.None;
    }

    public enum EndType
    {
        NotDefined,
        Fired,
        Death,
        People,
        Monster,
        None
    }
}
