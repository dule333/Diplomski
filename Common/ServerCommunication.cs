using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
	public static class ServerCommunication
	{
		public static string SendAndReceiveMessage(TcpClient tcpClient, string method, string parameters)
		{
			string result = null;
			var stream = tcpClient.GetStream();

			string message = method + "|" + parameters;
			byte[] payload = Encoding.ASCII.GetBytes(message);
			stream.Write(payload, 0, payload.Length);

			Byte[] bytes = new Byte[256];
			int i = stream.Read(bytes, 0, bytes.Length);
			result = Encoding.ASCII.GetString(bytes, 0, i);
			return result;
		}
	}
}
