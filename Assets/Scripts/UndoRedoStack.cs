using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndoRedoStack : MonoBehaviour
{
    private static UndoRedoStack instance;
    int index = 0;
    List<YarnBoardUndoRedoEvent> events = new List<YarnBoardUndoRedoEvent>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(this.gameObject);
            Debug.LogWarning("multiple undo stacks deleting this one");
            return;
        }
        Reset();
    }

    public static void Reset()
    {
        // clear it all
        instance.events = new List<YarnBoardUndoRedoEvent>();
    }

    public static void AddEvent(YarnBoardUndoRedoEvent e) {
        // prune everything above the current index
        if (instance.index < instance.events.Count)
        {
            // delete what's at index onwards
            instance.events.RemoveRange(instance.index, instance.events.Count - instance.index);
        }
        // then add it in
        instance.events.Add(e);
        instance.index = instance.events.Count;
    }

    public static void Undo()
    {
        if (instance.index == 0)
        {
            // we can't go back further so just return
            return;
        }
        // otherwise we can go back!
        instance.index--;
        // now undo what we just found!
        instance.events[instance.index].Undo();
    }

    public static void Redo()
    {
        if (instance.index >= instance.events.Count)
        {
            // we can't go forwards further so just return
            return;
        }
        // now undo what we just found!
        instance.events[instance.index].Redo();
        // we can go forwards!
        instance.index++;
    }
}
