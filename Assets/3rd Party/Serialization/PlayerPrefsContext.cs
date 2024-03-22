using System.Text;
using UnityEngine;

namespace Serialization
{
    public static class PlayerPrefsContext
    {
        private static StringBuilder StringBuilder;

        static PlayerPrefsContext()
        {
            StringBuilder = new StringBuilder();
        }

        private static string GetKey(string key) =>
            StringBuilder.Clear().Append(key).ToString();
        
        public static bool IsReadOnly() => false;

        public static bool Contains(string key) => PlayerPrefs.HasKey(GetKey(key));

        public static void SetString(string key, string value, bool writeToReadonly = false) =>
            PlayerPrefs.SetString(GetKey(key), value);

        public static string GetString(string key, string fallback = null) =>
            PlayerPrefs.GetString(GetKey(key), fallback);
        
        public static int GetInt(string key, int fallback = -1)
        {
            return int.TryParse(GetString(key, ""), out var parsedVal) ? parsedVal : fallback;
        }

        public static void SetInt(string key, int value, bool writeToReadOnly = false)
        {
            SetString(key, value.ToString(), writeToReadOnly);
        }

        public static long GetLong(string key, long fallback = -1)
        {
            return long.TryParse(GetString(key, ""), out var parsedVal) ? parsedVal : fallback;
        }

        public static void SetLong(string key, long value, bool writeToReadOnly = false)
        {
            SetString(key, value.ToString(), writeToReadOnly);
        }

        public static bool GetBool(string key, bool fallback)
        {
            if (!Contains(key))
            {
                return fallback;
            }

            var intVal = GetInt(key, -1);
            if (intVal < 0)
                return fallback;

            return intVal != 0;
        }

        public static void SetBool(string key, bool value)
        {
            SetInt(key, value ? 1 : 0);
        }

        public static void Push() => PlayerPrefs.Save();

        public static void Clear() => PlayerPrefs.DeleteAll();
    }
}