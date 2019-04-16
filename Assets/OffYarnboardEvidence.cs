using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OffYarnboardEvidence : MonoBehaviour
{

    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Text evidenceNameText;

    private YarnBoardEntity entity;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetContents(YarnBoardEntity e)
    {
        entity = e;
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

    // Update is called once per frame
    void Update()
    {
        
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
