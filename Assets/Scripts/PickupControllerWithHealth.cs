using UnityEngine;

/// <summary>
/// This is a sample pickup script, similar to the one in the video,
/// but modified to check for Hazards.
///
/// You need to compare this to YOUR existing pickup script and add the new logic.
/// </summary>
public class PickupControllerWithHealth : MonoBehaviour
{
    // --- Variables from the pickup video ---
    public Transform pickUpHolder; // The empty object that will hold the item
    public float pickUpRange = 5f;
    public float pickUpForce = 150f;
    private GameObject heldObject;
    private Rigidbody heldObjectRb;

    // --- NEW variables for the health system ---
    public PlayerHealth playerHealth; // Assign your Player object in the Inspector
    public Camera playerCamera; // Assign your player's camera

    void Update()
    {
        // --- This is the logic to drop an object (likely from your video) ---
        if (Input.GetKeyDown(KeyCode.F)) // Assuming 'F' is your drop key
        {
            if (heldObject != null)
            {
                DropObject();
            }
        }

        // --- This is the logic to pick up an object ---
        if (Input.GetKeyDown(KeyCode.E)) // Assuming 'E' is your pickup key
        {
            if (heldObject == null) // If we aren't holding anything
            {
                RaycastHit hit;
                if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, pickUpRange))
                {
                    // !--- NEW HEALTH CHECK LOGIC STARTS HERE ---!
                    //
                    // Check if the object we hit has the "Hazard" component
                    if (hit.collider.TryGetComponent<Hazard>(out Hazard hazard))
                    {
                        // It's dangerous! Take damage and DON'T pick it up.
                        Debug.Log("That's dangerous! Can't pick it up.");
                        playerHealth.TakeDamage(hazard.damageAmount);
                        return; // Stop the rest of the pickup logic
                    }
                    // !--- NEW HEALTH CHECK LOGIC ENDS HERE ---!


                    // --- Your existing pickup logic (from the video) ---
                    // We only run this if it's NOT a hazard.
                    // You might be checking for a tag like "trash" or a component.
                    // I'll assume you're checking for the "trash" tag like in your DropOffZone.
                    if (hit.collider.CompareTag("trash"))
                    {
                        Debug.Log("Picking up " + hit.collider.name);
                        PickUpObject(hit.collider.gameObject);
                    }
                }
            }
        }

        // --- Logic to move the held object (from the video) ---
        if (heldObject != null)
        {
            MoveObject();
        }
    }

    // --- Method from the video ---
    void PickUpObject(GameObject obj)
    {
        if (obj.TryGetComponent<Rigidbody>(out heldObjectRb))
        {
            heldObject = obj;
            heldObjectRb.useGravity = false;
            heldObjectRb.linearDamping = 10;
            heldObjectRb.constraints = RigidbodyConstraints.FreezeRotation;
            heldObjectRb.transform.parent = pickUpHolder;
            heldObject = obj;
        }
    }

    // --- Method from the video ---
    void DropObject()
    {
        heldObjectRb.useGravity = true;
        heldObjectRb.linearDamping = 1;
        heldObjectRb.constraints = RigidbodyConstraints.None;
        heldObject.transform.parent = null;
        heldObject = null;
    }

    // --- Method from the video ---
    void MoveObject()
    {
        // This logic might be different in your video, but it moves the object
        heldObject.transform.position = pickUpHolder.position;
    }
}