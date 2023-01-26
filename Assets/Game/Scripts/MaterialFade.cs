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
        AICharacterControl.FadeRoof += FadeMat;
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

    public void FadeMat(Material toFade, bool fade, float fadeSpeed)
    {
        this.fade = fade;
        this.fadeSpeed = fadeSpeed;
        objMat = toFade;
    }
}
