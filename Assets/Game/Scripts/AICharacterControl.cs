using System.Collections.Generic;
using UnityEngine;

public class AICharacterControl : CharacterControl
{
    [SerializeField] private GameObject player;
    [SerializeField] private List<Interactable> gunplayLocations;

    public bool isFriendly;

    protected override bool ToggleCrouch()
    {
        // KEY TO TOGGLE CROUCH
        if (Input.GetMouseButtonDown(0))
        {
            crouch = !crouch;
        }
        
        return base.ToggleCrouch();
    }

    protected override void Update()
    {
        base.Update();
        
        AIUpdate();
        
        // CODE FOR AI TO CHOOSE LOCATIONS TO MOVE TO AND SELECT COVER POSITIONS
        // BASED ON THE SITUATION SHOULD THE AI BE LOOKING FOR THE PLAYER, IN COMBAT OR PATROLLING
        
        // CODE TO SELECT A RANDOM COVER LOCATION TO GO TO
        if (Input.GetMouseButtonDown(0))
        {
            // crouch = false;
            // characterState = CharacterState.Exploration;
            // int location = Random.Range(0, gunplayLocations.Count);
            // targetTransform = gunplayLocations[location].targetLocation.transform;
            // currentInteractable = gunplayLocations[location];
            // targetIsInteractable = true;
        }
    }

    private void AIUpdate()
    {
        switch (characterState)
        {
            // CODE FOR AI TO CHOOSE SHOOTING LOCATION 
            case CharacterState.Gunplay:

                
                // transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
                // bulletTarget.transform.position = hit.point;
                //
                // if (Input.GetKeyDown(KeyCode.LeftControl)) 
                // {
                //     GameManager.IsInteracting = false;
                //     characterState = CharacterState.Exploration;
                //     weapon.gameObject.SetActive(false);
                //     anim.SetBool("GunDrawn", false);
                //     currentInteractable.transform.root.tag = "Interactable";
                //     cinemachineTarget.RemoveMember(currentInteractable.targetLocation.transform);
                // }
                
                break;
        }
    }

    protected override void InteractableTypeBehaviour()
    {
        isInteracting = true;
        base.InteractableTypeBehaviour();
    }

    protected void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Interactable":
                
                Interactable tempInteractable = other.GetComponent<Interactable>();
                if (tempInteractable.typeOfInteractable == Interactable.TypeOfInteractable.Cover)
                {
                    gunplayLocations.Add(tempInteractable);
                }

                break;
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        
        switch (other.tag)
        {
            case "Interactable":
                
                Interactable tempInteractable = other.GetComponent<Interactable>();
                if (tempInteractable.typeOfInteractable == Interactable.TypeOfInteractable.Cover)
                {
                    gunplayLocations.Remove(tempInteractable);
                }

                break;
        }
    }

    protected override void OnDrawGizmos()
    {
        // Gizmos.color = Color.green;
        // Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
