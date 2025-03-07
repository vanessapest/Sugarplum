using UnityEngine;

public class UtensilSignInteraction : MonoBehaviour
{
    public OvenManager ovenManager;
    public void OnMouseDown()
    {
        if (ovenManager != null)
        {
            if (ovenManager.cakeSelectionPanel != null)
            {
                ovenManager.cakeSelectionPanel.SetActive(true);

                CakeSelection cakeSelection = ovenManager.cakeSelectionPanel.GetComponent<CakeSelection>();
                if (cakeSelection != null)
                {
                    cakeSelection.ovenManager = ovenManager;
                }
                //einfache aber unschöne lösung
                Object.FindFirstObjectByType<CloseButtonHandler>().SetOvenManager(ovenManager);
            }
            if (ovenManager.utensilSign != null)
            {
                ovenManager.utensilSign.SetActive(false);
            }
        }
    }
}
