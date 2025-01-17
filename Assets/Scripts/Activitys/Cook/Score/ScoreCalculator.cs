using UniRx;

public class ScoreCalculator
{
    public ScoreCalculator(IScoreProperty scoreProperty)
    {
        //IScorePropertyを設定する
        _scoreProperty = scoreProperty;
    }

    // 現在のスコアを変更するためのプロパティ
    private IScoreProperty _scoreProperty = default;

    // スコアを計算して加算するメソッド
    public void ScoreCalucuration(int scorePoint, int chainCount)
    {
        _scoreProperty.ChainValue = chainCount;
        _scoreProperty.ScoreSetter += scorePoint * (1 + (int)(chainCount * 0.5f));
    }
}
