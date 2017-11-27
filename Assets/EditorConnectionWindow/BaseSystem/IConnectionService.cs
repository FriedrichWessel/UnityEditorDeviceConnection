namespace EditorConnectionWindow.BaseSystem
{
    public interface IConnectionService
    {
        string Address { get; set; }
        int Port { get; set; }
        string URL { get;  }
        string GetLocalIPAddress();
    }
}


