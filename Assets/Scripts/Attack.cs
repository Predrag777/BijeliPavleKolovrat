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
    [SerializeField] AudioClip speedSound;
    [SerializeField] AudioClip executionSound;
    [SerializeField] AudioClip jumpAttackExplosion; 
    [SerializeField] AudioClip swordBreakingSound;
    [SerializeField] ParticleSystem explosioEffect;
    [SerializeField] float speedOfAttack=1f; 

    

    AudioSource source;
    BoxCollider swordCollider;
    CapsuleCollider myCollider;
    SwordSounds swordSound;
    public bool isAI;
    CharacterController agent;
    [HideInInspector] public bool keepGuard=false;

    [HideInInspector] public bool superAttack=false;

    string [] attacks={"swing1", "swing2", "swing3"};
    string [] superAttacks={"swing1V1", "swing2V1", "swing3V1"};
    float speedSuperAttack=0.2f;
    int c=0;
    MoveAA aiMove;
    Move move;

    [HideInInspector] public bool finishHim=false;
    void Start()
    {
        if (explosioEffect!=null)
        {
            explosioEffect.Stop();
        }
        ps=GetComponent<PlayerStats>();
        myCollider=GetComponent<CapsuleCollider>();
        source = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();              
        swordCollider = sword.GetComponent<BoxCollider>();
        swordCollider.enabled = false;

        swordSound=GetComponentInChildren<SwordSounds>();
        agent=GetComponent<CharacterController>();
        if (isAI)
        {
            aiMove=GetComponent<MoveAA>();
        }
        else
        {
            
            move=GetComponent<Move>();
        }
    }

    void Update()
    {
        if(ps.health<=0f) return;
        if (isAI && aiMove.aiAttack && !isAttack)
        {
            /*if(aiMove!=null && aiMove.target!=null && aiMove.target.gameObject.GetComponent<Attack>().finishHim)
            {
                animator.SetBool("dead2", true);
                Debug.Log("Play dead2");
                return;
            }*/
            isAttack = true;
            hasHit = false;             // resetujem udarac

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
            if (Input.GetMouseButtonDown(0) && !isAttack && !isSpecialAttackRun && !Input.GetMouseButton(1))
            {
                
                
                isAttack = true;
                hasHit = false;             
                if(!agent.isGrounded){
                    animator.Play(attacks[2]);
                    StartCoroutine(jumpAttackSequence());
                }
                else{
                    animator.Play(attacks[c]);
                    c++;
                }
                if (c >= attacks.Length)
                {
                    c=0;
                }
                
                StartCoroutine(AttackRoutine());
            }
            if (Input.GetMouseButton(1)) 
            {
                keepGuard = true;
                //myCollider.enabled = false;
                animator.SetBool("block", true);

                // provjera smjera
                if (Input.GetKey(KeyCode.A))
                {
                    animator.SetBool("left", true);
                    animator.SetBool("right", false);
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    animator.SetBool("right", true);
                    animator.SetBool("left", false);
                }
                else
                {
                    animator.SetBool("left", false);
                    animator.SetBool("right", false);
                }

                move.speed=1f;
            }
            else
            {
                // prestao blok
                move.speed=5f;
                keepGuard = false;
                //myCollider.enabled = true;
                animator.SetBool("block", false);
                animator.SetBool("left", false);
                animator.SetBool("right", false);
            }


            if (Input.GetKey(KeyCode.T) &&!isAttack)
            {
                Debug.Log("ATTACK");
                StartCoroutine(specialAttack());
            }
            if(Input.GetKey(KeyCode.P) && !isAttack)
            {
                StartCoroutine(counterAttack());
            }
            if(Input.GetMouseButton(1) && Input.GetMouseButtonDown(0) && !isAttack && !finishHim)
            {
                StartCoroutine(pierceFinish());
            }


            if (Input.GetMouseButtonUp(1))
            {
                keepGuard=false;
                //myCollider.enabled=true;
                animator.SetBool("block", false);
                animator.SetBool("left", false);
                animator.SetBool("right", false);
                    
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
        if(swordSound!=null) swordSound.damage=1;


        swordCollider.enabled = true;

        source.PlayOneShot(swingSound);

        if (!hasHit)
        {
            // Nije bilo udarca → pusti swing zvuk
            
        }
        source.PlayOneShot(swingSound);
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
        if(swordSound!=null) swordSound.damage=1;
        
        for (int j = 0; j < 2; j++)
        {
            for (int i = 0; i < superAttacks.Length; i++)
            {
                // Animacija
                animator.Play(superAttacks[i]);

                // Udarac
                swordCollider.enabled = true;
                source.PlayOneShot(swingSound);

                yield return new WaitForSeconds(speedSuperAttack);

                swordCollider.enabled = false;

            }
        }
        animator.speed = 1f;
        Debug.Log("Specijalni napad END");
        isSpecialAttackRun = false;
        isAttack = false;
    }



    IEnumerator counterAttackSounds()
    {
        source.PlayOneShot(speedSound);
        yield return new WaitForSeconds(1f);
        swordCollider.enabled = true;
        source.PlayOneShot(executionSound);
    }

    IEnumerator counterAttack()
    {
        isAttack=true;
        if(swordSound!=null)   swordSound.damage=100;
        
        animator.Play("counter");
        StartCoroutine(counterAttackSounds());
        yield return new WaitForSeconds(1.5f);
        isAttack=false;
        swordCollider.enabled = false;
    }

    IEnumerator jumpAttackSequence()
    {

        yield return new WaitForSeconds(0.3f);
        if (explosioEffect!=null)
        {
            source.PlayOneShot(jumpAttackExplosion);
            explosioEffect.Play();
        }
    }

    IEnumerator pierceFinish()
    {
        
        Debug.Log("Pierce");
        animator.Play("pierce");
        finishHim=true;
        isAttack=true;
        source.PlayOneShot(swordBreakingSound);
        yield return new WaitForSeconds(0.45f);
        swordCollider.enabled = true;
        yield return new WaitForSeconds(1f);
        swordCollider.enabled = false;
        finishHim=false;
        isAttack=false;
    }

}
