using System.Collections;
using UnityEngine;

    public class FroggerBehavior : MonoBehaviour
    {
        private bool _isImmune = false;
        private bool _doubleTapMode = false;
        private bool _inverseControls = false;
        private bool _isPoweredUp = false;

        [SerializeField] private Sprite _poweredUpStaticSprite;
        [SerializeField] private Sprite _poweredUpJumpSprite;

        public bool IsImmune => _isImmune;

        public void SetImmunity(bool immune)
        {
            _isImmune = immune;
        }

        public void SetDoubleTapMode(bool enabled)
        {
            _doubleTapMode = enabled;
        }

        public void SetInverseControls(bool enabled)
        {
            _inverseControls = enabled;
        }

        public void SetSprite(Sprite newSprite)
        {
            if (_spriteRenderer != null)
            {
                _spriteRenderer.sprite = newSprite;
            }
        }

        public Sprite GetSprite()
        {
            return _spriteRenderer != null ? _spriteRenderer.sprite : null;
        }
        [SerializeField] private AudioClip moveSound;
        [SerializeField] private AudioClip deathSound;
        private AudioSource _audioSource;
        [SerializeField] private KeyCode _moveUpKey = KeyCode.UpArrow;
        [SerializeField] private KeyCode _moveDownKey = KeyCode.DownArrow;
        [SerializeField] private KeyCode _moveLeftKey = KeyCode.LeftArrow;
        [SerializeField] private KeyCode _moveRightKey = KeyCode.RightArrow;

        [SerializeField] private float minX = -7f;
        [SerializeField] private float maxX = 7f;
        [SerializeField] private float minY = -7f;
        [SerializeField] private float maxY = 8f;
        
        private float _frogHalfSize = 0.5f;
        private float _highestY;
        private Vector3 _lastMove = Vector3.zero;
        private float _lastTapTime = 0f;
        private float _tapWindow = 0.3f;

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
            _highestY = transform.position.y;
            _audioSource = GetComponent<AudioSource>();
        }
        private void Update()
        {
            if (GameBehavior.Instance.CurrentState != GameState.Play) return;

            Vector3? direction = null;

            if (Input.GetKeyDown(_inverseControls ? _moveDownKey : _moveUpKey)) {
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                direction = Vector3.up;
            }
            else if (Input.GetKeyDown(_inverseControls ? _moveUpKey : _moveDownKey)) {
                transform.rotation = Quaternion.Euler(0f, 0f, 180f);
                direction = Vector3.down;
            }
            else if (Input.GetKeyDown(_inverseControls ? _moveRightKey : _moveLeftKey)) {
                transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                direction = Vector3.left;
            }
            else if (Input.GetKeyDown(_inverseControls ? _moveLeftKey : _moveRightKey)) {
                transform.rotation = Quaternion.Euler(0f, 0f, -90f);
                direction = Vector3.right;
            }

            if (direction.HasValue)
            {
                if (_doubleTapMode)
                {
                    if (_lastMove == direction.Value && Time.time - _lastTapTime < _tapWindow)
                    {
                        Move(direction.Value);
                        _lastMove = Vector3.zero;
                    }
                    else
                    {
                        _lastMove = direction.Value;
                        _lastTapTime = Time.time;
                    }
                }
                else
                {
                    Move(direction.Value);
                }
            }

            if (transform.position.x <= minX || transform.position.x >= maxX)
            {
                GameOver();
                GameBehavior.Instance.Died();
            }
        }

        private void Move(Vector3 direction)
        { 
            Vector3 destination = transform.position + direction;
            float startY = transform.position.y;

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

            bool isDeadlyTile = (obstacle != null || water != null);
            bool isOnPlatform = platform != null;

            if (isDeadlyTile && !isOnPlatform)
            {
                if (!_isImmune)
                {
                    transform.position = destination;
                    GameOver();
                    GameBehavior.Instance.Died();
                }
                else
                {
                    transform.position = destination;
                }

                return;
            }
            Collider2D home = Physics2D.OverlapBox(destination, Vector2.zero, 0f);

            if (destination.y > _highestY)
            {
                GameBehavior.Instance.AdvancedRow();
                _highestY = destination.y;
            }
            StartCoroutine(Jump(destination));
            if (moveSound != null) _audioSource.PlayOneShot(moveSound);
        }

        private IEnumerator Jump(Vector3 destination)
        {
            Vector3 startPosition = transform.position;
                
            float elapsed = 0f;
            float duration = 0.125f;
            
            _spriteRenderer.sprite = _isPoweredUp ? _poweredUpJumpSprite : _jumpSprite;    

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                transform.position = Vector3.Lerp(startPosition, destination, t);
                elapsed += Time.deltaTime;
                yield return null;  
            }
            
            transform.position = destination;
            _spriteRenderer.sprite = _isPoweredUp ? _poweredUpStaticSprite : _staticSprite;
        }

        public void GameOver()
        {
            if (_isImmune) return;
            StopAllCoroutines();
            transform.rotation = Quaternion.identity;
            _spriteRenderer.sprite = _gameOverSprite;
            if (deathSound != null) _audioSource.PlayOneShot(deathSound);
            enabled = false;
        }

        public void Reset()
        {
            StopAllCoroutines();
            transform.rotation = Quaternion.identity;
            transform.position = _resetPosition;
            transform.SetParent(null);
            _spriteRenderer.sprite = _staticSprite;
            enabled = true;
            gameObject.SetActive(true);
            _highestY = _resetPosition.y;  
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_isImmune) return;
            if (enabled && other.gameObject.layer == LayerMask.NameToLayer("Obstacle") && transform.parent == null) {
                GameOver();
                GameBehavior.Instance.Died();
            }
        }

        public void SetPoweredUp(bool enabled)
        {
            _isPoweredUp = enabled;
        }
    }