using UnityEngine;

/// <summary>
/// Plays footstep sounds when the player moves.
/// Attach this to your Player object (PlayerCapsule).
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class PlayerFootsteps : MonoBehaviour
{
    [Header("Audio Settings")]
    [Tooltip("Drag your footstep audio clips here. Adding multiple adds variety.")]
    public AudioClip[] footstepSounds;
    
    [Range(0f, 1f)] 
    public float volume = 0.5f;

    [Header("Step Settings")]
    [Tooltip("How fast to play sounds. Lower = faster steps.")]
    public float stepInterval = 0.5f; 
    
    [Tooltip("Minimum speed required to play sound.")]
    public float velocityThreshold = 2.0f;

    private CharacterController controller;
    private AudioSource audioSource;
    private float stepTimer;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();

        if (controller == null)
        {
            Debug.LogError("PlayerFootsteps: Could not find CharacterController on this object!");
        }
    }

    void Update()
    {
        // If we don't have a controller or aren't touching the ground, don't play sounds.
        if (controller == null || !controller.isGrounded) return;

        // Check if the player is moving fast enough (ignoring small jitters)
        // We use horizontal magnitude to ignore falling speed
        Vector3 horizontalVelocity = new Vector3(controller.velocity.x, 0, controller.velocity.z);

        if (horizontalVelocity.magnitude > velocityThreshold)
        {
            // Countdown the timer
            stepTimer -= Time.deltaTime;

            // When timer hits 0, play a sound
            if (stepTimer <= 0)
            {
                PlayFootstep();
                stepTimer = stepInterval; // Reset timer
            }
        }
        else
        {
            // If we stop moving, reset the timer so the sound plays immediately next time we move
            stepTimer = 0;
        }
    }

    void PlayFootstep()
    {
        if (footstepSounds.Length == 0) return;

        // Pick a random sound from the array to make it sound natural
        int index = Random.Range(0, footstepSounds.Length);
        
        // Use PlayOneShot so it doesn't cut off other sounds (like taking damage)
        audioSource.PlayOneShot(footstepSounds[index], volume);
    }
}