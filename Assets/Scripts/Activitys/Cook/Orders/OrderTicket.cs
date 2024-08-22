public class OrderTicket
{
    public readonly IOrderable Orderable;
    public readonly CustomerInformation CustomerInformation;
    public OrderTicket(IOrderable orderable, CustomerInformation customer)
    {
        this.Orderable = orderable;
        this.CustomerInformation = customer;
    }
}