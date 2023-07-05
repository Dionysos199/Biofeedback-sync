using UnityEngine;

public class FishController : MonoBehaviour
{
    public float swimSpeed = 5f;
    public float rotationSpeed = 10f;
    public float jumpForce = 10f;
    public float jumpDuration = 2f;

    private bool canControlSpeed = true;
    private bool isJumping = false;
    private Rigidbody rb;
    private Vector3 initialPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        initialPosition = transform.position;
    }

    private void Update()
    {
        // Speed control
        float speedInput = canControlSpeed ? (Input.GetKey(KeyCode.Space) ? 1f : 0f) : 0f;
        float currentSpeed = speedInput * swimSpeed;
        rb.velocity = -transform.up * currentSpeed;

        // Pitch rotation
        float pitchInput = Input.GetAxis("Vertical");
        Vector3 rotation = new Vector3(pitchInput * rotationSpeed, 0f, 0f);
        transform.Rotate(rotation);

        // Yaw rotation
        float yawInput = Input.GetAxis("Horizontal");
        rotation = new Vector3(0f, yawInput * rotationSpeed, 0f);
        transform.Rotate(rotation, Space.World);

        // Parabolic jump
        if (transform.position.z > 0f && !isJumping)
        {
            isJumping = true;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            rb.useGravity = true; // Enable gravity during jump
            StartCoroutine(ResetJumpCoroutine());
        }
        else if (transform.position.z <= 0f && isJumping && rb.useGravity)
        {
            isJumping = false;
            rb.useGravity = false; // Disable gravity when back underwater
            rb.velocity = Vector3.zero; // Reset velocity to prevent unintended movement
        }
    }

    private System.Collections.IEnumerator ResetJumpCoroutine()
    {
        yield return new WaitForSeconds(jumpDuration);
        isJumping = false;
    }

    private void FixedUpdate()
    {
        // Adjust pitch rotation based on velocity during the jump
        if (isJumping)
        {
            Vector3 velocity = rb.velocity;
            Vector3 tangent = new Vector3(velocity.x, 0f, velocity.z).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(tangent);
            rb.MoveRotation(targetRotation);
        }
    }
}
