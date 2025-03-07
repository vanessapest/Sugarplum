using UnityEngine;

public class CloseButtonHandler : MonoBehaviour
{
    public OvenManager ovenManager;
    public GameObject panel; 

    public void SetOvenManager(OvenManager manager)
    {
        this.ovenManager = manager;
    }

    public void ClosePanel() 
    {
        if (panel != null)
        {
            panel.SetActive(false); 
        }

        // ensures the utensil sign becomes visible after the panel is closed
        if (ovenManager != null && ovenManager.utensilSign != null)
        {
            ovenManager.utensilSign.SetActive(true); // shows the utensil sign
            Debug.Log("[CloseButtonHandler] Utensil sign activated");
        }
        else 
        {
            Debug.LogWarning("[CloseButtonHandler] Utensil sign not found or OvenManager not assigned");
        }
    }
}
