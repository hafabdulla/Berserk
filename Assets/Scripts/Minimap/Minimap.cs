using UnityEngine;

public class MinimapIcon : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("The object this icon represents")]
    public Transform target;

    [Header("Icon Settings")]
    [Tooltip("Height above the play area where icon floats")]
    public float iconHeight = 50f;

    [Tooltip("Should the icon rotate with the target? (useful for player)")]
    public bool rotateWithTarget = false;

    void Start()
    {
        // If target not set, try to find parent
        if (target == null)
        {
            target = transform.parent;
        }

        // Set to Minimap layer
        gameObject.layer = LayerMask.NameToLayer("Minimap");
    }

    void LateUpdate()
    {
        if (target != null)
        {
            // Follow target's X and Z position, but stay at fixed height
            transform.position = new Vector3(
                target.position.x,
                iconHeight,
                target.position.z
            );

            // Optional: Rotate icon to match target's Y rotation
            if (rotateWithTarget)
            {
                transform.rotation = Quaternion.Euler(90, target.eulerAngles.y, 0);
            }
        }
    }
}