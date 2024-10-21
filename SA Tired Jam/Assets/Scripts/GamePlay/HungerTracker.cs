using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class HungerTracker : MonoBehaviour
{
    public static HungerTracker instance;
    [Header("Hunger")]
    public float foodStorage;
    public float hunger = 0;

    [Header("UI References")]
    [SerializeField] TMP_Text hungerAmount;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        hungerAmount.text = "Hunger: " + hunger;
    }

    private void Update()
    {
        hungerAmount.text = "Hunger: " + (int)hunger;
        if (hunger > 0)
        {
            hunger -= Time.deltaTime;
        }
        if (GameMenuManager.instance.gameActive && hunger <= 0)
        {
            GameMenuManager.instance.gameActive = false;
            GameMenuManager.instance.GameOver();
        }
    }
}
