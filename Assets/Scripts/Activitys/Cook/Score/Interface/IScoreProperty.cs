using UniRx;

public interface IScoreProperty
{
    /// <summary>
    /// 現在のスコアを設定するためのプロパティ
    /// </summary>
    // 現在のスコア
    int ScoreSetter { get;  set; }
    int ChainValue { set; }
}
