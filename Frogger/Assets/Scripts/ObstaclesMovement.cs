using UnityEngine;

public class ObstaclesMovement : MonoBehaviour
{
    [SerializeField] private Vector2 _direction = Vector2.right;
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _size = 1f;
    
    private Vector3 _leftLimit;
    private Vector3 _rightLimit;

    private void Start()
    {
        _leftLimit = Camera.main.ViewportToWorldPoint(Vector3.zero);
        _rightLimit = Camera.main.ViewportToWorldPoint(Vector3.right);

    }

    private void Update()
    {
        if (_direction.x > 0 && (transform.position.x - _size) > _rightLimit.x)
        {
            transform.position = new Vector3(_leftLimit.x - _size, transform.position.y, transform.position.z);
        }
        else if (_direction.x < 0 && (transform.position.x + _size) < _leftLimit.x)
        {
            transform.position = new Vector3(_rightLimit.x + _size, transform.position.y, transform.position.z);
        }
        else
        {
            transform.Translate(_speed * Time.deltaTime * _direction );
        }
    }
}
