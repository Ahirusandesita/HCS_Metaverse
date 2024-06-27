using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        /// <summary>
        /// Editor以外で使わないで
        /// </summary>
        Debug,
    }

    public enum SystemMessageType
    {
        JoinTheWorld,
        LeftTheWorld,
    }

    private static ChatSystem s_instance = default;
    public static ChatSystem Instance => s_instance;

    [SerializeField] private TextMeshProUGUI target = default;
    [SerializeField] private Scrollbar verticalScrollbar = default;
    [SerializeField] private ImproperWordAsset improperWordAsset = default;
    [Tooltip("ログにタイムスタンプを設置するかどうか")]
    [SerializeField] private bool enableTimeStamp = true;
    [Tooltip("ログに特定文字列置換機能（ブラックワードリスト）を適用するかどうか")]
    [SerializeField] private bool enableImproperMasking = true;

    private readonly StringBuilder sb = new StringBuilder();
    private readonly List<Log> chatLogs = new List<Log>();
    private readonly object lockObj = new object();

    private const int MAX_LOG_COUNT = 100;


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
        verticalScrollbar ??= transform.Find("Scrollbar Vertical").GetComponent<Scrollbar>();
    }

    private void Awake()
    {
        if (s_instance is not null)
        {
            Destroy(this);
            return;
        }

        s_instance = this;
    }

    /// <summary>
    /// 自由な文字列を送信する
    /// </summary>
    /// <param name="content">送信内容</param>
    public void SendManually(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return;
        }

        if (enableImproperMasking)
        {
            foreach (var word in improperWordAsset.ImproperWords)
            {
                content = Regex.Replace(content, word, new string(ImproperWordAsset.MASKED_CHAR, word.Length), RegexOptions.IgnoreCase);
            }
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

#if UNITY_EDITOR
            case LogType.Debug:
                var debugLogs = new List<Log>();
                foreach (var log in chatLogs)
                {
                    if (log.logType == LogType.Debug)
                    {
                        debugLogs.Add(log);
                    }
                }
                return debugLogs;
#endif

            default:
                throw new ArgumentOutOfRangeException(nameof(logType));
        }
    }

    /// <summary>
    /// ChatWindowにデバッグ用メッセージを送信する
    /// <br>※Awakeでの実行はエラーが出る可能性があるため非推奨</br>
    /// </summary>
    /// <param name="message"></param>
    [Conditional("UNITY_EDITOR")]
    public static void Debug(string message)
    {
        message = $"<color=red>{message}</color>";
        Instance.UpdateLog(message, LogType.Debug);
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
#if UNITY_EDITOR
                    if (logType == LogType.Debug)
                    {
                        timeStamp = $"<color=red>{timeStamp}</color>";
                    }
#endif
                }

                // StringBuilderで文字列連結
                sb.AppendLine(timeStamp + log.content);
            }

            // ログをテキストに代入
            // 最後の改行コードは冗長なため削除
            target.text = sb.ToString().TrimEnd();

            // チャットログが更新されると、スクロールバーを一番下に持ってくる（なくてもいい）
            verticalScrollbar.value = 0f;
        }
    }
}
