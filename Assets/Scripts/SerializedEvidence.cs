using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializedEvidence
{
    // this is used for storing evidence between scenes and whatever
    public int evidenceindex = -1; // the index of the evidence on the global list of all evidence
    public EvidenceState evidenceState = EvidenceState.NotInGame;
    public Vector2 location; // where the evidence is located on the yarn board
    public List<int> connectedEvidence; // the list of evidence connected to this evidence on (or off) the yarn board.

    public SerializedEvidence(int evidence)
    {
        evidenceindex = evidence;
    }

    public enum EvidenceState
    {
        NotInGame, NotFound, OffYarnBoard, OnYarnBoard
    }
}
