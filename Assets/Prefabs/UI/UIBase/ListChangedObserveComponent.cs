using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ListChangedObserveComponent<T, SingletonComponent> : MonoBehaviour
    where SingletonComponent : Singleton<SingletonComponent>
{
    public delegate void AvailableTaskChangeListner(T changed, bool isAdd);

    private static AvailableTaskChangeListner _broadcastAvailableTaskChanged = null;

    public static void BroadcastListChange(T changed, bool isAdd) 
        => _broadcastAvailableTaskChanged?.Invoke(changed, isAdd);

    protected virtual void Awake()
    {
        _broadcastAvailableTaskChanged += OnListChanged;
        Singleton<SingletonComponent>.ListenSingletonLoaded(() => LoadList());
    }

    protected virtual void OnListChanged(T changed, bool isAdd)
    {

    }

    protected abstract void LoadList();

    protected Dictionary<T, GameObject> _objectUIContentsHash = new Dictionary<T, GameObject>();
}