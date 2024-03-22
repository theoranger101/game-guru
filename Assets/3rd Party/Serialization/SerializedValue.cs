using System;

namespace Serialization
{
    [Serializable]
    public class SerializedValue<T>
    {
        public T value;

        private string m_Key;

        public SerializedValue(string serializationKey)
        {
            m_Key = serializationKey;
            
            value = (T) Convert.ChangeType(PlayerPrefsContext.GetString(serializationKey), typeof(T));
        }
    }
}