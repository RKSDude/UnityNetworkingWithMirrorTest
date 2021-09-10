using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(ConfigurableJoint))]

public class PlayerController : MonoBehaviour
{
    private PlayerMotor motor;
    private ConfigurableJoint joint;
    private Animator anim;
    private RaycastHit hit;

    [SerializeField] private float speed = 5f;
    [SerializeField] private float sensitivity = 10f;
    [SerializeField] private float thrustForce = 1000f;
    [SerializeField] private float thrustFuelBurn = 1f;
    [SerializeField] private float thrustFuelRegen = 0.3f;
    
    private float thrustFuelAmount = 1f;
    public float GetFuelAmount()
    {
        return thrustFuelAmount;
    }

    [Header("Joint options")]
    [SerializeField] private float jointSpring = 10f;    
    [SerializeField] private float jointMaxForce = 40f;

    private void Start()
    {
        motor = GetComponent<PlayerMotor>();
        joint = GetComponent<ConfigurableJoint>();
        anim = GetComponent<Animator>();

        SetJointSettings(jointSpring);
    }

    private void Update()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out hit, 100f))
        {
            joint.targetPosition = new Vector3(0f, -hit.point.y, 0f);
        }
        else
        {
            joint.targetPosition = new Vector3(0f, 0f, 0f);
        }

        #region Movement Calculations
        //XZ Movement
        float xMovement = Input.GetAxis("Horizontal");
        float zMovement = Input.GetAxis("Vertical");

        Vector3 moveHorizontal = transform.right * xMovement;
        Vector3 moveVertical = transform.forward * zMovement;

        Vector3 _velocity = (moveHorizontal + moveVertical) * speed;
        motor.Move(_velocity);

        //Animator movement
        anim.SetFloat("ForwardVelocity", zMovement);

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

        if(Input.GetButton("Jump") && thrustFuelAmount > 0)
        {
            thrustFuelAmount -= thrustFuelBurn * Time.deltaTime;

            if(thrustFuelAmount > 0.01f)
            {
                _thrusterForce = Vector3.up * thrustForce;
                SetJointSettings(0f);
            }
        }
        else
        {
            thrustFuelAmount += thrustFuelRegen * Time.deltaTime;

            SetJointSettings(jointSpring);
        }

        thrustFuelAmount = Mathf.Clamp(thrustFuelAmount, 0f, 1f);

        motor.Thrust(_thrusterForce);
        #endregion
    }

    private void SetJointSettings(float _jointSpring)
    {
        joint.yDrive = new JointDrive
        {
            positionSpring = _jointSpring, 
            maximumForce = jointMaxForce
        };
    }
}
