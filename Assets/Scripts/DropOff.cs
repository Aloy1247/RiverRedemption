using UnityEngine;
using System;

[RequireComponent(typeof(AudioSource))] 
public class DropOffZone : MonoBehaviour
{
    public event Action<int> OnScoreChanged;
    // --- NEW: Event for winning ---
    public event Action OnLevelComplete;
    // ---

    public static DropOffZone Instance { get; private set; }
    private int score = 0; 

    [Header("Audio")]
    public AudioClip sellSound; 
    private AudioSource audioSource;

    // --- NEW: Trash Tracking ---
    [Header("Level Settings")]
    [Tooltip("Drag the empty GameObject that holds ALL your trash/hazards here.")]
    public Transform trashContainer;
    
    private int totalTrashCount;
    private int trashSold;
    // ---

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        // Count how many trash items exist at the start of the game
        if (trashContainer != null)
        {
            // We assume every child object in the container is a piece of trash
            totalTrashCount = trashContainer.childCount;
            Debug.Log($"Level Start: {totalTrashCount} items to clean up.");
        }
        else
        {
            Debug.LogWarning("DropOffZone: No Trash Container assigned! Win condition won't work.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        bool isTrash = false;

        if (other.TryGetComponent<Hazard>(out Hazard hazard))
        {
            AddScore(hazard.coinValue);
            isTrash = true;
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("trash"))
        {
            AddScore(1); 
            isTrash = true;
            Destroy(other.gameObject);
        }

        // --- NEW: Check Progress ---
        if (isTrash)
        {
            trashSold++;
            CheckWinCondition();
        }
        // ---------------------------
    }

    void CheckWinCondition()
    {
        // If we haven't assigned a container, don't do anything
        if (trashContainer == null) return;

        // Check if we sold everything
        if (trashSold >= totalTrashCount)
        {
            Debug.Log("Level Complete! All trash sold.");
            OnLevelComplete?.Invoke();
        }
    }

    public int GetScore()
    {
        return score;
    }

    public void AddScore(int amount)
    {
        score += amount;
        
        if (audioSource != null && sellSound != null)
        {
            audioSource.PlayOneShot(sellSound);
        }

        OnScoreChanged?.Invoke(score);
    }

    public bool TrySpendScore(int amount)
    {
        if (score >= amount)
        {
            score -= amount;
            OnScoreChanged?.Invoke(score);
            return true;
        }
        else
        {
            return false;
        }
    }
}