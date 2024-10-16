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
    public bool sceneLoadFlag = false;

    private void Awake()
    {
        Init();
        Application.targetFrameRate = 60;
        Screen.SetResolution(1920, 1080, true);
        Data.OnAwake();
    }

    private void Start()
    {
        Inven.OnStart();
        Data.OnStart();
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
        StartCoroutine(Inven.ItemManage());
    }



    #region Scene이동관련

    [SerializeField]
    private GameObject loadingSceneUI;
    private CanvasGroup sceneLoaderCanvasGroup;
    private Image progressBar;

    public SceneName sceneName = SceneName.MainLobbyScene;
    public GameObject GameUI;

    private string loadSceneName;
    public void LoadScene(SceneName sceneName)
    {
        sceneLoadFlag = true;
        this.sceneName = sceneName;
        sceneLoaderCanvasGroup = Instantiate(loadingSceneUI).GetComponent<CanvasGroup>();
        sceneLoaderCanvasGroup.transform.parent = transform;
        progressBar = sceneLoaderCanvasGroup.transform.GetChild(1).GetComponent<Image>();

        SceneManager.sceneLoaded += LoadSceneEnd;
        loadSceneName = sceneName.ToString();

        //씬 이동시 그 해당 씬 관련 함수
        if(sceneName == SceneName.DungeonScene)
        {
            Game.GameUI = Instantiate(GameUI);
            Game.GameUI.transform.parent = transform;
            Game.OnGameSceneLoad();
            Inven.OnGameSceneLoad(Game.GameUI);
        }
        else
        {
            Game.CursorLock(false);
            Destroy(Game.GameUI);
        }
        // 씬이 이동됐을때 돈이 없고 아이템이 하나도 없다면 초기 아이템 추가
        if (Data.gold == 0 && Inven.GetBoxItems(ItemBoxType.Inventory).Count == 0
            && Inven.GetBoxItems(ItemBoxType.Stash).Count == 0
            && Inven.GetBoxItems(ItemBoxType.Equip).Count == 0)
        {
            Item initialItem = Data.itemData["ShortSword"].ItemDeepCopy(ItemRarity.Junk);
            Inven.AddItem(initialItem, ItemBoxType.Inventory);
        }
        StartCoroutine(Load(loadSceneName));
    }
    public Scene GetNowScene()
    {
        return SceneManager.GetActiveScene();
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
            sceneLoadFlag = false;
        }
    }

    private IEnumerator Fade(bool isFadeIn)
    {
        float timer = 0f;

        while (timer <= 1f)
        {
            timer += Time.unscaledDeltaTime * 1.5f;
            sceneLoaderCanvasGroup.alpha = Mathf.Lerp(isFadeIn ? 0 : 1, isFadeIn ? 1 : 0, timer);
            yield return null;
        }

        if (!isFadeIn)
            Destroy(sceneLoaderCanvasGroup.gameObject);
    }
    #endregion
}
