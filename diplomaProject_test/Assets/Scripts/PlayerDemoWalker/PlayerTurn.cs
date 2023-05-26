using UnityEngine;

public class PlayerTurn : MonoBehaviour
{

    [Header("Mouse Settings:")]
    [Range(0, 200)]
    [SerializeField] private float _mouseSensetivity = 100.0f;

    [Header("Turn Components:")]
    [SerializeField] private Transform _playerBody;

    private float _mouseX = 0;
    private float _mouseY = 0;

    private float _xRotation = 0;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        _mouseX = Input.GetAxis("Mouse X") * _mouseSensetivity * Time.deltaTime;
        _mouseY = Input.GetAxis("Mouse Y") * _mouseSensetivity * Time.deltaTime;

        _xRotation -= _mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        _playerBody.Rotate(Vector3.up * _mouseX);
    }
}
