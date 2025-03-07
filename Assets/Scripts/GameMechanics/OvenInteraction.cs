using UnityEngine;

public class OvenInteraction : MonoBehaviour
{
    public Timer timer; 

    private void OnMouseDown()
    {
        if (timer != null)
        {
            // all the method to show the UI for 5 seconds
            timer.ShowUIForFiveSeconds();
        }
    }
}
