using UnityEngine;

public class IdleWiggle : MonoBehaviour
{
    [SerializeField] private float wiggleSpeed = 2f;
    [SerializeField] private float wiggleAmount = 0.1f;

    private Vector3 _initialScale;

    private void Start()
    {
        _initialScale = transform.localScale;
    }

    private void Update()
    {
        float scale = 1 + Mathf.Sin(Time.time * wiggleSpeed) * wiggleAmount;
        transform.localScale = _initialScale * scale;
    }
}