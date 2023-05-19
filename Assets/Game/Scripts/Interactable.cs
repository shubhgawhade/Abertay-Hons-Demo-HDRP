using UnityEngine;

// [RequireComponent(typeof (Outline))]
public class Interactable : MonoBehaviour
{
    public enum TypeOfInteractable
    {
        Inspectable,
        Scripted,
        Cover,
    }

    public TypeOfInteractable typeOfInteractable = TypeOfInteractable.Scripted;
    public LayerMask ignoreLayer;


    public GameObject targetLocation;
    public GameObject aiTargetLocation;
    [SerializeField] public GameObject character;
    public bool isVisible;
    public int minIntel;
    public int rewardIntel;
    
    protected RaycastHit hit;
    protected int _basicLayer;

    [SerializeField] private LayerMask outlineColour;

    
    public bool isOccupied;

    //TEST VARIABLES
    [Header("TEST VARIABLE")]
    public bool testRay;

    public virtual void Awake()
    {
        _basicLayer = gameObject.layer;
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        
    }

    public virtual void OnTriggerStay(Collider other)
    {
        if (other.transform.root.CompareTag("Player") && other.transform.root.GetComponent<CharacterControl>().characterState == CharacterControl.CharacterState.Exploration)
        {
            if (Physics.Raycast(character.transform.position, transform.position - character.transform.position, out hit, Mathf.Infinity, ~ignoreLayer) && hit.collider.gameObject == gameObject &&
                GameManager.Intelligence >= minIntel && !isVisible)
            {
                // print(hit.collider.name);
                EnableOutline();
            }
            else if ((Physics.Raycast(character.transform.position, transform.position - character.transform.position, out hit, Mathf.Infinity, ~ignoreLayer) && hit.collider.gameObject != gameObject) || GameManager.Intelligence < minIntel)
            {
                // print(hit.collider.name);
                DisableOutline();
            }
            else
            {
            }
        }
        else if (!isVisible)
        {
            // DisableOutline();
        }
    }

    public virtual void OnTriggerExit(Collider other)
    {
        if (other.transform.root.CompareTag("Player") && isVisible)
        {
            DisableOutline();
        }
    }

    protected void EnableOutline()
    {
        isVisible = true;

        MeshRenderer[] tempMeshs = transform.GetComponentsInChildren<MeshRenderer>();
        SkinnedMeshRenderer[] skinnedMeshRenderers = transform.GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (MeshRenderer meshRenderer in tempMeshs)
        {
            meshRenderer.gameObject.layer = (int)Mathf.Log(outlineColour, 2);
        }
        
        foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinnedMeshRenderers)
        {
            skinnedMeshRenderer.gameObject.layer = (int)Mathf.Log(outlineColour, 2);
        }
    }
    
    protected void DisableOutline()
    {
        isVisible = false;
        MeshRenderer[] tempMeshs = transform.GetComponentsInChildren<MeshRenderer>();
        SkinnedMeshRenderer[] skinnedMeshRenderers = transform.GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (MeshRenderer meshRenderer in tempMeshs)
        {
            meshRenderer.gameObject.layer = _basicLayer;
        }
        
        foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinnedMeshRenderers)
        {
            skinnedMeshRenderer.gameObject.layer = _basicLayer;
        }
    }
    
    private void OnDrawGizmos()
    {
        if (GameManager.TestRay && testRay)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(character.transform.position, transform.position);
        }
    }
}
