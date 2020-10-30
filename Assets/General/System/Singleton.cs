using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : class
{
    private static Singleton<T> _instance = null;
    public static T GetInstance() => _instance as T;
    
    protected virtual void Awake()
    {
        if (_instance != null)
        {
            GlobalLogger.CallLogError(this.gameObject.name, GErrorType.SingletonDuplicated);
            Destroy(this.gameObject);
        }

        _instance = this;
    }
}
