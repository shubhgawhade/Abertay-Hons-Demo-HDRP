using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class BulletDecal : MonoBehaviour
{
    private DecalProjector decalProjector;
    private Timer fadeTime;

    private void Awake()
    {
        decalProjector = GetComponent<DecalProjector>();
        fadeTime = GetComponent<Timer>();
    }
    
    private void Update()
    {
        if (gameObject.activeSelf)
        {
            decalProjector.fadeFactor = 1;
            fadeTime.StartTimer(10);
            
            if (!fadeTime.isCompleted)
            {
                if(fadeTime.time > 8)
                {
                    decalProjector.fadeFactor = 0;
                }
            }
            else
            {
                gameObject.SetActive(false);
                fadeTime.time = 0;
                fadeTime.isCompleted = false;
            }
        }
    }
}
