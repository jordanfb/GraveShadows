using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkIfFreeColliderScript : MonoBehaviour
{
    // Start is called before the first frame update
    public int numChecks = 10;
    public Vector3 checkOffset = new Vector3(0, 0, .05f);
    public Vector3 safeSpace;

    private LayerMask collisionMask;

    private void Start()
    {
        collisionMask = ~LayerMask.GetMask("Evidence", "convoCollider", "Ignore Raycast", "Player");
    }

    public bool CheckExpandedCollisionsIsColliding()
    {
        // this function sweeps back and forth to check a wider swath of possible exit points for the player
        safeSpace = transform.position;
        Vector3 basePosition;
        for (int i = 0; i < numChecks * 2; i++)
        {
            float offset = i / 2 * (i % 2 == 0 ? 1 : -1);
            basePosition = transform.position + checkOffset * offset;
            //Debug.DrawLine(basePosition + transform.up * 1f, basePosition + transform.up * -1f);
            //if (i == numChecks * 2 - 1)
            //{
            //    for (int j = 0; j < 360; j += 15)
            //    {
            //        Vector3 furtherOffset = new Vector3(Mathf.Cos(j * Mathf.Deg2Rad), 0, Mathf.Sin(j * Mathf.Deg2Rad)) * .5f;
            //        Debug.DrawLine(basePosition + furtherOffset + transform.up * 1f, basePosition + furtherOffset + transform.up * -1f);
            //    }
            //}
            //Debug.Break();
            if (!Physics.CheckCapsule(basePosition + Vector3.up * .4f, basePosition + Vector3.up * -.4f, .5f, collisionMask))
            {
                // then we didn't collide! Tell the world that!
                safeSpace = basePosition;
                return false;
            }
        }
        return true; // only found collisions
    }
}
