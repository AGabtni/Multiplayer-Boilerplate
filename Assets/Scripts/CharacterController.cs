using UnityEngine;
using Mirror;



public class CharacterController : MonoBehaviour
{
    [Header("Scene reference gameobjects")]
    [SerializeField] CameraController cameraPrefab;
    CameraController cameraInstance;
    [SerializeField] Transform groundPosition;

    [Header("Movement and collision settings")]

    [SerializeField] private float interactionRadius = 20.0f;

    [SerializeField] private float rotSpeed = 6.0f;
    [SerializeField] private float moveSpeed = 6.0f;
    [SerializeField] private float groundDetectionRadius = 0.25f;
    [SerializeField] private float jumpSpeed = 10.0f;
    [SerializeField] private float gravity = -9.8f;
    private float vertSpeed;
    Rigidbody rb;

    Vector3 movement;
    void Start()
    {

        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }   
    public void CreateCamera()
    {

        cameraInstance = Instantiate(cameraPrefab) as CameraController;
        cameraInstance.focus = transform;


    }
    public void MoveStep(float horInput, float vertInput)
    {

        movement = Vector3.zero;
        movement.x = horInput * moveSpeed;
        movement.z = vertInput * moveSpeed;
        movement = Vector3.ClampMagnitude(movement, moveSpeed);

        Quaternion tmp = cameraInstance.transform.rotation;
        cameraInstance.transform.eulerAngles = new Vector3(0, cameraInstance.transform.eulerAngles.y, 0);
        movement = cameraInstance.transform.TransformDirection(movement);
        cameraInstance.transform.rotation = tmp;
        Quaternion direction = Quaternion.LookRotation(cameraInstance.transform.forward);
        direction.eulerAngles = new Vector3(0, direction.eulerAngles.y + 10.0f, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, direction, rotSpeed * Time.deltaTime);

    }
    public void Jump()
    {
        if (isGrounded())
            vertSpeed = jumpSpeed;

    }
    bool isGrounded()
    {

        Collider[] hitColliders = Physics.OverlapSphere(groundPosition.position, groundDetectionRadius, LayerMask.GetMask("Ground"));

        return hitColliders.Length > 0 ? true : false;


    }


    public void TriggerCursorLock(){

        Cursor.lockState = Cursor.lockState == CursorLockMode.None ? CursorLockMode.Locked : CursorLockMode.None ;
        cameraInstance.enabled = Cursor.lockState == CursorLockMode.None ? false : true;
    }
    void Update()
    {

        vertSpeed += gravity * 2 * Time.deltaTime;
        movement.y = vertSpeed;
        rb.velocity = movement;
        Debug.Log(isGrounded());

    }


}