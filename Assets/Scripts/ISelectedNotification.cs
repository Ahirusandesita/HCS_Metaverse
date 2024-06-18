
/// <summary>
/// �e�A�C�e�����͂܂��/�����������󂯎��C���^�[�t�F�[�X
/// </summary>
public interface ISelectedNotification
{
    /// <summary>
    /// �A�C�e�����͂܂ꂽ�Ƃ��ɌĂ΂�鏈��
    /// </summary>
    /// <param name="selectArgs">���M�f�[�^</param>
    void Select(SelectArgs selectArgs);
    /// <summary>
    /// �A�C�e���������ꂽ�Ƃ��ɌĂ΂�鏈��
    /// </summary>
    /// <param name="selectArgs">���M�f�[�^</param>
    void Unselect(SelectArgs selectArgs);
    /// <summary>
    /// �A�C�e�����|�C���g���ꂽ�Ƃ��ɌĂ΂�鏈��
    /// </summary>
    /// <param name="selectArgs">���M�f�[�^</param>
    void Hover(SelectArgs selectArgs) { }
    /// <summary>
    /// �A�C�e�����|�C���g��Ԃ��痣�ꂽ�Ƃ��ɌĂ΂�鏈��
    /// </summary>
    /// <param name="selectArgs">���M�f�[�^</param>
    void Unhover(SelectArgs selectArgs) { }
}

public sealed class NullSelectedNotification : ISelectedNotification
{
    void ISelectedNotification.Select(SelectArgs selectArgs) { }
    void ISelectedNotification.Unselect(SelectArgs selectArgs) { }
}