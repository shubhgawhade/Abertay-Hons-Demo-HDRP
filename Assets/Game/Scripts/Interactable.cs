using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;


[RequireComponent(typeof (TextReader))]
// [RequireComponent(typeof (Outline))]
public class Interactable : MonoBehaviour
{
    public GameObject targetLocation;
    public bool isVisible;

    [SerializeField] private GameObject player;
    
    private int _basicLayer;
    private RaycastHit hit;

    public int minIntel;
    public int playerIntelChange;
    
    //TEST VARIABLES
    private bool testRay;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _basicLayer = gameObject.layer;
    }
    
    private void OnTriggerStay(Collider other)
    {
        // if (other.CompareTag("Player") && GameManager.IsMoveable)
        if (other.CompareTag("Player") && other.GetComponent<AICharacterControl>().characterState == AICharacterControl.CharacterState.Exploration)
        {
            if (Physics.Raycast(player.transform.position, transform.position - player.transform.position, out hit) && hit.collider.CompareTag("Interactable") &&
                GameManager.Intelligence >= minIntel)
            {
                // testRay = true;
                print(hit.collider.name);
                isVisible = true;
                transform.Find("Mesh").gameObject.layer = LayerMask.NameToLayer("Outline");
            }
            else if (!hit.collider.CompareTag("Interactable"))
            {
                isVisible = false;
                transform.Find("Mesh").gameObject.layer = _basicLayer;
            }
            else
            {
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // isinteracted = false;
            // testRay = false;
            print("EXIT");

            isVisible = false;
            transform.Find("Mesh").gameObject.layer = _basicLayer;

            //OLD OUTLINE
            // outline.OutlineMode = Outline.Mode.OutlineHidden;
            // outline.needsUpdate = true;
        }
    }

    /*
    private void OnDrawGizmos()
    {
        // if (testRay)
        {
            Gizmos.DrawLine(player.transform.position, transform.position);
        }
    }
    */
}
