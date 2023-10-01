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
	public partial class AdminPanel : Window
	{
		public AdminPanel()
		{
			InitializeComponent();
		}

		private void createButton_Click(object sender, RoutedEventArgs e)
		{
			if (Electricity.IsChecked.HasValue && Electricity.IsChecked.Value)
				MessageBox.Show("Electricity User Created");
			if (Traffic.IsChecked.HasValue && Traffic.IsChecked.Value)
				MessageBox.Show("Traffic User Created");
			if (Water.IsChecked.HasValue && Water.IsChecked.Value)
				MessageBox.Show("Water User Created");

		}

		private void logoutButton_Click(object sender, RoutedEventArgs e)
		{
			Login login = new Login();
			login.Show();
			Close();
		}
	}
}
