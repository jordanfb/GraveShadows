using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Animations;

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
    public float investigateGoalDistance = 2; // meters

    [Header("What happens when the guard has fully sighted the player?")]
    public float fullySuspiciousLevel = 1;
    public bool forceStopConverstion = true;
    public bool stopWalking = true;
    private bool hasFiredStartSuspicionEvent = false; // a latch for only fully sighting something once
    public TextAsset sightedPlayerQuipsTextAsset;
    private string[] sightedPlayerQuips;
    [Tooltip("This event is fired as soon as the guard's suspicion reaches critical levels")]
    public UnityEvent onSightingEvent;
    [Tooltip("if we're in the crime scene then we reload the level, otherwise we skip")]
    public bool isCrimeScene = false;

    public bool spotCameraSpin = true; // spin the camera towards the cop that spots you.
    public bool spotPlayerStopWalking = true; // stop the player walking when the cop spots you

    [Header("What should happen if the guard looses sight of something?")]
    public TextAsset lostSightQuipsTextAsset;
    private string[] lostSightQuips;
    [Tooltip("This event is fired when the guard no longer cares")]
    public UnityEvent onLoseSuspicionEvent;

    [Header("Guard returning to conversation quips")]
    public TextAsset returnToConversationQuipsTextAsset;
    private string[] returnToConversationQuips;

    [Space]
    public Animator animator;
    private int animatorWalkingId;
    private int animatorSuspicousId;
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


    private Vector3 investigatePosition;
    public bool isInvestigating = false;
    private bool isWalkingOverToInvestigate = false;
    private int dayNum; // the current daynum


    private int pathingTargetNumber;
    private NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        LoadQuips(); // load the quips!
        animatorWalkingId = Animator.StringToHash("Walking");
        animatorSuspicousId = Animator.StringToHash("IsSuspicious");
        dayNum = GameplayManager.instance.dayNum; // so that multiple guards don't spot the player at once

        agent = GetComponent<NavMeshAgent>();
        if (positions.Count > 0)
        {
            pathingTargetNumber = 0;
            transform.position = positions[pathingTargetNumber];
            // keep track of your starting angle though
            startingDirection = transform.rotation;
        }
        else
        {
            // it's a stationary agent so keep track of the starting position and starting angle
            startingDirection = transform.rotation;
            pathingTargetNumber = 0;
            positions.Add(transform.position);
        }
        visibleObjects = FindObjectsOfType<AIObjectVisibility>();
        Debug.Assert(visibleObjects.Length == 1); // if it's not 1 then we're in trouble for the investigate code
    }

    private void LoadQuips()
    {
        spottedSomethingQuips = new string[0];
        sightedPlayerQuips = new string[0];
        lostSightQuips = new string[0];
        returnToConversationQuips = new string[0];


        if (spottedSomethingQuipsTextAsset)
            spottedSomethingQuips = spottedSomethingQuipsTextAsset.text.Split('\n');
        if (sightedPlayerQuipsTextAsset)
            sightedPlayerQuips = sightedPlayerQuipsTextAsset.text.Split('\n');
        if (lostSightQuipsTextAsset)
            lostSightQuips = lostSightQuipsTextAsset.text.Split('\n');
        if (returnToConversationQuipsTextAsset)
            returnToConversationQuips = returnToConversationQuipsTextAsset.text.Split('\n');
    }

    // Update is called once per frame
    void Update()
    {
        if (animator)
        {
            animator.SetBool(animatorWalkingId, agent.velocity.sqrMagnitude > 0.1f); // if it's moving then animate!
            animator.SetBool(animatorSuspicousId, suspicion > investigateSuspicionLevel);
        }
        if (isInvestigating)
        {
            // then walk over and investigate!
            if (Vector3.Distance(investigatePosition, transform.position) < investigateGoalDistance)
            {
                // look around you!
                isWalkingOverToInvestigate = false; // once they've arrived they look around and are no longer walking over
                Debug.LogWarning("STOPPED INVESTIGATING");
            } else
            {
                // try to walk over towards the position!
                isWalkingOverToInvestigate = true; // if they're walking over they're walking over!
                agent.SetDestination(investigatePosition);
                //bool generateNewPath = true;
                //if (agent.hasPath) {
                //    // check if the path ends close to the destination
                //    // if it's not close then we should generate a new path
                //    if (Vector3.Distance(agent.destination, investigatePosition) > investigateGoalDistance)
                //    {
                //        // then delete it and make a new path that takes us closer
                //        generateNewPath = true;
                //    }
                //    else
                //    {
                //        generateNewPath = false;
                //    }
                //}

                //if (generateNewPath)
                //{
                //    NavMeshPath possiblePath = new NavMeshPath();
                //    bool pathValid = agent.CalculatePath(investigatePosition, possiblePath);
                //    if (pathValid && possiblePath.status == NavMeshPathStatus.PathComplete)
                //    {
                //        // then we follow the path!
                //        agent.SetPath(possiblePath);
                //    } else if (pathValid && possiblePath.status == NavMeshPathStatus.PathPartial)
                //    {
                //        // for now we probably just follow the partial path
                //    }
                //    else
                //    {
                //        // then choose a random location nearby to try to investigate
                //    }
                //}
            }
        }
        else if (positions.Count == 1)
        {
            // then return to the original position and look whatever way you're looking
            if (Vector3.Distance(positions[0], transform.position) > .2f)
            {
                agent.SetDestination(positions[0]);
            } else if (Quaternion.Angle(transform.rotation, startingDirection) > 5)
            {
                // rotate towards the original starting direction
                transform.rotation = Quaternion.RotateTowards(transform.rotation, startingDirection, 90 * Time.deltaTime);
            }
            // otherwise just sit content and look pretty.
            // try uninterupting the conversation if you were having one!
        }
        else if (positions.Count > 1)
        {
            if (!agent.hasPath)
            {
                // follow the path!
                agent.SetDestination(positions[pathingTargetNumber]);
                pathingTargetNumber++;
                pathingTargetNumber %= positions.Count;
            }
            if (agent.hasPath && agent.remainingDistance < .5f)
            {
                // then it's there, so move to the next point
                agent.SetDestination(positions[pathingTargetNumber]);
                pathingTargetNumber++;
                pathingTargetNumber %= positions.Count;
            }
        } else
        {
            // walk back to your original location
            Debug.Log("here");
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

    [ContextMenu("Find Animator")]
    public void FindAnimatorInChildren()
    {
        // this finds the conversation member if one exists in this/the children of the game object
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
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

    public void RunFunctionAfterFinishedSpeaking(System.Action action, float timeAfter)
    {
        StartCoroutine(WaitUntilFinishedSpeaking(action, timeAfter));
    }

    public void RunFunctionAfterTime(System.Action action, float timeAfter)
    {
        StartCoroutine(WaitUntilFinishedSeconds(action, timeAfter));
    }

    private IEnumerator WaitUntilFinishedSeconds(System.Action functionToCall, float timeAfter)
    {
        // then wait however many additional seconds
        yield return new WaitForSeconds(timeAfter);
        // then call whatever
        functionToCall.Invoke();
    }


    private IEnumerator WaitUntilFinishedSpeaking(System.Action functionToCall, float timeAfter = 0)
    {
        if (conversationMember != null)
        {
            yield return new WaitUntil(() => conversationMember.IsFinished());
        }
        // then wait however many additional seconds
        yield return new WaitForSeconds(timeAfter);
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
                if (part == null)
                {
                    continue;
                }
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
                                investigatePosition = visibleObjects[i].transform.position; // update the investigate position
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
            if (suspicion > 0 && !isWalkingOverToInvestigate)
            {
                // then reduce the suspiciontimer if the guard isn't walking over to investigate
                suspicionTimer -= Time.deltaTime;
                if (suspicionTimer <= 0 && hasFiredStartSuspicionEvent)
                {
                    isInvestigating = false; // force them to stop investigating
                    // they ran out of suspicion.
                    agent.ResetPath();
                    suspicion = Mathf.Max(0, suspicion - Time.deltaTime * suspicionFallMultiplier);
                }
            }
        }
        //Debug.Log("Suspicion: " + suspicion);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // draw text above your head please
        UnityEditor.Handles.Label(guardHead.position + guardHead.up, "Suspicion level: " + suspicion + "\nIs investigating: " + isInvestigating + "\nIswalking over: " + isWalkingOverToInvestigate + "\ndisance: " + Vector3.Distance(investigatePosition, transform.position));
    }
#endif

    void HandleSuspicion()
    {
        // FIX INVESTIGATION
        if (suspicion > investigateSuspicionLevel && !isWalkingOverToInvestigate)
        {
            // record where we should investigate whenever we see things fix
            // then investigate
            isWalkingOverToInvestigate = true; // once you get close to the target position then this is false, and then the timer falls
            // and once the timer is down we stop investigating.
            isInvestigating = true;
        }

        if (suspicion > startSuspicionLevel && !hasFiredStartSuspicionEvent)
        {
            // then we are suspicious! fire off the suspicion function call
            hasFiredStartSuspicionEvent = true;
            onStartSuspicionEvent.Invoke();
            if (conversationMember != null && spottedSomethingQuips.Length > 0)
            {
                // say a random quip
                string line = spottedSomethingQuips[Random.Range(0, spottedSomethingQuips.Length)];
                Debug.Log("Delivered line when start suspicious: " + line);
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
                Debug.Log("Delivered line when fully suspicious: " + line);
                conversationMember.InterruptConversation(line);
            }
            onSightingEvent.Invoke();
            animator.SetTrigger("AimAtPlayer");
            // also call the code to trigger the start of the scene change!
            // also point the camera at this guard! FIX
            if (!isCrimeScene)
            {
                if (GameplayManager.instance.dayNum == dayNum)
                {
                    GameLevelManager gameLevel = FindObjectOfType<GameLevelManager>();
                    Debug.Assert(gameLevel != null); // duh it can't be null we need it in all our levels
                    GameplayManager.instance.SkipDay(GameplayManager.instance.GenerateTodaysRecipt(gameLevel.level, gameLevel.evidenceFoundThisDay, true, gameLevel.HasFoundEverything()));
                    if (conversationMember != null && sightedPlayerQuips.Length > 0)
                    {
                        RunFunctionAfterFinishedSpeaking(GameplayManager.instance.VisitHubScene, 1);
                    }
                    else
                    {
                        RunFunctionAfterTime(GameplayManager.instance.VisitHubScene, 1);
                    }
                }
            }
            else
            {
                // this is the crime scene
                // don't skip the day, instead reload the day
                GameLevelManager gameLevel = FindObjectOfType<GameLevelManager>();
                Debug.Assert(gameLevel != null); // duh it can't be null we need it in all our levels
                //GameplayManager.instance.ReloadLevel(GameplayManager.instance.GenerateTodaysRecipt(gameLevel.level, gameLevel.evidenceFoundThisDay, true, gameLevel.HasFoundEverything()));
                if (conversationMember != null && sightedPlayerQuips.Length > 0)
                {
                    RunFunctionAfterFinishedSpeaking(() => { GameplayManager.instance.VisitScene(SceneManager.GetActiveScene().name); }, 1);
                }
                else
                {
                    RunFunctionAfterTime(() => { GameplayManager.instance.VisitScene(SceneManager.GetActiveScene().name); }, 1);
                }
            }
            if (spotCameraSpin)
            {
                StartCoroutine(SpinMainCameraTowards(guardHead.gameObject));
            }
            if (spotPlayerStopWalking)
            {
                // stop the player walking
                FindObjectOfType<simplePlayerMovement>().isAllowedToWalk = false; // stop the player from walking!
            }
            if (stopWalking)
            {
                // just stop moving at all
                positions.Clear();
                isInvestigating = false;
                isWalkingOverToInvestigate = false;
                // FIX this this should be a bool rather than just clearing it
                agent.ResetPath();
                agent.speed = 0; // force the agent to stop lol this is bad practice FIX
            }
        }

        if (suspicion <= 0 && hasFiredStartSuspicionEvent)
        {
            hasFiredStartSuspicionEvent = false;
            if (conversationMember != null && lostSightQuips.Length > 0)
            {
                // say a random quip
                string line = lostSightQuips[Random.Range(0, lostSightQuips.Length)];
                Debug.Log("Delivered line when lost sight: " + line);
                conversationMember.InterruptConversation(line);
            }
            // we also fire the event which happens when they stop seeing them
            onLoseSuspicionEvent.Invoke();

            // here we should go back to our conversation after a segue. FIX
        }
    }

    public IEnumerator SpinMainCameraTowards(GameObject go, float time = .5f)
    {
        // spins the main camera towards this
        Transform mc = Camera.main.transform;
        Quaternion targetDir = Quaternion.LookRotation(go.transform.position - mc.position);
        Quaternion startRot = mc.rotation;
        // disable anything controlling it here:
        ThirdPersonCamera[] thirdPersonCameras = FindObjectsOfType<ThirdPersonCamera>();
        // disable them all
        for (int j = 0; j < thirdPersonCameras.Length; j++)
        {
            thirdPersonCameras[j].enabled = false;
            // disable them. This is the nuclear option but that's thematically appropriate for our game and also we don't need the camera
            // later anyways
        }

        float i = 0;
        while (i < 1)
        {
            mc.rotation = Quaternion.Lerp(startRot, targetDir, i);
            i += Time.deltaTime / time;
            yield return null;
        }
        i = 1;
        mc.rotation = Quaternion.Lerp(startRot, targetDir, i);
    }
}
