using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/*
 * faster spot if you are moving? (find the rigidbody on the visiblityObject and make it magic)
 * as soon as see anything say "did I hear something?"
 * then if you reach a level you should walk over to that spot and look around,
 * if you don't see anything then return and restart the conversation with a "as I was saying"
 * if they spot the player fully though they stop the conversation and call them out and end the level
 * 
 * 
 */


[RequireComponent(typeof(NavMeshAgent))]
public class GuardScript : MonoBehaviour
{
    //[Tooltip("Speed of the guard it seems? I'm not sure if this works")]
    //public float speed = .5f; // meters per second
    public float suspicionMultiplier = 2;
    public float suspicionFallMultiplier = 1; // how quickly suspicion falls
    [Tooltip("Time without seeing something before suspicion starts falling")]
    public float suspicionTime = 5; // number of seconds before suspicion gets reduced

    [Space]
    [Header("What happens when the guard starts to see something?")]
    public float startSuspicionLevel = .05f;
    private bool hasFiredSightingEvent = false; // a latch for only spotting something once
    public TextAsset spottedSomethingQuipsTextAsset;
    private string[] spottedSomethingQuips;
    [Tooltip("This event is fired as soon as the guard sees something")]
    public UnityEvent onStartSuspicionEvent;

    [Header("When to investigate?")]
    public float investigateSuspicionLevel = .25f;
    public bool shouldInvestigate = true;
    public TextAsset investigationQuipsTextAsset;
    private string[] investigationQuips;

    [Header("What happens when the guard has fully sighted the player?")]
    public float fullySuspiciousLevel = 1;
    public bool forceStopConverstion = true;
    public bool stopWalking = true;
    private bool hasFiredStartSuspicionEvent = false; // a latch for only fully sighting something once
    public TextAsset sightedPlayerQuipsTextAsset;
    private string[] sightedPlayerQuips;
    [Tooltip("This event is fired as soon as the guard's suspicion reaches critical levels")]
    public UnityEvent onSightingEvent;

    [Header("What should happen if the guard looses sight of something?")]
    public TextAsset lostSightQuipsTextAsset;
    private string[] lostSightQuips;
    [Tooltip("This event is fired when the guard no longer cares")]
    public UnityEvent onLoseSuspicionEvent;

    [Header("Guard returning to conversation quips")]
    public TextAsset returnToConversationQuipsTextAsset;
    private string[] returnToConversationQuips;

    [Space]
    [Tooltip("Right click on the component and choose \"Find ConversationMember\" to auto find it")]
    public ConversationMember conversationMember; // for integration with the conversation system for interuptions
    [Space]
    public bool editPositions = true; // if false it edits rotations
    public List<Vector3> positions = new List<Vector3>();
    private Quaternion startingDirection;

    AIObjectVisibility[] visibleObjects; // this gets found upon start to avoid calculating this every frame


    [Space]
    [Header("AI spotting properties")]
    [Tooltip("The origin of the view cone")]
    public Transform guardHead;
    public float viewConeDistance = 5;
    public float viewConeAngle = 30;

    private float suspicion = 0;
    private float suspicionTimer = 0; // if not seen something then suspicion goes down after a bit

    [HideInInspector]
    public int editIndex = 0;


    private int pathingTargetNumber;
    private NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        LoadQuips(); // load the quips!

