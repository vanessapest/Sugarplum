using UnityEngine;

public class FinishedSignInteraction : MonoBehaviour
{
    public OvenManager ovenManager;

    public void OnMouseDown()
    {
        if (ovenManager != null)
        {
            ovenManager.OnFinishedSignClicked();
        }
    }
}
