public class CommodityInformation
{
    public readonly CommodityAsset CommodityAsset;
    public readonly CustomerInformation CustomerInformation;
    public CommodityInformation(CommodityAsset commodityAsset,CustomerInformation customerInformation)
    {
        this.CommodityAsset = commodityAsset;
        this.CustomerInformation = customerInformation;
    }
}
