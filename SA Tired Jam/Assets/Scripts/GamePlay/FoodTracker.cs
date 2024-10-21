using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class FoodTracker : MonoBehaviour
{
    public static FoodTracker instance;
    [Header("Debug")]
    [SerializeField] bool gatheringFood = false;
    [SerializeField] float food;
    public int foodAmount;
    [Header("Settings")]
    [SerializeField] float foodCollectionModifier;
    //Events
    public static UnityEvent LoseChicken = new UnityEvent();

    //UI
    [Header("UI References")]
    [SerializeField] TMP_Text foodText;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        foodAmount = 0;
        foodText.text = "Food: " + foodAmount.ToString();
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
                foodText.text = "Food: " + foodAmount.ToString();
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

    public void DropFood()
    {
        if (foodAmount >= 1)
        {
            foodAmount--;
            foodText.text = "Food: " + foodAmount.ToString();
        }
    }

    public void FoodHome()
    {
        HungerTracker.instance.foodStorage += foodAmount;
        foodAmount = 0;
        foodText.text = "Food: " + foodAmount.ToString();

    }
}
