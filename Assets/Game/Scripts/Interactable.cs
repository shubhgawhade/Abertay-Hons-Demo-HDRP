using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityStandardAssets.Characters.ThirdPerson;


// [RequireComponent(typeof (Outline))]
public class Interactable : MonoBehaviour
{
    public enum TypeOfInteractable
    {
        Unscripted,
        Scripted,
        Cover,
    }

    public TypeOfInteractable typeOfInteractable = TypeOfInteractable.Unscripted;

    
    public GameObject targetLocation;
    [SerializeField] protected GameObject player;
    public bool isVisible;
    public int minIntel;
    public int rewardIntel;
    
    protected RaycastHit hit;
    protected int _basicLayer;

    [SerializeField] private LayerMask outlineColour;

    //TEST VARIABLES
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
            if (Physics.Raycast(player.transform.position, transform.position - player.transform.position, out hit) && hit.collider.gameObject == gameObject &&
                GameManager.Intelligence >= minIntel && !isVisible)
            {
                // print(hit.collider.name);
                isVisible = true;

                MeshRenderer[] tempMeshs = transform.GetComponentsInChildren<MeshRenderer>();

                foreach (MeshRenderer meshRenderer in tempMeshs)
                {
                    meshRenderer.gameObject.layer = (int)Mathf.Log(outlineColour, 2);
                }
            }
            if (Physics.Raycast(player.transform.position, transform.position - player.transform.position, out hit) && hit.collider.gameObject != gameObject)
            {
                isVisible = false;
                MeshRenderer[] tempMeshs = transform.GetComponentsInChildren<MeshRenderer>();

                foreach (MeshRenderer meshRenderer in tempMeshs)
                {
                    meshRenderer.gameObject.layer = _basicLayer;
                }
            }
            else
            {
            }
        }
    }

    public virtual void OnTriggerExit(Collider other)
    {
        if (other.transform.root.CompareTag("Player") && isVisible)
        {
            // isinteracted = false;
            // testRay = false;
            // print("EXIT");

            isVisible = false;
                
            MeshRenderer[] tempMeshs = transform.GetComponentsInChildren<MeshRenderer>();

            foreach (MeshRenderer meshRenderer in tempMeshs)
            {
                meshRenderer.gameObject.layer = _basicLayer;
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        if (GameManager.TestRay && testRay)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(player.transform.position, transform.position);
        }
    }
}
