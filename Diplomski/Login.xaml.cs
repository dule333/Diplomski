using Common;
using System;
using System.Collections.Generic;
using System.Linq;
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
	public partial class Login : Window
	{
		public Login()
		{
			InitializeComponent();
		}

		private void loginButton_Click(object sender, RoutedEventArgs e)
		{
			if (username.Text.Equals("admin") && password.Password.Equals("admin"))
			{
				MainWindow mainWindow = new MainWindow(UserPermissions.AdminUser);
				mainWindow.Show();
				Close();
				return;
			}
			if (username.Text.Equals("adminE") && password.Password.Equals("adminE"))
			{
				MainWindow mainWindow = new MainWindow(UserPermissions.AdminElectricity);
				mainWindow.Show();
				Close();
				return;
			}
			if (username.Text.Equals("userAdmin") && password.Password.Equals("userAdmin"))
			{
				AdminPanel panel = new AdminPanel();
				panel.Show();
				Close(); 
				return;
			}
			MessageBox.Show("Wrong username or password");
		}

		private void registerButton_Click(object sender, RoutedEventArgs e)
		{
			MainWindow mainWindow = new MainWindow(UserPermissions.User);
			mainWindow.Show();
			Close();
		}

		private void quitButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
