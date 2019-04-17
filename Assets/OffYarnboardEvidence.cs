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

    private float mouseDistance = 1; // distance to use for highlighting yourself
    private YarnBoardEntity entity;
    private YarnBoard yarnboard;
    private OffYarnboardEvidenceManager offYarnboardManager;

    Coroutine lerpingCoroutine = null;
    float lerpValue = 0;

    public void SetContents(OffYarnboardEvidenceManager manager, YarnBoard yb, YarnBoardEntity e, float spacing)
    {
        entity = e;
        yarnboard = yb;
        offYarnboardManager = manager;
        spriteRenderer.sprite = e.Photo;
        Suspect s = e as Suspect;
        lerpingCoroutine = null; // reset the lerping too
        lerpValue = 0;
        if (s != null)
        {
            evidenceNameText.text = "Suspect: " + s.CodeName;
        } else
        {
            evidenceNameText.text = "Evidence: " + e.Name;
        }
    }

    private void OnMouseDown()
    {
        StartAddToBoardCoroutine();
    }

    public void StartAddToBoardCoroutine()
    {
        if (lerpingCoroutine != null)
        {
            return; // can only add to board once
        }
        lerpingCoroutine = StartCoroutine(LerpToBoard());
    }

    private IEnumerator LerpToBoard()
    {
        Vector3 defaultPos = transform.position;
        Quaternion defaultRot = transform.rotation;

        Transform parentPos = yarnboard.GetYarnboardParent();
        Vector3 goalPos = parentPos.position + parentPos.up * .5f + parentPos.forward * -.04f;
        Quaternion goalRot = parentPos.rotation;
        // lerp to the camera position
        while (lerpValue < 1)
        {
            yield return null;
            lerpValue += Time.deltaTime;
            float t = DeskDayDescriptionItem.Smootherstep(lerpValue);
            transform.position = Vector3.Lerp(defaultPos, goalPos, t);
            transform.rotation = Quaternion.Lerp(defaultRot, goalRot, t);
        }
        lerpValue = 1;
        // move one last time to the camera position
        transform.position = goalPos;
        transform.rotation = goalRot;
        lerpingCoroutine = null;

        // then call the actual function to set everything up
        AddToYarnboard();
    }

    private void AddToYarnboard()
    {
        // selected us!
        SerializedEvidence e = EvidenceManager.instance.FindSerializedEvidence(entity);
        YarnBoardAddToYarnBoardEvent undoredoevent = new YarnBoardAddToYarnBoardEvent(e.evidenceindex, e, false);
        UndoRedoStack.AddEvent(undoredoevent); // create the undo/redo event
        // then redo it to add it to the yarnboard
        undoredoevent.Redo(); // add it to the yarnboard!

        Transform parentPos = yarnboard.GetYarnboardParent();
        e.location = parentPos.position + parentPos.up * .5f + parentPos.forward * -.04f; // set it to the center position I guess, plus an offset
        //Debug.Log("Spawned it at " + e.location);

        yarnboard.GenerateContent();
        offYarnboardManager.RebuildEvidenceItems();
        EvidenceManager.SaveEvideneToPlayerPrefs();
    }
}
