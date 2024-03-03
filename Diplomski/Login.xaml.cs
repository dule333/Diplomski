using Common;
using System;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace Diplomski
{
	/// <summary>
	/// Interaction logic for Login.xaml
	/// </summary>
	public partial class Login : Window
	{
		bool forceClosed = true;
		TcpClient tcpClient;
		public Login()
		{
			InitializeComponent();
			tcpClient = new TcpClient("127.0.0.1", 24567);
		}

		public Login(TcpClient tcpClient)
		{
			InitializeComponent();
			this.tcpClient = tcpClient;

		}

		private void loginButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				string[] results = ServerCommunication.SendAndReceiveMessage(tcpClient, "l", username.Text + "|" + password.Password).Split('|');
				UserPermissions userPermissions = (UserPermissions)Int32.Parse(results[0]);
				if (UserPermissions.AdminUser == userPermissions)
				{
					AdminPanel panel = new AdminPanel(tcpClient);
					panel.Show();
					forceClosed = false;
					Close();
					return;
				}

				if ((int)userPermissions >= 0 && (int)userPermissions < 5)
				{
					MainWindow mainWindow = new MainWindow(userPermissions, tcpClient, Int32.Parse(results[1]));
					mainWindow.Show();
					forceClosed = false;
					Close();
					return;
				}
				MessageBox.Show("Wrong username or password");
			}
			catch
			{
				MessageBox.Show("Wrong username or password.");
			}
		}

		private void registerButton_Click(object sender, RoutedEventArgs e)
		{
			var stream = tcpClient.GetStream();

			string message = "r|" + username.Text + "|" + password.Password + "|" + 0;
			byte[] payload = Encoding.ASCII.GetBytes(message);
			stream.Write(payload, 0, payload.Length);

			MessageBox.Show("Successfully registered. Please login to continue using the program.");
        }

		private void quitButton_Click(object sender, RoutedEventArgs e)
		{
			tcpClient.Close();
			Close();
		}

		protected override void OnClosed(EventArgs e)
		{
			if(forceClosed)
				tcpClient.Close();
			base.OnClosed(e);
		}
	}
}
