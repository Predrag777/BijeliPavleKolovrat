using UnityEngine;

public class MoveAA : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float viewRange = 30f;
    [SerializeField] private float gravity = -9.81f;

    private Transform target;
    private CharacterController agent;
    private GameObject opponent;

    private float verticalVelocity = 0f;
    public bool aiAttack=false;

    void Start()
    {
        opponent = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (agent.isGrounded)
            verticalVelocity = 0f;
        else
            verticalVelocity += gravity * Time.deltaTime;

        Vector3 move = Vector3.zero;

        if (target == null)
        {
            DetectOpponent();
        }
        else
        {
            float distance = Vector3.Distance(target.position, transform.position);
            if (distance > 2.5f)
            {
                aiAttack=false;
                Vector3 direction = (target.position - transform.position).normalized;
                direction.y = 0f;
                move = direction * speed;

                // --- ROTACIJA PREMA TEBI ---
                Vector3 lookDir = (target.position - transform.position);
                lookDir.y = 0;
                if (lookDir != Vector3.zero)
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookDir), 10f * Time.deltaTime);
            }
            else
            {
                aiAttack=true;
            }
        }

        // Dodaj gravitaciju
        move.y = verticalVelocity;

        agent.Move(move * Time.deltaTime);
    }

    void DetectOpponent()
    {
        if (Vector3.Distance(opponent.transform.position, transform.position) <= viewRange)
        {
            Debug.Log("VIDIM TE!!!");
            target = opponent.transform;
        }
    }
}