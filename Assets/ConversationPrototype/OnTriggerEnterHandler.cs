using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnTriggerEnterHandler : MonoBehaviour
{
    // this is used for story events currently.
    // There may be a nicer way but I'm not sure if it has to be on the player...

    [System.Serializable]
    public struct colliderActions {
        public Collider collider;
        public UnityEvent events;
        public bool onlyRunOnce;
    }

    public List<colliderActions> triggerEventPairs;

    public void TriggerOnTrigger(Collider other)
    {
        // this is so the player can call this I guess? This isn't the cleanest but I don't want rigidbodies
        // on all the colliders in the scene...
        OnTriggerEnter(other);
    }

    private void OnTriggerEnter(Collider other)
    {
        int i = 0;
        while (i < triggerEventPairs.Count)
        {
            if (triggerEventPairs[i].collider == other)
            {
                triggerEventPairs[i].events.Invoke();
                if (triggerEventPairs[i].onlyRunOnce)
                {
                    triggerEventPairs.RemoveAt(i);
                    i--; // so that we account for the fact that we shrunk the list
                }
            }
            i++;
        }
    }
}
