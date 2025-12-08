using UnityEngine;

public class MinimapCameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("The player object to follow")]
    public Transform player;

    [Header("Camera Settings")]
    [Tooltip("Height of camera above the player")]
    public float cameraHeight = 50f;

    [Tooltip("Should camera rotate with player?")]
    public bool rotateWithPlayer = false;

    [Header("Smoothing (Optional)")]
    [Tooltip("Enable smooth following")]
    public bool smoothFollow = false;

    [Tooltip("Smoothing speed (only if smoothFollow is enabled)")]
    public float smoothSpeed = 10f;

    void Start()
    {
        // Try to find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogWarning("MinimapCamera: Player not found! Please assign the player transform.");
            }
        }
    }

    void LateUpdate()
    {
        if (player == null) return;

        // Calculate target position (above player)
        Vector3 targetPosition = new Vector3(
            player.position.x,
            player.position.y + cameraHeight,
            player.position.z
        );

        // Move camera to follow player
        if (smoothFollow)
        {
            // Smooth following
            transform.position = Vector3.Lerp(
                transform.position,
                targetPosition,
                smoothSpeed * Time.deltaTime
            );
        }
        else
        {
            // Instant following
            transform.position = targetPosition;
        }

        // Optional: Rotate camera with player's Y rotation
        if (rotateWithPlayer)
        {
            // Keep the camera looking down but rotate around Y axis with player
            Quaternion targetRotation = Quaternion.Euler(
                90,
                player.eulerAngles.y,
                0
            );

            if (smoothFollow)
            {
                transform.rotation = Quaternion.Lerp(
                    transform.rotation,
                    targetRotation,
                    smoothSpeed * Time.deltaTime
                );
            }
            else
            {
                transform.rotation = targetRotation;
            }
        }
        else
        {
            // Keep camera always facing down, north-up
            transform.rotation = Quaternion.Euler(90, 0, 0);
        }
    }
}