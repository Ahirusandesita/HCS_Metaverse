using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

/// <summary>
/// ワールドチャットのシステム
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
    /// 自由な文字列を送信する
    /// </summary>
    /// <param name="content">送信内容</param>
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
    /// システムメッセージを送信する
    /// </summary>
    /// <param name="systemMessageType">システムメッセージの種類</param>
    /// <param name="userName">ユーザー名</param>
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
                message = $"{userName}がワールドに入りました";
                break;

            case SystemMessageType.LeftTheWorld:
                message = $"{userName}がワールドから離れました";
                break;
        }

        UpdateLog(message, LogType.System);
    }

    /// <summary>
    /// 保存されているログを取得する
    /// </summary>
    /// <param name="logType">取得するログの種類</param>
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
        // 排他制御
        lock (lockObj)
        {
            chatLogs.Add(new Log(content, DateTime.Now, logType));

            // ログが保存上限数を超えていたら、最古データを削除する
            if (chatLogs.Count > MAX_LOG_COUNT)
            {
                chatLogs.RemoveAt(0);
            }

            sb.Clear();

            // 保存しているログをすべて出力する
            foreach (var log in chatLogs)
            {
                // 設定によってタイムスタンプを抽出
                string timeStamp = string.Empty;
                if (enableTimeStamp)
                {
                    // [HH:mm] の形式で表示
                    timeStamp = $"[{log.timeStamp:HH:mm}] ";
                }

                // StringBuilderで文字列連結
                sb.Append(timeStamp + log.content + Environment.NewLine);
            }

            // ログをテキストに代入
            // 最後の改行コードは冗長なため削除
            target.text = sb.ToString().TrimEnd();

            // チャットログが更新されると、スクロールバーを一番下に持ってくる（なくてもいい）
            verticalScrollbar.value = 0f;
        }
    }
}
