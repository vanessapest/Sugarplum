using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvenManager : MonoBehaviour
{
    public GameObject fireSign;
    public GameObject utensilSign;
    public GameObject decoratingSign;
    public GameObject finishedSign;
    public GameObject ovenLight;
    public Timer timer;

    public GameObject[] cakes; 
    public GameObject[] decorations;

    public StarSpawner starSpawner;
    public Transform ovenTransform;

    public string selectedCake;
    public GameObject cakeSelectionPanel;

    // public int pointsRequiredToUnlock = 300;
    public bool isUnlocked = false;
    public int currentLevel = 1;
    public bool canUnlockOven = true;
    [SerializeField] private string ovenId;
    
    public Dictionary<string, int> cakePoints = new Dictionary<string, int>
    {
        {"Vanilla", 12},
        {"Chocolate", 21},
        {"Strawberry", 27},
        {"Blueberry", 210},
        {"Mango", 360}
    };

    private void Awake()
    {
        string savedId = PlayerPrefs.GetString(gameObject.name, "");

        if (string.IsNullOrEmpty(savedId))
        {
            ovenId = savedId;
            Debug.Log($"[OvenManager] Loaded saved ID for {gameObject.name}");
        }
        else 
        {
            Debug.LogWarning($"[OvenManager] No saved ID found for {gameObject.name}.");
        }   

        if (GameObject.Find(gameObject.name) == null) // Prevent duplicates
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        ResetOven();

        if (PlayerPrefs.GetInt($"OvenUnlocked_{gameObject.name}", 0) == 1)
        {
            isUnlocked = true;
            Debug.Log($"[OvenManager] Oven {gameObject.name} is unlocked.");
        }

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.SubscribeToScore(UpdateOvenProgress);
        }
        else 
        {
            Debug.LogWarning("[OvenManager] ScoreManager instance not found at start.");
        }

        StartCoroutine(InitializeOvenState());
    }

    public void OnFireSignClicked()
    {
        if (ovenLight != null) ovenLight.SetActive(true);
        if (fireSign != null) fireSign.SetActive(false);
        if (utensilSign != null) utensilSign.SetActive(true);
    }

    public void OnUtensilSignClicked()
    {
        if (utensilSign != null) utensilSign.SetActive(false);
        if (decoratingSign != null) decoratingSign.SetActive(false);
        if (cakeSelectionPanel != null) cakeSelectionPanel.SetActive(true);
    }

    public void SetSelectedCake(string cakeType)
    {
        selectedCake = cakeType;
        foreach (var cake in cakes) if (cake != null) cake.SetActive(false);

        switch (cakeType)
        {
            case "Vanilla": 
                if (cakes[0] != null) cakes[0].SetActive(true); 
                timer.StartTimer(10f);
                break;
            case "Chocolate": 
                if (cakes[1] != null) cakes[1].SetActive(true); 
                timer.StartTimer(12f);
                break;
            case "Strawberry": 
                if (cakes[2] != null) cakes[2].SetActive(true); 
                timer.StartTimer(14f);
                break;
            case "Blueberry": 
                if (cakes[3] != null) cakes[3].SetActive(true); 
                timer.StartTimer(16f);
                break;
            case "Mango": 
                if (cakes[4] != null) cakes[4].SetActive(true); 
                timer.StartTimer(18f);
                break;
        }

        if (cakePoints.ContainsKey(selectedCake))
        {
            int pointsPerStage = cakePoints[selectedCake] / 3;  

            if (starSpawner != null)
            {
                starSpawner.SpawnStar(ovenTransform, pointsPerStage);
            }

            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(pointsPerStage);
            }
        }
    }

    public void OnDecoratingSignClicked()
    {
        if (decoratingSign != null) decoratingSign.SetActive(false);

        foreach (var decoration in decorations) decoration.SetActive(false);

        switch (selectedCake)
        {
            case "Vanilla": if (decorations[0] != null) decorations[0].SetActive(true); break;
            case "Chocolate": if (decorations[1] != null) decorations[1].SetActive(true); break;
            case "Strawberry": if (decorations[2] != null) decorations[2].SetActive(true); break;
            case "Blueberry": if (decorations[3] != null) decorations[3].SetActive(true); break;
            case "Mango": if (decorations[4] != null) decorations[4].SetActive(true); break;
        }

        if (timer != null) timer.ResumeTimer();

        if (cakePoints.ContainsKey(selectedCake))
        {
            int pointsPerStage = cakePoints[selectedCake] / 3;  

            if (starSpawner != null)
            {
                starSpawner.SpawnStar(ovenTransform, pointsPerStage);
            }

            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(pointsPerStage);
            }
        }
    }

    public void OnFinishedSignClicked()
    {
        CartManager cartManager = Object.FindFirstObjectByType<CartManager>();
        if (cartManager != null && cartManager.IsCartFull())
        {
            Debug.LogWarning("[OvenManager] Cart is full. Keeping the cake in the oven.");
            if (finishedSign != null) finishedSign.SetActive(true);
            return;
        }

        ResetOven();

        if (cakePoints.ContainsKey(selectedCake))
        {
            int pointsPerStage = cakePoints[selectedCake] / 3; 

            if (starSpawner != null)
            {
                starSpawner.SpawnStar(ovenTransform, pointsPerStage);
            }

            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(pointsPerStage);
            }

            Debug.Log($"[OvenManager] Adding points: {pointsPerStage}");
        }

        GameObject selectedCakePrefab = null;
        GameObject selectedDecorationPrefab = null;

        switch (selectedCake)
        {
            case "Vanilla":
                selectedCakePrefab = cakes[0];
                selectedDecorationPrefab = decorations[0];
                break;
            case "Chocolate":
                selectedCakePrefab = cakes[1];
                selectedDecorationPrefab = decorations[1];
                break;
            case "Strawberry":
                selectedCakePrefab = cakes[2];
                selectedDecorationPrefab = decorations[2];
                break;
            case "Blueberry":
                selectedCakePrefab = cakes[3];
                selectedDecorationPrefab = decorations[3];
                break;
            case "Mango":
                selectedCakePrefab = cakes[4];
                selectedDecorationPrefab = decorations[4];
                break;
        }

        if (selectedCakePrefab != null && selectedDecorationPrefab != null)
        {
            CartManager cart = Object.FindFirstObjectByType<CartManager>();
            if (cart != null)
            {
                cart.StoreFinishedCake(selectedCakePrefab, selectedDecorationPrefab);
            }
            else
            {
                Debug.LogWarning("[OvenManager] CartManager not found!");
            }
        }
        else
        {
            Debug.LogWarning("[OvenManager] No prefab found for the selected cake or decoration.");
        }
    }

    public void UpdateOvenProgress(int totalPoints)
    {
        Debug.Log($"[OvenManager] Oven progress updated. Total points: {totalPoints}");
    }

    public void OnOvenUnlocked()
    {
        if (canUnlockOven)
        {
            canUnlockOven = false; 
            Debug.Log($"[OvenManager] Oven Unlocked at Level {currentLevel}");
        }
    }

    public void ResetOven()
    {
        if (ovenLight != null) ovenLight.SetActive(false);

        if (fireSign != null) fireSign.SetActive(true);
        if (utensilSign != null) utensilSign.SetActive(false);
        if (decoratingSign != null) decoratingSign.SetActive(false);
        if (finishedSign != null) finishedSign.SetActive(false);

        foreach (var cake in cakes) cake.SetActive(false);
        foreach(var decoration in decorations) decoration.SetActive(false);

        if (timer != null) timer.HideUI();
    }

    private IEnumerator InitializeOvenState()
    {
        yield return new WaitForEndOfFrame();

        if (OvenStateManager.Instance != null)
        {
            Debug.Log($"[OvenManager] Initializing oven {gameObject.name}");
            OvenStateManager.Instance.RestoreOvenState(gameObject.name, this);
        }
    }
}
