using EditorConnectionWindow.BaseSystem;
using UnityEngine;

public class ExampleConnectionPresenter : MonoBehaviour
{
	public UnityEngine.UI.Text MessageText;
	public UnityEngine.UI.Text ServerText;

	[SerializeField] private int ServerPort = 8081;
	[SerializeField] private int BroadcastPort = 15000;
	
	private EditorConnectionServer _server;

	// Use this for initialization
	void Start ()
	{
		Install();
		_server.DataReceived += UpdateDataText;
		_server.StartServer();
		MessageText.text = "Not received";
	}

	private void Install()
	{
		var service = new NetworkUtilities();
		var scheduler = new CommandScheduler();
		var serverConnection = new TcpConnectionServer(service.GetLocalIPAddress(), ServerPort);
		_server = new EditorConnectionServer(serverConnection, scheduler, BroadcastPort);
		ServerText.text = string.Format("Server running on {0}", serverConnection.Adress+":" + serverConnection.Port.ToString());
	}

	private void UpdateDataText(string message)
	{
		MessageText.text = message;
	}

	private void OnDestroy()
	{
		_server.Dispose();
	}

	void Update()
	{
		_server.Tick();
	}
}
