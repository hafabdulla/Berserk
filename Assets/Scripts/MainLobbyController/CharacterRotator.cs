using UnityEngine;

public class CharacterRotator : MonoBehaviour
{
    public float rotationSpeed = 20f; // Degrees per second
    public bool rotateOnY = true; // Rotate around Y axis (left/right)

    void Update()
    {
        if (rotateOnY)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }
}