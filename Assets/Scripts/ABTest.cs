using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FZ.HiddenObjectGame
{
    public class ABTest : MonoBehaviour
    {
        private AsyncOperation async;
        private void Start()
        {
            
        }

        private void OnGUI()
        {
            GUILayout.Space(50);
            if (GUILayout.Button("<<<<<<加载游戏>>>>>>",GUILayout.Width(160),GUILayout.Height(60)))
            {
                StartCoroutine(loadScene(@"/AssetBundle/scenes.assetbundle", "CSIcons"));
            }
            if (GUILayout.Button("<<<<<<退出游戏>>>>>>", GUILayout.Width(160), GUILayout.Height(60)))
            {
                Application.Quit();
            }
        }

        IEnumerator loadScene(string abPath, string sceneName)
        {
            print(abPath + "    " + sceneName);
            string fileFullPath = "file://" + Application.streamingAssetsPath + abPath;
            print(fileFullPath);
            WWW www = new WWW(fileFullPath);
            yield return www;
            if (www.error == null)
            {
                print("加载成功开始进入场景");
                //Initialization.Instance.ABScene = www.assetBundle;

                async = SceneManager.LoadSceneAsync(sceneName);
                async.allowSceneActivation = false;
                while (async.progress < 0.9f)
                {
                    Debug.Log(async.progress);
                    yield return new WaitForEndOfFrame();
                }
                //SceneManager.LoadScene(sceneName);                


                async.allowSceneActivation = true;
            }
            else
            {
                print(www.error.ToString());
            }
            www.Dispose();
            www = null;
        }
    }
}
