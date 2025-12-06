using UnityEngine;

public class SwordSounds : MonoBehaviour
{
    [SerializeField] AudioClip swingSound;
    [SerializeField] AudioClip blood;
    [SerializeField] AudioClip armor;
    [SerializeField] AudioClip super;

    [HideInInspector] public GameObject hitTarget;
    AudioSource source;
    Attack attack;
    
    // Start is called nce before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        source=GetComponent<AudioSource>();
        attack=GetComponentInParent<Attack>();
    }

    void Update()
    {
        
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        
        if (transform.parent != null  && other.CompareTag(transform.parent.tag)) return;
        // Odaberi zvuk
        if (other.CompareTag("Armor"))
        {
            source.PlayOneShot(armor);
        }
        else if (other.CompareTag("Enemy"))
        {
            if (attack.superAttack)
            {
                CharacterController cc = other.GetComponent<CharacterController>();
                if (cc != null)
                {
                    Vector3 pushDir = -other.transform.forward * 2.5f;
                    pushDir.y = 0;
                    
                    cc.Move(pushDir);
                }
                else
                {
                    other.transform.position += -other.transform.forward * 2.5f;
                }
                source.PlayOneShot(super);
            }else{
                Debug.Log("other  "+other.gameObject.tag);
                source.PlayOneShot(blood);
            }
            hitTarget=other.gameObject;
        }
        
    }
}
