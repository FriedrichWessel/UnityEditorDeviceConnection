namespace EditorConnectionWindow.BaseSystem
{
	public class AvailableServerData
	{
		public ServerData ServerData { get; private set; }
		public float LastConnectionTime { get; set; }
	
		public AvailableServerData(ServerData data)
		{
			ServerData = data;
			LastConnectionTime = 0;
		}
	
		public override string ToString()
		{
			return string.Format("{{{0} LastConnectionTime:{1}}}", ServerData.ToString(), LastConnectionTime);
		}
	}
}
