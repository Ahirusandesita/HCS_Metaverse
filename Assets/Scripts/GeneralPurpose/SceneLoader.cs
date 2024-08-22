using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

/// <summary>
/// シーン遷移を行うクラス
/// </summary>
public static class SceneLoader
{
    /// <summary>
    /// シーンをロードし、ロード先シーンのコンポーネントを取得する
    /// <br>- awaitで実行すること</br>
    /// </summary>
    /// <typeparam name="T">読み込んだシーン上の取得したいコンポーネント</typeparam>
    /// <returns>取得したコンポーネント</returns>
    public static UniTask<T> Load<T>(string sceneName, LoadSceneMode mode = LoadSceneMode.Single) where T : Component
    {
        var tcs = new UniTaskCompletionSource<T>();

        // シーンのロード完了後に実行する処理を登録
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(sceneName, mode);

        // タスクを返す（呼び出し元で、結果が確定されるまで待機する）
        return tcs.Task;

        void OnSceneLoaded(Scene nextScene, LoadSceneMode mode)
        {
            // 一度の実行でよいため登録を解除
            SceneManager.sceneLoaded -= OnSceneLoaded;
            // コンポーネントを探索
            T target = Object.FindObjectOfType<T>();

            // 結果を確定
            tcs.TrySetResult(target);
        }
    }

    /// <summary>
    /// シーンをロードする
    /// </summary>
    public static void Load(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
    {
        SceneManager.LoadScene(sceneName, mode);
    }
}