using UnityEngine;

public class AvatarMovement : MonoBehaviour
{
    public float minSpeed = 0.1f;
    public float maxSpeed = 2f;
    private float speed;
    public Vector3 direction;
    public float despawnDistance = 10f;

    private Animator animator;

    void Start()
    {
        speed = Random.Range(minSpeed, maxSpeed);
        direction = Random.Range(0, 2) == 0 ? Vector3.right : Vector3.left;
        animator = GetComponent<Animator>(); 

        if (direction == Vector3.left)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }

        if (animator != null)
        {
            animator.SetBool("IsWalking", true); 
        }
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
        
        if (Mathf.Abs(transform.position.x) > despawnDistance)
        {
            Destroy(gameObject);
        }
    }
}
