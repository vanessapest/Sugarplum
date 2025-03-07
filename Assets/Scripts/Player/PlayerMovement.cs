using UnityEngine;

public class RandomMovement : MonoBehaviour
{
    [SerializeField] private float speed = 0.3f; 
    [SerializeField] private float changeDirectionInterval = 2f; 

    private Vector2 randomDirection;
    private Rigidbody2D rb;
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Start()
    {
        InvokeRepeating(nameof(ChangeDirection), 0f, changeDirectionInterval);
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + randomDirection * speed * Time.fixedDeltaTime);

        if (randomDirection != Vector2.zero)
        {
            animator.SetFloat("X", randomDirection.x);
            animator.SetFloat("Y", randomDirection.y);
            animator.SetBool("IsWalking", true);
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }
    }

    private void ChangeDirection()
    {
        int direction = Random.Range(0, 4);
        switch (direction)
        {
            case 0: randomDirection = Vector2.up; break; 
            case 1: randomDirection = Vector2.down; break;  
            case 2: randomDirection = Vector2.left; break;  
            case 3: randomDirection = Vector2.right; break; 
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        randomDirection = -randomDirection;
    }
}
