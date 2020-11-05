using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TaskListCallbacks<T> : MonoBehaviour where T : PlayerKingdom.PlayerTask
{
    public delegate void AvailableTaskChangeListner(T changed, bool isAdd);

    public static AvailableTaskChangeListner BroadcastAvailableTaskChanged = null;

    protected virtual void Awake()
    {
        BroadcastAvailableTaskChanged += OnAvailableListChanged;
        LoadTaskList();
    }

    protected virtual void OnAvailableListChanged(T changed, bool isAdd)
    {

    }

    protected abstract void LoadTaskList();

    protected Dictionary<T, GameObject> _productUIContentsHash
        = new Dictionary<T, GameObject>();
}