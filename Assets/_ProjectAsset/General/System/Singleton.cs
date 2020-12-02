using System.Runtime.Serialization;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : class
{
    public delegate void OnSingletonLoadedCallback();

    public static void ListenSingletonLoaded(OnSingletonLoadedCallback callback)
    {
        if (_instance != null) callback();
        else _invokeCallbacks += callback;
    }

    private static OnSingletonLoadedCallback _invokeCallbacks = null;

    private static Singleton<T> _instance = null;
    public static T GetInstance() => _instance as T;
    
    protected virtual void Awake()
    {
        if (_instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;

            _invokeCallbacks?.Invoke();
        }
    }
}
