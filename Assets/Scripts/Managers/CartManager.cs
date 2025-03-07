using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CartManager : MonoBehaviour
{
    public static CartManager Instance { get; private set; }
    [SerializeField] private List<GameObject> storedCakes = new List<GameObject>();
    public GameObject cartObject;
    public GameObject cartStorage;
    public GameObject cartFullSign;
    public GameObject[] slots;
    public bool isOpen = false;
    private GameObject activeCart;
    private GameObject activeCartStorage;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        if (cartStorage != null) cartStorage.SetActive(false);
        if (cartFullSign != null) cartFullSign.SetActive(false);

        LoadCartState();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        activeCart = GameObject.FindWithTag("Cart");

        if (activeCart != null)
        {
            activeCartStorage = activeCart.transform.Find("Cart_Storage")?.gameObject;
        }

        if (activeCart == null)
        {
            Debug.LogWarning($"[CartManager] No cart found in {scene.name}.");
            return;
        }

        if (activeCartStorage == null)
        {
            Debug.LogWarning($"[CartManager] No cart storage found in {scene.name}.");
        }
        else
        {
            Debug.Log($"[CartManager] Cart storage found in {scene.name}");
        }

        // activeCartStorage.transform.SetParent(null);

        Debug.Log($"[CartManager] Active cart set for {scene.name}");

        ResetCartPosition(scene.name);
        ResetCartStoragePosition(scene.name);

        ShowCart(true);
        LoadCartState();
    }

    public void ShowCart(bool visible)
    {
        if (cartObject != null) cartObject.SetActive(visible);
    }

    public void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void OnMouseDown()
    {
        if (cartStorage != null)
        {
            isOpen = !isOpen;
            cartStorage.SetActive(isOpen);

            foreach (var slot in slots)
            {
                foreach (Transform child in slot.transform)
                {
                    child.gameObject.SetActive(isOpen);
                }
            }
        }
    }

    public void StoreFinishedCake(GameObject cakePrefab, GameObject decorationPrefab)
    {
        foreach (var slot in slots)
        {
            if (slot.transform.childCount == 0)
            {
                GameObject cakeInstance = Instantiate(cakePrefab, slot.transform);
                cakeInstance.transform.localPosition = Vector3.zero;

                GameObject decorationInstance = Instantiate(decorationPrefab, slot.transform);
                decorationInstance.transform.localPosition = new Vector3(0, 0.035f, 0); 

                cakeInstance.GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";
                decorationInstance.GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";

                cakeInstance.transform.localScale = new Vector3(0.16f, 0.16f, 0.16f);
                decorationInstance.transform.localScale = new Vector3(0.16f, 0.16f, 0.16f);

                Debug.Log($"[CartManager] Added {cakePrefab.name} and {decorationPrefab.name} to {slot.name}");
                CheckCartFull();
                return;
            }
        }
        Debug.LogWarning("[CartManager] No available slot to store the finished cake and decoration.");
        CheckCartFull();

        GameObject cakeWithDecoration = Instantiate(cakePrefab);
        if (decorationPrefab != null)
        {
            GameObject decoration = Instantiate(decorationPrefab, cakeWithDecoration.transform);
        }
        storedCakes.Add(cakeWithDecoration);
        Debug.Log($"[CartManager] Stored cake: {cakeWithDecoration.name}");
    }

    public void CheckCartFull()
    {
        bool isCartFull = true;
        foreach (var slot in slots)
        {
            if (slot.transform.childCount == 0)
            {
                isCartFull = false;
                break;
            }
        }

        if (cartFullSign != null)
        {
            cartFullSign.SetActive(isCartFull);
        }
    }

    public bool IsCartFull()
    {
        foreach (var slot in slots)
        {
            if (slot.transform.childCount == 0) return false;
        }

        if (cartFullSign != null) cartFullSign.SetActive(true);
        return true;
    }

    public List<string> GetStoredCakes()
    {
        List<string> cakeNames = new List<string>();
        foreach (var cake in storedCakes)
        {
            cakeNames.Add(cake.name);
        }
        return cakeNames;
    }

    public void RestoreStoredCakes(List<string> cakeNames)
    {
        storedCakes.Clear();

        foreach (var cakeName in cakeNames)
        {
            GameObject cakePrefab = Resources.Load<GameObject>(cakeName);
            if (cakePrefab != null)
            {
                GameObject restoredCake = Instantiate(cakePrefab);
                storedCakes.Add(restoredCake);
                Debug.Log($"[CartManager] Restored cake: {cakeName}");
            }
            else
            {
                Debug.LogWarning($"[CartManager] Cake prefab not found for: {cakeName}");  
            }
        }
    }

    public void SaveCartState()
    {
        List<string> storedCakesNames = new List<string>();
    
        foreach (GameObject slot in slots) 
        {
            if (slot.transform.childCount > 0) 
            {
                storedCakesNames.Add(slot.transform.GetChild(0).name);
            }
        }

        PlayerPrefs.SetInt("StoredCakesCount", storedCakesNames.Count);

        for (int i = 0; i < storedCakesNames.Count; i++)
        {
            PlayerPrefs.SetString($"StoredCake_{i}", storedCakesNames[i]);
        }
        PlayerPrefs.Save();
    }

    public void LoadCartState()
    {
        storedCakes.Clear();

        int storedCakeCount = PlayerPrefs.GetInt("StoredCakesCount", 0);

        for (int i = 0; i < storedCakeCount; i++)
        {
            string cakeName = PlayerPrefs.GetString($"StoredCake_{i}");
            GameObject cakePrefab = Resources.Load<GameObject>(cakeName);

            if (cakePrefab != null)
            {
                GameObject restoredCake = Instantiate(cakePrefab, GetAvailableSlot());
                storedCakes.Add(restoredCake);
                Debug.Log($"[CartManager] Restored cake: {cakeName}");
            }
            else
            {
                Debug.LogWarning($"[CartManager] Could not find prefab for {cakeName}");
            }
        }
    }

    private Transform GetAvailableSlot()
    {
        foreach (GameObject slot in slots)
        {
            if (slot.transform.childCount == 0)
            {
                return slot.transform; 
            }
        }
        return null;
    }

    private void ResetCartPosition(string sceneName)
    {
        Vector3 newPosition = Vector3.zero; 

        if (sceneName == "KitchenScenery")
        {
            newPosition = new Vector3(1.72f, 2.8f, 0); 
        }
        else if (sceneName == "CoffeeShopScenery")
        {
            newPosition = new Vector3(-2.15f, -2.16f, 0); 
        }

        if (activeCart != null)
        {
            activeCart.transform.position = newPosition;
            Debug.Log($"[CartManager] Cart position set to {newPosition} in {sceneName}");
        }
    }

    private void ResetCartStoragePosition(string sceneName)
    {
        Vector3 storageLocalPosition = Vector3.zero; // Local position relative to the parent (Cart)

        if (sceneName == "KitchenScenery")
        {
            storageLocalPosition = new Vector3(-2.68189f, -3.654271f, 0); // Set specific local position for the kitchen
        }
        else if (sceneName == "CoffeeShopScenery")
        {
            storageLocalPosition = new Vector3(3.387651f, 4.124778f, 0); // Set specific local position for the coffee shop
        }

        if (activeCartStorage != null)
        {
            activeCartStorage.transform.localPosition = storageLocalPosition;
            Debug.Log($"[CartManager] Cart storage local position set to {storageLocalPosition} in {sceneName}");
        }
        else
        {
            Debug.LogWarning($"[CartManager] Cart storage not found in {sceneName}, local position not set.");
        }
    }
}
