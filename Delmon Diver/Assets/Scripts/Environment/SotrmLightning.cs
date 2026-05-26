using UnityEngine;
using System.Collections;

public class StormLightning : MonoBehaviour
{
    public Light lightningLight;

    public float minTime = 4f;
    public float maxTime = 10f;

    public float flashIntensity = 5f;
    public float flashDuration = 0.08f;

    void Start()
    {
        if (lightningLight != null)
        {
            lightningLight.intensity = 0f;
        }

        StartCoroutine(LightningLoop());
    }

    IEnumerator LightningLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minTime, maxTime));

            if (lightningLight != null)
            {
                //first flash
                lightningLight.intensity = flashIntensity;
                yield return new WaitForSeconds(flashDuration);

                lightningLight.intensity = 0f;
                yield return new WaitForSeconds(0.06f);

                //second smaller flash
                lightningLight.intensity = flashIntensity * 0.6f;
                yield return new WaitForSeconds(flashDuration);

                lightningLight.intensity = 0f;
            }
        }
    }
}