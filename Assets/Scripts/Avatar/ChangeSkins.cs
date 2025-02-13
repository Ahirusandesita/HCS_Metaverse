using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Layer_lab._3D_Casual_Character;

public class ChangeSkins : MonoBehaviour, IDressUpEventSubscriber
{
    [SerializeField]
    private ItemBundleAsset _itemBundle = default;

    private Transform[] _partsChildren = default;
    private CharacterControlRPCManager characterControlRPCManager;
    [SerializeField]
    private Transform _partsParent = default;
    [SerializeField]
    private GameObject[] _bodys = new GameObject[4];
    private struct PartsIndex
    {
        public PartsIndex(int defaultValue = -1)
        {
            body = defaultValue;
            bag = defaultValue;
            bottom = defaultValue;
            eyewear = defaultValue;
            face = defaultValue;
            glove = defaultValue;
            hair = defaultValue;
            headgear = defaultValue;
            shoes = defaultValue;
            top = defaultValue;
        }

        public int body;
        public int bag;
        public int bottom;
        public int eyewear;
        public int face;
        public int glove;
        public int hair;
        public int headgear;
        public int shoes;
        public int top;

        public int GetParts(int index) => index switch
        {
            0 => body,
            1 => bag,
            2 => bottom,
            3 => eyewear,
            4 => face,
            5 => glove,
            6 => hair,
            7 => headgear,
            8 => shoes,
            9 => top,
            _ => 0
        };

        public void SetParts(int index, int value)
        {
            switch (index)
            {
                case 0:
                    body = value;
                    break;

                case 1:
                    bag = value;
                    break;

                case 2:
                    bottom = value;
                    break;

                case 3:
                    eyewear = value;
                    break;

                case 4:
                    face = value;
                    break;

                case 5:
                    glove = value;
                    break;

                case 6:
                    hair = value;
                    break;

                case 7:
                    headgear = value;
                    break;

                case 8:
                    shoes = value;
                    break;

                case 9:
                    top = value;
                    break;

                default:
                    break;
            }
        }
    }

    private string[] _partsNames = new string[]
    {
        "Body",
        "Bag",
        "Bottom",
        "Eyewear",
        "Face",
        "Glove",
        "Hair",
        "Headgear",
        "Shoes",
        "Top"
    };

    private PartsIndex _wearingPartsIndex = new();

    private const int UNWEAR_INDEX = -1;

    private async void Start()
    {
        _partsChildren = _partsParent.GetComponentsInChildren<Transform>(true);

        Transform[] wearing = _partsParent.GetComponentsInChildren<Transform>();

        for (int wearingIndex = 0; wearingIndex < wearing.Length; wearingIndex++)
        {
            string name = wearing[wearingIndex].name;

            for (int i = 0; i < _partsChildren.Length; i++)
            {
                if (!_partsChildren[i].name.Equals(name))
                {
                    continue;
                }

                int machIndex = UNWEAR_INDEX;

                for (int nameIndex = 0; nameIndex < _partsNames.Length; nameIndex++)
                {
                    if (_partsChildren[i].gameObject.name.Contains(_partsNames[nameIndex]))
                    {
                        machIndex = nameIndex;
                        break;
                    }
                }

                _wearingPartsIndex.SetParts(machIndex, i);
            }
        }

        RemoteView remoteView = await FindObjectOfType<LocalRemoteSeparation>().ReceiveRemoteView();

        characterControlRPCManager = remoteView.GetComponentInChildren<CharacterControlRPCManager>();

        characterControlRPCManager.RPC_ChangeBody();
    }

    public void OnDressUp(int id, string name)
    {
        if (id == UNWEAR_INDEX)
        {
            TakeOffDress(name);
            return;
        }

        for (int i = 0; i < _partsChildren.Length; i++)
        {
            if (!_partsChildren[i].name.Equals(name))
            {
                continue;
            }

            int machIndex = UNWEAR_INDEX;

            for (int nameIndex = 0; nameIndex < _partsNames.Length; nameIndex++)
            {
                if (_partsChildren[i].gameObject.name.Contains(_partsNames[nameIndex]))
                {
                    machIndex = nameIndex;
                    break;
                }
            }

            characterControlRPCManager.RPC_ChangeSkins(machIndex, i);
            _wearingPartsIndex.SetParts(machIndex, i);
        }
    }
    public void RPCDressUp(int machIndex, int i)
    {
        _partsChildren[i].gameObject.SetActive(true);

        int partsIndex = _wearingPartsIndex.GetParts(machIndex);

        if (partsIndex != UNWEAR_INDEX)
        {
            _partsChildren[partsIndex].gameObject.SetActive(false);
        }

        _wearingPartsIndex.SetParts(machIndex, i);
    }

    public void TakeOffDress(string name)
    {
        int machIndex = UNWEAR_INDEX;

        for (int nameIndex = 0; nameIndex < _partsNames.Length; nameIndex++)
        {
            if (name.Contains(_partsNames[nameIndex]))
            {
                machIndex = nameIndex;
                break;
            }
        }

        if (machIndex == UNWEAR_INDEX)
        {
            Debug.LogError($"{gameObject.name} : スキンのパーツ名が一致しないため脱げない");
            return;
        }

        int partsIndex = _wearingPartsIndex.GetParts(machIndex);

        if (partsIndex != UNWEAR_INDEX)
        {
            _partsChildren[partsIndex].gameObject.SetActive(false);
            _wearingPartsIndex.SetParts(machIndex, UNWEAR_INDEX);
            characterControlRPCManager.RPC_TakeOffSkins(partsIndex, machIndex);
            characterControlRPCManager.RPC_ChangeBody();
        }
    }

    public void RPCTakeOff(int partsIndex, int machIndex)
    {
        _partsChildren[partsIndex].gameObject.SetActive(false);
        _wearingPartsIndex.SetParts(machIndex, UNWEAR_INDEX);
    }

    private int getBodyIndex
    {
        get
        {
            if (_wearingPartsIndex.GetParts(5) != UNWEAR_INDEX && _wearingPartsIndex.GetParts(8) != UNWEAR_INDEX)
            {
                return 2;
            }
            else if (_wearingPartsIndex.GetParts(5) == UNWEAR_INDEX)
            {
                if (_wearingPartsIndex.GetParts(8) == UNWEAR_INDEX)
                {
                    return 3;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                return 0;
            }
        }
    }

    public void ChangeBody()
    {
        int nowBodyIndex = _wearingPartsIndex.GetParts(0);

        if (nowBodyIndex != UNWEAR_INDEX)
        {
            _bodys[nowBodyIndex].gameObject.SetActive(false);
        }

        int nextBodyIndex = getBodyIndex;

        _bodys[nextBodyIndex].gameObject.SetActive(true);

        _wearingPartsIndex.SetParts(0 ,nextBodyIndex);
    }
}
