using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class OvenState
{
    public bool isLightOn;
    public bool isFireSignOn;
    public bool isUtensilSignOn;
    public bool isDecoratingSignOn;
    public bool isFinishedSignOn;
    public string selectedCake;
    public float remainingBakeTime;
    public bool isUnlocked;
    // public bool isBlocked;
    // public bool hasUnlockedOvenThisLevel;
    public bool wasUnlockedInThisSession;
    public string ovenPrefabName;
    public int currentLevel;
}

public class OvenStateManager : MonoBehaviour
{
    public static OvenStateManager Instance { get; private set; }
    public Dictionary<string, OvenState> ovenStates = new Dictionary<string, OvenState>();
    public List<string> cartCakes = new List<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void SaveOvenState(string ovenId, OvenManager oven)
    {
        if (oven == null) return;

        try 
        {
            OvenState state = new OvenState
            {
                isLightOn = oven.ovenLight.activeSelf,
                isFireSignOn = oven.fireSign.activeSelf,
                isUtensilSignOn = oven.utensilSign.activeSelf,
                isDecoratingSignOn = oven.decoratingSign.activeSelf,
                isFinishedSignOn = oven.finishedSign.activeSelf,
                selectedCake = oven.selectedCake,
                remainingBakeTime = oven.timer.GetRemainingTime(),
                isUnlocked = oven.isUnlocked,
                wasUnlockedInThisSession = oven.isUnlocked,
                ovenPrefabName = oven.gameObject.name,
                currentLevel = oven.currentLevel
            };

            ovenStates[ovenId] = state;
            Debug.Log($"[OvenStateManager] Saved state for oven {ovenId}");

            PlayerPrefs.SetInt($"OvenUnlocked_{ovenId}", oven.isUnlocked ? 1 : 0);
            PlayerPrefs.Save();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[OvenStateManager] Error saving state for oven {ovenId}: {e.Message}");
        }
    }

    public void RestoreOvenState(string ovenId, OvenManager oven)
    {
        try 
        {
            if (!ovenStates.TryGetValue(ovenId, out OvenState state))
            {
                Debug.Log($"[OvenStateManager] No saved state found for oven {ovenId}");
                return;
            }

            // If no oven was passed in, try to find it in the scene
            if (oven == null)
            {
                GameObject ovenObj = GameObject.Find(ovenId);
                if (ovenObj != null)
                {
                    oven = ovenObj.GetComponent<OvenManager>();
                }
            }

            // If we still don't have a valid oven, log and return
            if (oven == null)
            {
                Debug.LogError($"[OvenStateManager] Cannot find oven {ovenId} in scene");
                return;
            }

            // Restore the state
            oven.isUnlocked = state.isUnlocked;
            oven.ovenLight?.SetActive(state.isLightOn);
            oven.fireSign?.SetActive(state.isFireSignOn);
            oven.utensilSign?.SetActive(state.isUtensilSignOn);
            oven.decoratingSign?.SetActive(state.isDecoratingSignOn);
            oven.finishedSign?.SetActive(state.isFinishedSignOn);

            oven.selectedCake = state.selectedCake;

            if (!string.IsNullOrEmpty(state.selectedCake))
            {
                oven.SetSelectedCake(state.selectedCake);
                if (state.remainingBakeTime > 0 && oven.timer != null)
                {
                    oven.timer.StartTimer(state.remainingBakeTime);
                }
            }
            
            oven.currentLevel = state.currentLevel;

            Debug.Log($"[OvenStateManager] Successfully restored state for oven {ovenId}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[OvenStateManager] Error restoring state for oven {ovenId}: {e.Message}");
        }
    }

    public void SaveCartState()
    {
        if (CartManager.Instance != null)
        {
            cartCakes = CartManager.Instance.GetStoredCakes();
            Debug.Log("[OvenStateManager] Saved cart state.");
        }
    }

    public void RestoreCartState()
    {
        if (CartManager.Instance != null)
        {
            CartManager.Instance.RestoreStoredCakes(cartCakes);
            Debug.Log("[OvenStateManager] Restored cart state.");
        }
    }
}
