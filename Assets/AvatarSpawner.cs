using UnityEngine;

public class AvatarSpawner : MonoBehaviour
{
    public GameObject[] characterPrefabs;
    public Transform leftSpawnPoint;
    public Transform rightSpawnPoint;
    public float spawnInterval = 5f;

    void Start()
    {
        InvokeRepeating(nameof(SpawnCharacter), 0f, spawnInterval);
    }

    void SpawnCharacter()
    {
        if (characterPrefabs.Length == 0) return;

        GameObject randomCharacterPrefab = characterPrefabs[Random.Range(0, characterPrefabs.Length)];

        Transform spawnPoint = Random.Range(0, 2) == 0 ? leftSpawnPoint : rightSpawnPoint;

        GameObject character = Instantiate(randomCharacterPrefab, spawnPoint.position, Quaternion.identity);

        AvatarMovement movement = character.GetComponent<AvatarMovement>();

        if (movement != null)
        {
            if (spawnPoint == rightSpawnPoint)
            {
                movement.direction = Vector3.left; // Move left
                character.GetComponent<SpriteRenderer>().flipX = true; // Flip sprite
            }
            else
            {
                movement.direction = Vector3.right; // Move right
            }
        }
    }
}
