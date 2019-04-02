using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class YarnBoardUndoRedoEvent
{
    // this is the parent class to undo and redo events. This is so we can undo and redo editing the yarn board
    // events that we can undo/redo:
    // toggle evidence on/off yarnboard
    // moving evidence to a new location
    // adding/deleting yarn

    public abstract void Undo();
    public abstract void Redo();
}


[System.Serializable]
public class YarnBoardAddToYarnBoardEvent : YarnBoardUndoRedoEvent
{
    // this is the parent class to undo and redo events. This is so we can undo and redo editing the yarn board
    // events that we can undo/redo:
    // toggle evidence on/off yarnboard
    // moving evidence to a new location
    // adding/deleting yarn
    int itemIndex;

    public YarnBoardAddToYarnBoardEvent(int index, SerializedEvidence e)
    {
        itemIndex = index;
    }

    public override void Undo()
    {
        // remove it from the yarn board

    }
    public override void Redo()
    {

    }
}
