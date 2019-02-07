using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(GuardScript))]
public class GuardEditor : Editor
{
    private void OnSceneGUI()
    {
        GuardScript guard = (GuardScript)target;

        // draw the AI viewcone
        if (guard.guardHead != null)
        {
            // draw a cone
            Vector3 endPos = guard.guardHead.position + guard.guardHead.forward * guard.viewConeDistance;
            Debug.DrawLine(guard.guardHead.position, endPos, Color.blue, 0, true);
            float RadiusOfCircle = Mathf.Tan(guard.viewConeAngle * Mathf.Deg2Rad) * guard.viewConeDistance;
            int resolution = 16;
            for (int i = 0; i < resolution; i++)
            {
                // draw the circle ending
                float a1 = Mathf.PI * 2 * i / resolution;
                Vector3 a1Pos = guard.guardHead.right * RadiusOfCircle * Mathf.Cos(a1) + guard.guardHead.up * RadiusOfCircle * Mathf.Sin(a1);
                float a2 = Mathf.PI * 2 * (i + 1) / resolution;
                Vector3 a2Pos = guard.guardHead.right * RadiusOfCircle * Mathf.Cos(a2) + guard.guardHead.up * RadiusOfCircle * Mathf.Sin(a2);
                Debug.DrawLine(endPos + a1Pos, endPos + a2Pos, Color.blue, 0, true);
                Debug.DrawLine(endPos, endPos + a1Pos, Color.blue, 0, true);
                Debug.DrawLine(guard.guardHead.position, endPos + a2Pos, Color.blue, 0, true);
            }

        }

        // draw lines between each of the points so we know the order
        for (int i = 0; i < guard.positions.Count; i++)
        {
            Handles.DrawLine(guard.positions[i], guard.positions[(i + 1) % guard.positions.Count]);
        }

        EditorGUI.BeginChangeCheck();

        for (int i = 0; i < Mathf.Min(guard.positions.Count, guard.rotations.Count); i++)
        {
            if (guard.editPositions)
            {
                // handles where each of the points are
                guard.positions[i] = Handles.PositionHandle(guard.positions[i], guard.rotations[i]);
            }
            else
            {
                // handles for each of the rotations where the points are
                guard.rotations[i] = Handles.RotationHandle(guard.rotations[i], guard.positions[i]);
            }
        }

        if (EditorGUI.EndChangeCheck())
        {
            // then save it all!
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GuardScript guard = (GuardScript)target;

        if (guard.rotations.Count != guard.positions.Count) {
            Debug.LogError("Non-Equal number of rotations and positions");
            return;
        }

        //// check if the user deleted or add points and handle that nicelyish.
        //bool foundDifference = false;
        //int brokenDetector = 0;
        //do
        //{
        //    brokenDetector++;
        //    if (brokenDetector > 100)
        //    {
        //        Debug.Log("Error loop forced break");
        //        break;
        //    }
        //    foundDifference = false;
        //    int posDifference = FindDifference(guard.BackupPositions, guard.positions);
        //    int rotDifference = FindDifference(guard.BackupRotations, guard.rotations);
        //    if (posDifference != -1)
        //    {
        //        if (guard.backupPositions.Count > guard.positions.Count)
        //        {
        //            // deleted
        //            guard.backupPositions.RemoveAt(posDifference);
        //            foundDifference = true;
        //        }
        //        else if (guard.positions.Count > 0)
        //        {
        //            // added
        //            foundDifference = true;
        //            if (posDifference == guard.backupPositions.Count)
        //            {
        //                guard.backupPositions.Add(guard.positions[posDifference]);
        //            }
        //            else
        //            {
        //                guard.backupPositions.Insert(posDifference, guard.positions[posDifference]);
        //            }
        //        }
        //    }
        //    if (rotDifference != -1)
        //    {
        //        if (guard.backupRotations.Count > guard.rotations.Count)
        //        {
        //            // deleted
        //            foundDifference = true;
        //            guard.backupRotations.RemoveAt(rotDifference);
        //        }
        //        else if (guard.rotations.Count > 0)
        //        {
        //            // added
        //            foundDifference = true;
        //            if (rotDifference == guard.backupRotations.Count)
        //            {
        //                guard.backupRotations.Add(guard.rotations[rotDifference]);
        //            }
        //            else
        //            {
        //                guard.backupRotations.Insert(rotDifference, guard.rotations[rotDifference]);
        //            }
        //        }
        //    }
        //} while (foundDifference); // so that we can handle multiple additions/removes


        // then check if we should add more items I guess?

        guard.editIndex = EditorGUILayout.DelayedIntField("Index:", guard.editIndex);
        int index = guard.editIndex;
        if (index < 0)
        {
            index = guard.positions.Count + index + 1; // so we can index from the back as well!
        }
        if (GUILayout.Button("Add"))
        {
            if (index >= 0 && index < guard.positions.Count)
            {
                guard.positions.Insert(index, Vector3.zero);
                guard.rotations.Insert(index, Quaternion.identity);
            } else if (index == guard.positions.Count)
            {
                guard.positions.Add(Vector3.zero);
                guard.rotations.Add(Quaternion.identity);
            }
        }
        else if (GUILayout.Button("Remove"))
        {
            if (index >= 0 && index < guard.positions.Count)
            {
                guard.positions.RemoveAt(index);
                guard.rotations.RemoveAt(index);
            }
        }
    }

    private int FindDifference(List<Vector3> original, List<Vector3> newList)
    {
        int minCount = Mathf.Min(original.Count, newList.Count);
        if (minCount == 0)
        {
            return 0;
        }
        // otherwise loop over until they aren't equivalent any more
        // return the item index that isn't the same
        for (int i = 0; i < minCount; i++)
        {
            if (original[i] != newList[i])
            {
                return i;
            }
        }
        if (original.Count != newList.Count)
        {
            return minCount + 1;
        }
        return -1;
    }

    private int FindDifference(List<Quaternion> original, List<Quaternion> newList)
    {
        int minCount = Mathf.Min(original.Count, newList.Count);
        if (minCount == 0)
        {
            return 0;
        }
        // otherwise loop over until they aren't equivalent any more
        // return the item index that isn't the same
        for (int i = 0; i < minCount; i++)
        {
            if (original[i] != newList[i])
            {
                return i;
            }
        }
        if (original.Count != newList.Count)
        {
            return minCount + 1;
        }
        return -1;
    }
}
