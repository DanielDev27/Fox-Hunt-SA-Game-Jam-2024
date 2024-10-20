using UnityEngine;
using UnityEngine.Events;

public class ChickenCoup : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] bool foxInCoup = false;
    [SerializeField] bool makingNoise;
    [SerializeField] float chickensRemaining;
    [Header("References")]
    [SerializeField] SphereCollider noiseCollider;
    [Header("Settings")]
    [SerializeField] float MaxNoiseRadius;
    [SerializeField] float chickensTotal;
    //Event
    public static UnityEvent<bool> foodCount = new UnityEvent<bool>();
    //Value
    public static bool gatheringFood;

    void Start()
    {
        noiseCollider.radius = 0;
        chickensRemaining = chickensTotal;
    }
    private void OnEnable()
    {
        FoodTracker.LoseChicken.AddListener(OnChickenLost);
    }
    private void OnDisable()
    {
        FoodTracker.LoseChicken.RemoveListener(OnChickenLost);
    }

    void Update()
    {
        if (foxInCoup && noiseCollider.radius < MaxNoiseRadius && chickensRemaining > 0)
        {
            noiseCollider.radius += Time.deltaTime;
        }
        if (noiseCollider.radius > 0)
        {
            if (!foxInCoup)
            {
                noiseCollider.radius -= Time.deltaTime;
            }
            if (chickensRemaining <= 0)
            {
                noiseCollider.radius -= Time.deltaTime * 3;
            }
        }
        //Debug
        if (noiseCollider.radius > 0)
        {
            makingNoise = true;
        }
        else
        {
            makingNoise = false;
        }
    }
    public void FoxEnterCoup()
    {
        foxInCoup = true;
        if (chickensRemaining > 1)
        {
            BeginCoupNoise();
        }
    }
    void BeginCoupNoise()
    {
        if (chickensRemaining > 0)
        {
            Debug.Log("Begin Coup Noise");
            gatheringFood = foxInCoup;
            foodCount?.Invoke(gatheringFood);
        }
    }
    public void FoxExitCoup()
    {
        foxInCoup = false;
        EndCoupNoise();
    }
    void EndCoupNoise()
    {
        Debug.Log("End Coup Noise");
        if (chickensRemaining > 0)
        {
            gatheringFood = foxInCoup;
            foodCount?.Invoke(gatheringFood);
        }
        else
        {
            gatheringFood = false;
        }
    }

    void OnChickenLost()
    {
        if (foxInCoup)
        {
            chickensRemaining -= 1;
            if (chickensRemaining <= 0)
            {
                gatheringFood = false;
                foodCount?.Invoke(gatheringFood);
                EndCoupNoise();
            }
        }
    }
}
