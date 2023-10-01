using Common;
using GoogleMapsApi.Entities.Elevation.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Diplomski
{
	/// <summary>
	/// Interaction logic for Login.xaml
	/// </summary>
	public partial class Login : Window
	{
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

		private string SendAndReceiveMessage(string method, string parameters)
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

		private void loginButton_Click(object sender, RoutedEventArgs e)
		{
			int userPerms = Int32.Parse(SendAndReceiveMessage("l", username.Text + "|" + password.Password));
			if (UserPermissions.AdminUser == (UserPermissions)userPerms)
			{
				AdminPanel panel = new AdminPanel(tcpClient);
				panel.Show();
				Close(); 
				return;
			}

			if(userPerms >= 0 && userPerms < 5)
			{
				MainWindow mainWindow = new MainWindow((UserPermissions)userPerms, tcpClient);
				mainWindow.Show();
				Close();
				return;
			}

			MessageBox.Show("Wrong username or password");
			return;

		}

		private void registerButton_Click(object sender, RoutedEventArgs e)
		{
			var stream = tcpClient.GetStream();

			string message = "r|" + username.Text + "|" + password.Password + "|" + 0;
			byte[] payload = Encoding.ASCII.GetBytes(message);
			stream.Write(payload, 0, payload.Length);

			MainWindow mainWindow = new MainWindow(UserPermissions.User, tcpClient);
			mainWindow.Show();
			Close();
		}

		private void quitButton_Click(object sender, RoutedEventArgs e)
		{
			tcpClient.Close();
			Close();
		}

		protected override void OnClosed(EventArgs e)
		{
			tcpClient.Close();
			base.OnClosed(e);
		}
	}
}
