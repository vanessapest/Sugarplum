using UnityEngine;

public class OvenBlocker : MonoBehaviour
{
    public bool IsUnlocked { get; private set; } = false;
    public GameObject unlockSign;
    public GameObject openedOven;
    public OvenManager associatedOven;

    private void OnMouseDown()
    {
        if (!IsUnlocked && unlockSign.activeSelf && OvenBlockerManager.Instance.unlockMode)
        {
            Unlock();
            OvenBlockerManager.Instance.UnlockOven(associatedOven, this);
        }
    }

    public void ShowUnlockSign()
    {
        if (!IsUnlocked)
        {
            unlockSign.SetActive(true);
        }
    }

    public void HideUnlockSign()
    {
        unlockSign.SetActive(false);
    }

    public void Unlock()
    {
        IsUnlocked = true;
        unlockSign.SetActive(false);
        
        if (openedOven != null)
        {
            openedOven.SetActive(true);
        }
        gameObject.SetActive(false);
        Debug.Log($"{gameObject.name} unlocked!");
    }
}
