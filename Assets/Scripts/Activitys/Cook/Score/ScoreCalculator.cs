using UniRx;

public class ScoreCalculator
{
    public ScoreCalculator(IScoreProperty scoreProperty)
    {
        //IScoreProperty��ݒ肷��
        _scoreProperty = scoreProperty;
    }

    // ���݂̃X�R�A��ύX���邽�߂̃v���p�e�B
    private IScoreProperty _scoreProperty = default;

    // �X�R�A���v�Z���ĉ��Z���郁�\�b�h
    public void ScoreCalucuration(int scorePoint, int chainCount)
    {
        _scoreProperty.ChainValue = chainCount;
        _scoreProperty.ScoreSetter += scorePoint * (1 + (int)(chainCount * 0.5f));
    }
}
