using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class QuitGame : MonoBehaviour
{
    public static QuitGame Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
        
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            Quit();
    }

    void Quit()
    {

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        // Function runs when the game is running from build
            Application.Quit();
#endif  
    }
}