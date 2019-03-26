using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializedYarnBoard
{
    public List<SerializedEvidence> evidence;
    public List<YarnBoardUndoRedoEvent> undoStack;
}
