using System.Collections;
using UnityEngine;
using TMPro;

public enum GameState { Play, Pause, GameOver }

[DefaultExecutionOrder(-1)]
public class GameBehavior : MonoBehaviour
{
    public static GameBehavior Instance { get; private set; }

    [SerializeField] private HomeBehavior[] _homes;
    [SerializeField] private FroggerBehavior _frogger;
    [SerializeField] private GameObject _gameOverMenu;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private TMP_Text _timeText;
    [SerializeField] private TMP_Text _livesText;
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _levelText;
    [SerializeField] private GameObject _levelTextObject;

    [SerializeField] private AudioClip allHomesCompletedSound;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip playAgainSound;
    [SerializeField] private AudioClip pauseSound;
    [SerializeField] private AudioClip levelUpSound;

    private AudioSource _audioSource;

    public int Lives { get; private set; } = 3;
    public int Score { get; private set; } = 0;
    public int TimeRemaining { get; private set; } = 30;
    public int Level { get; private set; } = 1;

    public GameState CurrentState { get; private set; } = GameState.Play;

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        NewGame();
        Time.timeScale = 1f;
        _pauseMenu.SetActive(false);
        _audioSource = GetComponent<AudioSource>();
    }

    private void NewGame()
    {
        _gameOverMenu.SetActive(false);
        SetScore(0);
        SetLives(3);
        SetLevel(1);
        CurrentState = GameState.Play;

        for (int i = 0; i < _homes.Length; i++)
        {
            _homes[i].ResetHome();
        }

        ObstaclesMovement.ResetSpeed();
        ObstaclesMovement.ResetSpeed();
        
        _levelTextObject.SetActive(true);

        NewLevel();
    }

    private void NewLevel()
    {
        for (int i = 0; i < _homes.Length; i++)
        {
            _homes[i].ResetHome();
        }

        Respawn();
        if (_levelText != null) _levelText.text = "LEVEL " + Level;
        _levelTextObject.SetActive(true);
    }

    private void Respawn()
    {
        _frogger.gameObject.SetActive(true);
        _frogger.enabled = true;
        _frogger.Reset();

        StopAllCoroutines();
        StartCoroutine(Timer(30));
        CurrentState = GameState.Play;
    }

    private IEnumerator Timer(int duration)
    {
        TimeRemaining = duration;
        _timeText.text = TimeRemaining.ToString();

        while (TimeRemaining > 0)
        {
            yield return new WaitForSeconds(1);
            TimeRemaining--;
            _timeText.text = TimeRemaining.ToString();
        }

        CurrentState = GameState.GameOver;
        GameOver();
    }

    public void Died()
    {
        SetLives(Lives - 1);

        if (Lives > 0)
        {
            Invoke(nameof(Respawn), 1f);
        }
        else
        {
            Invoke(nameof(GameOver), 1f);
        }
    }

    private void GameOver()
    {
        if (gameOverSound != null) _audioSource.PlayOneShot(gameOverSound);
        _frogger.gameObject.SetActive(false);
        _gameOverMenu.SetActive(true);
        _levelTextObject.SetActive(false); 

        StopAllCoroutines();
        StartCoroutine(CheckForPlayAgain());
    }

    private IEnumerator CheckForPlayAgain()
    {
        bool playAgain = false;

        while (!playAgain)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (playAgainSound != null) _audioSource.PlayOneShot(playAgainSound);
                playAgain = true;
            }

            yield return null;
        }

        NewGame();
    }

    public void AdvancedRow()
    {
        SetScore(Score + 10);
    }

    public void HomeOccupied()
    {
        int bonusPoints = TimeRemaining * 20;
        SetScore(Score + bonusPoints + 50);
        IncreaseDifficulty();

        if (Cleared())
        {
            if (allHomesCompletedSound != null) _audioSource.PlayOneShot(allHomesCompletedSound);
            SetLives(Lives + 1);
            SetScore(Score + 1000);
            SetLevel(Level + 1);
            IncreaseDifficulty();
            Invoke(nameof(NewLevel), 1f);
        }
        else
        {
            Invoke(nameof(Respawn), 1f);
        }
    }

    private bool Cleared()
    {
        for (int i = 0; i < _homes.Length; i++)
        {
            if (!_homes[i].IsOccupied())
            {
                return false;
            }
        }

        return true;
    }

    private void SetScore(int score)
    {
        Score = score;
        _scoreText.text = score.ToString();
    }

    public void SetLives(int lives)
    {
        Lives = lives;
        _livesText.text = lives.ToString();
    }
    
    public void AddLife()
    {
        SetLives(Lives + 1);
    }

    private void SetLevel(int level)
    {
        Level = level;
        if (_levelText != null) _levelText.text = "LEVEL " + Level;
        if (levelUpSound != null) _audioSource.PlayOneShot(levelUpSound);
    }

    private void IncreaseDifficulty()
    {
        ObstaclesMovement.IncreaseSpeed(0.1f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (pauseSound != null) _audioSource.PlayOneShot(pauseSound);
            if (CurrentState == GameState.Play)
            {
                CurrentState = GameState.Pause;
                Time.timeScale = 0f;
                _pauseMenu.SetActive(true);
            }
            else if (CurrentState == GameState.Pause)
            {
                CurrentState = GameState.Play;
                Time.timeScale = 1f;
                _pauseMenu.SetActive(false);
            }
        }
    }
}
