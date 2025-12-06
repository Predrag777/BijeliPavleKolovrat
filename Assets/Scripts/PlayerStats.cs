using UnityEngine;
using UnityEngine.UI; // za Image
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    public int maxHealth = 5;
    public int health = 5;
    bool hit = false;
    public float energy=10f;

    [SerializeField] AudioClip swordHit;
    [SerializeField] ParticleSystem blood;
    [SerializeField] ParticleSystem deathParticle;
    AudioSource source;
    Animator animator;

    public bool isAI = false;
    [SerializeField] Image healthbar; // Image koji prati health
    [SerializeField] Image energybar;
    bool isDeath = false;
    bool runOnceRecovery=false;

    void Start()
    {
        animator = GetComponent<Animator>();
        blood.Stop();
        source = GetComponent<AudioSource>();

        if (!isAI && healthbar == null)
        {
            // pronalazi Image po imenu "points"
            healthbar = GameObject.Find("points").GetComponent<Image>();
        }

        health = maxHealth;
        UpdateHealthBar();
    }

    void Update()
    {
        if(health<=0f) return;
        if (energy < 8f && !runOnceRecovery)
        {
            StartCoroutine(giveEnergyBack());
        }
    }

    void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Sword") && !hit)
        {
            Transform otherParent = other.transform.parent;
            if (otherParent != null && otherParent.CompareTag(transform.tag)) return;
            hit = true;
            Debug.Log("Udario si me");
            StartCoroutine(takeHealth());
        }
    }

    IEnumerator takeHealth()
    {
        health -= 1;
        UpdateHealthBar();
        blood.Play();

        if (health <= 0)
        {
            if (!isDeath)
            {
                Instantiate(deathParticle, transform.position, Quaternion.identity);
                isDeath = true;
                Destroy(gameObject);
            }
        }

        yield return new WaitForSeconds(0.5f);
        hit = false;
    }

    void UpdateHealthBar()
    {
        if (healthbar != null)
        {
            healthbar.fillAmount = (float)health / maxHealth;
        }
    }

    public void UpdateEnergyBar()
    {
        energybar.fillAmount=(float)energy/10f;
    }

    IEnumerator giveEnergyBack()
    {
        runOnceRecovery=true;
        energy+=0.5f;
        UpdateEnergyBar();
        yield return new WaitForSeconds(1f);
        runOnceRecovery=false;
    } 

    /*IEnumerator DeathSequence()
    {
        transform.Rotate(new Vector3(0f,90f,0f));
        yield return new WaitForSeconds(1.3f);
        Destroy(gameObject);
    }*/
}
