using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Layer_lab._3D_Casual_Character;

public class ChangeSkins : MonoBehaviour, IDressUpEventSubscriber
{
    [SerializeField]
    private ItemBundleAsset _itemBundle = default;

    private Transform[] _children = default;

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

    private void Start()
    {
        Transform partParent = transform.Find("Parts");
        _children = partParent.GetComponentsInChildren<Transform>(true);

        Transform[] wearing = partParent.GetComponentsInChildren<Transform>();

        for (int wearingIndex = 0; wearingIndex < wearing.Length; wearingIndex++)
        {
            string name = wearing[wearingIndex].name;

            for (int i = 0; i < _children.Length; i++)
            {
                if (!_children[i].name.Contains(name))
                {
                    continue;
                }

                int machIndex = UNWEAR_INDEX;

                for (int nameIndex = 0; nameIndex < _partsNames.Length; nameIndex++)
                {
                    if (_children[i].gameObject.name.Contains(_partsNames[nameIndex]))
                    {
                        machIndex = nameIndex;
                        break;
                    }
                }

                _wearingPartsIndex.SetParts(machIndex, i);
            }
        }
    }

    public void OnDressUp(int id, string name)
    {
        if (id == UNWEAR_INDEX)
        {
            TakeOffDress(name);
            return;
        }

        for (int i = 0; i < _children.Length; i++)
        {
            if (!_children[i].name.Contains(name))
            {
                continue;
            }

            _children[i].gameObject.SetActive(true);

            int machIndex = UNWEAR_INDEX;

            for (int nameIndex = 0; nameIndex < _partsNames.Length; nameIndex++)
            {
                if (_children[i].gameObject.name.Contains(_partsNames[nameIndex]))
                {
                    machIndex = nameIndex;
                    break;
                }
            }

            int partsIndex = _wearingPartsIndex.GetParts(machIndex);

            if (partsIndex != UNWEAR_INDEX)
            {
                _children[partsIndex].gameObject.SetActive(false);
            }

            _wearingPartsIndex.SetParts(machIndex, i);
        }
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

        int partsIndex = _wearingPartsIndex.GetParts(machIndex);

        if (partsIndex != UNWEAR_INDEX)
        {
            _children[partsIndex].gameObject.SetActive(false);
            _wearingPartsIndex.SetParts(machIndex, UNWEAR_INDEX);
        }
    }
}
