using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollToZeroOnEnable : MonoBehaviour
{
    public Scrollbar bar;
    public RectTransform view;

    private void OnEnable()
    {
        // set the scroll bar value to zero I guess :/
        bar.enabled = false;
        bar.size = 0;
        bar.value = 0;
        view.localPosition = Vector3.zero;
        bar.enabled = true;
    }
}
