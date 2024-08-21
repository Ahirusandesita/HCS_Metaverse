using UnityEngine;

namespace HCSMeta.Activity
{
    public abstract class SelectArgs
    {
        private static SelectArgs s_empty = default;
        public static SelectArgs Empty
        {
            get
            {
                s_empty ??= new NullSelectArgs();
                return s_empty;
            }
        }
    }
    public class NullSelectArgs : SelectArgs { }

    public class ItemSelectArgs : SelectArgs
    {
        public readonly int id = default;
        public readonly string name = default;
        public readonly Vector3 position = default;
        public readonly GameObject gameObject = default;

        public ItemSelectArgs(int id, string name, Vector3 position = default, GameObject gameObject = null)
        {
            this.id = id;
            this.name = name;
            this.position = position;
            this.gameObject = gameObject;
        }
    }
}