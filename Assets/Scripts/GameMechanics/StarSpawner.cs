using System.Collections;
using UnityEngine;

public class StarSpawner : MonoBehaviour
{
    public GameObject starPrefab;

    public void SpawnStar(Transform ovenTransform, int points)
    {
        if (ovenTransform == null || starPrefab == null)
        {
            Debug.LogError("OvenTransform or StarPrefab is not assigned!");
            return;
        }

        float radius = 1f;
        float angle = Random.Range(0, Mathf.PI * 2);

        Vector3 targetPosition = ovenTransform.position + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);

        Vector3 screenPosition = Camera.main.WorldToScreenPoint(targetPosition);

        screenPosition.x = Mathf.Clamp(screenPosition.x, 50, Screen.width - 50); 
        screenPosition.y = Mathf.Clamp(screenPosition.y, 50, Screen.height - 50);

        targetPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, Camera.main.nearClipPlane));
        targetPosition.z = 0; 

        GameObject star = Instantiate(starPrefab, ovenTransform.position, Quaternion.identity);

        StartCoroutine(MoveStar(star, targetPosition));
    }

    public IEnumerator MoveStar(GameObject star, Vector3 targetPosition)
    {
        float duration = 0.5f; 
        float elapsedTime = 0f;

        Vector3 startPosition = star.transform.position;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / duration;

            float height = Mathf.Sin(Mathf.PI * t) * 0.5f; 

            Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, t);
            currentPosition.y += height; 

            star.transform.position = currentPosition;
            yield return null;
        }

        star.transform.position = targetPosition;

        Destroy(star, 2f);
    }
}
