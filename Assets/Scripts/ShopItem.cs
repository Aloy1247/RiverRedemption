using UnityEngine;

public class ShopItem : MonoBehaviour
{
    public enum ItemType { HazardUnlock, SpawnableItem }

    public ItemType itemType;
    public int cost = 5;

    [Header("Hazard Unlock Settings")]
    public HazardType unlockType;

    [Header("Spawnable Item Settings")]
    public GameObject itemPrefab;
    public Transform spawnPoint;

    // --- NEW: Sound Effects ---
    [Header("Audio")]
    public AudioClip buySound;
    public AudioClip errorSound; // Sound if you can't afford it
    // ---

    public void TryToBuy()
    {
        if (DropOffZone.Instance.TrySpendScore(cost))
        {
            // --- Play Buy Sound (at the item's position) ---
            if (buySound != null)
            {
                AudioSource.PlayClipAtPoint(buySound, transform.position);
            }
            // ---

            switch (itemType)
            {
                case ItemType.HazardUnlock:
                    PurchaseHazardUnlock();
                    break;

                case ItemType.SpawnableItem:
                    PurchaseSpawnableItem();
                    break;
            }
        }
        else
        {
            Debug.Log("Purchase failed.");
            // --- Play Error Sound ---
            if (errorSound != null)
            {
                AudioSource.PlayClipAtPoint(errorSound, transform.position);
            }
            // ---
        }
    }

    void PurchaseHazardUnlock()
    {
        PlayerInventory inventory = FindObjectOfType<PlayerInventory>();
        if (inventory != null)
        {
            inventory.UnlockHazardType(unlockType);
            Destroy(this.gameObject); 
        }
    }

    void PurchaseSpawnableItem()
    {
        if (itemPrefab == null || spawnPoint == null)
        {
            DropOffZone.Instance.AddScore(cost); // Refund
            return;
        }

        Instantiate(itemPrefab, spawnPoint.position, spawnPoint.rotation);

        PlayerInventory inventory = FindObjectOfType<PlayerInventory>();
        if (inventory != null)
        {
            inventory.UnlockCart(); 
        }

        Destroy(this.gameObject); 
    }
}