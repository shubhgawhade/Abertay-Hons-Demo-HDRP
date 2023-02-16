using UnityEngine;

public class CoverInteractables : Interactable
{
    void Start()
    {
        _basicLayer = gameObject.layer;
        typeOfInteractable = TypeOfInteractable.Cover;
    }
    
    public override void OnTriggerStay(Collider other)
    {
        base.OnTriggerStay(other);
    }

    public override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
    }
}
