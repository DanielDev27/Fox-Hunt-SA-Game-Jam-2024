using UnityEngine;

public class DogNoiseTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 11)
        {
            Debug.Log("Dog can hear noise");
        }
    }
}
