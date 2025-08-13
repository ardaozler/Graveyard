using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(CharacterController))]
public class PlayerMover : MonoBehaviour
{
    [SerializeField] private Camera _mainCam;

    [Header("Movement Settings")] [SerializeField]
    private float _speed = 5f;

    [SerializeField] private float _runSpeed = 10f;


    [Header("Look Settings")] [SerializeField]
    private float _lookSpeed = 2f;

    [SerializeField] private float _lookXLimit = 45f;


    [Header("Physics")] [SerializeField] private float _gravity = 9.81f;
    private Vector3 _moveDirection = Vector3.zero;
    private float _rotationX = 0f;
    private float _verticalVelocity = 0f;

    public bool canMove = true;

    private CharacterController _controller;

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!canMove) return;

        // Handle look rotation
        _rotationX -= Input.GetAxis("Mouse Y") * _lookSpeed;
        _rotationX = Mathf.Clamp(_rotationX, -_lookXLimit, _lookXLimit);


        var localEulerAngles = transform.localEulerAngles;
        localEulerAngles = new Vector3(localEulerAngles.x,
            localEulerAngles.y + Input.GetAxis("Mouse X") * _lookSpeed, 0);
        transform.localEulerAngles = localEulerAngles;

        _mainCam.transform.localEulerAngles = new Vector3(_rotationX, 0, 0);

        // Handle movement
        float moveSpeed = Input.GetKey(KeyCode.LeftShift) ? _runSpeed : _speed;

        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 move = transform.TransformDirection(input) * moveSpeed;

        // idk revise this
        if (_controller.isGrounded)
        {
            _verticalVelocity = -_gravity * Time.deltaTime; // stick to ground
        }
        else
        {
            _verticalVelocity -= _gravity * Time.deltaTime; // falling 
        }

        move.y = _verticalVelocity;

        _controller.Move(move * Time.deltaTime);
    }
}