using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Result = UnityEngine.Networking.UnityWebRequest.Result;

/// <summary>
/// Shopとデータベースの送受信を行う。現時点では各Shopごとにインスタンスを所持する設計。
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

    public async UniTask<OnEntryData> PostEntry(int shopId)
    {
        WWWForm form = new WWWForm();
        form.AddField("shopId", shopId);
        using var request = UnityWebRequest.Post(DETABASE_PATH, form);
        await request.SendWebRequest();

        switch (request.result)
        {
            case Result.InProgress:
                throw new System.InvalidOperationException("ネットワーク通信が未だ進行中。");

            case Result.ConnectionError or Result.ProtocolError or Result.DataProcessingError:
                var itemList = new List<OnEntryData.Lineup>();
                itemList.Add(new OnEntryData.Lineup(20004, 15000, 0, 3, 1));
                itemList.Add(new OnEntryData.Lineup(10001, 10000, 0, 5, 0));
                itemList.Add(new OnEntryData.Lineup(10002, 8000, 0.1f, 1, 0));
                itemList.Add(new OnEntryData.Lineup(20001, 500, 0, 15, 1));
                itemList.Add(new OnEntryData.Lineup(20002, 24000, 0.9f, 6, 0));
                itemList.Add(new OnEntryData.Lineup(20003, 16000, 1f, 1, 1));
                return new OnEntryData(itemList);
                //throw new System.InvalidOperationException(request.error);
        }

        var onEntryData = JsonUtility.FromJson<OnEntryData>($"{request.downloadHandler.text}");
        return onEntryData;
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
                throw new System.InvalidOperationException("ネットワーク通信が未だ進行中。");

            case Result.ConnectionError or Result.ProtocolError or Result.DataProcessingError:
                throw new System.InvalidOperationException(request.error);
        }

        var onPaymentData = JsonUtility.FromJson<OnPaymentData>($"{request.downloadHandler.text}");
        return onPaymentData;
    }

    [System.Serializable]
    public class OnEntryData
    {
        public OnEntryData(List<Lineup> itemList)
        {
            this.itemList = itemList;
        }

        [SerializeField] private List<Lineup> itemList = default;

        public IReadOnlyList<Lineup> ItemLineup => itemList;

        [System.Serializable]
        public class Lineup
        {
            public Lineup(int itemId, int salesPrice, float discount, int stock, int size)
            {
                this.itemId = itemId;
                this.salesPrice = salesPrice;
                this.discount = discount;
                this.stock = stock;
                this.size = size;
            }

            [SerializeField] private int itemId = default;
            [SerializeField] private int salesPrice = default;
            [SerializeField] private float discount = default;
            [SerializeField] private int stock = default;
            [SerializeField] private int size = default;

            public int ItemID => itemId;
            public int Price => salesPrice;
            public float Discount => discount;
            public int Stock => stock;
            /// <summary>
            /// 0: large, 1: small
            /// </summary>
            public int Size => size;
        }
    }

    [System.Serializable]
    public class OnPaymentData
    {
        public OnPaymentData(List<Inventory> inventory, int money, int stock, int userId)
        {
            this.inventory = inventory;
            this.money = money;
            this.stock = stock;
            this.userId = userId;
        }

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