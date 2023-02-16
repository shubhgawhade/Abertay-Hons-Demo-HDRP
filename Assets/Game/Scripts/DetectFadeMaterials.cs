using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

//ARRAY WITH ALL FLOORS HAVING THIS VOLUME TRIGGER
//WHICHEVER COLLIDER GETS ACTIVATED, SETS FADE TO FALSE FOR ITSELF
//OTHER COLLIDERS ABOVE IT SETS FADE TO TRUE

public class DetectFadeMaterials : MonoBehaviour
{
    // public bool enableCollidor;
    // public int lastExited;
    // public int lastEntered;
    public bool fade;
    public float fadeSpeed;

    // public void ToggleCollider(bool )
    // {
    //     enableCollidor = !enableCollidor;
    // }

    private void Awake()
    {
        CharacterControl.Detect += Detect;
    }

    private void Detect(bool fade)
    {
        this.fade = fade;
    }
    
    private void OnCollisionStay(Collision collision)
    {
        // print(collision.gameObject.name);
        if (fade)
        {
            //HIDE ANYTHING THAT COLLIDES FROM ON TOP OF THIS OBJECT
            if (collision.collider.GetComponent<MaterialInvisible>())
            {
                // print(collision.collider.name + " IS INVISBLE");
                collision.collider.GetComponent<MaterialInvisible>().isVisible = false;
            }
            else if (collision.collider.GetComponent<MaterialFade>())
            {
                collision.collider.GetComponent<MaterialFade>().FadeMat(collision.gameObject.GetComponent<MeshRenderer>().material, fade, fadeSpeed);
            }
        }
        else
        {
            if (collision.collider.GetComponent<MaterialInvisible>())
            {
                collision.collider.GetComponent<MaterialInvisible>().isVisible = true;
            }
            else if (collision.collider.GetComponent<MaterialFade>())
            {
                collision.collider.GetComponent<MaterialFade>().FadeMat(collision.gameObject.GetComponent<MeshRenderer>().material, fade, fadeSpeed);
            }
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        
        if (collision.GetComponent<Collider>().CompareTag("Player"))
        {
            // print("ENTERED " + collision.name);
            //for loop to check which Floor Trigger
            for (int i = 0; i < FloorTriggers.detectFadeMats.Length; i++)
            {
                if (FloorTriggers.detectFadeMats[i].name == gameObject.name)
                {
                    FloorTriggers.detectFadeMats[i].fade = false;
                    print(i + " IS NOT FADING");
                    
                    // print(FloorTriggers.detectFadeMats[i+1].name);
                    for (int j = i + 1; j < FloorTriggers.detectFadeMats.Length; j++)
                    {
                        FloorTriggers.detectFadeMats[j].fade = true;
                        print(j + " IS FADING");
                    }
                    return;
                }
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.GetComponent<Collider>().CompareTag("Player"))
        {
            //for loop to check which Floor Trigger
            for (int i = 0; i < FloorTriggers.detectFadeMats.Length; i++)
            {
                if (FloorTriggers.detectFadeMats[i].name == gameObject.name)
                {
                    if (i == 0)
                    {
                        foreach (DetectFadeMaterials detectFadeMaterials in FloorTriggers.detectFadeMats)
                        {
                            detectFadeMaterials.fade = false;
                        }
                    }
                    
                    // print(FloorTriggers.detectFadeMats[i].name);
                    // FloorTriggers.detectFadeMats[i - 1].fade = false;
                    // FloorTriggers.detectFadeMats[i].fade = true;
                }
            }
        }
    }

}
