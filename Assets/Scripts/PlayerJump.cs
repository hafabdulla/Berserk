using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    public AudioController audioController;

    private CharacterController controller;
    private bool wasGrounded = true;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        bool grounded = controller.isGrounded;

        // Trigger landing when transitioning from air to ground
        if (grounded && !wasGrounded)
        {
            Debug.Log("Player landed, playing sound.");
            audioController.playLandingSound();
        }

        wasGrounded = grounded;
    }
}
