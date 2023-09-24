using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Common
{
    public class POI
    {
        private Point location;
        private string address;

		public POI(Point location, string address)
		{
			this.location = location;
			this.address = address;
		}

		public string Address { get => address;}
		public Point Location { get => location;}

		public override string ToString()
		{
			return Address;
		}
	}
}
