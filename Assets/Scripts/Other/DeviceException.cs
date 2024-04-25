namespace System
{
    /// <summary>
    /// 想定されたデバイス以外が検出されたときにthrowされる例外
    /// </summary>
    public class DeviceException : Exception
    {
        public DeviceException() : base() { }
        public DeviceException(string message) : base(message) { }
        public DeviceException(string message, Exception innerException) : base(message, innerException) { }
    }
}
