using Common;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Diplomski
{
	/// <summary>
	/// Interaction logic for Login.xaml
	/// </summary>
	public partial class AdminPanel : Window
	{
		TcpClient tcpClient;
		bool forceClosed = true;
		public AdminPanel(TcpClient tcpClient)
		{
			InitializeComponent();
			this.tcpClient = tcpClient;
		}

		private void SendMessage(string method, string parameters)
		{
			var stream = tcpClient.GetStream();

			string message = method + "|" + parameters;
			byte[] payload = Encoding.ASCII.GetBytes(message);
			stream.Write(payload, 0, payload.Length);
		}

		private void createButton_Click(object sender, RoutedEventArgs e)
		{
			UserPermissions? userPermissions = null;
			if (Electricity.IsChecked.HasValue && Electricity.IsChecked.Value)
				userPermissions = UserPermissions.AdminElectricity;
			if (Traffic.IsChecked.HasValue && Traffic.IsChecked.Value)
				userPermissions = UserPermissions.AdminTraffic;
			if (Water.IsChecked.HasValue && Water.IsChecked.Value)
				userPermissions = UserPermissions.AdminWater;
			if (userPermissions.HasValue)
				SendMessage("r", username.Text + "|" + password.Password + "|" + (int)userPermissions);
		}

		private void logoutButton_Click(object sender, RoutedEventArgs e)
		{
			Login login = new Login(tcpClient);
			login.Show();
			forceClosed = false;
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
