using UnityEngine;
using System.Collections;

public class Attack : MonoBehaviour
{
    Animator animator;
    public bool isAttack = false;
    bool hasHit = false;   
    PlayerStats ps;
    public bool isSpecialAttackRun=false;

    [SerializeField] GameObject sword;

    [SerializeField] AudioClip swingSound;
    [SerializeField] AudioClip blood;
    [SerializeField] AudioClip armor;
    [SerializeField] float speedOfAttack=0.5f; 
    [SerializeField] ParticleSystem speedEffect;


    [SerializeField] float distance=1.5f;

    AudioSource source;
    BoxCollider swordCollider;
    CapsuleCollider myCollider;
    public bool isAI;
    [HideInInspector] public bool keepGuard=false;

    [HideInInspector] public bool superAttack=false;

    string [] attacks={"swing1", "swing2", "swing3"};
    float speedSuperAttack=0.2f;

    int c=0;
    MoveAA aiMove;
    void Start()
    {
        speedEffect.Stop();
        ps=GetComponent<PlayerStats>();
        myCollider=GetComponent<CapsuleCollider>();
        source = GetComponent<AudioSource>();
        animator = GetComponentInChildren<Animator>();
        swordCollider = sword.GetComponent<BoxCollider>();
        swordCollider.enabled = false;

        if (isAI)
        {
            aiMove=GetComponent<MoveAA>();
        }
    }

    void Update()
    {
        if(ps.health<=0f) return;
        if (isAI && aiMove.aiAttack && !isAttack)
        {
            isAttack = true;
            hasHit = false;             

            animator.Play(attacks[c]);
            c++;
            if (c >= attacks.Length)
            {
                c=0;
            }
            
            StartCoroutine(AttackRoutine());
        }
        else if(!isAI)
        {
            if (Input.GetMouseButtonDown(0) && !isAttack && !isSpecialAttackRun)
            {
                
                isAttack = true;
                hasHit = false;             

                animator.Play(attacks[c]);
                c++;
                if (c >= attacks.Length)
                {
                    c=0;
                }
                
                StartCoroutine(AttackRoutine());
            }
            if (Input.GetMouseButtonDown(1))
            {
                keepGuard=true;
                myCollider.enabled=false;
                animator.SetBool("block", true);
            }
            if (Input.GetKey(KeyCode.T) &&!isAttack)
            {
                Debug.Log("ATTACK");
                StartCoroutine(specialAttack());
            }


            if (Input.GetMouseButtonUp(1))
            {
                keepGuard=false;
                myCollider.enabled=true;
                animator.SetBool("block", false);
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                superAttack = true;
                ps.energy-=3f;
                ps.UpdateEnergyBar();
            }
            if (Input.GetKeyUp(KeyCode.R))
            {
                superAttack = false;
            }

        }
        
    }

    IEnumerator AttackRoutine()
    {
        swordCollider.enabled = true;

        source.PlayOneShot(swingSound);
        speedEffect.Play();
        transform.position+=transform.forward*distance;
        if (!hasHit)
        {
            // Nije bilo udarca → pusti swing zvuk
            
        }
        source.PlayOneShot(swingSound);
        // Još malo drži collider aktivnim ako želiš
        yield return new WaitForSeconds(speedOfAttack);

        swordCollider.enabled = false;
        isAttack = false;
    }

    IEnumerator specialAttack()
    {
        Debug.Log("Specijalni napad START");
        isSpecialAttackRun = true;
        isAttack = true;
        animator.speed = 3f;
        for (int j = 0; j < 2; j++)
        {
            for (int i = 0; i < attacks.Length; i++)
            {
                // Animacija
                animator.Play(attacks[i]);

                // Udarac
                swordCollider.enabled = true;
                source.PlayOneShot(swingSound);

                yield return new WaitForSeconds(speedSuperAttack);

                swordCollider.enabled = false;

                // Ako je prekinuto u toku rada
                /*if (!Input.GetMouseButton(0) || !Input.GetKey(KeyCode.T))
                {
                    Debug.Log("Specijalni napad PREKINUT");
                    isSpecialAttackRun = false;
                    isAttack = false;
                    yield break;
                }*/
            }
        }
        animator.speed = 1f;
        Debug.Log("Specijalni napad END");
        isSpecialAttackRun = false;
        isAttack = false;
    }



}
