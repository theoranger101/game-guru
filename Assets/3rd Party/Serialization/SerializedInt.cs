namespace Serialization
{
	public class SerializedInt
	{
		public string Key;
		
		public int Value
		{
			get => PlayerPrefsContext.GetInt(Key, 0);
			set => PlayerPrefsContext.SetInt(Key, value);
		}
	}
}
