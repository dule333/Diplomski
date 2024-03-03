using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Common
{
	public class Outage
	{
		private int outageId;
		private Point[] area;
		private DateTime outageStart;
		private DateTime outageEnd;
		private OutageType outageType;

		public Outage(int outageId, Point[] area, DateTime outageStart, DateTime outageEnd, OutageType outageType)
		{
			this.outageId = outageId;
			this.area = area;
			this.outageStart = outageStart;
			this.outageEnd = outageEnd;
			this.outageType = outageType;
		}
		public int OutageId { get => outageId; }
		public Point[] Area { get => area; }
		public DateTime OutageStart { get => outageStart; }
		public DateTime OutageEnd { get => outageEnd; }
		public OutageType OutageType { get => outageType; }

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			bool first = true;
			sb.Append(this.OutageId);
			sb.Append("|");
			foreach (Point point in this.Area)
			{
				if (!first) { sb.Append(';'); }
				sb.Append(point.X);
				sb.Append(",");
				sb.Append(point.Y);
				first = false;
			}
			sb.Append("|");
			sb.Append(this.OutageStart.ToString());
			sb.Append("|");
			sb.Append(this.OutageEnd.ToString());
			sb.Append("|");
			sb.Append(this.outageType);
			return sb.ToString();
		}
		public static Outage Deserialize(string value)
		{
			string[] parts = value.Split('|');
			Outage outage = new Outage(
				int.Parse(parts[0]),
				GetPoints(parts[1]),
				DateTime.Parse(parts[2]),
				DateTime.Parse(parts[3]),
				(parts[4] == "0") ? OutageType.Electricity : (parts[4] == "1") ? OutageType.Water : OutageType.Traffic
				);
			return outage;
		}
		private static Point[] GetPoints(string value)
		{
			string[] strings = value.Split(';');
			List<Point> points = new List<Point>();
			foreach (string s in strings)
			{
				Point point = new Point(Double.Parse(s.Split(',')[0]), Double.Parse(s.Split(',')[1]));
				points.Add(point);
			}

			return points.ToArray();
		}
	}
}
