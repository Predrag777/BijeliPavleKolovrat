using UnityEngine;

public class Kolovrat : MonoBehaviour
{
    [SerializeField] GameObject church;
    [SerializeField] AudioClip kolovratFound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        AudioSource source=church.GetComponent<AudioSource>();
        source.clip=kolovratFound;
        source.Play();
        Destroy(gameObject);
    }
}
