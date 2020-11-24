using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ListChangedObserveComponent<T, SingletonComponent> : MonoBehaviour
    where SingletonComponent : Singleton<SingletonComponent>
{
    public delegate void AvailableTaskChangeListner(T changed, bool isAdd);

    public static AvailableTaskChangeListner BroadcastAvailableTaskChanged = null;

    public static void BroadcastListChange(T changed, bool isAdd) 
        => BroadcastAvailableTaskChanged?.Invoke(changed, isAdd);

    protected virtual void Awake()
    {
        BroadcastAvailableTaskChanged += OnListChanged;
        Singleton<SingletonComponent>.ListenSingletonLoaded(() => LoadList());
    }

    protected virtual void OnListChanged(T changed, bool isAdd)
    {

    }

    protected abstract void LoadList();
}