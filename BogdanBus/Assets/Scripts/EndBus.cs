using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class EndBus : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform _breakPoint;
    [SerializeField] private float _speed = 5f;

    [Header("*** View zone ***")]
    [SerializeField] private bool _isAbleToCrash;

    public void StartCrashing() { _isAbleToCrash = true; }

    private void Update()
    {
        if (_isAbleToCrash)
        {
            MoveTo(_breakPoint.position);
        }
    }

    private void MoveTo(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;

        transform.position = Vector3.MoveTowards(transform.position, target, _speed * Time.deltaTime);

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            float targetYRotation = targetRotation.eulerAngles.y;

            Quaternion smoothRotation = Quaternion.Euler(0, targetYRotation, 0);


            transform.rotation = Quaternion.Slerp(transform.rotation, smoothRotation, Time.deltaTime * _speed * 2);
        }

        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Bus>())
        {
            GameEnd.Singletone.FinshDeath();
            this.gameObject.SetActive(false);
        }
    }
}
