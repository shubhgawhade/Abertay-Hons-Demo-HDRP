using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class MaterialFade : MonoBehaviour
{
    public int layer;
    
    public Material objMat;
    public bool fade;
    public float fadeSpeed = 1;
    
    // Start is called before the first frame update
    void Start()
    {
        AICharacterControl.FadeRoof += FadeMat;
        objMat = gameObject.GetComponent<MeshRenderer>().material;
        layer = gameObject.layer;
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
            // SETS OBJECT TO IGNORE RAYCAST LAYER IF FADING
            gameObject.layer = 2;
            Color col = objMat.color;
            col.a = 0;
            objMat.color = Color.Lerp(objMat.color, col, Time.deltaTime * fadeSpeed);
        }
        else
        {
            gameObject.layer = layer;
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
