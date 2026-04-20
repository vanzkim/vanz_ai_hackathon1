using UnityEngine;

public class EndingSequenceTrigger : MonoBehaviour
{
    public EndingSequenceManager manager;

    private void OnTriggerEnter(Collider other)
    {
        // Debug to see what hit it
        Debug.Log("Trigger hit by: " + other.gameObject.name);
        
        if (other.CompareTag("Player") || other.name.Contains("Player"))
        {
            if (manager != null)
            {
                manager.OnTriggerHit();
                gameObject.SetActive(false); // Trigger only once
            }
        }
    }
}
