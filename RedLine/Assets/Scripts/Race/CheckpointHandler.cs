using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointHandler : MonoBehaviour
{

    public int GetNumberOfChildren()
    {
        return transform.childCount;
    }
    /// <summary>
    /// Return the position of the checkpoint at the given index.
    /// </summary>
    public Transform GetCheckpoint(int index)
    {
        return transform.GetChild(index);
    }

    public void Inistialize()
    {
        GameManager.gManager.checkpointParent = this;
    }


    /// <summary>
    /// Returns the next index.
    /// </summary>
    public int GetNextIndex(int current)
    {
        int nextIndex = current + 1;
        if (nextIndex >= transform.childCount)
        {
            return 0;
        }
        return nextIndex;
    }
}
