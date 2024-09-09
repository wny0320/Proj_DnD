using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    private static Manager instance = null;
    public static Manager Instance { get { Init(); return instance; } }

    #region Managers
    GameManager _game = new GameManager();
    DataManager _data = new DataManager();
    InputManager _input = new InputManager();
    UIManager _ui= new UIManager();
    InvenManager _inven = new InvenManager();

    public static GameManager Game { get { return instance._game; } }
    public static DataManager Data { get { return instance._data; } }
    public static InputManager Input { get { return instance._input; } }
    public static UIManager UI { get { return instance._ui; } }
    public static InvenManager Inven { get { return instance._inven; } }
    #endregion

    public Coroutine invenCoroutine = null;
    private void Awake()
    {
        Init();
        Game.OnAwake();
        Data.OnAwake();
    }

    private void Start()
    {
        Inven.OnStart();
        StartInvenCoroutine();
    }

    private void Update()
    {
        Input.OnUpdate();
        Inven.OnUpdate();
    }

    private void FixedUpdate()
    {
        Input.OnFixedUpdate();
        Game.OnFixedUpdate();
    }

    private static void Init()
    {
        if (instance == null)
        {
            GameObject go = GameObject.Find("Managers");
            if (go == null)
            {
                go = new GameObject { name = "Managers" };
                go.AddComponent<Manager>();
            }

            DontDestroyOnLoad(go);
            instance = go.GetComponent<Manager>();
        }
    }
    public void StartInvenCoroutine()
    {
        if (invenCoroutine == null)
        {
            invenCoroutine = StartCoroutine(Inven.MoveItem());
            Debug.Log(invenCoroutine);
        }
        else
            return;
    }



    #region Scene�̵�����

    [SerializeField]
    private GameObject loadingSceneUI;
    private CanvasGroup sceneLoaderCanvasGroup;
    private Image progressBar;

    private string loadSceneName;
    public void LoadScene(string sceneName)
    {
        sceneLoaderCanvasGroup = Instantiate(loadingSceneUI).GetComponent<CanvasGroup>();
        progressBar = sceneLoaderCanvasGroup.transform.GetChild(1).GetComponent<Image>();

        SceneManager.sceneLoaded += LoadSceneEnd;
        loadSceneName = sceneName;
        StartCoroutine(Load(sceneName));
    }

    private IEnumerator Load(string sceneName)
    {
        progressBar.fillAmount = 0f;
        yield return StartCoroutine(Fade(true));

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        float timer = 0.0f;
        while (!op.isDone)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;

            if (op.progress < 0.9f)
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, op.progress, timer);
                if (progressBar.fillAmount >= op.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);

                if (progressBar.fillAmount == 1.0f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }

    private void LoadSceneEnd(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name == loadSceneName)
        {
            StartCoroutine(Fade(false));
            SceneManager.sceneLoaded -= LoadSceneEnd;
        }
    }

    private IEnumerator Fade(bool isFadeIn)
    {
        float timer = 0f;

        while (timer <= 1f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime * 2f;
            sceneLoaderCanvasGroup.alpha = Mathf.Lerp(isFadeIn ? 0 : 1, isFadeIn ? 1 : 0, timer);
        }

        if (!isFadeIn)
            Destroy(sceneLoaderCanvasGroup.gameObject);
    }
    #endregion
}
