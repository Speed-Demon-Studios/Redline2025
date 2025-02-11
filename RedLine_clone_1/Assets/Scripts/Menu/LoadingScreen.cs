using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public Image loadingBar;
    float m_targetLoading;
    float m_currentLoading;
    string m_sceneID = "SceneID";
    // Start is called before the first frame update
    void Start()
    {
        m_currentLoading = 0;
        m_targetLoading = 0;
        int Id = PlayerPrefs.GetInt(m_sceneID);
        StartCoroutine(LoadingScene(Id));
    }

    IEnumerator LoadingScene(int sceneID)
    {
        loadingBar.fillAmount = 0;
        yield return new WaitForSeconds(1f);
        AsyncOperation operation =  SceneManager.LoadSceneAsync(sceneID);

        while (!operation.isDone)
        {
            float currentLoading = Mathf.Clamp01(operation.progress / 0.9f);
            loadingBar.fillAmount = currentLoading;
            yield return null;
        }

    }
}