        agent = GetComponent<NavMeshAgent>();
        if (positions.Count > 0)
        {
            pathingTargetNumber = 0;
            transform.position = positions[pathingTargetNumber];
            // keep track of your starting angle though
            startingDirection = transform.rotation;
        } else
        {
            // it's a stationary agent so keep track of the starting position and starting angle
            startingDirection = transform.rotation;
            pathingTargetNumber = 0;
            positions.Add(transform.position);
        }
        visibleObjects = FindObjectsOfType<AIObjectVisibility>();
    }

    private void LoadQuips()
    {
        spottedSomethingQuips = spottedSomethingQuipsTextAsset.text.Split('\n');
        investigationQuips = investigationQuipsTextAsset.text.Split('\n');
        sightedPlayerQuips = sightedPlayerQuipsTextAsset.text.Split('\n');
        lostSightQuips = lostSightQuipsTextAsset.text.Split('\n');
        returnToConversationQuips = returnToConversationQuipsTextAsset.text.Split('\n');
    }

    // Update is called once per frame
    void Update()
    {
        if (positions.Count > 0)
        {
            if (!agent.hasPath)
            {
                // follow the path!
                agent.SetDestination(positions[pathingTargetNumber]);
                pathingTargetNumber++;
                pathingTargetNumber %= positions.Count;
            }
            if (agent.remainingDistance < .5f)
            {
                // then it's there, so move to the next point
                agent.SetDestination(positions[pathingTargetNumber]);
                pathingTargetNumber++;
                pathingTargetNumber %= positions.Count;
            }
        }

        // spot the character
        SpotTheCharacter();
        // then handle suspicion in the state machine I guess?
        HandleSuspicion();
    }

    [ContextMenu("Find ConversationMember")]
    public void FindConversationMemberInChildren()
    {
        // this finds the conversation member if one exists in this/the children of the game object
        conversationMember = GetComponent<ConversationMember>();
        if (conversationMember == null)
        {
            conversationMember = GetComponentInChildren<ConversationMember>();
        }
    }

    public void DebugLogString(string s)
    {
        // this is used as a simple testing function for the guard suspicion levels
        Debug.Log(s);
    }

    public void SpeakThenRestartLevel()
    {
        // this function is called to wait until the guard is finished talking and then reset the level
        // it starts an async call to do so
        StartCoroutine(WaitUntilFinishedSpeaking(ResetLevel));
    }

    public void ResetLevel()
    {
        Debug.Log("Reset the level");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator WaitUntilFinishedSpeaking(System.Action functionToCall)
    {
        Debug.Log("Started coroutine");
        if (conversationMember != null)
        {
            yield return new WaitUntil(() => conversationMember.IsFinished());
        }
        // then call whatever
        functionToCall.Invoke();
    }

    void SpotTheCharacter()
    {
        // raycast from the head of the guard to various points on the player body. If it sees them and they're within the view cone
        // then the guard spots you.

        for (int i = 0; i < visibleObjects.Length; i++)
        {
            // check the visibility for all parts of that object
            float visibility = 0;
            for (int j = 0; j < visibleObjects[i].parts.Length; j++)
            {
                // check if they're within the view angle and view distance
                Transform part = visibleObjects[i].parts[j].part;
                if (Vector3.Distance(part.position, guardHead.position) < viewConeDistance)
                {
                    // then it's within the distance, check within angle
                    Vector3 localDifference = part.position - guardHead.position;
                    float angle = Vector3.Angle(localDifference, guardHead.forward);
                    if (angle < viewConeAngle)
                    {
                        // then it's possible to see them!
                        // check a raycast
                        // for now we're just going to say that it sees it
                        RaycastHit hit;
                        LayerMask mask = ~LayerMask.GetMask("Enemy") & ~LayerMask.GetMask("Ignore Raycast");
                        if (Physics.Raycast(guardHead.position, localDifference, out hit, viewConeDistance, mask))
                        {
                            if (hit.collider.gameObject.layer.Equals(visibleObjects[i].gameObject.layer))
                            {
                                // this may be incorrect if we have multiple visible objects of the same type... FIX
                                visibility += visibleObjects[i].parts[j].visibilityPercent;
                                suspicionTimer = suspicionTime;
                            }
                        }
                    }
                }
                if (visibility >= 1)
                {
                    // then we can break early
                    break;
                }
            }
            suspicion += Time.deltaTime * Mathf.Min(1, visibility) * suspicionMultiplier;
            if (suspicion > 0)
            {
                // then reduce the suspiciontimer
                suspicionTimer -= Time.deltaTime;
                if (suspicionTimer <= 0 && hasFiredStartSuspicionEvent)
                {
                    // they ran out of suspicion. FIX this to work with investigation.
                    suspicion = Mathf.Max(0, suspicion - Time.deltaTime * suspicionFallMultiplier);
                }
            }
        }
        //Debug.Log("Suspicion: " + suspicion);
    }

    void HandleSuspicion()
    {
        // FIX INVESTIGATION
        //if (suspicion > investigateSuspicionLevel)
        //{
        //    // record where we should investigate whenever we see things FIX
        //    if (conversationMember != null && spottedSomethingQuips.Length > 0)
        //    {
        //        // say a random quip
        //        string line = spottedSomethingQuips[Random.Range(0, spottedSomethingQuips.Length)];
        //        conversationMember.InterruptConversation(line);
        //    }
        //    //onStartSuspicionEvent.Invoke();
        //}

        if (suspicion > startSuspicionLevel && !hasFiredStartSuspicionEvent)
        {
            // then we are suspicious! fire off the suspicion function call
            hasFiredStartSuspicionEvent = true;
            onStartSuspicionEvent.Invoke();
            if (conversationMember != null && spottedSomethingQuips.Length > 0)
            {
                // say a random quip
                string line = spottedSomethingQuips[Random.Range(0, spottedSomethingQuips.Length)];
                conversationMember.InterruptConversation(line);
            }
        }

        if (suspicion >= fullySuspiciousLevel && !hasFiredSightingEvent)
        {
            // then we are suspicious! fire off the suspicion function call
            hasFiredSightingEvent = true;
            if (conversationMember != null && sightedPlayerQuips.Length > 0)
            {
                // say a random quip
                string line = sightedPlayerQuips[Random.Range(0, sightedPlayerQuips.Length)];
                conversationMember.InterruptConversation(line);
            }
            onSightingEvent.Invoke();
            if (stopWalking)
            {
                positions.Clear();
                // FIX this this should be a bool rather than just clearing it
                agent.SetDestination(transform.position);
            }
        }

        if (suspicion <= 0 && hasFiredStartSuspicionEvent)
        {
            hasFiredStartSuspicionEvent = false;
            if (conversationMember != null && lostSightQuips.Length > 0)
            {
                // say a random quip
                string line = lostSightQuips[Random.Range(0, lostSightQuips.Length)];
                conversationMember.InterruptConversation(line);
            }
            // we also fire the event which happens when they stop seeing them
            onLoseSuspicionEvent.Invoke();

            // here we should go back to our conversation after a segue. FIX
        }
    }
}
