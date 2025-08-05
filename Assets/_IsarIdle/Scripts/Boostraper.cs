using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Boostraper : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        #if UNITY_EDITOR
        Scene _currentScene = SceneManager.GetActiveScene();
        #endif

        if (!SceneManager.GetSceneByName("InitScene").isLoaded)
        {
            foreach (GameObject obj in FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                obj.SetActive(false);
                
            SceneManager.LoadScene("InitScene");
        }
        
        #if UNITY_EDITOR
        if (_currentScene.IsValid())
            SceneManager.LoadSceneAsync(_currentScene.name, LoadSceneMode.Additive);
        #endif
    }
}