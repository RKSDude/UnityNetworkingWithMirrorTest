using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(ConfigurableJoint))]
public class PlayerController : MonoBehaviour
{
    private PlayerMotor motor;
    private ConfigurableJoint joint;

    [SerializeField] private float speed = 5f;
    [SerializeField] private float sensitivity = 10f;
    [SerializeField] private float thrustForce = 1000f;

    [Header("Joint options")]
    [SerializeField] private JointDriveMode jointMode = JointDriveMode.Position;
    [SerializeField] private float jointSpring = 10f;    
    [SerializeField] private float jointMaxForce = 40f;

    private void Start()
    {
        motor = GetComponent<PlayerMotor>();
        joint = GetComponent<ConfigurableJoint>();

        SetJointSettings(jointSpring);
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

        //Y Rotation (LEFT/RIGHT ROTATION NOT VERTICAL (rotation about the Y axis))
        float yRotation = Input.GetAxisRaw("Mouse X");

        Vector3 _rotation = new Vector3 (0f, yRotation, 0f) * sensitivity;

        motor.Rotate(_rotation);

        //Camera Rotation (Vertical (rotation about the X axis))
        float xRotation = Input.GetAxisRaw("Mouse Y");

        float _camRotationX = xRotation * sensitivity;

        motor.RotateCamera(_camRotationX);

        //thruster stuff
        Vector3 _thrusterForce = Vector3.zero;

        if(Input.GetButton("Jump"))
        {
            _thrusterForce = Vector3.up * thrustForce;
            SetJointSettings(0f);
        }
        else
        {
            SetJointSettings(jointSpring);
        }

        motor.Thrust(_thrusterForce);
    }

    private void SetJointSettings(float _jointSpring)
    {
        joint.yDrive = new JointDrive
        {
            mode = jointMode, 
            positionSpring = _jointSpring, 
            maximumForce = jointMaxForce
        };
    }
}
