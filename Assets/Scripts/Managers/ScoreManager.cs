using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public Slider levelProgressSlider;
    public Text levelText;
    public GameObject[] ovens;
    public int score;
    public int currentLevel = 1;
    public int pointsPerLevel = 300;
    public int maxLevel = 10;
    public bool ovensRestored = false;
    
    // delegate and event for notifying score updates
    public delegate void OnScoreChange(int totalPoints);
    public static OnScoreChange onScoreChanged;

    // delegate for the level up event
    public delegate void OnLevelUpEvent(int newLevel);
    public static event OnLevelUpEvent onLevelUp;

    public void Start() 
    {
        Instance = this;
        UpdateUI(score);

        //sample wenn du von außen subscriben willst:
        //ScoreManager.Instance.subscribeToScore(UpdateOven);

        onLevelUp += HandleOvenUnlocking;
    }

    /*public void OnSceneLoaded()
    {
        if (ovensRestored) return;

        ovens = GameObject.FindGameObjectsWithTag("Oven");
        foreach (var oven in ovens)
        {
            if (oven != null && oven.GetComponent<OvenManager>() != null)
            {
                onScoreChanged += oven.GetComponent<OvenManager>().UpdateOvenProgress;
            }
            else 
            {
                Debug.LogWarning("[ScoreManager] An oven is null or missing the OvenManager component.");
            }
        }
        ovensRestored = true;
    }*/
    
    public void AddScore(int scorePoints) 
    {
        score += scorePoints;

        if (onScoreChanged != null)
        {
            onScoreChanged.Invoke(score);
        }

        UpdateUI(score); 
    } 

    public void SubscribeToScore (OnScoreChange newEvent) 
    {
        onScoreChanged += newEvent;
    }

    public void UpdateUI(int totalPoints)
    {
        if (levelProgressSlider != null)
        {
            levelProgressSlider.maxValue = pointsPerLevel;
            levelProgressSlider.value = totalPoints % pointsPerLevel;
        }

        int calculatedLevel = Mathf.Min(totalPoints / pointsPerLevel + 1, maxLevel);

        if (calculatedLevel > currentLevel)
        {
            LevelUp(calculatedLevel);
        }

        if (levelText != null)
        {
            levelText.text = $"Lv. {currentLevel}";
        }
    }

    public void LevelUp(int newLevel)
    {
        if (newLevel > currentLevel)
        {
            currentLevel = newLevel;
            Debug.Log($"[ScoreManager] Level Up! Current Level: {currentLevel}");  

            onLevelUp.Invoke(currentLevel);
        }
    }

    public void HandleOvenUnlocking(int newLevel)
    {
        if (OvenBlockerManager.Instance != null)
        {
            OvenBlockerManager.Instance.EnableUnlockSignsForBlockedOvens();
        }
    }

    /*public void UpdateOven(int totalPoints) 
    {
        Debug.Log($"[ScoreManager] Updating ovens with total points: {totalPoints}");

        foreach (var oven in ovens)
        {
            OvenManager ovenManager = oven.GetComponent<OvenManager>();
            if (ovenManager != null)
            {
                ovenManager.UpdateOvenProgress(totalPoints);
            }
        }
    }*/
}
