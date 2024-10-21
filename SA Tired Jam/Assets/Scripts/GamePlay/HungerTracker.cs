using UnityEngine;

public class HungerTracker : MonoBehaviour
{
    public static HungerTracker instance;
    public float foodStorage;
    [SerializeField] float hunger;

    private void Awake()
    {
        instance = this;
    }
}
