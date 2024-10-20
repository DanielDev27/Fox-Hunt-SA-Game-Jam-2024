using UnityEngine;
using UnityEngine.Events;

public class FoodTracker : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] bool gatheringFood = false;
    [SerializeField] float food;
    public int foodAmount;
    [Header("Settings")]
    [SerializeField] float foodCollectionModifier;
    //Events
    public static UnityEvent LoseChicken = new UnityEvent();
    private void Start()
    {
        foodAmount = 0;
    }
    private void OnEnable()
    {
        ChickenCoup.foodCount.AddListener(GatherFood);
    }
    private void OnDisable()
    {
        ChickenCoup.foodCount.RemoveListener(GatherFood);
    }
    private void Update()
    {
        if (gatheringFood)
        {
            food += Time.deltaTime * foodCollectionModifier;
            if (food > foodAmount + 1)
            {
                foodAmount = (int)food;
                LoseChicken?.Invoke();
            }
        }
    }
    public void GatherFood(bool _gatheringFood)
    {
        this.gatheringFood = _gatheringFood;
        if (!gatheringFood)
        {
            food = foodAmount;
        }
    }
}
