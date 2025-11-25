using UnityEngine;
using System;

[RequireComponent(typeof(AudioSource))] // Ensure we have an AudioSource
public class PlayerHealth : MonoBehaviour
{
    public event Action<int, int> OnHealthChanged;

    public int maxHealth = 100;
    public int currentHealth;
    public Vector3 respawnPoint = new Vector3(0, 1, 0);

    // --- NEW: Sound Effects ---
    [Header("Audio")]
    public AudioClip damageSound;
    public AudioClip deathSound;
    private AudioSource audioSource;
    // ---

    private CharacterController controller;

    void Start()
    {
        currentHealth = maxHealth;
        TryGetComponent<CharacterController>(out controller);
        
        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        
        // --- Play Damage Sound ---
        if (audioSource != null && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }
        // ---

        Debug.Log("Player took " + damageAmount + " damage.");
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // --- Play Death Sound ---
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }
        // ---

        Debug.Log("Player has died.");

        if (controller != null)
        {
            controller.enabled = false;
            transform.position = respawnPoint;
            controller.enabled = true;
        }
        else
        {
            transform.position = respawnPoint;
        }

        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}