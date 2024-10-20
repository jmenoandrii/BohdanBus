using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passenger : MonoBehaviour
{
    [Header("Passenger Data")]
    [SerializeField]
    private Door.DoorMark _doorMark;
    public Door.DoorMark DoorMark { get => _doorMark; }
    [SerializeField]
    public Door door;
    [SerializeField]
    public bool hasData;

    [Header("Bus Triggers")]
    [SerializeField]
    private bool _isInBus;
    public bool IsInBus { get => _isInBus; }
    [SerializeField]
    public bool canGoToBus;
    [SerializeField]
    private bool _isFarePaid;
    [SerializeField]
    private bool _isSitting;

    [Header("Bus Points")]
    [SerializeField]
    public Transform doorPoint;
    [SerializeField]
    public Seat driverPoint;
    [SerializeField]
    public Seat seatPoint;
    [SerializeField]
    public List<Transform> controlPointList;

    [Header("Movement Settings")]
    [SerializeField]
    private float _speed = 2f; // Швидкість переміщення
    [SerializeField]
    private float _deltaBottom = 0.5f; // Висота для підйому до дверей
    [SerializeField]
    private float _deltaForward = 1f; // Висота для підйому до дверей
    private Vector3 targetPosition;
    [SerializeField]
    private GoTo _goTo;

    private void Update()
    {
        if (!hasData) return;

        _goTo = GoTo.None;

        if (!_isInBus && canGoToBus && door.IsOpen)
        {
            _goTo = GoTo.Bus;
            if (Vector3.Distance(transform.position, targetPosition) > _deltaForward)
                MoveTo(doorPoint.position - Vector3.up * _deltaBottom);
            else
                MoveTo(doorPoint.position);
        }
        else if (_isInBus && !_isFarePaid && !driverPoint.IsTaken)
        {
            if (_goTo != GoTo.Driver)
            {
                Debug.Log($"Take: {this}");
                driverPoint.Take();
            }
            _goTo = GoTo.Driver;
            MoveTo(driverPoint.transform.position);
        }
        else if (_isInBus && !_isSitting)
        {
            if (_isFarePaid && _goTo != GoTo.Seat)
            {
                Debug.Log($"GiveUp: {this}");
                driverPoint.GiveUp();
            }
            _goTo = GoTo.Seat;
            MoveTo(seatPoint.transform.position);
        }
        else
        {
            Stand();
        }
    }

    private void MoveTo(Vector3 target)
    {
        targetPosition = target;

        // Обчислюємо напрямок до цільової точки
        Vector3 direction = (targetPosition - transform.position).normalized;

        // Плавно переміщуємо пасажира до цільової позиції
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, _speed * Time.deltaTime);

        // Поворот до цільової точки
        if (direction != Vector3.zero)
        {
            // Обчислюємо напрямок руху
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Отримуємо тільки Y-значення цільового обертання
            float targetYRotation = targetRotation.eulerAngles.y;

            // Створюємо новий кватерніон з попереднім X і Z, але з новим Y
            Quaternion smoothRotation = Quaternion.Euler(0, targetYRotation, 0);

            // Затримка повороту
            transform.rotation = Quaternion.Slerp(transform.rotation, smoothRotation, Time.deltaTime * _speed * 2); // Збільште швидкість повороту для кращої реакції
        }

        // Якщо досягли цільової позиції, перевіряємо наступний стан
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            if (_goTo == GoTo.Bus)
            {
                Debug.Log($"ToBus: {this}");
                _isInBus = true;
            }
            else if (_goTo == GoTo.Driver)
            {
                Debug.Log($"ToDriver: {this}");
                StartCoroutine(WaitForPayment());
            }
            else if (_goTo == GoTo.Seat)
            {
                Debug.Log($"ToSeat: {this}");
                _isSitting = true;

                // Поворот пасажира вперед після того, як він сів
                Quaternion forwardRotation = Quaternion.Euler(0, seatPoint.transform.rotation.eulerAngles.y, 0);
                transform.rotation = forwardRotation;
            }
        }
    }


    private IEnumerator WaitForPayment()
    {
        while (!_isFarePaid)
        {
            // Чекаємо, поки гравець натисне кнопку 'P'
            if (Input.GetKeyDown(KeyCode.P))
            {
                _isFarePaid = true;
                Debug.Log($"Pay: {this}");

                // Тепер сідаємо на місце
                yield return new WaitForSeconds(0.5f); // Затримка перед переходом на місце
                MoveTo(seatPoint.transform.position);
            }
            yield return null; // Чекаємо наступного кадру
        }
    }

    private void Stand()
    {
        // Можливо, тут можна реалізувати анімацію стояння
    }

    private enum GoTo
    {
        None,
        Bus,
        Driver,
        Seat
    }
}
