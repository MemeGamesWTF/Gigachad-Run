using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Static instance for the singleton
    public static GameManager Instance { get; private set; }

    public int GameID = 0;

    public GameObject GameOverScreen, GameWinScreen, InfoScreen;
    public GameObject  Information;
    public bool GameState = false;
    public BasePlayer Player;

    private ScoreObj Score;
    public GameObject[] face;
    public Transform[] spawnPoints;
    public float spawnDelay = 1f;
   
  



    private Vector2 tapPosition;
    public ParticleSystem poff;
    private List<GameObject> spawnedFaces = new List<GameObject>();

    public GameObject OldGame;
    public GameObject NewGame;

    public Text ScoreText;
    private int currentScore;

    public AudioSource GameOverReal;
    public AudioSource GameOverCartoon;
    public AudioSource Coin;
    public AudioSource Tap;
    public AudioSource UISound;


    public GameObject[] Balls;

    [DllImport("__Internal")]
    private static extern void SendScore(int score, int game);

#if UNITY_WEBGL && !UNITY_EDITOR
  
    [DllImport("__Internal")]
    private static extern void Initialization();
    [DllImport("__Internal")]
    private static extern void InitializeOGP(string gameId, string playerId);
    [DllImport("__Internal")]
    private static extern void SavePoints(int points);
#else
    // Editor or other platforms: provide safe stubs
  //  private static void SendScore(int score, int game) { Debug.Log($"(Stub) SendScore {score} {game}"); }
    private static void Initialization() { Debug.Log("(Stub) Initialization"); }
    private static void InitializeOGP(string gameId, string playerId) { Debug.Log($"(Stub) InitializeOGP {gameId} {playerId}"); }
    private static void SavePoints(int points) { Debug.Log($"(Stub) SavePoints {points}"); }
#endif


    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Another instance of GameManager already exists. Destroying this instance.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes
    }

    private void Start()
    {
        InfoScreen.SetActive(true);
        StartCoroutine(SpawnFacesCoroutine());

        if (Balls != null)
        {
            for (int i = 0; i < Balls.Length; i++)
            {
                if (Balls[i] != null)
                    Balls[i].SetActive(false);
            }
        }

        ActivateRandomBall();

        Initialization();
        InitializeOGP("3127439d-765c-442f-b4da-8d9071496fbd", "");
    }

    void Update()
    {
        if (!GameState)
            return;


    }

    public void PlayGame()
    {
        
        Time.timeScale = 1;
        Information.SetActive(false);
    }
    public void PauseGame()
    {

        Information.SetActive(true);
        StartCoroutine(Pause());
    }
    public void uiSound()
    {
        UISound.Play();
    }

    IEnumerator Pause()
    {
 

        // Wait for a specified duration (adjust the delay as needed)
        yield return new WaitForSeconds(0.2f);
        Time.timeScale = 0;


    }
    public void ActivateRandomBall()
    {
        if (Balls == null || Balls.Length == 0)
        {
            Debug.LogWarning("No balls assigned in GameManager!");
            return;
        }

        // Pick one random index
        int randomIndex = Random.Range(0, Balls.Length);

        // Loop through array: activate only the chosen one
        for (int i = 0; i < Balls.Length; i++)
        {
            if (Balls[i] != null)
                Balls[i].SetActive(i == randomIndex);
        }
    }


    IEnumerator SpawnFacesCoroutine()
    {
        while (true)
        {
            if (GameState)
            {
                // spawn between 0 and 1 faces
                int facesThisWave = Random.Range(1, 3);

                for (int i = 0; i < facesThisWave; i++)
                {
                    if (spawnPoints.Length == 0)
                    {
                        Debug.LogWarning("No spawn points assigned!");
                        break;
                    }

                    int faceIndex = Random.Range(0, face.Length);
                    int spawnIndex = Random.Range(0, spawnPoints.Length);

                    GameObject spawnedFace = Instantiate(
                        face[faceIndex],
                        spawnPoints[spawnIndex].position,
                        Quaternion.identity
                    );
                    spawnedFaces.Add(spawnedFace);
                }

                // wait before next wave
                yield return new WaitForSeconds(spawnDelay);
            }
            else
            {
                // if game is paused/not started, wait a frame and recheck
                yield return null;
            }
        }
    }



    private void DestroyAllSpawnedFaces()
    {
        foreach (GameObject faceObj in spawnedFaces)
        {
            if (faceObj != null)
            {
                Destroy(faceObj);
            }
        }
        spawnedFaces.Clear();
    }

    public void GameWin()
    {
        GameState = false;
        GameWinScreen.SetActive(true);
        Debug.Log(currentScore);
        SavePoints(currentScore);
        SendScore(currentScore, 133);
    }

    public void GameOVer()
    {
        DestroyAllSpawnedFaces();
        GameState = false;
        GameOverScreen.SetActive(true);
        Debug.Log(currentScore);
        SavePoints(currentScore);
        SendScore(currentScore, 133);
    }

    public void GameResetScreen()
    {
        ScoreText.text = "0";
        Score.score = 0;
        currentScore = 0;
        InfoScreen.SetActive(false);
        GameOverScreen.SetActive(false);
        GameWinScreen.SetActive(false);
        GameState = true;
        Player.Reset();
    }

    public void AddScore()
    {


        if (int.TryParse(ScoreText.text, out currentScore))
        {
            currentScore += 10;
            ScoreText.text = currentScore.ToString();
        }
        else
        {

            ScoreText.text = "0";
        }
    }


    public void AddScore(float f)
    {
        Score.score += f;
    }



    //HELPER FUNTION TO GET SPAWN POINT
    public Vector2 GetRandomPointInsideSprite(SpriteRenderer SpawnBounds)
    {
        if (SpawnBounds == null || SpawnBounds.sprite == null)
        {
            Debug.LogWarning("Invalid sprite renderer or sprite.");
            return Vector2.zero;
        }

        Bounds bounds = SpawnBounds.sprite.bounds;
        Vector2 randomPoint = new Vector2(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y)
        );

        // Transform local point to world space
        return SpawnBounds.transform.TransformPoint(randomPoint);
    }


    public struct ScoreObj
    {
        public float score;
    }
}
