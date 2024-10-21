using UnityEngine;
using TMPro;
public class HungerTracker : MonoBehaviour
{
    public static HungerTracker instance;
    public float foodStorage;
    [SerializeField] float hunger = 0;
    [SerializeField] bool gameActive = true;
    [Header("UI References")]
    [SerializeField] TMP_Text hungerAmount;

    private void Awake()
    {
        instance = this;
        gameActive = true;
    }
    private void Start()
    {
        hungerAmount.text = "Hunger: " + hunger;
    }

    private void Update()
    {
        if (hunger > 0)
        {
            hunger -= Time.deltaTime;
        }
        if (gameActive)
        {
            gameActive = false;
            
        }
    }
}
