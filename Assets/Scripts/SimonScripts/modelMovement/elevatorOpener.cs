using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class elevatorOpener : MonoBehaviour
{
    public float ammoutToOpen;
    public float openSpeed = 50f;
    public GameObject assocLeftDoor;
    public GameObject assocRightDoor;

    public Vector3 leftStartingPos;
    public Vector3 rightStartingPos;

    private bool isOpening = false;
    float t = 0; // start closed


    public void Start()
    {
        leftStartingPos = assocLeftDoor.transform.position;
        rightStartingPos = assocRightDoor.transform.position;
    }

    private void UpdateDoorPos(float x)
    {
        assocLeftDoor.transform.position = Vector3.Lerp(leftStartingPos, leftStartingPos - assocLeftDoor.transform.right * ammoutToOpen, x);
        assocRightDoor.transform.position = Vector3.Lerp(rightStartingPos, rightStartingPos + assocLeftDoor.transform.right * ammoutToOpen, x);
    }

    void Update()
    {
        t += (isOpening ? 1 : -1) * Time.deltaTime * openSpeed;
        t = Mathf.Clamp01(t);
        UpdateDoorPos(DeskDayDescriptionItem.Smootherstep(t));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isOpening = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isOpening = false;
        }
    }
}