public class ConnectionChecker
{
    public ConnectionChecker()
    {
        GateOfFusion.Instance.OnShutdown += CutOff;
    }

    private bool wasCutOff = false;

    public bool IsConnection => (wasCutOff ? false: GateOfFusion.Instance.IsActivityConnected && GateOfFusion.Instance.NetworkRunner.IsSharedModeMasterClient);

    private void CutOff()
    {
        wasCutOff = true;
    }
}
