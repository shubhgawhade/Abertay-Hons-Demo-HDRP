using UnityEngine;

[RequireComponent(typeof (TextReader))]
public class ScriptedInteractables : Interactable
{
    // Start is called before the first frame update
    void Start()
    {
        typeOfInteractable = TypeOfInteractable.Scripted;
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
