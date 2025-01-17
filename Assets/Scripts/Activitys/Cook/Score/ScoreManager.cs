using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;
public class ScoreManager : MonoBehaviour, IScoreProperty, IScoreCalculator
{
    [SerializeField, Tooltip("スコアを表示するためのText")]
    private Text _scoreText = default;

    [SerializeField]
    private TextMeshProUGUI chainText;

    // ScoreCalcuratorのインスタンス保持用
    private ScoreCalculator _scoreCalculator = default;

    // ScoreCalculatorを受け渡すためのプロパティ
    public ScoreCalculator GetScoreCalculator => _scoreCalculator;

    // 現在のスコア
    private ReactiveProperty<int> _nowScore = new ReactiveProperty<int>();

    // 現在のスコアを設定するためのプロパティ
    public int ScoreSetter { get { return _nowScore.Value; } set { _nowScore.Value = value; } }
    private int chainValue = 0;

    // スコア表示の固定部分
    private const string SCORE_PREAMBLE = "SCORE : ";

    private void Awake()
    {
        // 現在のスコアを講読して表示を変更できるようにする
        _nowScore.Subscribe((score) => { DisplayNowScore(); });

        // ScoreCalculatorのインスタンスを取得する
        _scoreCalculator = new ScoreCalculator(this);
    }

    /// <summary>
    /// ゲームを開始したときに実行する初期化メソッド
    /// </summary>
    public void GameStart()
    {
        // 現在のスコアを初期化する
        _nowScore.Value = 0;
    }

    /// <summary>
    /// 現在のスコアの表示を行うメソッド
    /// </summary>
    public void DisplayNowScore()
    {
        // 現在のスコアを表示する
        _scoreText.text = SCORE_PREAMBLE + _nowScore.Value.ToString();
    }
    public int ChainValue 
    { 
        set 
        {
            chainValue = value;
            chainText.text = chainValue.ToString();
        }
    }
}
