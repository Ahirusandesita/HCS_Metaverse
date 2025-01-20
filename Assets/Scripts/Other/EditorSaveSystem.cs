using System.IO;
using UnityEngine;

#if UNITY_EDITOR
namespace UnityEditor.HCSMeta
{
    public static class EditorSaveSystem
    {
        public static void Save(string path, string data)
		{
            var wr = new StreamWriter(path, false);
            wr.Write(data);
            wr.Close();
        }

        public static void Save<T>(string fileName, T data) where T : new()
        {
            string path = $"{Application.dataPath}/{fileName}";
            string json = EditorJsonUtility.ToJson(data);
            var wr = new StreamWriter(path, false);
            wr.WriteLine(json);
            wr.Close();
        }

        public static string Load(string path)
		{
            string data = default;
            try
            {
                var rd = new StreamReader(path);
                data = rd.ReadToEnd();
                rd.Close();
            }
            catch (FileNotFoundException)
            {
                Save(path, data);
                data = Load(path);
            }

            return data;
        }

        public static void Load<T>(string fileName, T objectToOverwrite) where T : new()
        {
            string path = $"{Application.dataPath}/{fileName}";
            string json;

            try
            {
                var rd = new StreamReader(path);
                json = rd.ReadToEnd();
                rd.Close();
            }
            catch (FileNotFoundException)
            {
                Save(fileName, new T());
                Load(fileName, objectToOverwrite);
                return;
            }

            EditorJsonUtility.FromJsonOverwrite(json, objectToOverwrite);
        }
    }
}
#endif