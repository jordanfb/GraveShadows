using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OffYarnboardEvidence : MonoBehaviour
{
    public Vector3 evidenceOffsetFromYarnboardHit = new Vector3(-.005f, 0, 0);
    public Vector3 evidenceOffsetFromWallHit = new Vector3(-.1f, 0, 0);
    public Vector2 wallAngles = new Vector3(0, 90, 0); // this is terrible code but it's what we've got to do...
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private TextMeshPro evidenceNameText;

    private float mouseDistance = 1; // distance to use for highlighting yourself
    private YarnBoardEntity entity;
    private YarnBoard yarnboard;
    private OffYarnboardEvidenceManager offYarnboardManager;
    private Vector3 hitGoalPosition = Vector3.zero;
    private bool onYarnboard = false;

    Coroutine lerpingCoroutine = null;
    float lerpValue = 0;
    bool beingSelected = false; // used for dragging the evidence
    private Vector3 smoothDampVelocity = Vector3.zero;
    Quaternion startingRot;
    Vector3 startingPos;

    public void Start()
    {
        startingRot = transform.rotation;
        startingPos = transform.position;
    }

    public void SetContents(OffYarnboardEvidenceManager manager, YarnBoard yb, YarnBoardEntity e, float spacing)
    {
        entity = e;
        yarnboard = yb;
        offYarnboardManager = manager;
        beingSelected = false;
        spriteRenderer.sprite = e.Photo;
        Suspect s = e as Suspect;
        smoothDampVelocity = Vector3.zero;
        if (lerpingCoroutine != null)
        {
            StopCoroutine(lerpingCoroutine);
            lerpingCoroutine = null; // reset the lerping too
        }
        lerpValue = 0;
        if (s != null)
        {
            evidenceNameText.text = "Suspect: " + s.CodeName;
            evidenceNameText.transform.localPosition = new Vector3(0, 3.2f, 0);
            BoxCollider b = GetComponent<BoxCollider>();
            b.size = new Vector3(3.7f, 4.5f, .2f);
        } else
        {
            evidenceNameText.text = "Evidence: " + e.Name;
            evidenceNameText.transform.localPosition = new Vector3(0, 2.26f, 0);
            BoxCollider b = GetComponent<BoxCollider>();
            b.size = new Vector3(2.048f, 2.436001f, .2f);
        }
    }

    private void OnMouseDown()
    {
        //StartAddToBoardCoroutine();
        beingSelected = true;
        startingRot = transform.rotation; // hopefully when it's hovering to get the right angle for it...
        startingPos = transform.position; // same deal with position this is gonna cause some simple bugs but they're worth it...
    }

    private void Update()
    {
        // if you're being selected!
        if (beingSelected)
        {
            // new method is to follow the mouse onto the board instead of doing the coroutine so taht we can make it nicer!
            // if you're over the board and the mouse lets go, then lerp down and place yourself
            // if the mouse lets go not over the board lerp back to start and reset!
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // On hit, check if it's a piece of evidence
            bool hitSomething = Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("YarnBoard", "WallLayer"));
            bool hitYarnboard = false;
            if (hitSomething)
            {
                Quaternion rot;
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("YarnBoard"))
                {
                    rot = Quaternion.Euler(wallAngles);
                    hitYarnboard = true;
                    hitGoalPosition = hit.point + evidenceOffsetFromYarnboardHit;
                }
                else
                {
                    // move it further away from the wall like by a lot
                    hitGoalPosition = hit.point + evidenceOffsetFromWallHit;
                    rot = startingRot;
                }
                // then lerp to the position on the board
                // now lerp there I guess?
                transform.position = Vector3.SmoothDamp(transform.position, hitGoalPosition, ref smoothDampVelocity, .1f);
                transform.rotation = rot;

                if (Input.GetMouseButtonUp(0))
                {
                    beingSelected = false;
                    // the drag button was released so we're no longer being moved
                    if (hitYarnboard)
                    {
                        // spawn the yarn item where we are!
                        AddToYarnboardHere();
                    }
                    else
                    {
                        // lerp me back to my spawn location I guess
                        StartReturnToHand();
                    }
                }
            } else
            {
                // check if we need to lerp away from the yarnboard I guess?
                Debug.LogError("Error yarnboard item didn't hit anything somehow?");
            }
        }
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

    public void StartReturnToHand()
    {
        if (lerpingCoroutine != null)
        {
            // kill it
            StopCoroutine(lerpingCoroutine);
        }
        lerpingCoroutine = StartCoroutine(LerpBackToPlace());
    }

    private IEnumerator LerpBackToPlace()
    {
        Vector3 defaultPos = transform.position;
        Quaternion defaultRot = transform.rotation;

        float lerp_t = 0;
        // lerp to the camera position
        while (lerp_t < 1)
        {
            yield return null;
            lerp_t += Time.deltaTime;
            float t = DeskDayDescriptionItem.Smootherstep(lerp_t);
            transform.position = Vector3.Lerp(defaultPos, startingPos, t);
            transform.rotation = Quaternion.Lerp(defaultRot, startingRot, t);
        }
        // move one last time to the camera position
        transform.position = startingPos;
        transform.rotation = startingRot;
        lerpingCoroutine = null;
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
    }

    private void AddToYarnboardHere()
    {
        // selected us!
        SerializedEvidence e = EvidenceManager.instance.FindSerializedEvidence(entity);
        e.location = hitGoalPosition;
        YarnBoardAddToYarnBoardEvent undoredoevent = new YarnBoardAddToYarnBoardEvent(e.evidenceindex, e, false);
        UndoRedoStack.AddEvent(undoredoevent); // create the undo/redo event
        // then redo it to add it to the yarnboard
        undoredoevent.Redo(); // add it to the yarnboard!

        yarnboard.GenerateContent();
        offYarnboardManager.AddToYarnboard(this);
    }
}
