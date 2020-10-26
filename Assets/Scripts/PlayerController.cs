using UnityEngine;
using Mirror;
public class PlayerController : MonoBehaviour
{

    [SerializeField] CameraController cameraPrefab;
    [SerializeField] Transform playerInputSpace = default;
    [SerializeField, Range(0f, 100f)] float maxSpeed = 10f;

    [SerializeField, Range(0f, 100f)] float maxAcceleration = 10f, maxAirAcceleration = 1f;

    [SerializeField, Range(0f, 10f)] float jumpHeight = 2f;

    [SerializeField, Range(0, 5)] int maxAirJumps = 0;

    [SerializeField, Range(0, 90)] float maxGroundAngle = 25f;

    Vector3 velocity, desiredVelocity;

    Rigidbody body;
    Vector3 contactNormal;
    public bool desiredJump = false;
    int groundContactCount;
    bool OnGround => groundContactCount > 0;
    int jumpPhase;
    float minGroundDotProduct;



    void Start()
    {
        //foreach(var cam in FindObjectsOfType<CameraController>())
        //    cam.gameObject.SetActive(false);

        CreateCamera();
        body = GetComponent<Rigidbody>();
        OnValidate();
    }

    public void CreateCamera()
    {

        CameraController newCamera = Instantiate(cameraPrefab) as CameraController;
        newCamera.focus = transform;
        playerInputSpace = newCamera.transform;


    }

    void Update()
    {

        Vector2 playerInput;
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);

        if (playerInputSpace)
        {
            //Relative movement to cam . 
            Vector3 forward = playerInputSpace.forward;
            //Remove Y component from relative space forward vector to make the player only move on XZ plane .
            forward.y = 0f;
            forward.Normalize();
            Vector3 right = playerInputSpace.right;
            right.y = 0f;
            right.Normalize();
            desiredVelocity = (forward * playerInput.y + right * playerInput.x) * maxSpeed;



        }
        else
        {
            desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;
        }

        //Rotate character towards movement direction
        //Quaternion direction = Quaternion.LookRotation(desiredVelocity);
        //transform.rotation = Quaternion.Lerp(transform.rotation, direction, 2f * Time.deltaTime);

    
    }   

    void FixedUpdate()
    {

        UpdateState();
        AdjustVelocity();
        desiredJump |= Input.GetButtonDown("Jump");

        if (desiredJump)
        {
            desiredJump = false;
            Jump();
        }
        body.velocity = velocity;
        ClearState();
    }
    void ClearState()
    {
        groundContactCount = 0;
        contactNormal = Vector3.zero;
    }

    void UpdateState()
    {

        velocity = body.velocity;
        if (OnGround)
        {
            jumpPhase = 0;
            if (groundContactCount > 1)
            {
                contactNormal.Normalize();
            }
        }
        else
        {
            contactNormal = Vector3.up;
        }
    }

    void AdjustVelocity()
    {
        Vector3 xAxis = ProjectOnContactPlane(Vector3.right).normalized;
        Vector3 zAxis = ProjectOnContactPlane(Vector3.forward).normalized;

        float currentX = Vector3.Dot(velocity, xAxis);
        float currentZ = Vector3.Dot(velocity, zAxis);

        float acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
        float maxSpeedChange = acceleration * Time.deltaTime;

        float newX = Mathf.MoveTowards(currentX, desiredVelocity.x, maxSpeedChange);
        float newZ = Mathf.MoveTowards(currentZ, desiredVelocity.z, maxSpeedChange);

        velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
    }
    void OnValidate()
    {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
    }

    void Jump()
    {
        if (OnGround || jumpPhase < maxAirJumps)
        {
            jumpPhase += 1;
            float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
            float alignedSpeed = Vector3.Dot(velocity, contactNormal);
            if (alignedSpeed > 0f)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
            }
            velocity += contactNormal * jumpSpeed;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        EvaluateCollision(collision);
    }

    void OnCollisionStay(Collision collision)
    {
        EvaluateCollision(collision);
    }

    void EvaluateCollision(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            if (normal.y >= minGroundDotProduct)
            {
                groundContactCount += 1;
                contactNormal += normal;
            }
        }
    }

    Vector3 ProjectOnContactPlane(Vector3 vector)
    {
        return vector - contactNormal * Vector3.Dot(vector, contactNormal);
    }
}