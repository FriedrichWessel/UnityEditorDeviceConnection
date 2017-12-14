using EditorConnectionWindow.BaseSystem;
using UnityEngine;

public class CommandReceiver : MonoBehaviour
{

	public UnityEngine.UI.Text MessageText;
	public UnityEngine.UI.Text ServerText; 
	
	private IConnectionServer _server;
	private ICommandScheduler _scheduler;
	private UdpBroadcastCommand _broadcastCommand;
	
	// Use this for initialization
	void Start ()
	{
		var service = new ConnectionService();
		_scheduler = new CommandScheduler();
		_server = new TcpConnectionServer(service.GetLocalIPAddress(), 8081);
		_server.MessageReceived += UpdateMessageText;
		_server.StartServer();
		ServerText.text = string.Format("Server running on {0}", _server.Adress+":" + _server.Port.ToString());
		MessageText.text = "Not received";
		_broadcastCommand = new UdpBroadcastCommand(15000, _server.Adress+":" + _server.Port.ToString());
		_scheduler.AddCommand(_broadcastCommand);
	}
	
	private void UpdateMessageText(string message)
	{
		MessageText.text = message;
	}

	private void OnDestroy()
	{
		_server.StopServer();
		_scheduler.RemoveCommand(_broadcastCommand);
	}

	void Update()
	{
		_server.Tick();
		_scheduler.Tick();
	}
}
