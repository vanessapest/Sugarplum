using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[GameManager] Instance created.");
        }
        else
        {
            Debug.Log("[GameManager] Duplicate instance detected.");
        }
    }

    public void AddPoints(int points)
    {
        //this important 
        ScoreManager.Instance.AddScore(points);
    }
}
