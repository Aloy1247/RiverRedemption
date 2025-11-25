using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Attach this script to your Player object (PlayerCapsule).
/// It keeps track of which hazard types the player has unlocked.
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    // A HashSet is a very fast way to check if a value exists in a list.
    // This will store all the HazardTypes the player is allowed to pick up.
    public HashSet<HazardType> unlockedHazardTypes = new HashSet<HazardType>();

    // --- This is the "permission slip" for the cart ---
    public bool hasUnlockedCart = false;
    // ---

    /// <summary>
    /// Call this to add a new hazard type to the player's "unlocked" list.
    /// </summary>
    public void UnlockHazardType(HazardType type)
    {
        if (!unlockedHazardTypes.Contains(type))
        {
            unlockedHazardTypes.Add(type);
            Debug.Log($"Player has unlocked: {type}");
        }
    }

    /// <summary>
    //  Checks if the player is allowed to pick up a specific hazard type.
    /// </summary>
    public bool CanPickUpHazard(HazardType type)
    {
        return unlockedHazardTypes.Contains(type);
    }

    // --- Methods for the cart ---

    /// <summary>
    /// Call this from the shop to permanently unlock cart pushing.
    /// </summary>
    public void UnlockCart()
    {
        hasUnlockedCart = true;
        Debug.Log("Player has unlocked the cart!");
    }

    /// <summary>
    /// Checks if the player is allowed to push carts.
    /// </summary>
    public bool CanPushCart()
    {
        return hasUnlockedCart;
    }
    // ---
}