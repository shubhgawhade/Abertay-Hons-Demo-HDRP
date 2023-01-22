using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialInvisible : MonoBehaviour
{
    public bool isVisible = true;
    public Material tempMat;
    public Material invisibleMat;
    Material mat;
    
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
        tempMat = mat;
    }

    // Update is called once per frame
    void Update()
    {
        if (isVisible)
        {
            GetComponent<MeshRenderer>().material = tempMat;
        }
        else
        {
            tempMat = mat;
            GetComponent<MeshRenderer>().material = invisibleMat;
        }
    }
    
    
}
