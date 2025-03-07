using UnityEngine;

public class CakeSelection : MonoBehaviour
{
    public OvenManager ovenManager;

    public void OpenPanel(OvenManager ovenManager)
    {
        this.ovenManager = ovenManager;
    }

    public void ShowCake(string cakeType)
    {
        if (ovenManager != null)
        {
            // pass the selected cake type to the oven manager
            ovenManager.SetSelectedCake(cakeType);

            // hide the panel after selection
            gameObject.SetActive(false);
        }
    }
}