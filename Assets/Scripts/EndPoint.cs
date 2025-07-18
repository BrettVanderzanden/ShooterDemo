using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndPoint : MonoBehaviour
{
    public static Action OnExitReached;

    [SerializeField] float sceneLoadDelay = 2f;
    [SerializeField] int nextSceneIndex = 0; // dev use, should ideally load next scene from unity scene manager

    bool _exitReached = false;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_exitReached && collision.gameObject.GetComponent<PlayerController>())
        {
            _exitReached = true;
            // Level Complete Fanfare here
            OnExitReached?.Invoke();
            StartCoroutine(WaitAndLoad(nextSceneIndex, sceneLoadDelay));
        }
    }

    IEnumerator WaitAndLoad(int sceneIndex, float delay)
    {
        yield return new WaitForSeconds(delay);
        // Unload the level?
        Destroy(PlayerController.Instance.gameObject);
        SceneManager.LoadScene(sceneIndex);
    }
}
