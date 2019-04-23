using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffYarnboardEvidenceManager : MonoBehaviour
{
    public float widthToDisplay = 0.5f; // real world coords probably?


    public HubManager hubManager = null;
    public Transform offYarnboardItemsParent = null;
    public Transform offYarnboardItemsHoverLocation = null; // where to move to
    public GameObject offYarnbardItemPrefab = null;
    public YarnBoard yarnboard;

    private List<OffYarnboardEvidence> offYarnboardEvidenceItems = new List<OffYarnboardEvidence>();
    private bool fallingEdgeYarnboardMode = false;

    private Vector3 defaultPos;
    private Quaternion defaultRot;

    private float lerpValue = 0;
    private Coroutine lerpingCoroutine; // for lerping back and forth from our regular positions

    private void Start()
    {
        defaultPos = offYarnboardItemsParent.position;
        defaultRot = offYarnboardItemsParent.rotation;
    }

    public void AddToYarnboard(OffYarnboardEvidence e)
    {
        // remove e from our lists
        offYarnboardEvidenceItems.Remove(e);
        Destroy(e.gameObject);
        RebuildEvidenceItems();
    }

    [ContextMenu("Rebuild items")]
    public void RebuildEvidenceItems()
    {
        // updates the list!
        List<YarnBoardEntity> entities = EvidenceManager.instance.GetEvidenceNotOnYarnboard();
        if (entities.Count < offYarnboardEvidenceItems.Count)
        {
            // then delete the extra
            for (int i = offYarnboardEvidenceItems.Count - 1; i >= entities.Count; i--)
            {
                // delete that file
                Destroy(offYarnboardEvidenceItems[i].gameObject);
                offYarnboardEvidenceItems.RemoveAt(i);
            }
        }
        // then create new ones if we need to, initializing them along the way
        for (int i = 0; i < entities.Count; i++)
        {
            OffYarnboardEvidence item;
            if (i >= offYarnboardEvidenceItems.Count)
            {
                GameObject go = Instantiate(offYarnbardItemPrefab, offYarnboardItemsParent);
                item = go.GetComponent<OffYarnboardEvidence>();
                offYarnboardEvidenceItems.Add(item);
            }
            else
            {
                item = offYarnboardEvidenceItems[i];
            }
            item.SetContents(this, yarnboard, entities[i], widthToDisplay / (entities.Count + 1));
            float x = -widthToDisplay / 2 + (i + 1) * widthToDisplay / (entities.Count + 1);
            item.transform.localPosition = new Vector3(x, 0, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (EvidenceManager.instance.GetEvidenceNotOnYarnboardCount() != offYarnboardEvidenceItems.Count)
        {
            RebuildEvidenceItems();
        }

        if (hubManager.cameraMode == HubManager.CameraMode.LookAtYarnBoard)
        {

            if (!fallingEdgeYarnboardMode)
            {
                // then it's a rising edge
                // move everything up I guess?
                fallingEdgeYarnboardMode = true;
                Hover();
            }
            // then we're enabled!
            // adjust all the positions based on the mouse or whatever

        } else if (fallingEdgeYarnboardMode)
        {
            // move everything back down to the desk
            fallingEdgeYarnboardMode = false;
            ResetPosition();

        }
    }

    public void Hover()
    {
        if (lerpingCoroutine != null)
        {
            StopCoroutine(lerpingCoroutine);
        }
        lerpingCoroutine = StartCoroutine(LerpToHover());
    }

    public void ResetPosition()
    {
        if (lerpingCoroutine != null)
        {
            StopCoroutine(lerpingCoroutine);
        }
        lerpingCoroutine = StartCoroutine(LerpToDesk());
    }

    private IEnumerator LerpToDesk()
    {
        // lerp to the camera position
        while (lerpValue > 0)
        {
            yield return null;
            lerpValue -= Time.deltaTime * hubManager.deskItemsLerpSpeed;
            float t = DeskDayDescriptionItem.Smootherstep(lerpValue);
            offYarnboardItemsParent.position = Vector3.Lerp(defaultPos, offYarnboardItemsHoverLocation.position, t);
            offYarnboardItemsParent.rotation = Quaternion.Lerp(defaultRot, offYarnboardItemsHoverLocation.rotation, t);
        }
        lerpValue = 0;
        // move to your orignal location
        offYarnboardItemsParent.position = defaultPos;
        offYarnboardItemsParent.rotation = defaultRot;
        lerpingCoroutine = null;
    }

    private IEnumerator LerpToHover()
    {
        // lerp to the camera position
        while (lerpValue < 1)
        {
            yield return null;
            lerpValue += Time.deltaTime * hubManager.deskItemsLerpSpeed;
            float t = DeskDayDescriptionItem.Smootherstep(lerpValue);
            offYarnboardItemsParent.position = Vector3.Lerp(defaultPos, offYarnboardItemsHoverLocation.position, t);
            offYarnboardItemsParent.rotation = Quaternion.Lerp(defaultRot, offYarnboardItemsHoverLocation.rotation, t);
        }
        lerpValue = 1;
        // move one last time to the camera position
        offYarnboardItemsParent.position = offYarnboardItemsHoverLocation.position;
        offYarnboardItemsParent.rotation = offYarnboardItemsHoverLocation.rotation;
        lerpingCoroutine = null;
    }
}
