using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Common;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Net;
using System.Xml.Linq;
using System.Net.Sockets;

namespace Diplomski
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		readonly string APIKey = "";
		readonly ScaleTransform scale = new ScaleTransform();
		PointCollection points = new PointCollection();
		OutageType outageType;
		UserPermissions currentUserPermissions = UserPermissions.User;
		TcpClient tcpClient;
		int userId = -1;
		bool forceClosed = true;

		Point topLeftPoint = new Point(45.278616, 19.794296);
		Point bottomRightPoint = new Point(45.232626, 19.895024);

		List<SolidColorBrush> colors = new List<SolidColorBrush>
		{
			new SolidColorBrush(Colors.Yellow),
			new SolidColorBrush(Colors.Blue),
			new SolidColorBrush(Colors.Red),
		};

		bool mapPointSet = false;
		Point currentPoint = new Point();
		string currentAddress = "tempAddress";
		Rectangle mapPin = new Rectangle 
		{
			Width = 33,
			Height = 49,
			Fill = new ImageBrush(new BitmapImage(new Uri("../../Images/mapPoint.png", UriKind.Relative)))
		};
		List<POI> pOIs = new List<POI>();
		List<Outage> outages = new List<Outage>();

		public MainWindow(UserPermissions userPermissions, TcpClient tcpClient, int userId)
		{
			InitializeComponent();
			canvas.LayoutTransform = scale;
			DataContext = this;
			pois.ItemsSource = pOIs;
			currentUserPermissions = userPermissions;
			HideUnnecessaryActions();
			this.tcpClient = tcpClient;
			this.userId = userId;
		}

		private void HideUnnecessaryActions()
		{
			if(currentUserPermissions == UserPermissions.User)
			{
				actions.Children.Remove(AddEOutage);
				actions.Children.Remove(AddWOutage);
				actions.Children.Remove(AddTOutage);
				dates.Children.Remove(OutageStartDateTime);
				dates.Children.Remove(OutageEndDateTime);
				dates.Children.Remove(OutageStart);
				dates.Children.Remove(OutageEnd);
			}

			if(currentUserPermissions > 0)
			{
				actions.Children.Remove(addPOI);
				actions.Children.Remove(addressButton);
				actions.Children.Remove(addressBox);
				actions.Children.Remove(checkPOIs);
				dates.Children.Remove(pointsOfInterest);
				dates.Children.Remove(pois);
			}

			if(currentUserPermissions == UserPermissions.AdminElectricity)
			{
				actions.Children.Remove(AddWOutage);
				actions.Children.Remove(AddTOutage);
			}

			if (currentUserPermissions == UserPermissions.AdminWater)
			{
				actions.Children.Remove(AddEOutage);
				actions.Children.Remove(AddTOutage);
			}

			if (currentUserPermissions == UserPermissions.AdminTraffic)
			{
				actions.Children.Remove(AddEOutage);
				actions.Children.Remove(AddWOutage);
			}
			return;
		}

		private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			scale.ScaleX = ((Slider)sender).Value;
			scale.ScaleY = ((Slider)sender).Value;
		}

		private void WriteOutage(Outage outage)
		{
			var stream = tcpClient.GetStream();

			string message = "O|" + outage.ToString();
			byte[] payload = Encoding.ASCII.GetBytes(message);
			stream.Write(payload, 0, payload.Length);

			MessageBox.Show("Created an outage.");
		}

		private void DrawPolygon()
		{
			if(!OutageStartDateTime.Value.HasValue || !OutageEndDateTime.Value.HasValue) 
			{
				MessageBox.Show("Please input start and end time of outage.");
				return;
			}

			Polygon polygon = new Polygon
			{
				Fill = colors[(int)outageType],
				Stroke = new SolidColorBrush(Colors.Black),
				StrokeThickness = 1,
				Points = new PointCollection(points),
				Opacity = 0.5,
				ToolTip = new ToolTip() { Content = "Starts from: " + OutageStartDateTime.Value.ToString() + "\nLasts until: " + OutageEndDateTime.Value.ToString() }
			};
			points.Clear();

			Outage outage = new Outage(-1, polygon.Points.ToArray(), OutageStartDateTime.Value.Value, OutageEndDateTime.Value.Value, outageType);

			outages.Add(outage);

			WriteOutage(outage);

			canvas.Children.Add(polygon);
		}

		private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (points.Count > 2)
			{
				DrawPolygon();
				return;
			}

			mapPointSet = true;
			currentPoint.X = e.GetPosition(canvas).X;
			currentPoint.Y = e.GetPosition(canvas).Y;
			currentAddress = "tempAddress";
			canvas.Children.Remove(mapPin);
		}

		private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			points.Add(new Point(e.GetPosition(canvas).X, e.GetPosition(canvas).Y));
		}

		private void ElectricityOutage(object sender, RoutedEventArgs e)
		{
			outageType = OutageType.Electricity;
		}
		private void WaterOutage(object sender, RoutedEventArgs e)
		{
			outageType = OutageType.Water;
		}
		private void TrafficOutage(object sender, RoutedEventArgs e)
		{
			outageType = OutageType.Traffic;
		}

		private void AddPOI(object sender, RoutedEventArgs e)
		{
			if (mapPointSet)
			{
				if(currentAddress == "tempAddress")
				{
					Point currentLocation = ConvertPointToGeo(currentPoint);
					string latlng = currentLocation.Y + "," + currentLocation.X;
					string requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/xml?key={1}&latlng={0}&sensor=false", Uri.EscapeDataString(latlng), APIKey);

					WebRequest request = WebRequest.Create(requestUri);
					WebResponse response = request.GetResponse();
					XDocument xdoc = XDocument.Load(response.GetResponseStream());

					XElement result = xdoc.Element("GeocodeResponse").Element("result");
					XElement address = result.Element("formatted_address");

					currentAddress = address.Value.Split(',')[0];
				}
				pOIs.Add(new POI(currentPoint, currentAddress));
				mapPointSet = false;
				pois.Items.Refresh();
				currentAddress = "tempAddress";
				canvas.Children.Remove(mapPin);
			}
		}

		private void AddressButton_Click(object sender, RoutedEventArgs e)
		{
			string address = addressBox.Text + ", Novi Sad";
			string requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/xml?key={1}&address={0}&sensor=false", Uri.EscapeDataString(address), APIKey);

			WebRequest request = WebRequest.Create(requestUri);
			WebResponse response = request.GetResponse();
			XDocument xdoc = XDocument.Load(response.GetResponseStream());

			XElement result = xdoc.Element("GeocodeResponse").Element("result");
			XElement locationElement = result.Element("geometry").Element("location");
			XElement lat = locationElement.Element("lat");
			XElement lng = locationElement.Element("lng");

			Point location = ConvertGeoToPoint((double)lat, (double)lng);

			MessageBox.Show("X:" + location.X + "\nY:" + location.Y);
			canvas.Children.Remove(mapPin);
			mapPointSet = true;
			currentAddress = address;
			currentPoint.X = location.X;
			currentPoint.Y = location.Y;
			mapPin.SetValue(Canvas.LeftProperty, currentPoint.X - 16);
			mapPin.SetValue(Canvas.TopProperty, currentPoint.Y - 49);
			canvas.Children.Add(mapPin);
		}

		private Point ConvertGeoToPoint(double lat, double lng)
		{ 
			Point point = new Point(lat, lng);
			return new Point(Math.Abs((point.Y - topLeftPoint.Y) / (bottomRightPoint.Y - topLeftPoint.Y) * canvas.Width), 
				Math.Abs((point.X - topLeftPoint.X) / (topLeftPoint.X - bottomRightPoint.X) * canvas.Height));
		}

		private Point ConvertPointToGeo(Point point)
		{
			return new Point(point.X / canvas.Width * Math.Abs(bottomRightPoint.Y - topLeftPoint.Y) + topLeftPoint.Y, topLeftPoint.X - point.Y / canvas.Height * Math.Abs(topLeftPoint.X - bottomRightPoint.X));
		}

		private bool IsInPolygon(Point[] poly, Point p)
		{
			Point p1, p2;
			bool inside = false;

			if (poly.Length < 3)
			{
				return inside;
			}

			var oldPoint = new Point(
				poly[poly.Length - 1].X, poly[poly.Length - 1].Y);

			for (int i = 0; i < poly.Length; i++)
			{
				var newPoint = new Point(poly[i].X, poly[i].Y);

				if (newPoint.X > oldPoint.X)
				{
					p1 = oldPoint;
					p2 = newPoint;
				}
				else
				{
					p1 = newPoint;
					p2 = oldPoint;
				}

				if ((newPoint.X < p.X) == (p.X <= oldPoint.X)
					&& (p.Y - (long)p1.Y) * (p2.X - p1.X)
					< (p2.Y - (long)p1.Y) * (p.X - p1.X))
				{
					inside = !inside;
				}

				oldPoint = newPoint;
			}

			return inside;
		}

		private void CheckPOIs(object sender, RoutedEventArgs e)
		{
			if (!InterestDateTime.Value.HasValue)
			{
				return;
			}
			foreach (var POI in pOIs)
			{
				foreach (var outage in outages)
				{
					if (InterestDateTime.Value.Value < outage.OutageEnd && InterestDateTime.Value.Value > outage.OutageStart && IsInPolygon(outage.Area, POI.Location))
					{
						new ToastContentBuilder()
							.AddText("Issue with POI")
							.AddText("There is an outage in " + POI.Address)
							.Show();
					}
				}
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
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
