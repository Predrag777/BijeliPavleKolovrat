using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FurlanLastText : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] Camera furlanCamera;
    [SerializeField] Camera pavleCam;
    [SerializeField] GameObject[] knights;
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip furlanText;
    [SerializeField] GameObject pavle;
    [SerializeField] GameObject fixedPosition;
    [SerializeField] AudioClip pavleSpeech;

    bool playOneTime = false;

    void Update()
    {
        if (!checkIfTheyExist() && !playOneTime)
        {
            StartCoroutine(MoveToPositionAndDisable(pavle, fixedPosition.transform.position, 25f));

            playOneTime = true;

            mainCamera.enabled = false;
            furlanCamera.enabled = true;

            StartCoroutine(PlayFurlanText());
        }
    }

    bool checkIfTheyExist()
    {
        foreach (GameObject knight in knights)
        {
            if (knight != null)
                return true;
        }
        return false;
    }

    IEnumerator PlayFurlanText()
    {
        // Furlan priča
        source.PlayOneShot(furlanText);
        yield return new WaitForSeconds(furlanText.length);

        // Prelaz na Pavla
        furlanCamera.enabled = false;
        pavleCam.enabled = true;

        pavle.GetComponent<AudioSource>().PlayOneShot(pavleSpeech);

        // Čekaj da Pavle završi
        yield return new WaitForSeconds(pavleSpeech.length);

        // Učitaj END SCENU
        SceneManager.LoadScene("EndGame");

        Destroy(gameObject);
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

        obj.transform.position = target;
        obj.SetActive(false);
    }
}
