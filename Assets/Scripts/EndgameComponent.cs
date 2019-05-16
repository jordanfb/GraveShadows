using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class EndgameComponent : MonoBehaviour
{

    public Text nameText;
    public Button button;
    public Suspect suspect;

    public void SetStuff(Suspect s, UnityAction action)
    {
        suspect = s;
        button.image.sprite = s.Photo;
        button.onClick.AddListener(action);
    }

    [ContextMenu("find stuff")]
    public void FindStuff()
    {
        button = GetComponent<Button>();
        nameText = GetComponentInChildren<Text>();
    }
}