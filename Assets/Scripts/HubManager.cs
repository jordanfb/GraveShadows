using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubManager : MonoBehaviour
{

    public Transform DeskCameraLocation;
    public Transform YarnBoardCameraLocation;
    public CameraMode cameraMode = CameraMode.LookAtDesk; // looking at desk, player, or yarnboard

    [Header("Desk Item Variables")]
    public Transform deskItemParent;
    public Vector3 deskItemFinalOffset;
    public List<DeskDayDescriptionItem> deskItems; // in order by days!
    public float deskItemsLerpSpeed = 1;
    public Vector3 deskItemCameraOffset = Vector3.down * .5f;

    [Header("Gun variables")]
    public Transform gunTransform;
    public Transform finalGunPosition;
    public Material GlowingGunMaterial; // for the final day it's highlighted

    // Start is called before the first frame update
    void Start()
    {
        // it needs to load all the summaries of the days here
        // it gets the info from the static gameplaymanager
        LoadDesk();
    }

    public void ExitDesk()
    {
        // this is called when the collider is clicked probably
        cameraMode = CameraMode.FollowPlayer; // go back to following the player
        Debug.Log("Exitintg desk");
    }

#if UNITY_EDITOR
    // only have these cheat keys if we're in the editor, not a build
    // this is just because I don't trust myself not to forget them
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            // skip a day
            GameplayManager.instance.SkipDay("Visited stuff saw death la-di-da\n\nOh yeah and I was caught");
            LoadDesk();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            // skip a day
            GameplayManager.instance.NextDay("Visited stuff found stuff whoop-di-do");
            LoadDesk();
        }
    }
#endif

    public void ClickOnGun()
    {
        if (GameplayManager.instance.IsChoosingDay() && cameraMode == CameraMode.LookAtDesk)
        {
            // only do anything if you click on it at the right time
            Debug.Log("Clicked on the gun");
        }
    }

    private void LoadDesk()
    {
        // load the desk UI stuff here
        // setting the text and whatever
        string[] dayNames = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday\nLast Day" };
        for (int i = 0; i < deskItems.Count; i++)
        {
            string content = "<size=.05><u>" + dayNames[i] + "</u></size>\n";
            content += GameplayManager.instance.dayData[i].dayContent;
            if (i == GameplayManager.instance.dayNum)
            {
                // activate that day's buttons
                deskItems[i].EnableButtons();
                content += "<size=.03>Where should I look for evidence?</size>";
            } else
            {
                // deactivate them
                deskItems[i].DisableButtons();
            }
            deskItems[i].SetContents(content);
        }

        // if it's the last day then move the desk item parent up,
        // move the gun down, and enable the gun and stuff like that
        if (GameplayManager.instance.IsChoosingDay())
        {
            // then move everything
            deskItemParent.position += deskItemFinalOffset;
            for(int i = 0; i < deskItems.Count; i++)
            {
                deskItems[i].SetOriginalPositions(); // make sure it knows where to return to now
            }
            gunTransform.position = finalGunPosition.position;
            gunTransform.rotation = finalGunPosition.rotation;
            // also enable the gun clicking but that's the next problem
            gunTransform.GetComponent<MeshRenderer>().material = GlowingGunMaterial;
            // also enable the clicking on script
            //gunTransform.GetComponent<>
        }
    }

    [ContextMenu("To Desk Camera")]
    public void ToDeskLocation()
    {
        Camera.main.transform.position = DeskCameraLocation.position;
        Camera.main.transform.rotation = DeskCameraLocation.rotation;
    }

    [ContextMenu("To Yarn Board Camera")]
    public void ToYarnboardLocation()
    {
        Camera.main.transform.position = YarnBoardCameraLocation.position;
        Camera.main.transform.rotation = YarnBoardCameraLocation.rotation;
    }

    public enum CameraMode
    {
        FollowPlayer, LookAtDesk, LookAtYarnBoard
    }
}
