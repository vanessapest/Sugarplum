using System.Collections.Generic;
using UnityEngine;

public class OvenBlockerManager : MonoBehaviour
{
    public static OvenBlockerManager Instance { get; private set; }
    public List<OvenBlocker> ovenBlockers = new List<OvenBlocker>(); 
    public bool unlockMode = false;
    
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.onLevelUp += OnLevelUpHandler;
        }
    }

    public void OnLevelUpHandler(int newLevel)
    {
        Debug.Log($"[OvenBlockerManager] Level-up event triggered. Current Level: {newLevel}");
        EnableUnlockSignsForBlockedOvens();
    }

    public void EnableUnlockSignsForBlockedOvens()
    {
        if (!unlockMode)
        {
            unlockMode = true;
            foreach (var blocker in ovenBlockers)
            {
                if (!blocker.IsUnlocked)
                {
                    blocker.ShowUnlockSign();
                    Debug.Log($"[OvenBlockerManager] Showing unlock sign for {blocker.name}.");
                }
            }
            Debug.Log("[OvenBlockerManager] Unlock mode enabled.");
        }
    }

    public void UnlockOven (OvenManager ovenManager, OvenBlocker selectedBlocker)
    {
        if (!unlockMode) return;

        selectedBlocker.Unlock();
        unlockMode = false;

        foreach (var blocker in ovenBlockers)
        {
            blocker.HideUnlockSign();
        }

        Debug.Log($"[OvenBlockerManager] Oven unlocked: {selectedBlocker.name}");
        ovenManager.OnOvenUnlocked();

        PlayerPrefs.SetInt($"OvenUnlocked_{ovenManager.gameObject.name}", 1);
        PlayerPrefs.Save();
    }

    /*public void ReEnableUnlockMode()
    {
        unlockMode = true;
        EnableUnlockSignsForBlockedOvens();
        Debug.Log("[OvenBlockerManager] Unlock mode re-enabled.");
    }*/
}
