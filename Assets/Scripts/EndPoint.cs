using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndPoint : MonoBehaviour
{
    [SerializeField] float sceneLoadDelay = 2f;
    [SerializeField] int nextSceneIndex = 0; // dev use, should ideally load next scene from unity scene manager

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            // Level Complete Fanfare here
            StartCoroutine(WaitAndLoad(nextSceneIndex, sceneLoadDelay));
        }
    }

    IEnumerator WaitAndLoad(int sceneIndex, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(PlayerController.Instance.gameObject);
        SceneManager.LoadScene(sceneIndex);
    }
}
