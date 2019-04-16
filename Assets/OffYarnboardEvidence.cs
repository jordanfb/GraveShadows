using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OffYarnboardEvidence : MonoBehaviour
{

    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private TextMeshPro evidenceNameText;

    private YarnBoardEntity entity;
    private YarnBoard yarnboard;

    public void SetContents(YarnBoard yb, YarnBoardEntity e)
    {
        entity = e;
        yarnboard = yb;
        spriteRenderer.sprite = e.Photo;
        Suspect s = e as Suspect;
        if (s != null)
        {
            evidenceNameText.text = "Suspect: " + s.CodeName;
        } else
        {
            evidenceNameText.text = "Evidence: " + e.Name;
        }
    }

    private void OnMouseEnter()
    {
        // enable the name text
        evidenceNameText.gameObject.SetActive(true);
    }

    private void OnMouseExit()
    {
        // disable the name text
        evidenceNameText.gameObject.SetActive(false);
    }

    private void OnMouseDown()
    {
        // selected us!
    }
}
