using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStartOnTriggerEnter : MonoBehaviour
{

    public OnTriggerEnterHandler handler;

    private void OnTriggerEnter(Collider other)
    {
        //handler.TriggerOnTrigger(other); // forward it to that game object this is kinda terrible but it works
    }
}
