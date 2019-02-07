using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public class GuardScript : MonoBehaviour
{
    public float speed = .5f; // meters per second
    [Space]
    public bool editPositions = true; // if false it edits rotations
    public List<Vector3> positions = new List<Vector3>();
    public List<Quaternion> rotations = new List<Quaternion>();

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

    //[HideInInspector]
    //public List<Vector3> backupPositions = new List<Vector3>(); // these are for the editor
    //[HideInInspector]
    //public List<Quaternion> backupRotations = new List<Quaternion>(); // these are for the editor

    private int target;
    private Quaternion rotation = Quaternion.identity;
    private Quaternion previousRotation = Quaternion.identity;
    float percentProgress = 0; // this is for lerping the quaternions
    float percentSpeedThing = 1; // this is also for quaternions

    private NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (positions.Count > 0)
        {
            target = 0;
            transform.position = positions[target];
            transform.rotation = rotations[target];
        }
        visibleObjects = FindObjectsOfType<AIObjectVisibility>();
    }

    // Update is called once per frame
    void Update()
    {
        if (positions.Count > 0)
        {
            /*percentProgress += Time.deltaTime / percentSpeedThing;
            // move towards each of the positions in turn
            transform.position = Vector3.MoveTowards(transform.position, positions[target], speed * Time.deltaTime);
            //transform.rotation = Quaternion.Lerp(previousRotation, rotation, percentProgress);
            if (Vector3.Distance(transform.position, positions[target]) != 0)
            {
                transform.LookAt(positions[target]);
            }

            if (transform.position == positions[target])
            {
                // then find the next target
                target++;
                target %= positions.Count;
                rotation = rotations[target];
                previousRotation = transform.rotation;
                // now calculate the time to turn over
                float distance = Vector3.Distance(transform.position, positions[target]);
                percentSpeedThing = distance / speed;
            }*/
            if (!agent.hasPath)
            {
                agent.SetDestination(positions[target]);
                target++;
                target %= positions.Count;
            }
            if (agent.remainingDistance < .5f)
            {
                // then it's there, so move to the next point
                agent.SetDestination(positions[target]);
                target++;
                target %= positions.Count;
            }
        }

        // spot the character
        SpotTheCharacter();
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
                        LayerMask mask = ~LayerMask.GetMask("Enemy");
                        if (Physics.Raycast(guardHead.position, localDifference, out hit, viewConeDistance, mask))
                        {
                            if (hit.collider.gameObject.layer.Equals(visibleObjects[i].gameObject.layer))
                            {
                                // this may be incorrect if we have multiple visible objects of the same type... FIX
                                visibility += visibleObjects[i].parts[j].visibilityPercent;
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
            suspicion += Time.deltaTime * Mathf.Max(1, visibility);
        }
        Debug.Log("Suspicion: " + suspicion);
    }

    public List<Vector3> BackupPositions
    {
        get;set;
    }

    public List<Quaternion> BackupRotations
    {
        get;set;
    }
}
