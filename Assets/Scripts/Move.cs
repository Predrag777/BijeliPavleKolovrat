using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour
{
    [SerializeField] private float speed=5f;

    private CharacterController agent;
    float horizontalSpeed=10f;
    float gravity=-9.81f;
    float height=2f;
    PlayerStats ps;

    Vector3 playerVelovicty;
    Attack attack;

    SwordSounds sw;

    [SerializeField] float dodgeDistance = 5f;
    [SerializeField] float dodgeDuration = 0.15f;
    [SerializeField] ParticleSystem dustEffect; 
    AudioSource source;
    [SerializeField] AudioClip dashSound;
    bool isDodging = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ps=GetComponent<PlayerStats>();
        attack=GetComponent<Attack>();
        agent=GetComponent<CharacterController>();
        Cursor.lockState=CursorLockMode.Locked;

        sw=GetComponentInChildren<SwordSounds>();
        dustEffect.Stop();
        source=GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(ps.health<=0f) return;
        if(attack.isAttack) return;
        if(Input.GetKeyDown(KeyCode.Space)) Jump();
        
        playerVelovicty.y+=gravity*Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.LeftShift))
            TryDodge();

        if(!isDodging){
            if (sw.hitTarget)
            {
                RotateTowardsTarget(sw.hitTarget.transform);
            }
            else
            {
                RotationController();
            }
        MoveController();
        }

        agent.Move(playerVelovicty*Time.deltaTime);
    }

    void RotateTowardsTarget(Transform target)
    {
        Vector3 direction = target.position - transform.position;
        direction.y = 0f;   

        if (direction.magnitude < 0.01f) return;

        Quaternion desiredRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            desiredRotation,
            Time.deltaTime * 10f 
        );
    }


    void MoveController(){
        float horizontal=Input.GetAxis("Horizontal");
        float vertical=Input.GetAxis("Vertical");

        Vector3 step=transform.forward*vertical+transform.right*horizontal;

        agent.Move(step*speed*Time.deltaTime);
    }

    void RotationController(){
        float horizontal=horizontalSpeed*Input.GetAxis("Mouse X");
        transform.Rotate(0f,horizontal,0f);
    }

    void Jump()
    {
        if (agent.isGrounded)
        {
            playerVelovicty.y=Mathf.Sqrt(height*(-3f)*gravity);
        }
    }



    //////////////////////////PSECIAL MOVES
    /// 
    void TryDodge()
    {
        if(ps.energy<3f) return;
        Vector3 dir = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) dir = transform.forward;
        else if (Input.GetKey(KeyCode.S)) dir = -transform.forward;
        else if (Input.GetKey(KeyCode.A)) dir = -transform.right;
        else if (Input.GetKey(KeyCode.D)) dir = transform.right;

        if (dir != Vector3.zero){
            StartCoroutine(Dodge(dir));}
    }

    IEnumerator Dodge(Vector3 direction)
    {
        isDodging = true;
        source.PlayOneShot(dashSound);
        float timer = 0f;
        Vector3 dodgeStep = direction.normalized * (dodgeDistance / dodgeDuration);

        while (timer < dodgeDuration)
        {
            dustEffect.Play();
            agent.Move(dodgeStep * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
        ps.energy-=3f;
        ps.UpdateEnergyBar();
        isDodging = false;
        dustEffect.Stop();
    }

}
