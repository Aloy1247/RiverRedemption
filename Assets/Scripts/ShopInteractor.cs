using UnityEngine;

/// <summary>
/// Attach this script to your Player object (PlayerCapsule).
/// It allows the player to "interact" with shop items by looking
/// at them and pressing a key.
/// </summary>
public class ShopInteractor : MonoBehaviour
{
    [Tooltip("Assign your player's main camera (the one that moves with the mouse).")]
    public Camera playerCamera;

    [Tooltip("How close the player needs to be to buy an item.")]
    public float interactRange = 5f;

    [Tooltip("The key the player presses to buy.")]
    public KeyCode buyKey = KeyCode.B;

    void Update()
    {
        // Check if the player pressed the buy key
        if (Input.GetKeyDown(buyKey))
        {
            TryToBuy();
        }
    }

    void TryToBuy()
    {
        RaycastHit hit;
        // Shoot a raycast from the camera, just like the pickup script
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, interactRange))
        {
            // Check if the object we're looking at is a ShopItem
            if (hit.transform.TryGetComponent<ShopItem>(out ShopItem item))
            {
                // It is! Call its TryToBuy method.
                Debug.Log($"Trying to buy {item.name}...");
                item.TryToBuy();
            }
        }
    }
}