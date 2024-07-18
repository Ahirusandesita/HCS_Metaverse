using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using Result = UnityEngine.Networking.UnityWebRequest.Result;

public enum ItemSize
{
    Small = 0,
    Large = 1
}

public class ShopRequester : MonoBehaviour
{
    [System.Serializable]
    private class LineupData
    {
        [SerializeField] private int responseCode = default;
        [SerializeField] private string message = default;
        [SerializeField] private Body body = default;

        public int ResponseCode => responseCode;
        public string Message => message;

        public int ID => body.ID;
        public int Price => body.Price;
        public float Discount => body.Discount;
        public int Stock => body.Stock;
        public ItemSize Size => body.Size;

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

    private const string DEFAULT_PATH = "http://10.11.39.210:8080/shop/getitemlist";


    public async UniTask Get(int genre, int large, int small)
    {
        string path = $"{DEFAULT_PATH}?genre={genre}&large={large}&small={small}";
        using var request = UnityWebRequest.Get(path);
        await request.SendWebRequest();

        switch (request.result)
        {
            case Result.InProgress:
                throw new System.InvalidOperationException("ネットワーク通信が未だ進行中。");

            case Result.ConnectionError or Result.ProtocolError or Result.DataProcessingError:
                throw new System.InvalidOperationException(request.error);
        }

        var lineupData = JsonUtility.FromJson<LineupData>($"{request.downloadHandler.text}");
        Debug.Log($"<color=green>{lineupData.ResponseCode}</color>");
        Debug.Log($"<color=green>{lineupData.Message}</color>");
        Debug.Log($"<color=green>{lineupData.ID}</color>");
        Debug.Log($"<color=green>{lineupData.Price}</color>");
        Debug.Log($"<color=green>{lineupData.Discount}</color>");
        Debug.Log($"<color=green>{lineupData.Stock}</color>");
        Debug.Log($"<color=green>{lineupData.Size}</color>");
    }
}
