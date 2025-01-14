using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private Transform _camera;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _sensitivity;
    [SerializeField] private float _clampAngle;
    private float currentRotation;

    private void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;
        Vector3 moveDirection = Quaternion.Euler(0, _camera.eulerAngles.y, 0) * direction;

        bool kSpace = Input.GetKey(KeyCode.Space);
        bool kLeftShift = Input.GetKey(KeyCode.LeftShift);
        
        if (kSpace && !kLeftShift) moveDirection.y = 1;
        if (kLeftShift && !kSpace) moveDirection.y = -1;

        transform.position += _moveSpeed * Time.deltaTime * moveDirection;
    }

    private void Rotate()
    {
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            transform.Rotate(Vector3.up, mouseX * _sensitivity);

            currentRotation -= mouseY * _sensitivity;
            currentRotation = Mathf.Clamp(currentRotation, -_clampAngle, _clampAngle);

            _camera.localEulerAngles = new Vector3(currentRotation, 0, 0);
        }
    }

    private void Start()
    {
        currentRotation = _camera.eulerAngles.y;
    }
    private void Update()
    {
        if(Input.GetMouseButtonDown(1)) Cursor.lockState = CursorLockMode.Locked;
        if(Input.GetMouseButtonUp(1)) Cursor.lockState = CursorLockMode.None;
        Rotate();
        Move();
    }
}
