using System;

namespace EditorConnectionWindow.BaseSystem
{
	public class ServerData
	{
		public string IpAddress { get; private set; }
		public int Port { get; private set; }
	
		public ServerData(string url)
		{
			var data = url.Split(':');
			IpAddress = data[0];
			Port = Convert.ToInt32(data[1]);
		}
		
		public static bool operator== (ServerData a, ServerData b)
		{
			if (ReferenceEquals(a,b))
			{
				return true;
			}
			if (ReferenceEquals(null,a))
			{
				return false;
			}
			if (ReferenceEquals(null, b))
			{
				return false;
			}
			bool result = true;
			result &= a.IpAddress == b.IpAddress;
			result &= a.Port == b.Port;
			return result;
		}
		
		public static bool operator!= (ServerData a, ServerData b)
		{
			return !(a == b);
		}
	
		public override string ToString()
		{
			return string.Format("{{IP:{0} Port:{1}}}", IpAddress, Port);
		}

		// this avoids warnings for the override
		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
