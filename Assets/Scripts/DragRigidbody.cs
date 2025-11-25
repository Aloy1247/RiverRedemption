using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))] // Add AudioSource to the Trash object
public class DragRigidbodyWithHealth : MonoBehaviour
{
    public float force = 600;
    public float damping = 6;
    public float distance = 15;

    public LineRenderer lr;
    public Transform lineRenderLocation;

    Transform jointTrans;
    float dragDepth;

    // --- Audio Settings ---
    [Header("Audio")]
    public AudioClip pickupSound;
    public AudioClip dropSound;
    public AudioClip errorSound; // Sound when you touch a hazard without upgrade
    private AudioSource audioSource;
    // ---

    private PlayerHealth playerHealth;
    private PlayerInventory playerInventory; 

    void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Get the audio source
        playerHealth = FindObjectOfType<PlayerHealth>();
        playerInventory = FindObjectOfType<PlayerInventory>(); 

        if (playerHealth == null) Debug.LogError("DragRigidbody: Could not find PlayerHealth!");
        if (playerInventory == null) Debug.LogError("DragRigidbody: Could not find PlayerInventory!");
    }

    void OnMouseDown()
    {
        HandleInputBegin(Input.mousePosition);
    }

    void OnMouseUp()
    {
        HandleInputEnd(Input.mousePosition);
    }

    void OnMouseDrag()
    {
        HandleInput(Input.mousePosition);
    }

    public void HandleInputBegin(Vector3 screenPosition)
    {
        var ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, distance))
        {
            // Ignore clicks on the cart (Pushables)
            if (hit.transform.GetComponent<Pushable>() != null)
            {
                return; 
            }

            // --- Hazard Check ---
            bool canPickUp = false; 

            if (hit.transform.TryGetComponent<Hazard>(out Hazard hazard))
            {
                if (playerInventory != null && playerInventory.CanPickUpHazard(hazard.hazardType))
                {
                    Debug.Log($"Player has upgrade for {hazard.hazardType}. Picking up.");
                    canPickUp = true;
                }
                else
                {
                    Debug.Log($"Dangerous! Player needs the {hazard.hazardType} upgrade.");
                    if (playerHealth != null)
                    {
                        playerHealth.TakeDamage(hazard.damageAmount);
                    }
                    
                    // --- Play Error Sound ---
                    if (audioSource != null && errorSound != null) 
                    {
                        audioSource.PlayOneShot(errorSound);
                    }
                    // ------------------------

                    canPickUp = false;
                    return; 
                }
            }
            else
            {
                canPickUp = true;
            }
            
            if (canPickUp && hit.transform.gameObject.layer == LayerMask.NameToLayer("Interactive"))
            {
                dragDepth = CameraPlane.CameraToPointDepth(Camera.main, hit.point);
                jointTrans = AttachJoint(hit.rigidbody, hit.point);

                // --- Play Pickup Sound ---
                if (audioSource != null && pickupSound != null) 
                {
                    audioSource.PlayOneShot(pickupSound);
                }
                // -------------------------
            }
        }

        lr.positionCount = 2;
    }

    public void HandleInput(Vector3 screenPosition)
    {
        if (jointTrans == null) return;
        var worldPos = Camera.main.ScreenToWorldPoint(screenPosition);
        jointTrans.position = CameraPlane.ScreenToWorldPlanePoint(Camera.main, dragDepth, screenPosition);
        DrawRope();
    }

    public void HandleInputEnd(Vector3 screenPosition)
    {
        DestroyRope();
        if (jointTrans != null)
        {
            Destroy(jointTrans.gameObject);
            
            // --- Play Drop Sound ---
            // (Only play if we were actually holding something)
            if (audioSource != null && dropSound != null) 
            {
                audioSource.PlayOneShot(dropSound);
            }
            // -----------------------
        }
    }

    Transform AttachJoint(Rigidbody rb, Vector3 attachmentPosition)
    {
        GameObject go = new GameObject("Attachment Point");
        go.hideFlags = HideFlags.HideInHierarchy;
        go.transform.position = attachmentPosition;

        var newRb = go.AddComponent<Rigidbody>();
        newRb.isKinematic = true;

        var joint = go.AddComponent<ConfigurableJoint>();
        joint.connectedBody = rb;
        joint.configuredInWorldSpace = true;
        // ... (Joint drive settings remain the same)
        joint.xDrive = NewJointDrive(force, damping);
        joint.yDrive = NewJointDrive(force, damping);
        joint.zDrive = NewJointDrive(force, damping);
        joint.slerpDrive = NewJointDrive(force, damping);
        joint.rotationDriveMode = RotationDriveMode.Slerp;

        return go.transform;
    }

    private JointDrive NewJointDrive(float force, float damping)
    {
        JointDrive drive = new JointDrive();
        drive.mode = JointDriveMode.Position;
        drive.positionSpring = force;
        drive.positionDamper = damping;
        drive.maximumForce = Mathf.Infinity;
        return drive;
    }

    private void DrawRope()
    {
        if (jointTrans == null) return;
        lr.SetPosition(0, lineRenderLocation.position);
        lr.SetPosition(1, this.transform.position);
    }

    private void DestroyRope()
    {
        lr.positionCount = 0;
    }
}