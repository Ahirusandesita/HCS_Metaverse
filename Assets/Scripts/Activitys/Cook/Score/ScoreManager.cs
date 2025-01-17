using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;
public class ScoreManager : MonoBehaviour, IScoreProperty, IScoreCalculator
{
    [SerializeField, Tooltip("�X�R�A��\�����邽�߂�Text")]
    private Text _scoreText = default;

    [SerializeField]
    private TextMeshProUGUI chainText;

    // ScoreCalcurator�̃C���X�^���X�ێ��p
    private ScoreCalculator _scoreCalculator = default;

    // ScoreCalculator���󂯓n�����߂̃v���p�e�B
    public ScoreCalculator GetScoreCalculator => _scoreCalculator;

    // ���݂̃X�R�A
    private ReactiveProperty<int> _nowScore = new ReactiveProperty<int>();

    // ���݂̃X�R�A��ݒ肷�邽�߂̃v���p�e�B
    public int ScoreSetter { get { return _nowScore.Value; } set { _nowScore.Value = value; } }
    private int chainValue = 0;

    // �X�R�A�\���̌Œ蕔��
    private const string SCORE_PREAMBLE = "SCORE : ";

    private void Awake()
    {
        // ���݂̃X�R�A���u�ǂ��ĕ\����ύX�ł���悤�ɂ���
        _nowScore.Subscribe((score) => { DisplayNowScore(); });

        // ScoreCalculator�̃C���X�^���X���擾����
        _scoreCalculator = new ScoreCalculator(this);
    }

    /// <summary>
    /// �Q�[�����J�n�����Ƃ��Ɏ��s���鏉�������\�b�h
    /// </summary>
    public void GameStart()
    {
        // ���݂̃X�R�A������������
        _nowScore.Value = 0;
    }

    /// <summary>
    /// ���݂̃X�R�A�̕\�����s�����\�b�h
    /// </summary>
    public void DisplayNowScore()
    {
        // ���݂̃X�R�A��\������
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
