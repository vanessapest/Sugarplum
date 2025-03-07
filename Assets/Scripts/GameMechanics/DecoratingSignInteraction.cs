using System.Collections;
using UnityEngine;

public class DecoratingSignInteraction : MonoBehaviour
{    
    public OvenManager ovenManager;

    public void OnMouseDown()
    {
        if (ovenManager != null)
        {
            ovenManager.OnDecoratingSignClicked();
        }
    }
}
