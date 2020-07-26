using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.Globalization;

namespace AilianBT.Services
{
    public class AppCenterService
    {
        private const string APPCENTER_SECRET = "f978b679-ae61-4054-adc9-17abb67a41f4";

        public AppCenterService()
        {
            AppCenter.Start(APPCENTER_SECRET, typeof(Analytics), typeof(Crashes));
            var countryCode = RegionInfo.CurrentRegion.TwoLetterISORegionName;
            AppCenter.SetCountryCode(countryCode);
        }

        public async void TrackError(Exception e)
        {
            Crashes.TrackError(e);
        }
    }
}
