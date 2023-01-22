using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityStandardAssets.Characters.ThirdPerson;

public class MaterialFade : MonoBehaviour
{
    public Material objMat;
    public bool fade;
    public float fadeSpeed = 1;
    
    // Start is called before the first frame update
    void Start()
    {
        AICharacterControl.FadeMat += FadeMat;
        objMat = gameObject.GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        FadeLerp();
    }

    public void FadeLerp()
    {
        if (fade)
        {
            Color col = objMat.color;
            col.a = 0;
            objMat.color = Color.Lerp(objMat.color, col, Time.deltaTime * fadeSpeed);
        }
        else
        {
            Color col = objMat.color;
            col.a = 1;
            objMat.color = Color.Lerp(objMat.color, col, Time.deltaTime * fadeSpeed); //0.2f
        }
    }

    private void FadeMat(Material toFade, bool fade)
    {
        this.fade = fade;
        objMat = toFade;
    }

    private void OnCollisionStay(Collision collision)
    {
        print(collision.gameObject.name);
        if (fade)
        {
            //HIDE ANYTHING THAT COLLIDES FROM ON TOP OF THIS OBJECT
            if (collision.collider.GetComponent<MaterialInvisible>())
            {
                collision.collider.GetComponent<MaterialInvisible>().isVisible = false;
            }
            else if (collision.collider.GetComponent<MaterialFade>())
            {
                collision.collider.GetComponent<MaterialFade>().FadeMat(collision.gameObject.GetComponent<MeshRenderer>().material, fade);
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
                collision.collider.GetComponent<MaterialFade>().FadeMat(collision.gameObject.GetComponent<MeshRenderer>().material, fade);
            }
        }
    }
}
