using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MeshClickScript : MonoBehaviour
{
    public UnityEvent onClickEvent;

    // formerly known as the "ClickOnGunScript"
    private void OnMouseDown()
    {
        onClickEvent.Invoke();
    }

    // we could also have events for onmouseneter and onmouseexit if we
    // wanted to have a hover box for the mouse or something explainig
    // what to do but we'll probably just have a monologue
}
