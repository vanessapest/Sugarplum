using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public string activeScene = "";

    public bool hasRestoredOvens = false;
    public void SwitchToScene(string sceneName)
    {
        StartCoroutine(SwitchSceneCoroutine(sceneName));
    }

    public void Start() 
    {
        if (!SceneManager.GetSceneByName("MasterScene").isLoaded)
        {
            SceneManager.LoadScene("MasterScene", LoadSceneMode.Additive);
        }
        SwitchToScene("KitchenScenery");
    }

    public IEnumerator SwitchSceneCoroutine(string sceneName)
    {
        if (!hasRestoredOvens)
        {
            SaveSceneState();
            hasRestoredOvens = true;
        }
        
        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.isLoaded)
        {
            SaveSceneState(currentScene.name);
        }

        yield return new WaitForSeconds(0.1f);
        

        if (!string.IsNullOrEmpty(activeScene))
        {
            yield return SceneManager.UnloadSceneAsync(activeScene);
        }

        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        activeScene = sceneName;

        Scene newScene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(newScene);

        RestoreSceneState();

        Debug.Log($"[SceneSwitcher] Completed switch to {sceneName}");
    }

    private void SaveSceneState(string sceneName = null)
    {
        var ovens = FindObjectsOfType<OvenManager>();
        foreach (var oven in ovens)
        {
            if (oven != null && oven.gameObject != null)
            {
                Debug.Log($"[SceneSwitcher] Saving state for oven in {sceneName}");
                OvenStateManager.Instance.SaveOvenState(oven.gameObject.name, oven);
            }
        }
        OvenStateManager.Instance.SaveCartState();
    }
    private void RestoreSceneState()
    {
        // First try to restore any saved states to existing ovens in the scene
        if (OvenStateManager.Instance != null)
        {
            foreach (var ovenState in OvenStateManager.Instance.ovenStates)
            {
                string ovenId = ovenState.Key;
                GameObject ovenObj = GameObject.Find(ovenId);
                if (ovenObj != null)
                {
                    OvenManager oven = ovenObj.GetComponent<OvenManager>();
                    if (oven != null)
                    {
                        OvenStateManager.Instance.RestoreOvenState(ovenId, oven);
                    }
                }
            }
        }

        OvenStateManager.Instance?.RestoreCartState();
    }
}
