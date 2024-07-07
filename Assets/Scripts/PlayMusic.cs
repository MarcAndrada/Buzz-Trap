using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayMusic : MonoBehaviour
{
    [SerializeField]
    private AudioClip music;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitEndFrame());

        IEnumerator WaitEndFrame()
        {
            AudioManager.instance.Play2dLoop(music, "Master", 1f, 1f, 0.2f);
            yield return new WaitForEndOfFrame();
            SceneManager.LoadScene("MenuScene");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
