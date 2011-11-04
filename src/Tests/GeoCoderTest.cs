﻿using System;
using System.Linq;
using System.Globalization;
using System.Threading;
using Xunit;
using Xunit.Extensions;

namespace GeoCoding.Tests
{
	public abstract class GeoCoderTest
	{
		readonly IGeoCoder geoCoder;

		public GeoCoderTest()
		{
			geoCoder = CreateGeoCoder();
		}

		protected abstract IGeoCoder CreateGeoCoder();

		[Fact]
		public void CanGeoCodeAddress()
		{
			Address[] addresses = geoCoder.GeoCode("1600 pennsylvania ave washington dc").ToArray();
			AssertWhiteHouseAddress(addresses[0]);
		}

		[Fact]
		public void CanGeoCodeNormalizedAddress()
		{
			Address[] addresses = geoCoder.GeoCode("1600 pennsylvania ave", "washington", "dc", null, null).ToArray();
			AssertWhiteHouseAddress(addresses[0]);
		}

		[Theory]
		[InlineData("en-US")]
		[InlineData("cs-CZ")]
		public void CanGeoCodeAddressUnderDifferentCultures(string cultureName)
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);

			Address[] addresses = geoCoder.GeoCode("24 sussex drive ottawa, ontario").ToArray();

			Address addr = addresses[0];

			Assert.True(addr.FormattedAddress.Contains("24 Sussex"));
			Assert.True(addr.FormattedAddress.Contains("Ottawa, ON"));
			Assert.True(addr.FormattedAddress.Contains("K1M"));
		}

		private void AssertWhiteHouseAddress(Address address)
		{
			Assert.True(address.FormattedAddress.Contains("The White House") || address.FormattedAddress.Contains("1600 Pennsylvania Ave NW"));
			Assert.True(address.FormattedAddress.Contains("Washington, DC"));

			//just hoping that each geocoder implementation gets it somewhere near the vicinity
			double lat = Math.Round(address.Coordinates.Latitude, 2);
			Assert.Equal(38.90, lat);

			double lng = Math.Round(address.Coordinates.Longitude, 2);
			Assert.Equal(-77.04, lng);
		}
	}
}