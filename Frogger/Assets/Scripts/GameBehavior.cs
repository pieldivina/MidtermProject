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

    public int Lives { get; private set; } = 3;
    public int Score { get; private set; } = 0;
    public int TimeRemaining { get; private set; } = 30;

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
    }

    private void NewGame()
    {
        _gameOverMenu.SetActive(false);

        SetScore(0);
        SetLives(3);
        NewLevel();
    }

    private void NewLevel()
    {
        for (int i = 0; i < _homes.Length; i++)
        {
            _homes[i].enabled = false;
        }

        Respawn();
    }

    private void Respawn()
    {
        _frogger.Reset();

        StopAllCoroutines();
        StartCoroutine(Timer(30));
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

        _frogger.GameOver();
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
        _frogger.gameObject.SetActive(false);
        _gameOverMenu.SetActive(true);

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

        if (Cleared())
        {
            SetLives(Lives + 1);
            SetScore(Score + 1000);
            Invoke(nameof(NewLevel), 1f);
        }
    }

    private bool Cleared()
    {
        for (int i = 0; i < _homes.Length; i++)
        {
            if (!_homes[i].enabled)
            {
                return false;
            }
        }

        return true;
    }

    private void SetScore(int score)
    {
        this.Score = score;
        _scoreText.text = score.ToString();
    }

    private void SetLives(int lives)
    {
        this.Lives = lives;
        _livesText.text = lives.ToString();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
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