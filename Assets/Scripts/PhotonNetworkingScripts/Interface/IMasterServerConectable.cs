using Cysharp.Threading.Tasks;
public interface IMasterServerConectable
{
	UniTask Connect(string SessionName);
}