using UnityEngine;
using System.Collections;

public class PristControll : MonoBehaviour
{
    [SerializeField] GameObject[] needToKill;
    [SerializeField] AudioClip story;

    [SerializeField] Camera mainCamera;
    [SerializeField] Camera secondCamera;

    private AudioSource source;
    private bool audioPlayed = false; // da ne puca audio više puta
    private Coroutine currentCoroutine;

    void Start()
    {
        source = gameObject.AddComponent<AudioSource>();
        source.clip = story;
    }

    void Update()
    {
        if (!audioPlayed && AllObjectsDestroyed())
        {
            currentCoroutine = StartCoroutine(PlayAudioAndSwitchCameras());
            audioPlayed = true;
        }

        // Skipaj sve na Space
        if (Input.GetKeyDown(KeyCode.Space) && audioPlayed)
        {
            SkipAudio();
        }
    }

    private bool AllObjectsDestroyed()
    {
        foreach (GameObject obj in needToKill)
        {
            if (obj != null)
                return false;
        }
        return true;
    }

    private IEnumerator PlayAudioAndSwitchCameras()
    {
        // Ugasimo mainCamera i upalimo secondCamera
        mainCamera.gameObject.SetActive(false);
        secondCamera.gameObject.SetActive(true);

        // Pronađemo objekat sa tagom "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Postavimo ga 5f ispred vlasnika ove skripte
            player.transform.position = transform.position + transform.forward * 5f;
        }

        // Pokrenemo audio
        source.Play();

        // Čekamo da audio završi ili da igrač pritisne Space
        float elapsed = 0f;
        while (elapsed < source.clip.length)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                break;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Zaustavimo audio ako još uvijek svira
        if (source.isPlaying)
            source.Stop();

        // Ponovo upalimo mainCamera i ugasimo secondCamera
        mainCamera.gameObject.SetActive(true);
        secondCamera.gameObject.SetActive(false);
    }

    private void SkipAudio()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            // Zaustavimo audio ako još uvijek svira
            if (source.isPlaying)
                source.Stop();

            // Vratimo kamere
            mainCamera.gameObject.SetActive(true);
            secondCamera.gameObject.SetActive(false);
        }
    }
}
