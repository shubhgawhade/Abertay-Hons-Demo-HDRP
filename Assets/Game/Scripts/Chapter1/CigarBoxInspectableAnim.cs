using UnityEngine;

public class CigarBoxInspectableAnim : MonoBehaviour
{
    private InspectableInteractables cigarBox;
    private Animator anim;

    private void Awake()
    {
        cigarBox = GetComponent<InspectableInteractables>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cigarBox.studioSetupComplete)
        {
         
            anim = cigarBox.tempstudioModel.GetComponent<Animator>();
            //PLAY OPEN ANIMATION
            anim.enabled = true;
        }
        else
        {
            if (anim)
            {
                anim.enabled = false;
            }
        }
    }
}
