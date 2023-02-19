using UnityEngine;

public class CoverInteractables : Interactable
{
    public override void Awake()
    {
        base.Awake();
        
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
