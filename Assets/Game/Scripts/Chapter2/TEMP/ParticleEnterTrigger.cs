using System.Collections.Generic;
using UnityEngine;

public class ParticleEnterTrigger : MonoBehaviour
{
    [SerializeField] private MiniGamePlayer playerScript;
    public int numInside;
    
    private ParticleSystem ps;
    
    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void OnParticleTrigger()
    {
        List<ParticleSystem.Particle> inside = new List<ParticleSystem.Particle>();
        numInside = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, inside);

        // print(numInside);
        for (int i = 0; i < numInside; i++)
        {
            playerScript.IsDrinking(ps);
        }
    }
}
