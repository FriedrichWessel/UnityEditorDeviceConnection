namespace EditorConnectionWindow.BaseSystem
{
	public interface IConnectionClient  {

		string IpAddress { get;  }
		bool HasData { get; }
		string GetData();
	}
}
