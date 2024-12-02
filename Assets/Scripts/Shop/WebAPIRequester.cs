using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Result = UnityEngine.Networking.UnityWebRequest.Result;

/// <summary>
/// Shop�ƃf�[�^�x�[�X�̑���M���s���B�����_�ł͊eShop���ƂɃC���X�^���X����������݌v�B
/// </summary>
public class WebAPIRequester : MonoBehaviour
{
    private static WebAPIRequester _apiRequester = default;

    private void Awake()
    {
        if (_apiRequester == null)
        {
            _apiRequester = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
    }

    // this path is for debug.
    private const string DETABASE_PATH = "http://10.11.39.210:8080/shop/buy";


    private async void Start()
    {
        //var a = new List<OnPaymentData.Inventory>();
        //a.Add(new OnPaymentData.Inventory(30001, 5));
        //a.Add(new OnPaymentData.Inventory(30002, 1));
        //var b = await PostPayment(a, 10001, 20001);
        //XDebug.Log(b.InventoryList[0].ItemID);
        //XDebug.Log(b.InventoryList[0].Count);
        //XDebug.Log(b.Money);
        //XDebug.Log(b.Stock);
        //XDebug.Log(b.UserID);
    }

    public async UniTask Get(int genre, int large, int small)
    {
        WWWForm form = new WWWForm();
        form.AddField("genre", genre);
        form.AddField("large", large);
        form.AddField("small", small);
        using var request = UnityWebRequest.Post(DETABASE_PATH, form);
        await request.SendWebRequest();

        switch (request.result)
        {
            case Result.InProgress:
                throw new System.InvalidOperationException("�l�b�g���[�N�ʐM�������i�s���B");

            case Result.ConnectionError or Result.ProtocolError or Result.DataProcessingError:
                throw new System.InvalidOperationException(request.error);
        }

        var lineupData = JsonUtility.FromJson<LineupData>($"{request.downloadHandler.text}");
        foreach (var item in lineupData.Lineup)
        {
            //XDebug.Log(item.ID, "green");
        }
    }

    public async UniTask<Dictionary<int, int>> UpdateStock(int shopID)
    {
        using var request = UnityWebRequest.Post(DETABASE_PATH, new WWWForm());
        await request.SendWebRequest();
        return default;
    }

    public async UniTask<OnPaymentData> PostPayment(List<OnPaymentData.Inventory> inventory, int shopId, int userId)
    {
        WWWForm form = new WWWForm();
        form.AddField("inventory", JsonUtility.ToJson(inventory));
        form.AddField("shopId", shopId);
        form.AddField("userId", userId);
        using var request = UnityWebRequest.Post(DETABASE_PATH, form);
        await request.SendWebRequest();

        switch (request.result)
        {
            case Result.InProgress:
                throw new System.InvalidOperationException("�l�b�g���[�N�ʐM�������i�s���B");

            case Result.ConnectionError or Result.ProtocolError or Result.DataProcessingError:
                throw new System.InvalidOperationException(request.error);
        }

        var onPaymentData = JsonUtility.FromJson<OnPaymentData>($"{request.downloadHandler.text}");
        return onPaymentData;
    }

    [System.Serializable]
    public class LineupData
    {
        [SerializeField] private int responseCode = default;
        [SerializeField] private string message = default;
        [SerializeField] private List<Body> body = default;

        public int ResponseCode => responseCode;
        public string Message => message;
        public IReadOnlyList<Body> Lineup => body;

        [System.Serializable]
        public class Body
        {
            [SerializeField] private int id = default;
            [SerializeField] private int price = default;
            [SerializeField] private float discount = default;
            [SerializeField] private int stock = default;
            [SerializeField] private ItemSize size = default;

            public int ID => id;
            public int Price => price;
            public float Discount => discount;
            public int Stock => stock;
            public ItemSize Size => size;
        }
    }

    [System.Serializable]
    public class OnPaymentData
    {
        [SerializeField] private List<Inventory> inventory = default;
        [SerializeField] private int money = default;
        [SerializeField] private int stock = default;
        [SerializeField] private int userId = default;

        public IReadOnlyList<Inventory> InventoryList => inventory;
        public int Money => money;
        public int Stock => stock;
        public int UserID => userId;

        [System.Serializable]
        public class Inventory
        {
            public Inventory(int itemId, int count)
            {
                this.itemId = itemId;
                this.count = count;
            }

            [SerializeField] private int itemId = default;
            [SerializeField] private int count = default;

            public int ItemID => itemId;
            public int Count => count;
        }
    }
}