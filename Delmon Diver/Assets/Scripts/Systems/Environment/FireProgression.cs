using UnityEngine;

public class FireProgression : MonoBehaviour
{
    [Header("Fire Settings")]
    public ParticleSystem[] fireParticles; // Assign the main fire particle systems here
    public HazardVolume associatedHazard;
    
    public bool isActiveAtStart = false;
    
    private void Start()
    {
        if (!isActiveAtStart)
        {
            Extinguish();
        }
        else
        {
            Ignite();
        }
    }

    public void Ignite()
    {
        foreach (var ps in fireParticles)
        {
            if(!ps.isPlaying)
                ps.Play();
        }
        
        if (associatedHazard != null)
            associatedHazard.gameObject.SetActive(true);
    }

    public void Extinguish()
    {
        foreach (var ps in fireParticles)
        {
            if(ps.isPlaying)
                ps.Stop();
        }
        
        if (associatedHazard != null)
            associatedHazard.gameObject.SetActive(false);
    }
}
