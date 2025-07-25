using UnityEngine;
using System.Collections;

public abstract class PowerUpBehavior : MonoBehaviour
{
    protected float _duration = 5f;
    public abstract void Activate(FroggerBehavior frogger);
    public abstract void Deactivate(FroggerBehavior frogger);

    [SerializeField] private AudioClip pickupSound;
    private AudioSource _audioSource;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;
    private bool _isTeleporting = false;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        StartCoroutine(TeleportRoutine());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            FroggerBehavior frogger = other.GetComponent<FroggerBehavior>();
            if (frogger != null)
            {
                if (pickupSound != null && _audioSource != null)
                    _audioSource.PlayOneShot(pickupSound);

                StartCoroutine(PowerUpDuration(frogger));
                _collider.enabled = false;
                _spriteRenderer.enabled = false;
            }
        }
    }

    protected IEnumerator PowerUpDuration(FroggerBehavior frogger)
    {
        Activate(frogger);
        yield return new WaitForSeconds(_duration);
        Deactivate(frogger);
        yield return new WaitForSeconds(2f);
        Respawn();
    }

    private void Respawn()
    {
        Vector3 newPos = GetRandomPosition();
        transform.position = newPos;
        _spriteRenderer.enabled = true;
        _collider.enabled = true;

        if (!_isTeleporting)
            StartCoroutine(TeleportRoutine());
    }

    protected IEnumerator TeleportRoutine()
    {
        _isTeleporting = true;

        while (_spriteRenderer.enabled)
        {
            yield return new WaitForSeconds(5f);

            _spriteRenderer.enabled = false;
            _collider.enabled = false;

            yield return new WaitForSeconds(1f);

            Vector3 newPos = GetRandomPosition();
            transform.position = newPos;

            _spriteRenderer.enabled = true;
            _collider.enabled = true;
        }

        _isTeleporting = false;
    }

    private Vector3 GetRandomPosition()
    {
        float minX = -7f, maxX = 7f;
        float minY = -6f, maxY = 7f;
        int x = Mathf.RoundToInt(Random.Range(minX, maxX));
        int y = Mathf.RoundToInt(Random.Range(minY, maxY));
        return new Vector3(x, y, 0);
    }
}