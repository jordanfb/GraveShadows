using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GuardScript : MonoBehaviour
{
    public float speed = .5f; // meters per second
    [Space]
    public bool editPositions = true; // if false it edits rotations
    public List<Vector3> positions = new List<Vector3>();
    public List<Quaternion> rotations = new List<Quaternion>();

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

    // Start is called before the first frame update
    void Start()
    {
        if (positions.Count > 0)
        {
            target = 0;
            transform.position = positions[target];
            transform.rotation = rotations[target];
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (positions.Count > 0)
        {
            percentProgress += Time.deltaTime / percentSpeedThing;
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
            }
        }
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
