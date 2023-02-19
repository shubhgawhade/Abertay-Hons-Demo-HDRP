using UnityEngine;

[RequireComponent(typeof (TextReader))]
public class ScriptedInteractables : Interactable
{
    private TextReader textReader;

    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
        
        textReader = GetComponent<TextReader>();
        typeOfInteractable = TypeOfInteractable.Scripted;
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (textReader.textAsset != null)
        {
            // textReader.LoadScript();
        }
        else
        {
            Debug.LogWarning($"{gameObject.name.ToUpper()} HAS NO SCRIPT ATTACHED");
        }
    }

    public override void OnTriggerStay(Collider other)
    {
        base.OnTriggerStay(other);
    }

    public override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        
        if (textReader.interactableStates == InteractableStates.Interacting)
        {
            textReader.EndDialogue();
        }
    }
}
