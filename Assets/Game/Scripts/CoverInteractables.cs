using UnityEngine;

public class CoverInteractables : Interactable
{
    public enum WeaponType
    {
        Gun,
        BigGun,
        Length
    }

    public WeaponType weaponType;

    public override void Awake()
    {
        base.Awake();
        
        _basicLayer = gameObject.layer;
        typeOfInteractable = TypeOfInteractable.Cover;
    }
    
    public override void OnTriggerStay(Collider other)
    {
        base.OnTriggerStay(other);

        if (CompareTag("Cover"))
        {
            DisableOutline();
            
            // IGNORE RAYCAST TO THIS OBJECT
        }
    }

    public override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
    }
}
