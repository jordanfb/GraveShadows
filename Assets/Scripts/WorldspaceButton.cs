using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class WorldspaceButton : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Color originalColor;
    public Color hoverColor;
    public Color clickColor;
    public Image image;
    [Space]
    public UnityEvent OnClick;

    // Start is called before the first frame update
    void Start()
    {
        originalColor = image.color;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        image.color = clickColor;
        OnClick.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = originalColor;
    }
}
