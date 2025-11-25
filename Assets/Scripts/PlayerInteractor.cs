using UnityEngine;

/// <summary>
/// This REPLACES ShopInteractor.cs. Attach this script to your Player object.
///
/// This script handles all player interactions:
/// - Pressing 'B' (Buy Key) to buy from Shop Items
/// - Pressing 'E' (Interact Key) to attach/detach from Pushable objects (like carts)
/// </summary>
public class PlayerInteractor : MonoBehaviour
{
    [Tooltip("Assign your player's main camera (the one that moves with the mouse).")]
    public Camera playerCamera;

    [Tooltip("How close the player needs to be to interact.")]
    public float interactRange = 5f;

    [Header("Interaction Keys")]
    [Tooltip("The key the player presses to buy.")]
    public KeyCode buyKey = KeyCode.B;
    
    [Tooltip("The key the player presses to push/pull or interact.")]
    public KeyCode interactKey = KeyCode.E;

    // --- Cart Pushing Fields ---
    private SpringJoint cartJoint;     // The physics joint connecting us to the cart
    private Pushable currentCart;   // A reference to the cart we are currently pushing

    // --- NEW: Reference to PlayerInventory ---
    private PlayerInventory playerInventory;

    void Start()
    {
        // Get the inventory component from this same Player object
        // We need this to check for the "permission slip"
        playerInventory = GetComponent<PlayerInventory>();
        if (playerInventory == null)
        {
            Debug.LogError("PlayerInteractor: Could not find PlayerInventory script on this object!");
        }
    }


    void Update()
    {
        // Check for the "Buy" key
        if (Input.GetKeyDown(buyKey))
        {
            TryToBuy();
        }

        // Check for the "Interact" key
        if (Input.GetKeyDown(interactKey))
        {
            TryToPushOrReleaseCart();
        }
    }

    /// <summary>
    /// This is the logic for buying from a ShopItem.
    /// </summary>
    void TryToBuy()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, interactRange))
        {
            if (hit.transform.TryGetComponent<ShopItem>(out ShopItem item))
            {
                Debug.Log($"Trying to buy {item.name}...");
                item.TryToBuy();
            }
        }
    }

    /// <summary>
    /// This is the logic for pushing/releasing the cart.
    /// </summary>
    void TryToPushOrReleaseCart()
    {
        // --- IF WE ARE ALREADY PUSHING A CART, RELEASE IT ---
        if (currentCart != null && cartJoint != null)
        {
            // We are attached, so let's detach
            currentCart.SetPlayerAttached(false);
            Destroy(cartJoint);
            cartJoint = null;
            currentCart = null;
            return;
        }

        // --- IF WE ARE NOT PUSHING A CART, TRY TO FIND ONE ---
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, interactRange))
        {
            // Check if the object we're looking at is a Pushable
            if (hit.transform.TryGetComponent<Pushable>(out Pushable pushable))
            {
                // --- THIS IS THE FIX ---
                // Before attaching, check if the player is allowed to push
                // (i.e. do they have the "permission slip"?)
                if (playerInventory != null && playerInventory.CanPushCart())
                {
                    // Player has bought the cart, proceed with attaching
                    Debug.Log("Cart unlocked, attaching to player.");
                    currentCart = pushable;
                    
                    cartJoint = gameObject.AddComponent<SpringJoint>();
                    cartJoint.connectedBody = currentCart.rb;
                    cartJoint.spring = 1000f;  
                    cartJoint.damper = 200f;   
                    cartJoint.minDistance = 0f;
                    cartJoint.maxDistance = 0.2f; 
                    cartJoint.enableCollision = false; 

                    currentCart.SetPlayerAttached(true);
                }
                else
                {
                    // Player has not bought the cart. Do nothing.
                    Debug.Log("You must buy the cart from the shop first!");
                }
                // --- END OF FIX ---
            }
        }
    }
}