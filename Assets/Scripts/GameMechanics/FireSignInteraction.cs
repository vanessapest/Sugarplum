using UnityEngine;

public class FireSignInteraction : MonoBehaviour
{
    public OvenManager ovenManager;

    public void OnMouseDown()
    {
        if (ovenManager != null)
        {
            ovenManager.OnFireSignClicked();
        }
    }
}
