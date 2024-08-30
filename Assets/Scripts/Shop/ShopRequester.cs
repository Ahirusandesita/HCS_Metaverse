using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Result = UnityEngine.Networking.UnityWebRequest.Result;

/// <summary>
/// Shopとデータベースの送受信を行う。現時点では各Shopごとにインスタンスを所持する設計。
/// </summary>
public class ShopRequester : MonoBehaviour
{
    [System.Serializable]
    private class LineupData
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

    // this path is for debug.
    private const string DETABASE_PATH = "http://10.11.39.210:8080/shop/getitemlist";


    private async void Start()
    {
        await Get(0, 2, 3);
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
                throw new System.InvalidOperationException("ネットワーク通信が未だ進行中。");

            case Result.ConnectionError or Result.ProtocolError or Result.DataProcessingError:
                throw new System.InvalidOperationException(request.error);
        }

        var lineupData = JsonUtility.FromJson<LineupData>($"{request.downloadHandler.text}");
        foreach (var item in lineupData.Lineup)
        {
            //XDebug.Log(item.ID, "green");
        }
    }
}