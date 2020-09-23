using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingUIController : MonoBehaviour
{
    #region Singleton
    private static LoadingUIController instance;
    public static LoadingUIController Instance
    {
        get
        {
            if(instance == null)
            {
                var obj = FindObjectOfType<LoadingUIController>();
                if (obj != null)
                {
                    instance = obj;
                }
                else
                {
                    instance = Create();
                }
            }

            return instance;
        }
    }

    private static LoadingUIController Create()
    {
        return Instantiate(Resources.Load<LoadingUIController>("Prefabs/UI/LoadingUI"));
    }

    private void Awake()
    {
        if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    #endregion Singleton

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image progressBar;

    private string loadSceneName;

    public void LoadScene(string sceneName)
    {
        gameObject.SetActive(true);
        SceneManager.sceneLoaded += OnSceneLoaded;
        loadSceneName = sceneName;
        StartCoroutine(LoadSceneProcess());
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if(arg0.name == loadSceneName)
        {
            StartCoroutine(Fade(false));
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private IEnumerator LoadSceneProcess()
    {
        progressBar.fillAmount = 0f;
        // 호출한 코루틴이 끝날 때까지 대기시킬 수 있음.
        yield return StartCoroutine(Fade(true));

        AsyncOperation op = SceneManager.LoadSceneAsync(loadSceneName);
        op.allowSceneActivation = false;

        float timer = 0f;
        while(!op.isDone)
        {
            yield return null;
            
            if(op.progress < 0.9f)
            {
                progressBar.fillAmount = op.progress;
            }
            else
            {
                timer += Time.unscaledDeltaTime;
                progressBar.fillAmount = Mathf.Lerp(0.9f, 1f, timer);

                if(progressBar.fillAmount >= 1f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }

    private IEnumerator Fade(bool isFadeIn)
    {
        float timer = 0f;
        while(timer <= 1f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime * 3f;
            // 매개변수로 받은 bool값이 true이면 FadeIn을 false이면 FadeOut을 연출합니다.
            canvasGroup.alpha = isFadeIn ? Mathf.Lerp(0f, 1f, timer) : Mathf.Lerp(1f, 0f, timer);
        }
        
        // FadeOut이 끝나면 GameObject를 비활성화 시켜줍니다.
        if(!isFadeIn)
        {
            gameObject.SetActive(false);
        }
    }
}
