using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorDoorManager : MonoBehaviour
{

    public Transform leftDoor;
    public Transform rightDoor;

    public Vector3 leftStartPos;
    public Vector3 rightStartPos;
    public Vector3 leftDelta;

    public float speed = 1;
    public float t = 0; // 0 is closed, 1 = open
    bool opening = false;

    public AudioClip doorMoving;
    public AudioClip bell;
    private AudioSource soundSource;

    // Start is called before the first frame update
    void Start()
    {
        soundSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        t += (opening ? 1 : -1) * Time.deltaTime * speed;
        t = Mathf.Max(0, Mathf.Min(1, t));
        UpdateDoorPos(Smootherstep(t));
    }

    [ContextMenu("Open Doors In Game")]
    public void OpenDoors()
    {
        opening = true;
        soundSource.clip = doorMoving;
        soundSource.Play();
    }

    [ContextMenu("Close Doors In Game")]
    public void CloseDoors()
    {
        opening = false;
        soundSource.clip = doorMoving;
        soundSource.Play();
    }

    private void UpdateDoorPos(float x)
    {
        leftDoor.position = Vector3.Lerp(leftStartPos, leftStartPos + leftDelta, x);
        rightDoor.position = Vector3.Lerp(rightStartPos, rightStartPos - leftDelta, x);
    }



    [ContextMenu("Set Left Door Start Pos")]
    public void SetStartPos()
    {
        leftStartPos = leftDoor.position;
        rightStartPos = rightDoor.position;
    }

    [ContextMenu("Set Left Door Delta")]
    public void SetLeftDelta()
    {
        leftDelta = leftDoor.position - leftStartPos;
    }

    [ContextMenu("Move To Start")]
    public void ResetPos()
    {
        leftDoor.position = leftStartPos;
        rightDoor.position = rightStartPos;
    }

    [ContextMenu("Move To Open")]
    public void ToOpenPos()
    {
        leftDoor.position = leftStartPos + leftDelta;
        rightDoor.position = rightStartPos - leftDelta;
    }

    float Smootherstep(float x)
    {
        // adapted from wikipedia
        return x * x * x * (x * (x * 6 - 15) + 10);
    }
}
