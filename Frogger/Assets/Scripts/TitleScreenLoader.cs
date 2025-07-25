using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenLoader : MonoBehaviour
{
    [SerializeField] private AudioClip startSound;
    private AudioSource _audioSource;
    private bool _hasStarted = false;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!_hasStarted && Input.GetKeyDown(KeyCode.Return))
        {
            _hasStarted = true;
            _audioSource.PlayOneShot(startSound);
            Invoke(nameof(LoadScene), startSound.length);
        }
    }

    private void LoadScene()
    {
        SceneManager.LoadScene("Frogger");
    }
}