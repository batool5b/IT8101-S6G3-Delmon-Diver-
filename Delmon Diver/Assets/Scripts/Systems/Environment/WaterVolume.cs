using UnityEngine;

public class WaterVolume : MonoBehaviour
{
    // The density of water to calculate buoyancy
    public float fluidDensity = 1000f;
    
    // Gets the height of the water at exactly this position
    // If you want waves later, you can add maths here. For now, it's just flat.
    public float GetWaterLevelAtPos(Vector3 position)
    {
        return transform.position.y;
    }

    private void OnTriggerEnter(Collider other)
    {
        BuoyancyObject buoyant = other.GetComponentInParent<BuoyancyObject>();
        if (buoyant != null)
        {
            buoyant.inWater = true;
            buoyant.waterVolume = this;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        BuoyancyObject buoyant = other.GetComponentInParent<BuoyancyObject>();
        if (buoyant != null)
        {
            buoyant.inWater = false;
            buoyant.waterVolume = null;
        }
    }
}
