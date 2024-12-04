public class CustomerInformation
{
    public readonly int OrderCode;

    public readonly float OrderWaitingTime;
    public readonly OrderWaitingType OrderWaitingType;
    public float RemainingTime { get; set; }
    public bool IsFirst { get; set; }
    public CustomerInformation(int orderCode,float orderWaitingTime,OrderWaitingType orderWaitingType)
    {
        this.OrderCode = orderCode;
        this.OrderWaitingTime = orderWaitingTime;
        this.OrderWaitingType = orderWaitingType;
        IsFirst = true;
    }
}
