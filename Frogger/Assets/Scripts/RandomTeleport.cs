using UnityEngine;
using System.Collections;

public class RandomTeleport : MonoBehaviour
{
    [SerializeField] private float teleportInterval = 5f;

    [SerializeField] private float minX = -5f;
    [SerializeField] private float maxX = 5f;
    [SerializeField] private float minY = -4.4f;
    [SerializeField] private float maxY = 3f;

    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        StartCoroutine(TeleportRoutine());
    }

    private IEnumerator TeleportRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(teleportInterval);

            // Disappear
            _spriteRenderer.enabled = false;
            _collider.enabled = false;

            yield return new WaitForSeconds(1f); // Time invisible

            // Change position
            Vector3 newPos = GetRandomPosition();
            transform.position = newPos;

            // Reappear
            _spriteRenderer.enabled = true;
            _collider.enabled = true;
        }
    }

    private Vector3 GetRandomPosition()
    {
        int x = Mathf.RoundToInt(Random.Range(minX, maxX));
        int y = Mathf.RoundToInt(Random.Range(minY, maxY));
        return new Vector3(x, y, 0);
    }
}