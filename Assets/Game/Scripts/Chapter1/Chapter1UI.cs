using UnityEngine;

public class Chapter1UI : MonoBehaviour
{
    [SerializeField] private CharacterControl playerCharacterControl;

    public void StopInspecting()
    {
        playerCharacterControl.currentInteractable.GetComponent<InspectableInteractables>().StopInspecting();
    }
}
