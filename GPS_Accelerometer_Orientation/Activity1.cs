using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Hardware;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Widget;

namespace com.xamarin.recipes.getlocation
{
    [Activity(Label = "GPS_Accelerometer_Orientation", MainLauncher = true, Icon = "@drawable/icon")]
    public class Activity1 : Activity, ILocationListener, ISensorEventListener
    {
        static readonly string TAG = "X:" + typeof(Activity1).Name;
        // TextView _addressText;
        Location _currentLocation;
        LocationManager _locationManager;
        string _locationProvider;
        TextView _locationText;

        static readonly object _syncLock = new object();
        SensorManager _sensorManagerOrient;
        SensorManager _sensorManagerAccel;
        TextView _sensorTextView;

        private float x, y, z, pitch, roll, azimuth, longitude, latitude;

        public /* async */ void OnLocationChanged(Location location)
        {
            _currentLocation = location;
            if (_currentLocation == null)
            {
                _locationText.Text = "Unable to determine your location. Try again in a short while.";
            }
            else
            {
                _locationText.Text = string.Format("latitude={0:f6}\nlongitude={1:f6}\n", _currentLocation.Latitude, _currentLocation.Longitude);
                //    Address address = await ReverseGeocodeCurrentLocation(); nah
                //   DisplayAddress(address);

            }
        }

        public void OnProviderDisabled(string provider) { }

        public void OnProviderEnabled(string provider) { }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            Log.Debug(TAG, "{0}, {1}", provider, status);
        }

        public void OnSensorChanged(SensorEvent e)
        {
            lock (_syncLock)
            {
                Sensor sensor = e.Sensor;
                if (sensor.Type.ToString().Contains("ccelerometer"))
                {
                    x = e.Values[0];
                    y = e.Values[1];
                    z = e.Values[2];
                }

                else if (sensor.Type.ToString().Contains("rientation"))
                {
                    azimuth = e.Values[0];
                    pitch = e.Values[1];
                    roll = e.Values[2];

                }
                _sensorTextView.Text = string.Format("x={0:f}\n y={1:f}\n z={2:f}\n Pitch={3:f}\n Roll={4:f}\n Azimuth={5:f}", x, y, z, pitch, roll, azimuth);
            }
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            //_addressText = FindViewById<TextView>(Resource.Id.address_text);
            _locationText = FindViewById<TextView>(Resource.Id.location_text);
            //   FindViewById<TextView>(Resource.Id.get_address_button).Click += AddressButton_OnClick;

            InitializeLocationManager();


            _sensorManagerOrient = (SensorManager)GetSystemService(SensorService);
            _sensorManagerAccel = (SensorManager)GetSystemService(SensorService);
            // _sensorTextView = FindViewById<TextView>(Resource.Id.accelerometer_text);
            _sensorTextView = FindViewById<TextView>(Resource.Id.accel_text);
        }

        void InitializeLocationManager()
        {
            _locationManager = (LocationManager)GetSystemService(LocationService);
            Criteria criteriaForLocationService = new Criteria
            {
                Accuracy = Accuracy.Fine
            };
            IList<string> acceptableLocationProviders = _locationManager.GetProviders(criteriaForLocationService, true);

            if (acceptableLocationProviders.Any())
            {
                _locationProvider = acceptableLocationProviders.First();
            }
            else
            {
                _locationProvider = string.Empty;
            }
            Log.Debug(TAG, "Using " + _locationProvider + ".");
        }

        protected override void OnResume()
        {
            base.OnResume();
            _locationManager.RequestLocationUpdates(_locationProvider, 0, 0, this);
            Log.Debug(TAG, "Listening for location updates using " + _locationProvider + ".");

            _sensorManagerAccel.RegisterListener(this,
                                             _sensorManagerAccel.GetDefaultSensor(SensorType.Accelerometer),
                                             SensorDelay.Ui);

            _sensorManagerOrient.RegisterListener(this,
                                           _sensorManagerOrient.GetDefaultSensor(SensorType.Orientation),
                                           SensorDelay.Ui);
        }

        protected override void OnPause()
        {
            base.OnPause();
            _locationManager.RemoveUpdates(this);
            Log.Debug(TAG, "No longer listening for location updates.");


            _sensorManagerOrient.UnregisterListener(this);
            _sensorManagerAccel.UnregisterListener(this);

        }


        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
        }
        //async void AddressButton_OnClick(object sender, EventArgs eventArgs)
        //{
        //    if (_currentLocation == null)
        //    {
        //        _addressText.Text = "Can't determine the current address. Try again in a few minutes.";
        //        return;
        //    }

        //    Address address = await ReverseGeocodeCurrentLocation();
        //    DisplayAddress(address);
        //}

        //async Task<Address> ReverseGeocodeCurrentLocation()
        //{
        //    Geocoder geocoder = new Geocoder(this);
        //    IList<Address> addressList =
        //        await geocoder.GetFromLocationAsync(_currentLocation.Latitude, _currentLocation.Longitude, 10);

        //    Address address = addressList.FirstOrDefault();
        //    return address;
        //}

        //void DisplayAddress(Address address)
        //{
        //    if (address != null)
        //    {
        //        StringBuilder deviceAddress = new StringBuilder();
        //        for (int i = 0; i < address.MaxAddressLineIndex; i++)
        //        {
        //            deviceAddress.AppendLine(address.GetAddressLine(i));
        //        }
        //        // Remove the last comma from the end of the address.
        //        _addressText.Text = deviceAddress.ToString();
        //    }
        //    else
        //    {
        //        _addressText.Text = "Unable to determine the address. Try again in a few minutes.";
        //    }
        //}
    }
}