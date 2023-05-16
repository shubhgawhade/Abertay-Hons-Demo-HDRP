using UnityEngine;
using UnityEngine.Serialization;

public class MaterialFade : MonoBehaviour
{
    public int layer;
    
    public Material objMat;
    public Material tempObjMat;
    public bool fade;
    public float fadeSpeed = 1;
    public Material transparentMat;
    public Material tempTransparentMat;
    
    // Start is called before the first frame update
    void Start()
    {
        // objMat = gameObject.GetComponent<MeshRenderer>().material;
        layer = gameObject.layer;
        tempTransparentMat = new Material(transparentMat);
        tempTransparentMat.name = "tempTransparent";
        // tempTransparentMat.CopyPropertiesFromMaterial(objMat);
        // tempTransparentMat.SetFloat("_SurfaceType", 1);
        tempObjMat = new Material(objMat);
        tempObjMat.name = "tempOpaque";
        tempObjMat.CopyPropertiesFromMaterial(objMat);
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
            // tempMat.CopyPropertiesFromMaterial(GetComponent<Renderer>().material);
            // tempMat.SetFloat("_SurfaceType", 1);
            if (GetComponent<Renderer>().material != tempTransparentMat)
            {
                GetComponent<Renderer>().material = tempTransparentMat;
            }

            // SETS OBJECT TO IGNORE RAYCAST LAYER IF FADING
            gameObject.layer = 2;
            Color col = tempTransparentMat.color;
            col.a = 0;
            tempTransparentMat.color = Color.Lerp(tempTransparentMat.color, col, Time.deltaTime * fadeSpeed);
        }
        else
        {
            if (objMat.color.a > 0.99f)
            {
                GetComponent<Renderer>().material = tempObjMat;
            }
            
            gameObject.layer = layer;
            Color col1 = tempTransparentMat.color;
            col1.a = 1;
            tempTransparentMat.color = col1;
            
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
