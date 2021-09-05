using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float sensitivity = 10f;
    private PlayerMotor motor;

    private void Start()
    {
        motor = GetComponent<PlayerMotor>();
    }

    private void Update()
    {
        //XZ Movement
        float xMovement = Input.GetAxisRaw("Horizontal");
        float zMovement = Input.GetAxisRaw("Vertical");

        Vector3 moveHorizontal = transform.right * xMovement;
        Vector3 moveVertical = transform.forward * zMovement;

        Vector3 _velocity = (moveHorizontal + moveVertical).normalized * speed;

        motor.Move(_velocity);

        //Y Rotation (LEFT/RIGHT ROTATION NOT VERTICAL)
        float yRotation = Input.GetAxisRaw("Mouse X");

        Vector3 _rotation = new Vector3 (0f, yRotation, 0f) * sensitivity;

        motor.Rotate(_rotation);
    }
}
