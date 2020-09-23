using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneController : MonoBehaviour
{
    static string nextScene;

    [SerializeField] Image progressBar;

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        // 로딩 중 다른 작업이 불가능하다 (동기 방식)
        SceneManager.LoadScene("LoadingScene");
    }

    void Start()
    {
        StartCoroutine(LoadSceneProcess());
    }

    IEnumerator LoadSceneProcess()
    {
        // 로딩 중 다른 작업이 가능하다. (비동기 방식)
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        // 씬을 비동기로 불러들일 때 로딩이 끝나면 자동으로 불러온 씬으로 이동할 것인지 설정하는 진릿값
        // false로 설정할 경우 씬을 90% 까지만 로드한 상태로 대기합니다.
        // true로 설정할 경우 남은 10% 부분을 로딩하고 씬을 불러옵니다.
        // 1. Fake 로딩을 위한 작업
        // 2. 씬 뿐만 아니라 에셋 번들을 불러오기 위한 작업
        op.allowSceneActivation = false;

        float timer = 0f;
        while(!op.isDone)
        {
            yield return null;
            if(op.progress < 0.9f)
            {
                // UI로 로딩이 어느정도 진행 되고 있는지 표시
                progressBar.fillAmount = op.progress;
            }
            else
            {
                timer += Time.unscaledDeltaTime;
                progressBar.fillAmount = Mathf.Lerp(0.9f, 1f, timer);
                
                if(progressBar.fillAmount > 0.99f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }

    }
}
