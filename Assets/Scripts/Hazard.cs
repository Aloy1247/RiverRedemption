using UnityEngine;

/// <summary>
/// This enum defines all the different types of hazards in your game.
/// You can add more types to this list.
/// </summary>
public enum HazardType
{
    Biohazard,
    Sharp,
    Radioactive,
    Electric,
    Heavy
    // Add more as needed
}

/// <summary>
/// Attach this script to any "dangerous" object.
/// You can now select what type of hazard it is in the Inspector.
/// </summary>
public class Hazard : MonoBehaviour
{
    [Tooltip("What kind of hazard is this?")]
    public HazardType hazardType;

    [Tooltip("How much damage it deals if picked up without an upgrade.")]
    public int damageAmount = 25;

    // --- NEW FIELD ---
    [Tooltip("How many coins this is worth when sold.")]
    public int coinValue = 5; // Default value for hazards
    // --- END OF NEW FIELD ---
}