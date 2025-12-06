using UnityEngine;
using System.Collections;

public class FinalScene : MonoBehaviour
{
    [SerializeField] Camera firstCam;    // fokus na Furlana
    [SerializeField] Camera secondCam;   // fokus na Pavla
    [SerializeField] Camera thirdCam;  //ledja furlana
    [SerializeField] Camera mainCam;

    [SerializeField] GameObject furlan;
    [SerializeField] GameObject pavle;

    [Header("Furlanovi tekstovi")]
    [SerializeField] AudioClip furlanText1;
    [SerializeField] AudioClip furlanText2;
    [SerializeField] AudioClip furlanText3;
    [SerializeField] AudioClip furlanText4;

    [Header("Pavlovi tekstovi")]
    [SerializeField] AudioClip pavleText1;
    [SerializeField] AudioClip pavleText2;
    [SerializeField] AudioClip pavleText3;

    private AudioSource furlanAudio;
    private AudioSource pavleAudio;

    [SerializeField] GameObject soldier1;
    [SerializeField] GameObject soldier2;
    [SerializeField] GameObject soldier3;

    [SerializeField] GameObject fixedPosition;
    



    private bool started = false;

    void Start()
    {
        furlanAudio = furlan.GetComponent<AudioSource>();
        pavleAudio = pavle.GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (started) return;
        started = true;

        StartCoroutine(MoveToPositionAndDisable(pavle, fixedPosition.transform.position, 25f)); // 5f = brzina

        mainCam.enabled = false;

        StartCoroutine(DialogFlow());
    }


    private IEnumerator DialogFlow()
    {
        // *** FURLAN 1 ***
        firstCam.enabled = true;
        secondCam.enabled = false;
        Play(furlanAudio, furlanText1);
        yield return new WaitUntil(() => !furlanAudio.isPlaying);

        // *** PAVLE 1 ***
        firstCam.enabled = false;
        secondCam.gameObject.SetActive(true);
        secondCam.enabled = true;
        Play(pavleAudio, pavleText1);
        yield return new WaitUntil(() => !pavleAudio.isPlaying);

        // *** FURLAN 2 ***
        firstCam.enabled = true;
        secondCam.enabled = false;
        Play(furlanAudio, furlanText2);
        yield return new WaitUntil(() => !furlanAudio.isPlaying);

        // *** PAVLE 2 ***
        firstCam.enabled = false;
        secondCam.enabled = true;
        Play(pavleAudio, pavleText2);
        yield return new WaitUntil(() => !pavleAudio.isPlaying);

        // *** FURLAN 3 ***
        //firstCam.enabled = true;
        thirdCam.gameObject.SetActive(true);
        thirdCam.enabled=true;

        secondCam.enabled = false;
        Play(furlanAudio, furlanText3);
        yield return new WaitUntil(() => !furlanAudio.isPlaying);

        // *** PAVLE 3 ***
        firstCam.enabled = false;
        thirdCam.enabled=false;
        secondCam.enabled = true;
        Play(pavleAudio, pavleText3);
        yield return new WaitUntil(() => !pavleAudio.isPlaying);

        // *** FURLAN 4 (ZADNJI) ***
        firstCam.enabled = true;
        secondCam.enabled = false;
        Play(furlanAudio, furlanText4);
        yield return new WaitUntil(() => !furlanAudio.isPlaying);

        // *** GOTOVA SCENA – VRATIMO MAIN KAMERU ***
        firstCam.enabled = false;
        secondCam.enabled = false;
        mainCam.enabled = true;

        MoveAA sm1= soldier1.GetComponent<MoveAA>();
        MoveAA sm2= soldier2.GetComponent<MoveAA>();
        MoveAA sm3= soldier3.GetComponent<MoveAA>();

        Attack sa1= soldier1.GetComponent<Attack>();
        Attack sa2 = soldier2.GetComponent<Attack>();
        Attack sa3=soldier3.GetComponent<Attack>();

        sm1.enabled=true;
        sm2.enabled=true;
        sm3.enabled=true;

        sa1.enabled=true;
        sa2.enabled=true;
        sa3.enabled=true;

        Destroy(gameObject);
    }

    private void Play(AudioSource s, AudioClip c)
    {
        s.clip = c;
        s.Play();
    }


    IEnumerator MoveToPositionAndDisable(GameObject obj, Vector3 target, float speed)
    {
        CharacterController cc = obj.GetComponent<CharacterController>();
        
        while (Vector3.Distance(obj.transform.position, target) > 0.1f)
        {
            Vector3 dir = (target - obj.transform.position).normalized;
            cc.Move(dir * speed * Time.deltaTime);
            yield return null;
        }

        // Postavi Pavla tačno na odredište
        obj.transform.position = target;

        // Kada stigne, ugasi objekat ili skriptu
        obj.SetActive(false); // ili npr. neka druga skripta: obj.GetComponent<SomeScript>().enabled = false;
    }
}
