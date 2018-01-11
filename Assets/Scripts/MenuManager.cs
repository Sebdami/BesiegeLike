using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {
    [SerializeField]
    GameObject levelButtonPrefab;

    [SerializeField]
    RectTransform levelButtons;

    [SerializeField]
    GameObject LevelPanel;


    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    public IEnumerator LoadLevelBundle(string bundleName)
    {
        AssetBundle bundle = null;

        if (AssetBundleManager.GetAssetBundle(bundleName))
        {
            bundle = AssetBundleManager.GetAssetBundle(bundleName);
        }
        else
        {
            string uri = "file:///" + Application.dataPath + "/../" + bundleName;
            UnityWebRequest request = UnityWebRequest.GetAssetBundle(uri, 0);
            yield return request.Send();

            try
            {
                bundle = DownloadHandlerAssetBundle.GetContent(request);
            }
            catch (Exception e)
            {
                Debug.LogError("Level loading failed " + e.Message);
            }
            AssetBundleManager.AddAssetBundle(bundleName, bundle);
        }
        
        string LevelName = bundle.GetAllScenePaths()[0];

        SceneManager.LoadScene(LevelName);
        yield return null;
    }

    public void UpdateLevelPanel()
    {
        while (levelButtons.childCount > 0)
        {
            DestroyImmediate(levelButtons.GetChild(0).gameObject);
        }

        string[] levelFileNames = Directory.GetFiles(Directories.LEVEL_BUNDLES_DIRECTORY, "*.manifest");
        foreach (string fileName in levelFileNames)
        {
            string bundleName = fileName.Replace(".manifest", "");
            string uri = "file:///" + Application.dataPath + "/../" + bundleName;
            string levelName = bundleName.Replace(Directories.LEVEL_BUNDLES_DIRECTORY, "");
            Button levelButton = Instantiate(levelButtonPrefab, levelButtons).GetComponent<Button>();
            levelButton.onClick.RemoveAllListeners();
            levelButton.onClick.AddListener(() => StartCoroutine(LoadLevelBundle(bundleName)));
            levelButton.onClick.AddListener(() => CloseLevelPanel());
            levelButton.GetComponentInChildren<Text>().text = levelName;
        }
    }

    public void OpenLevelPanel()
    {
        UpdateLevelPanel();
        LevelPanel.SetActive(true);
    }

    public void CloseLevelPanel()
    {
        LevelPanel.SetActive(false);
    }

    public void ExitApp()
    {
        Application.Quit();
    }
}
