using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffYarnboardEvidenceManager : MonoBehaviour
{
    public float widthToDisplay = 0.5f; // real world coords probably?


    public HubManager hubManager = null;
    public Transform offYarnboardItemsParent = null;
    public GameObject offYarnbardItemPrefab = null;
    public YarnBoard yarnboard;

    private List<OffYarnboardEvidence> offYarnboardEvidenceItems = new List<OffYarnboardEvidence>();
    private bool fallingEdgeYarnboardMode = false;

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
            item.SetContents(yarnboard, entities[i]);
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
            }
            // then we're enabled!
            // adjust all the positions
        } else if (fallingEdgeYarnboardMode)
        {
            // move everything back down to the desk
            fallingEdgeYarnboardMode = false;

        }
    }
}
