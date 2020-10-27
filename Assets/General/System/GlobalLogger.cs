using UnityEngine;

public enum GErrorType
{
    SingletonDuplicated,
    ComponentNull
}

public class GlobalLogger
{
    #region Message String
    private static readonly string _singletonDuplicated = " : Singleton Component Has Duplicated, Please Check Component";
    private static readonly string _componentNull = " : Component Null Error, Please Check Component is Exist";

    private static readonly string _typeNotSelected = " : Error Type Has Not Selected, Check Script";
    #endregion

    #region Public Method
    public static void CallLogError(string objectName, GErrorType etype) 
    {
        string message;

        switch (etype)
        {
            case GErrorType.SingletonDuplicated:
                message = _singletonDuplicated;
                break;

            case GErrorType.ComponentNull:
                message = _componentNull;
                break;

            default:
                message = _typeNotSelected;
                break;
        }

        Debug.LogError(objectName + message);
    }
    #endregion
}
