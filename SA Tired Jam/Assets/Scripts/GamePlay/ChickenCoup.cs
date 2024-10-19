using UnityEngine;

public class ChickenCoup : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] bool foxInCoup = false;
    [Header("References")]
    [SerializeField] SphereCollider noiseCollider;
    [Header("Settings")]
    [SerializeField] float MaxNoiseRadius;
    void Start()
    {
        noiseCollider.radius = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (foxInCoup && noiseCollider.radius < MaxNoiseRadius)
        {
            noiseCollider.radius += Time.deltaTime;
        }
        if (!foxInCoup && noiseCollider.radius > 0)
        {
            noiseCollider.radius -= Time.deltaTime;
        }
    }
    public void BeginCoupNoise()
    {
        Debug.Log("Begin Coup Noise");
        foxInCoup = true;
    }

    public void EndCoupNoise()
    {
        Debug.Log("End Coup Noise");
        foxInCoup = false;
    }
}
