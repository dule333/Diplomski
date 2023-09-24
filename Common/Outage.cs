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
		private Point[] area;
		private DateTime outageStart;
		private DateTime outageEnd;
		private OutageType outageType;

		public Outage(Point[] area, DateTime outageStart, DateTime outageEnd, OutageType outageType)
		{
			this.area = area;
			this.outageStart = outageStart;
			this.outageEnd = outageEnd;
			this.outageType = outageType;
		}

		public Point[] Area { get => area; }
		public DateTime OutageStart { get => outageStart; }
		public DateTime OutageEnd { get => outageEnd; }
		public OutageType OutageType { get => outageType; }
	}
}
