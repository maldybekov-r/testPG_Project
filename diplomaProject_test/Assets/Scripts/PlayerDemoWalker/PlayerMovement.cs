using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Components:")]
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _layerMask;

    private float _movementSpeed = 12f;
    private float _gravity = -19.62f;
    private float _jumpHeight = 3f;

    private float _groundDistance = 0.4f;
    private bool _isGrounded;

    private float _x = 0;
    private float _z = 0;

    private Vector3 _velocity;

    void Update()
    {
        _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, _layerMask);

        if (_isGrounded && _velocity.y < 0)
            _velocity.y = -2f;

        _x = Input.GetAxis("Horizontal");
        _z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * _x + transform.forward * _z;
        _characterController.Move(move * _movementSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && _isGrounded)
            _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);

        _velocity.y += _gravity * Time.deltaTime;

        _characterController.Move(_velocity * Time.deltaTime);
    }
}
