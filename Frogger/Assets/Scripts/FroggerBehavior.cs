using System.Collections;
using UnityEngine;

    public class FroggerBehavior : MonoBehaviour
    {
        [SerializeField] private KeyCode _moveUpKey = KeyCode.UpArrow;
        [SerializeField] private KeyCode _moveDownKey = KeyCode.DownArrow;
        [SerializeField] private KeyCode _moveLeftKey = KeyCode.LeftArrow;
        [SerializeField] private KeyCode _moveRightKey = KeyCode.RightArrow;

        [SerializeField] private float minX = -7f;
        [SerializeField] private float maxX = 7f;
        [SerializeField] private float minY = -7f;
        [SerializeField] private float maxY = 8f;
        
        private float _frogHalfSize = 0.5f;

        private void Start()
        {
            minX += _frogHalfSize;
            maxX -= _frogHalfSize;
            minY += _frogHalfSize;
            maxY -= _frogHalfSize;
        }

        private SpriteRenderer _spriteRenderer;
        [SerializeField] private Sprite _staticSprite;
        [SerializeField] private Sprite _jumpSprite;
        [SerializeField] private Sprite _gameOverSprite;

        private Vector3 _resetPosition;


        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _resetPosition = transform.position;
        }
        private void Update()
        {
            if (Input.GetKeyDown(_moveUpKey)) {
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                Move(Vector3.up);
            }
            else if (Input.GetKeyDown(_moveDownKey)) {
                transform.rotation = Quaternion.Euler(0f, 0f, 180f);
                Move(Vector3.down);
            }
            else if (Input.GetKeyDown(_moveLeftKey)) {
                transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                Move(Vector3.left);
            }
            else if (Input.GetKeyDown(_moveRightKey)) {
                transform.rotation = Quaternion.Euler(0f, 0f, -90f);
                Move(Vector3.right);
            } 
        }

        private void Move(Vector3 direction)
        { 
            Vector3 destination = transform.position + direction;

            destination.x = Mathf.Clamp(destination.x, minX, maxX);
            destination.y = Mathf.Clamp(destination.y, minY, maxY);
            
            Debug.DrawLine(destination - Vector3.one * 0.5f, destination + Vector3.one * 0.5f, Color.green, 1f);
            
            Collider2D platform = Physics2D.OverlapBox(destination, Vector2.zero, 0f, LayerMask.GetMask("Platform"));
            Collider2D obstacle = Physics2D.OverlapBox(destination, Vector2.zero, 0f, LayerMask.GetMask("Obstacle"));
            Collider2D water = Physics2D.OverlapBox(destination, Vector2.zero, 0f, LayerMask.GetMask("Water"));

            if (platform != null) {
                transform.SetParent(platform.transform.root);
            } else {
                transform.SetParent(null);
            }

            if ((obstacle != null || water != null) && platform == null)
            {
                transform.position = destination;
                GameOver();
                GameBehavior.Instance.Died();
                return;
            }
            Collider2D home = Physics2D.OverlapBox(destination, Vector2.zero, 0f);
            if (home != null && home.CompareTag("Home"))
            {
                transform.position = destination;
                GameBehavior.Instance.HomeOccupied();
                return;
            }

            StartCoroutine(Jump(destination));
        }

        private IEnumerator Jump(Vector3 destination)
        {
            Vector3 startPosition = transform.position;
                
            float elapsed = 0f;
            float duration = 0.125f;
            
            _spriteRenderer.sprite = _jumpSprite;    

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                transform.position = Vector3.Lerp(startPosition, destination, t);
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            transform.position = destination;
            _spriteRenderer.sprite = _staticSprite;
        }

        public void GameOver()
        {
            StopAllCoroutines();
            transform.rotation = Quaternion.identity;
            _spriteRenderer.sprite = _gameOverSprite;
            enabled = false;

            Invoke(nameof(Reset), 1f);
        }

        public void Reset()
        {
            StopAllCoroutines();
            transform.rotation = Quaternion.identity;
            transform.position = _resetPosition;
            _spriteRenderer.sprite = _staticSprite;
            gameObject.SetActive(true);
            enabled = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (enabled && other.gameObject.layer == LayerMask.NameToLayer("Obstacle") && transform.parent == null) {
                GameOver();
                GameBehavior.Instance.Died();
            }
        }
    }