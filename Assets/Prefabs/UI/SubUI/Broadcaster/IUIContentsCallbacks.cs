using UnityEngine.UI;

public interface IUIContentsCallbacks
{
    void OnClickProductContents(Button clicked, PlayerKingdom.ProductionTask pTask);
    void OnClickShipDataContents(Button clicked, PlayerKingdom.ProductWrapper pWrapper);
}
