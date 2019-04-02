using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DeskDayDescriptionItem : MonoBehaviour, IPointerDownHandler
{
    public TextMeshProUGUI textGUI;
    Vector3 originalPosition;
    Quaternion originalRotation;
    Transform originalParent;
    HubManager hubManager;

    public List<GameObject> levelButtons; // this is for enabling/disabling UI

    Coroutine lerpingCoroutine;

    private float lerpValue = 0; // 0 is desk, 1 is by camera

    // Start is called before the first frame update
    void Start()
    {
        // this is so that we can support dynamic numbers of days I swear there's a reason
        hubManager = GameObject.FindObjectOfType<HubManager>();
        SetOriginalPositions();
    }

    // Update is called once per frame
    void Update()
    {
        if (hubManager.cameraMode == HubManager.CameraMode.LookAtDesk)
        {
            // if it's highlighted
            if (lerpValue > 0 && Input.GetMouseButtonDown(0))
            {
                // lerp back to the desk
                ResetPosition();
            }
        }
    }

    public void EnableButtons()
    {
        for (int i = 0; i < levelButtons.Count; i++)
        {
            levelButtons[i].SetActive(true);
        }
    }

    public void DisableButtons()
    {
        for (int i = 0; i < levelButtons.Count; i++)
        {
            levelButtons[i].SetActive(false);
        }
    }

    public void SetContents(string text)
    {
        textGUI.text = text;
    }

    public void SetOriginalPositions()
    {
        // this is used for the last day as well
        originalParent = transform.parent.parent; // this is on the panel and it's getting the canvas welp whatever
        originalPosition = transform.parent.position;
        originalRotation = transform.parent.rotation;
    }

    public void ClickedVisitOffice()
    {
        if (hubManager.cameraMode == HubManager.CameraMode.LookAtDesk)
        {
            //Debug.Log("Click visit office");
            GameplayManager.instance.VisitOffice();
        }
    }

    public void ClickVisitFactory()
    {
        if (hubManager.cameraMode == HubManager.CameraMode.LookAtDesk)
        {
            //Debug.Log("Visit factory");
            GameplayManager.instance.VisitFactory();
        }
    }

    private Vector3 GetCameraPosition()
    {
        // calculates the camera position and returns it
        Vector3 o = Camera.main.transform.position;
        o += Camera.main.transform.forward * hubManager.deskItemCameraOffset.z;
        o += Camera.main.transform.right* hubManager.deskItemCameraOffset.x;
        o += Camera.main.transform.up * hubManager.deskItemCameraOffset.y;
        return o;
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
            float t = Smootherstep(lerpValue);
            transform.parent.position = Vector3.Lerp(originalPosition, GetCameraPosition(), t);
            transform.parent.rotation = Quaternion.Lerp(originalRotation, Camera.main.transform.rotation, t);
        }
        lerpValue = 0;
        // move to your orignal location
        transform.parent.SetParent(originalParent, true);
        transform.parent.position = originalPosition;
        transform.parent.rotation = originalRotation;

        lerpingCoroutine = null;
    }

    private IEnumerator LerpToCamera()
    {
        // lerp to the camera position
        while (lerpValue < 1)
        {
            yield return null;
            lerpValue += Time.deltaTime * hubManager.deskItemsLerpSpeed;
            float t = Smootherstep(lerpValue);
            transform.parent.position = Vector3.Lerp(originalPosition, GetCameraPosition(), t);
            transform.parent.rotation = Quaternion.Lerp(originalRotation, Camera.main.transform.rotation, t);
        }
        lerpValue = 1;
        // move one last time to the camera position
        transform.parent.SetParent(Camera.main.transform, true);
        transform.parent.position = GetCameraPosition();
        transform.parent.rotation = Camera.main.transform.rotation;
        lerpingCoroutine = null;
    }

    public static float Smootherstep(float x)
    {
        // adapted from wikipedia
        return x * x * x * (x * (x * 6 - 15) + 10);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // if the desk is being looked at, zoom towards the camera.
        // if the player clicks elsewhere then zoom back to the position
        if (hubManager.cameraMode == HubManager.CameraMode.LookAtDesk)
        {
            // then you can be clicked, so lerp to the camera
            if (lerpingCoroutine != null)
            {
                StopCoroutine(lerpingCoroutine);
            }
            lerpingCoroutine = StartCoroutine(LerpToCamera());
        }
    }
}
