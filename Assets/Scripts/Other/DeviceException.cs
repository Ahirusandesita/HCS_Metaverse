namespace System
{
    /// <summary>
    /// �z�肳�ꂽ�f�o�C�X�ȊO�����o���ꂽ�Ƃ���throw������O
    /// </summary>
    public class DeviceException : Exception
    {
        public DeviceException() : base() { }
        public DeviceException(string message) : base(message) { }
        public DeviceException(string message, Exception innerException) : base(message, innerException) { }
    }
}
