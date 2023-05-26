using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    private const int _BULLET_AMOUNT = 10;

    [Header("Shooting Components:")]
    [SerializeField] private Bullete _bulletePref;
    [SerializeField] private Transform _shootOrigin;
    [SerializeField] private Light _light;

    private Queue<Bullete> _bulletesPool = new Queue<Bullete>();

    private void Start()
    {
        Bullete _currentBullete;

        for (int i = 0; i < _BULLET_AMOUNT; ++i)
        {
            _currentBullete = Instantiate(_bulletePref, _shootOrigin.position, Quaternion.identity);
            _currentBullete.OnHide();
            _bulletesPool.Enqueue(_currentBullete);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            ShootBullete();

        if (Input.GetKeyDown(KeyCode.F))
            _light.enabled = !_light.enabled;
    }

    private void ShootBullete()
    {
        Bullete bullete = _bulletesPool.Dequeue();
        bullete.OnShow();
        bullete.OnPushed(_shootOrigin.transform.forward, _shootOrigin.position);

        _bulletesPool.Enqueue(bullete);
    }
}
