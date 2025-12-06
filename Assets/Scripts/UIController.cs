using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Obavezno za TMP_Text

public class UIController : MonoBehaviour
{
    [SerializeField] TMP_Text text; // referenca na TMP_Text komponentu
    [SerializeField] float blinkInterval = 0.5f; // interval treptanja

    void Start()
    {
        StartCoroutine(BlinkText());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("MainScene");
        }
    }

    System.Collections.IEnumerator BlinkText()
    {
        while (true)
        {
            text.enabled = !text.enabled;
            yield return new WaitForSeconds(blinkInterval);
        }
    }
}
