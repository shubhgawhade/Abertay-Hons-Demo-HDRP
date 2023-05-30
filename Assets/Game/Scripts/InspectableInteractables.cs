using System;
using UnityEngine;

public class InspectableInteractables : Interactable
{
    [SerializeField] private GameObject chapterManager;
    [SerializeField] private Camera studioCam;
    [SerializeField] private CameraZoom cameraZoom;
    [SerializeField] private Transform studioTransform;
    [SerializeField] private GameObject model;

    public bool studioSetupComplete;
    public bool isRotateable;
    public int unlockContent = -1;
    [Space(10)]
    [Header("DIALOGUE OPTIONS")]
    [SerializeField] private bool hasDialogue;
    private TextReader textReader;

    private Interactable interactable;
    private GameObject temp;
    private Bounds bounds;
    private Vector3 center;

    public static Action<Transform> RemoveCinemachineTarget;
    public static Action<int> Unlock;

    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();

        typeOfInteractable = TypeOfInteractable.Inspectable;
        interactable = GetComponent<Interactable>();

        if (hasDialogue)
        {
            textReader = GetComponent<TextReader>();
            textReader.LoadScript();
        }
    }

    public void SetupStudio()
    {
        studioTransform.gameObject.SetActive(true);
        // cameraZoom.enabled = false;
        
        temp = Instantiate(model, studioTransform.position, Quaternion.identity);
        GetBoundsWithChildren(temp);
        center = bounds.center;
        // temp.transform.position = new Vector3(temp.transform.position.x, temp.transform.position.y - mr.bounds.center.y, temp.transform.position.z);
        temp.transform.LookAt(studioCam.transform.position);
        temp.transform.eulerAngles = new Vector3(0, temp.transform.eulerAngles.y, 0);
        
        if (bounds.extents.x > bounds.extents.y)
        {
            print($"{temp.name} is WIDER");
            float distanceFromObject = -bounds.extents.x;
            studioCam.transform.position = new Vector3(0, center.y, distanceFromObject - 1.5f);
        }
        else
        {
            print($"{temp.name} is TALLER");
            float distanceFromObject = -bounds.extents.y / Mathf.Tan(studioCam.fieldOfView * 0.5f * Mathf.Deg2Rad);
            studioCam.transform.position = new Vector3(0, center.y, distanceFromObject - 1.5f);
        }

        if (hasDialogue)
        {
            textReader.ToggleUI();
        }
        
        studioSetupComplete = true;
    }
    
    private void GetBoundsWithChildren(GameObject obj)
    {
        Renderer parentRenderer = obj.GetComponent<Renderer>();
        Renderer[] childrenRenderers = obj.GetComponentsInChildren<Renderer>();

        if (parentRenderer)
        {
            bounds = parentRenderer.bounds;
        }
        else
        {
            Renderer first = null;
            foreach (var renderer in childrenRenderers)
            {
                if (renderer.enabled)
                {
                    first = renderer;
                    break;
                }
            }

            bounds = first.bounds;
        }
 
        if (childrenRenderers.Length > 0)
        {
            foreach (Renderer renderer in childrenRenderers)
            {
                if (renderer.enabled)
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }
        }
    }

    private void Update()
    {
        if (studioSetupComplete && isRotateable)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            if (horizontalInput != 0)
            {
                temp.transform.RotateAround(center, -Vector3.up, horizontalInput);
            }

            if (verticalInput != 0)
            {
                temp.transform.RotateAround(center, Vector3.right, verticalInput);

            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                StopInspecting();
            }
        }
    }

    private void StopInspecting()
    {
        if (GameManager.IsInteracting)
        {
            RemoveCinemachineTarget(interactable.targetLocation.transform);
            GameManager.IsInteracting = false;
            isOccupied = false;
        }

        if (!alreadyInteracted)
        {
            if (unlockContent != -1)
            {
                Unlock(unlockContent);
            }
            
            GameManager.Intelligence += interactable.rewardIntel;
            alreadyInteracted = true;
        }

        if (hasDialogue && textReader.dialogueTracker != 0)
        {
            textReader.EndDialogue();
        }
        
        studioTransform.gameObject.SetActive(false);
        Destroy(temp);
        studioSetupComplete = false;
    }
    
    public override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);

        
        if (other.transform.root.CompareTag("Player") && studioSetupComplete)
        {
            if (hasDialogue && textReader.dialogueTracker != 0)
            {
                textReader.EndDialogue();
            }
            
            StopInspecting();
        }
    }
}
