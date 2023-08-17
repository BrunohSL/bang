using System.Collections;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.Localization.Settings;

public class GameController : MonoBehaviour
{
    private bool _shoot;
    private bool canShoot;
    private bool bangActive;
    private bool localeActive = false;
    private float countdownTime;
    private float bangTimer;
    private int topPlayerScore;
    private int bottomPlayerScore;

    [Header("Texts")]
    [SerializeField] private TMP_Text _countDownText;
    [SerializeField] private TMP_Text _topPlayerScoreText;
    [SerializeField] private TMP_Text _bottomPlayerScoreText;
    [SerializeField] private TMP_Text _topWinnerText;
    [SerializeField] private TMP_Text _bottomWinnerText;

    [Space(10)]
    [Header("Game Objects")]
    [SerializeField] private GameObject _restartButton;
    [SerializeField] private GameObject _bangImage;
    [SerializeField] private GameObject _topPlayer;
    [SerializeField] private GameObject _bottomPlayer;
    [SerializeField] private GameObject _gameScreen;
    [SerializeField] private GameObject _gameOverScreen;
    [SerializeField] private GameObject _topShootButton;
    [SerializeField] private GameObject _bottomShootButton;

    [Space(10)]
    [Header("Animators")]
    [SerializeField] private Animator _topPlayerAnimator;
    [SerializeField] private Animator _bottomPlayerAnimator;

    void Update()
    {
        if (countdownTime >= 0.6f)
        {
            _countDownText.gameObject.SetActive(true);
            countdownTime -= Time.deltaTime;
            _countDownText.text = countdownTime.ToString("F0");
        }

        if (countdownTime <= 0.6f)
        {
            _countDownText.gameObject.SetActive(false);
            canShoot = true;
        }

        if (canShoot && bangTimer >= 0f && countdownTime <= 0.6f)
        {
            bangTimer -= Time.deltaTime;
            _topShootButton.SetActive(true);
            _bottomShootButton.SetActive(true);
        }

        if (bangTimer <= 0f)
        {
            _bangImage.SetActive(true);
            bangActive = true;
        }

        CheckForEarly();
        CheckForGameWinner();
        // validar se atirou antes do tempo (Deixar desarmado)
        // Condição - Caso os dois players atirem antes do tempo, empata a rodada e inicia novamente
        // fazer melhor de 5
    }

    public void ChangeLocale(int localeId)
    {
        if (localeActive)
        {
            return;
        }

        StartCoroutine(SetLocale(localeId));
    }

    IEnumerator SetLocale(int localeId)
    {
        localeActive = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeId];
        localeActive = false;
    }

    async private void CheckForGameWinner()
    {
        if (topPlayerScore == 3)
        {
            // StartCoroutine(GameOverWait());
            await Task.Delay(1000);
            _gameScreen.SetActive(false);
            _gameOverScreen.SetActive(true);
            _topWinnerText.text = "You Win";
            _bottomWinnerText.text = "You Lose";
        }

        if (bottomPlayerScore == 3)
        {
            // StartCoroutine(GameOverWait());
            await Task.Delay(1000);
            _gameScreen.SetActive(false);
            _gameOverScreen.SetActive(true);
            _topWinnerText.text = "You Lose";
            _bottomWinnerText.text = "You Win";
        }
    }

    IEnumerator GameOverWait()
    {
        yield return new WaitForSeconds(2);
    }

    private void CheckForEarly()
    {
        if (_topPlayerAnimator.GetBool("early") == true && _bottomPlayerAnimator.GetBool("early") == true)
        {
            // DRAW
            _countDownText.gameObject.SetActive(true);
            _countDownText.text = "DRAW";
            _restartButton.SetActive(true);
            bangTimer = 0f;
            _bangImage.SetActive(false);
        }
    }

    public void debugzinho() {
        Debug.Log(Random.Range(1f, 10f));
    }

    public void StartRound()
    {
        _restartButton.SetActive(false);
        _bangImage.SetActive(false);
        bangActive = false;
        _topShootButton.SetActive(false);
        _bottomShootButton.SetActive(false);
        ResetAnimators();
        _shoot = false;
        bangTimer = Random.Range(1f, 10f);
        countdownTime = 3.5f;
    }

    public void StartGame()
    {
        topPlayerScore = 0;
        bottomPlayerScore = 0;
        _topPlayerScoreText.text = string.Empty;
        _bottomPlayerScoreText.text = string.Empty;
        StartRound();
    }

    public void Shoot(string button) {
        if (!bangActive)
        {
            if (button == "top") {
                _topPlayerAnimator.SetBool("early", true);
                _topShootButton.SetActive(false);
            }

            if (button == "bottom") {
                _bottomPlayerAnimator.SetBool("early", true);
                _bottomShootButton.SetActive(false);
            }
        }

        if (!_shoot && bangActive)
        {
            _shoot = true;
            CheckForRoundWinner(button);
            _restartButton.SetActive(true);
        }
    }

    private void CheckForRoundWinner(string player)
    {
        if (player == "top" && !_topPlayerAnimator.GetBool("early"))
        {
            _topPlayerAnimator.SetBool("shoot", true);
            _bottomPlayerAnimator.SetBool("hit", true);
            ResetEarlyAnimator();
            SetPlayerScore(player);
        }

        if (player == "bottom" && !_bottomPlayerAnimator.GetBool("early"))
        {
            _topPlayerAnimator.SetBool("hit", true);
            _bottomPlayerAnimator.SetBool("shoot", true);
            ResetEarlyAnimator();
            SetPlayerScore(player);
        }
    }

    private void SetPlayerScore(string player)
    {
        if (player == "top")
        {
            topPlayerScore++;
            _topPlayerScoreText.text = "";
            for (int i = 0; i < topPlayerScore; i++)
            {
                _topPlayerScoreText.text += " I";
            }
        }

        if (player == "bottom")
        {
            bottomPlayerScore++;
            _bottomPlayerScoreText.text = "";
            for (int i = 0; i < bottomPlayerScore; i++)
            {
                _bottomPlayerScoreText.text += " I";
            }
        }
    }

    private void ResetEarlyAnimator()
    {
        _topPlayerAnimator.SetBool("early", false);
        _bottomPlayerAnimator.SetBool("early", false);
    }

    private void ResetAnimators()
    {
        _topPlayerAnimator.SetBool("shoot", false);
        _topPlayerAnimator.SetBool("hit", false);
        _topPlayerAnimator.SetBool("early", false);
        _bottomPlayerAnimator.SetBool("shoot", false);
        _bottomPlayerAnimator.SetBool("hit", false);
        _bottomPlayerAnimator.SetBool("early", false);
    }
}
