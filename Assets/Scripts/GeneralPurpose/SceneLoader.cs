using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

/// <summary>
/// �V�[���J�ڂ��s���N���X
/// </summary>
public static class SceneLoader
{
    /// <summary>
    /// �V�[�������[�h���A���[�h��V�[���̃R���|�[�l���g���擾����
    /// <br>- await�Ŏ��s���邱��</br>
    /// </summary>
    /// <typeparam name="T">�ǂݍ��񂾃V�[����̎擾�������R���|�[�l���g</typeparam>
    /// <returns>�擾�����R���|�[�l���g</returns>
    public static UniTask<T> Load<T>(string sceneName, LoadSceneMode mode = LoadSceneMode.Single) where T : Component
    {
        var tcs = new UniTaskCompletionSource<T>();

        // �V�[���̃��[�h������Ɏ��s���鏈����o�^
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(sceneName, mode);

        // �^�X�N��Ԃ��i�Ăяo�����ŁA���ʂ��m�肳���܂őҋ@����j
        return tcs.Task;

        void OnSceneLoaded(Scene nextScene, LoadSceneMode mode)
        {
            // ��x�̎��s�ł悢���ߓo�^������
            SceneManager.sceneLoaded -= OnSceneLoaded;
            // �R���|�[�l���g��T��
            T target = Object.FindObjectOfType<T>();

            // ���ʂ��m��
            tcs.TrySetResult(target);
        }
    }

    /// <summary>
    /// �V�[�������[�h����
    /// </summary>
    public static void Load(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
    {
        SceneManager.LoadScene(sceneName, mode);
    }
}