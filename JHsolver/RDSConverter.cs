﻿/// <summary>
/// rijksdriehoek converter.
/// <see cref="https://www.roelvanlisdonk.nl/?p=2950"/>
/// </summary>
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JHsolver
{
	public struct Location{
		public readonly double latitude;
		public readonly double longitude;
		public Location(double latitude, double longitude){
			this.latitude = latitude;
			this.longitude = longitude;
		}
	}

	public interface IRijksdriehoekComponent
	{
		Location ConvertToLatLong(double x, double y);
	}

	public class RijksdriehoekComponent : IRijksdriehoekComponent
	{
		public Location ConvertToLatLong(double x, double y)
		{
			Location result;

			// The city "Amsterfoort" is used as reference "Rijksdriehoek" coordinate.
			int referenceRdX = 155000;
			int referenceRdY = 463000;

			double dX = (double)(x - referenceRdX) * (double)(Math.Pow(10,-5));
			double dY = (double)(y - referenceRdY) * (double)(Math.Pow(10,-5));

			double sumN = 
				(3235.65389 * dY) + 
				(-32.58297 * Math.Pow(dX, 2)) + 
				(-0.2475 * Math.Pow(dY, 2)) + 
				(-0.84978 * Math.Pow(dX, 2) * dY) + 
				(-0.0655 * Math.Pow(dY, 3)) + 
				(-0.01709 * Math.Pow(dX, 2) * Math.Pow(dY, 2)) + 
				(-0.00738 * dX) + 
				(0.0053 * Math.Pow(dX, 4)) + 
				(-0.00039 * Math.Pow(dX, 2) * Math.Pow(dY, 3)) + 
				(0.00033 * Math.Pow(dX, 4) * dY) + 
				(-0.00012 * dX * dY);
			double sumE = 
				(5260.52916 * dX) + 
				(105.94684 * dX * dY) + 
				(2.45656 * dX * Math.Pow(dY, 2)) + 
				(-0.81885 * Math.Pow(dX, 3)) + 
				(0.05594 * dX * Math.Pow(dY, 3)) + 
				(-0.05607 * Math.Pow(dX, 3) * dY) + 
				(0.01199 * dY) + 
				(-0.00256 * Math.Pow(dX, 3) * Math.Pow(dY, 2)) + 
				(0.00128 * dX * Math.Pow(dY, 4)) + 
				(0.00022 * Math.Pow(dY, 2)) + 
				(-0.00022 * Math.Pow(dX, 2)) + 
				(0.00026 * Math.Pow(dX, 5));


			// The city "Amsterfoort" is used as reference "WGS84" coordinate.
			double referenceWgs84X = 52.15517;
			double referenceWgs84Y = 5.387206;

			double latitude = referenceWgs84X + (sumN / 3600);
			double longitude = referenceWgs84Y + (sumE / 3600);

			// Input
			// x = 122202
			// y = 487250
			//
			// Result
			// "52.372143838117, 4.90559760435224"
			result = new Location(latitude, longitude);

			return result;
		}
	}
}