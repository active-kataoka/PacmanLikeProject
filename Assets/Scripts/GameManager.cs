using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // シングルトン
    public static GameManager instance
    {
        get;
        private set;
    }

    public enum GameMode
    {
        Title,
        Playing,
        GameOver,
        Clear,
        Retry,
    }

    [SerializeField]
    private Transform Cookies;
    [SerializeField]
    private GameObject GameTitleUI;
    [SerializeField]
    private GameObject GameClearUI;
    [SerializeField]
    private GameObject GameOverUI;
    [SerializeField]
    private GameObject GamePlayingUI;
    [SerializeField]
    private GameObject txtTimer;

    // サウンド周り
    private AudioSource bgmSource;
    private AudioSource seSource;

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip playBgm;
    [SerializeField]
    private AudioClip cookieSE;
    [SerializeField]
    private AudioClip gameoverSE;
    [SerializeField]
    private AudioClip clearSE;

    private GameMode currentGameMode;
    private int remainingCookies;
    private float gameTimer;

    // Start is called before the first frame update
    void Start()
    {
       remainingCookies = Cookies.childCount;
       ChangeGameMode(GameMode.Title);
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            // 既にinstanceが既に存在する場合　→　新しいinstanceを破棄
            Destroy(gameObject);
            return;
        }

        bgmSource = gameObject.AddComponent<AudioSource>();
        seSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (currentGameMode == GameMode.Playing)
        {
            gameTimer += Time.deltaTime;

            // タイマー更新 → 小数点第2位まで表示
            SetTextTimer(txtTimer, "" + gameTimer.ToString("F2"));
        }
    }

    public void ChangeGameMode(GameMode mode)
    {
        currentGameMode = mode;

        GameClearUI.SetActive(false);
        GameOverUI.SetActive(false);
        GamePlayingUI.SetActive(false);

        switch (currentGameMode)
        {
            case GameMode.Title:
                Debug.Log("GameMode.Title");
                GameTitleUI.SetActive(true);
                txtTimer.SetActive(false);
                break;

            case GameMode.Playing:
                Debug.Log("GameMode.Playing");
                GameTitleUI.SetActive(false);
                GamePlayingUI.SetActive(true);
                txtTimer.SetActive(true);
                //PlayBGM(playBgm);
                break;

            case GameMode.Clear:
                Debug.Log("GameMode.Clear");
                GameClearUI.SetActive(true);
                txtTimer.SetActive(true);
                PlaySE(clearSE);
                StopBGM();
                SetTextTimer(txtTimer, "クリアタイム：" + gameTimer.ToString("F2"));
                break;

            case GameMode.GameOver:
                Debug.Log("GameMode.GameOver");
                GameOverUI.SetActive(true);
                txtTimer.SetActive(true);
                PlaySE(gameoverSE);
                StopBGM();
                break;

            case GameMode.Retry:
                Debug.Log("GameMode.Retry");
                SceneManager.LoadScene(SceneName.MainScene);
                break;

            default:
                Debug.Log("GameMode.None");
                break;
        }
    }

    // 現在のモード取得
    public GameMode CurrentGameMode
    {
        get { return currentGameMode; }
    }

    // クッキーの取得を監視
    public void CheckCookies()
    {
        remainingCookies--;
        PlaySE(cookieSE);

        if (remainingCookies <= 0)
        {
            GameClear();
        }
    }

    public void GamePlay()
    {
        ChangeGameMode(GameMode.Playing);
    }

    public void GameClear()
    {
        ChangeGameMode(GameMode.Clear);
    }

    public void GameOver()
    {
        ChangeGameMode(GameMode.GameOver);
    }

    public void GameRetry()
    {
        ChangeGameMode(GameMode.Retry);
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip != null)
        {
            bgmSource.clip = clip;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    public void PlaySE(AudioClip clip)
    {
        if (clip != null)
        {
            seSource.PlayOneShot(clip, 0.75f);
        }
    }

    public void StopBGM()
    {
        if (bgmSource.isPlaying)
        {
            bgmSource.Stop();
        }
    }

    // タイマーのセット
    private void SetTextTimer(GameObject obj, string str)
    {
        if (null == obj || null == obj.GetComponent<TextMeshProUGUI>()) return;

        print("SetTextTimerの処理に入る");

        obj.GetComponent<TextMeshProUGUI>().text = str;
    }
}
