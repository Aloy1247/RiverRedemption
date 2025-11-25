using UnityEngine;

/// <summary>
/// Attach this script to any object you want the player to be
/// able to push or pull with a physics joint, like a cart.
///
/// This object MUST have a Rigidbody component.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Pushable : MonoBehaviour
{
    // Public reference to the Rigidbody
    public Rigidbody rb;

    void Awake()
    {
        // Automatically find and assign the Rigidbody on this object
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Called by the player script when we attach or detach.
    /// We can use this later to change handling, add sounds, etc.
    /// </summary>
    /// <param name="isAttached"></param>
    public void SetPlayerAttached(bool isAttached)
    {
        if (isAttached)
        {
            Debug.Log("Cart attached to player!");
        }
        else
        {
            Debug.Log("Cart detached from player!");
        }
    }
}