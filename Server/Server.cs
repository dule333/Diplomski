using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;
using System.Data.SqlClient;

namespace Server
{
	internal class Server
	{
		private static SqlConnection connection = null;
		private static SqlConnection ConnectToDatabase()
		{
			return new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\minec\\source\\repos\\Diplomski\\Server\\Database.mdf;Integrated Security=True");
		}

		private static void Register(string username, string password, int userPermissions)
		{
			if(username != null && password != null)
			{
				try
				{
					String commandText = "INSERT INTO [dbo].Users (Username, Password, UserType)" +
						"VALUES ('" + username + "', '" + password + "', " + userPermissions + ")";
					SqlCommand command = new SqlCommand(commandText, connection);

					connection.Open();
					command.ExecuteNonQuery();
					connection.Close();
				}
				catch (Exception e) 
				{
                    Console.WriteLine(e.Message);
					return;
                }
				Console.WriteLine("Registered user " +  username + ":" +  password);

			}
		}

		private static string Login(string username, string password)
		{
			string result = "";
			if (username != null && password != null)
			{
				try
				{
					String commandText = "SELECT UserType,Id FROM Users WHERE Username = '" + username + "' AND Password = '" + password + "'";
					SqlCommand command = new SqlCommand(commandText, connection);

					connection.Open();
					using(var reader = command.ExecuteReader())
					{
						reader.Read();
						result = reader[0] + "|" + reader[1];
					}
					connection.Close();
					
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
				}
			}

			return result;
		}

		private static void CreateOutage(string points, string startTime, string endTime, string type)
		{
			if(points != null && startTime != null && type != null)
			{
				try
				{
					Console.WriteLine("Before insert");
					String commandText = "INSERT INTO [dbo].Outages (OutagePoints, OutageStartTime, OutageEndTime, OutageType)" +
						"VALUES ('" + points + "', '" + startTime + "', '" + endTime + "', '" + type + "')";
					SqlCommand command = new SqlCommand(commandText, connection);

					connection.Open();
					command.ExecuteNonQuery();
					connection.Close();
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
					return;
				}
				Console.WriteLine("Created outage " + points + ":" + startTime + ":" + endTime + ":" + type);
			}
		}


		static void Main(string[] args)
		{
			connection = ConnectToDatabase();
			Action<object> action = (object obj) =>
			{
				TcpClient client = (TcpClient)obj;
				Console.WriteLine("Connected!");

				String data = null;
				Byte[] bytes = new Byte[256];

				NetworkStream stream = client.GetStream();

				int i;

				while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
				{
					data = Encoding.ASCII.GetString(bytes, 0, i);
					Console.WriteLine("Received: {0}", data);

					string[] input = data.Split('|');

					switch (input[0])
					{
						case "r":
							Register(input[1], input[2], Int32.Parse(input[3]));
							break;
						case "l":
							bytes = Encoding.ASCII.GetBytes(Login(input[1], input[2]).ToString());
							stream.Write(bytes, 0, bytes.Length);
							break;
						case "o": //Read outages

							break;
						case "O": //Write outage
							CreateOutage(input[2], input[3], input[4], input[5]);
							break;
						default:
							break;
					}

					data = null;
					bytes = new Byte[2048];

				}
                Console.WriteLine("Connection closed.");
                client.Dispose();
			};

			TcpListener server = null;
			try
			{
				Int32 port = 24567;
				IPAddress localAddr = IPAddress.Parse("127.0.0.1");
				server = new TcpListener(localAddr, port);
				server.Start();

				while (true)
				{
					Console.WriteLine("Waiting for a connection... ");
					TcpClient client = server.AcceptTcpClient();

					Task task = new Task(action, client);
					task.Start();
				}
			}
			catch (SocketException e)
			{
				Console.WriteLine("SocketException: {0}", e);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
			finally
			{
				server.Stop();
			}
		}
	}
}
