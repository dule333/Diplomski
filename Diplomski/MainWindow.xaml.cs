using GoogleMapsApi.Entities.Common;
using GoogleMapsApi.Entities.Directions.Request;
using GoogleMapsApi.Entities.Directions.Response;
using GoogleMapsApi.Entities.Geocoding.Request;
using GoogleMapsApi.Entities.Geocoding.Response;
using GoogleMapsApi.StaticMaps.Entities;
using GoogleMapsApi.StaticMaps;
using GoogleMapsApi;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Common;
using System.Data;
using Microsoft.Toolkit.Uwp.Notifications;

namespace Diplomski
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		readonly ScaleTransform scale = new ScaleTransform();
		PointCollection points = new PointCollection();
		OutageType outageType;

		List<SolidColorBrush> colors = new List<SolidColorBrush>
		{
			new SolidColorBrush(Colors.Yellow),
			new SolidColorBrush(Colors.Blue),
			new SolidColorBrush(Colors.Red),
		};

		bool mapPointSet = false;
		Point currentPoint = new Point();
		Rectangle mapPin = new Rectangle 
		{
			Width = 33,
			Height = 49,
			Fill = new ImageBrush(new BitmapImage(new Uri("../../Images/mapPoint.png", UriKind.Relative)))
		};
		List<POI> pOIs = new List<POI>();
		List<Outage> outages = new List<Outage>();

		public MainWindow()
		{
			InitializeComponent();
			canvas.LayoutTransform = scale;
			DataContext = this;
			pois.ItemsSource = pOIs;
		}

		private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			scale.ScaleX = ((Slider)sender).Value;
			scale.ScaleY = ((Slider)sender).Value;
		}

		private void DrawPolygon()
		{
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

			outages.Add(new Outage(polygon.Points.ToArray(), OutageStartDateTime.Value.Value, OutageEndDateTime.Value.Value, outageType));
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
				pOIs.Add(new POI(currentPoint, "TempAddress"));
				mapPointSet = false;
				pois.Items.Refresh();
				canvas.Children.Remove(mapPin);
			}
		}

		private void AddressButton_Click(object sender, RoutedEventArgs e)
		{
			mapPointSet = true;
			currentPoint.X = 750;
			currentPoint.Y = 450;
			mapPin.SetValue(Canvas.LeftProperty, currentPoint.X - 16);
			mapPin.SetValue(Canvas.TopProperty, currentPoint.Y - 24);
			canvas.Children.Add(mapPin);
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
			foreach (var POI in pOIs)
			{
				foreach (var outage in outages)
				{
					if (IsInPolygon(outage.Area, POI.Location) && InterestDateTime.Value.Value < outage.OutageEnd && InterestDateTime.Value.Value > outage.OutageStart)
					{
						new ToastContentBuilder()
							.AddText("Issue with POI")
							.AddText("There is an outage in " + POI.Address)
							.Show();
					}
				}
			}
		}
	}

}
