using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class DetectFadeMaterials : MonoBehaviour
{
    // public bool enableCollidor;
    public bool fade;

    // public void ToggleCollider(bool )
    // {
    //     enableCollidor = !enableCollidor;
    // }

    private void Awake()
    {
        AICharacterControl.Detect += Detect;
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
                print(collision.collider.name + " IS INVISBLE");
                collision.collider.GetComponent<MaterialInvisible>().isVisible = false;
            }
            else if (collision.collider.GetComponent<MaterialFade>())
            {
                collision.collider.GetComponent<MaterialFade>().FadeMat(collision.gameObject.GetComponent<MeshRenderer>().material, fade, 8);
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
                collision.collider.GetComponent<MaterialFade>().FadeMat(collision.gameObject.GetComponent<MeshRenderer>().material, fade, 8);
            }
        }
    }
}
