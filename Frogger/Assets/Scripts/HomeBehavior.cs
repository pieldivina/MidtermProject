using UnityEngine;

public class HomeBehavior : MonoBehaviour
{
    [SerializeField] private AudioClip homeReachedSound;
    private AudioSource _audioSource;
    public GameObject Frog;
    private bool _occupied = false;

    private void Start()
    {
        Frog.SetActive(false);
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_occupied) return;

        if (other.CompareTag("Player"))
        {
            _occupied = true;
            Frog.SetActive(true);
            Debug.Log($"{name} reached and occupied.");

            FroggerBehavior frogger = other.GetComponent<FroggerBehavior>();
            if (frogger != null)
            {
                if (homeReachedSound != null) _audioSource.PlayOneShot(homeReachedSound);
                frogger.gameObject.SetActive(false);
                GameBehavior.Instance.HomeOccupied();
            }
        }
    }

    public bool IsOccupied()
    {
        Debug.Log($"{name} occupied: {_occupied}");
        return _occupied;
    }

    public void ResetHome()
    {
        _occupied = false;
        Frog.SetActive(false);
        gameObject.SetActive(true);
        enabled = true;
        Debug.Log($"{name} reset.");
    }
}