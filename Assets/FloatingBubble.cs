using UnityEngine;

public class FloatingBubble : MonoBehaviour
{
    public float verticalSpeed = 1f;     // The speed at which the bubble moves vertically
    public float horizontalRange = 1f;   // The range of horizontal movement
    public float horizontalSpeed = 1f;   // The speed at which the bubble moves horizontally

    private Vector3 initialPosition;     // The initial position of the bubble
    private float horizontalOffset;      // The horizontal offset of the bubble

    private void Start()
    {
        initialPosition = transform.position;
        horizontalOffset = 0f;
    }

    private void Update()
    {
        // Calculate the new Y position based on vertical speed
        float newY = transform.position.y + verticalSpeed * Time.deltaTime;

        // Calculate the new X position based on horizontal movement
        float newX = initialPosition.x + Mathf.Sin(horizontalOffset) * horizontalRange;

        // Calculate the new position with the updated X and Y
        Vector3 newPosition = new Vector3(newX, newY, initialPosition.z);

        // Move the bubble
        transform.position = newPosition;

        // Update the horizontal offset for the next frame
        horizontalOffset += horizontalSpeed * Time.deltaTime;
    }
}

