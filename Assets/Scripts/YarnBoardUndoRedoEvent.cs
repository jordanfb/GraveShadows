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
    int itemIndex;
    SerializedEvidence evidence;

    bool inverted = false;

    public YarnBoardAddToYarnBoardEvent(int index, SerializedEvidence e, bool removeFromYarnboardEvent = false)
    {
        itemIndex = index;
        evidence = e;

        inverted = removeFromYarnboardEvent; // if this event should be inverted, so that we don't need two different classes
    }

    public override void Undo()
    {
        if (inverted)
        {
            // you removed it so undo the remove (i.e. add it back).
            AddToBoard();
        }
        else
        {
            // remove it from the yarn board
            RemoveFromBoard();
        }
    }
    public override void Redo()
    {
        if (inverted)
        {
            // redo the fact that you removed it from the board
            RemoveFromBoard();
        }
        else
        {
            // redo the fact that you added it to the board
            AddToBoard();
        }
    }

    private void AddToBoard()
    {
        evidence.evidenceState = SerializedEvidence.EvidenceState.OnYarnBoard;
    }

    private void RemoveFromBoard()
    {
        evidence.evidenceState = SerializedEvidence.EvidenceState.OffYarnBoard;
    }
}


[System.Serializable]
public class YarnBoardMoveEvidenceEvent : YarnBoardUndoRedoEvent
{
    int itemIndex;
    SerializedEvidence evidence;

    Vector3 start;
    Vector3 end;

    public YarnBoardMoveEvidenceEvent(int index, SerializedEvidence e, Vector3 startPos, Vector3 endPos)
    {
        itemIndex = index;
        evidence = e;

        start = startPos;
        end = endPos;
    }

    public override void Undo()
    {
        evidence.location = start;
    }
    public override void Redo()
    {
        evidence.location = end;
    }
}


[System.Serializable]
public class YarnBoardConnectEvent : YarnBoardUndoRedoEvent
{
    int itemIndex;
    SerializedEvidence evidence;
    int otherItemIndex;
    SerializedEvidence otherEvidence;


    bool inverted = false;

    public YarnBoardConnectEvent(int index, SerializedEvidence e, int otherIndex, SerializedEvidence otherEvidence, bool removeConnectionEvent = false)
    {
        itemIndex = index;
        evidence = e;
        inverted = removeConnectionEvent;
        otherItemIndex = otherIndex;
        this.otherEvidence = otherEvidence;
    }

    public override void Undo()
    {
        if (inverted)
        {
            // if you disconnected them undo that by connecting
            Connect();
        } else
        {
            Disconnect();
        }
    }
    public override void Redo()
    {
        if (inverted)
        {
            Disconnect();
        }
        else
        {
            Connect();
        }
    }

    private void Connect()
    {
        // first disconnect to make sure we only have one connection:
        Disconnect();
        // then add the connections in
        evidence.connectedEvidence.Add(otherItemIndex);
        otherEvidence.connectedEvidence.Add(itemIndex);
    }

    private void Disconnect()
    {
        evidence.connectedEvidence.RemoveAll((int x) => { return x == otherItemIndex; });
        otherEvidence.connectedEvidence.RemoveAll((int x) => { return x == itemIndex; });
    }
}
