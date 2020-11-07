using UnityEngine.UI;

using PlayerKindom.PlayerKindomTypes;

public interface IUIContentsCallbacks
{
    void OnClickProductContents(Button clicked, ProductionTask pTask);
    void OnClickShipDataContents(Button clicked, ProductWrapper pWrapper);
}
