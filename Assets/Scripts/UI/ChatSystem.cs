using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

/// <summary>
/// ���[���h�`���b�g�̃V�X�e��
/// </summary>
public class ChatSystem : MonoBehaviour
{
    public class Log
    {
        public readonly string content = default;
        public readonly DateTime timeStamp = default;
        public readonly LogType logType = default;

        public Log(string content, DateTime timeStamp, LogType logType)
        {
            this.content = content;
            this.timeStamp = timeStamp;
            this.logType = logType;
        }
    }

    public enum LogType
    {
        All,
        Manually,
        System,
    }

    public enum SystemMessageType
    {
        JoinTheWorld,
        LeftTheWorld,
    }


    [SerializeField] private TextMeshProUGUI target = default;
    [SerializeField] private Scrollbar verticalScrollbar = default;
    [SerializeField] private ImproperWordAsset improperWordAsset = default;
    [SerializeField] private bool enableTimeStamp = true;
    [SerializeField] private bool enableImproperMasking = false;

    private readonly StringBuilder sb = new StringBuilder();
    private readonly List<Log> chatLogs = new List<Log>();
    private readonly object lockObj = new object();

    private const int MAX_LOG_COUNT = 100;

    // Debug
    private int count = 0;


    public bool EnableTimeStamp
    {
        get => enableTimeStamp;
        set => enableTimeStamp = value;
    }

    public bool EnableImproperMasking

    {
        get => enableImproperMasking;
        set => enableImproperMasking = value;
    }


    [Conditional("UNITY_EDITOR")]
    private void Reset()
    {
        target ??= GetComponentInChildren<TextMeshProUGUI>();
        try
        {
            verticalScrollbar ??= transform.Find("Scrollbar Vertical").GetComponent<Scrollbar>();
        }
        catch (NullReferenceException) { }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            SendManually(count.ToString());
            count++;
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            SendManually("Yanagi");
            count++;
        }
    }

    /// <summary>
    /// ���R�ȕ�����𑗐M����
    /// </summary>
    /// <param name="content">���M���e</param>
    /// <exception cref="ArgumentNullException"></exception>
    public void SendManually(string content)
    {
        if (content is null)
        {
            throw new ArgumentNullException(nameof(content));
        }

        foreach (var word in improperWordAsset.ImproperWords)
        {
            content = Regex.Replace(content, $"{word}", new string(ImproperWordAsset.MASKED_CHAR, word.Length), RegexOptions.IgnoreCase);
        }

        UpdateLog(content, LogType.Manually);
    }

    /// <summary>
    /// �V�X�e�����b�Z�[�W�𑗐M����
    /// </summary>
    /// <param name="systemMessageType">�V�X�e�����b�Z�[�W�̎��</param>
    /// <param name="userName">���[�U�[��</param>
    /// <exception cref="ArgumentNullException"></exception>
    public void SendSystemMessage(SystemMessageType systemMessageType, string userName)
    {
        if (userName is null)
        {
            throw new ArgumentNullException(nameof(userName));
        }

        string message = string.Empty;

        switch (systemMessageType)
        {
            case SystemMessageType.JoinTheWorld:
                message = $"{userName}�����[���h�ɓ���܂���";
                break;

            case SystemMessageType.LeftTheWorld:
                message = $"{userName}�����[���h���痣��܂���";
                break;
        }

        UpdateLog(message, LogType.System);
    }

    /// <summary>
    /// �ۑ�����Ă��郍�O���擾����
    /// </summary>
    /// <param name="logType">�擾���郍�O�̎��</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <returns></returns>
    public IReadOnlyList<Log> GetLogs(LogType logType)
    {
        switch (logType)
        {
            case LogType.All:
                return chatLogs;

            case LogType.Manually:
                var manuallyLogs = new List<Log>();
                foreach (var log in chatLogs)
                {
                    if (log.logType == LogType.Manually)
                    {
                        manuallyLogs.Add(log);
                    }
                }
                return manuallyLogs;

            case LogType.System:
                var systemLogs = new List<Log>();
                foreach (var log in chatLogs)
                {
                    if (log.logType == LogType.System)
                    {
                        systemLogs.Add(log);
                    }
                }
                return systemLogs;

            default:
                throw new ArgumentOutOfRangeException(nameof(logType));
        }
    }

    private void UpdateLog(string content, LogType logType)
    {
        // �r������
        lock (lockObj)
        {
            chatLogs.Add(new Log(content, DateTime.Now, logType));

            // ���O���ۑ�������𒴂��Ă�����A�ŌÃf�[�^���폜����
            if (chatLogs.Count > MAX_LOG_COUNT)
            {
                chatLogs.RemoveAt(0);
            }

            sb.Clear();

            // �ۑ����Ă��郍�O�����ׂďo�͂���
            foreach (var log in chatLogs)
            {
                // �ݒ�ɂ���ă^�C���X�^���v�𒊏o
                string timeStamp = string.Empty;
                if (enableTimeStamp)
                {
                    // [HH:mm] �̌`���ŕ\��
                    timeStamp = $"[{log.timeStamp:HH:mm}] ";
                }

                // StringBuilder�ŕ�����A��
                sb.Append(timeStamp + log.content + Environment.NewLine);
            }

            // ���O���e�L�X�g�ɑ��
            // �Ō�̉��s�R�[�h�͏璷�Ȃ��ߍ폜
            target.text = sb.ToString().TrimEnd();

            // �`���b�g���O���X�V�����ƁA�X�N���[���o�[����ԉ��Ɏ����Ă���i�Ȃ��Ă������j
            verticalScrollbar.value = 0f;
        }
    }
}
