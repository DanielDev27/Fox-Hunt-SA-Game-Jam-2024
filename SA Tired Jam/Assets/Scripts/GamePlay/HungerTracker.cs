using UnityEngine;
using TMPro;
public class HungerTracker : MonoBehaviour
{
    public static HungerTracker instance;
    public float foodStorage;
    [SerializeField] float hunger = 0;
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
}
