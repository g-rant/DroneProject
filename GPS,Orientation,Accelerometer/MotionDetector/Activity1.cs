using Android.App;
using Android.Hardware;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MotionDetector
{
    [Activity(Label = "MotionDetector and GPS", MainLauncher = true, Icon = "@drawable/icon")]
    public class Activity1 : Activity, ISensorEventListener, ILocationListener
    {
        static readonly string TAG = "X:" + typeof(Activity1).Name;
        Location _currentLocation;
        LocationManager _locationManager;
        TextView _locationText;

        static readonly object _syncLock = new object();
        SensorManager _sensorManagerOrient;
        SensorManager _sensorManagerAccel;
         TextView _sensorTextView;

        private float x, y, z, pitch, roll, azimuth,longitude, latitude;
        string _locationProvider;

        //Activity1()
        //{
        //    x = 0f;
        //    y = 0f;
        //    z = 0f;
        //    pitch = 0f;
        //    roll = 0f;
        //    azimuth = 0f;
        //}

        public void OnLocationChanged(Location location)
        {
            _currentLocation = location;
            if (_currentLocation == null)
            {
                _locationText.Text = "Unable to determine your location. Try again in a short while.";
            }
            else
            {
                _locationText.Text = string.Format("{0:f6},{1:f6}", _currentLocation.Latitude, _currentLocation.Longitude);
                //    Address address = await ReverseGeocodeCurrentLocation();
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

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);

            _locationText = FindViewById<TextView>(Resource.Id.location_text);

            InitializeLocationManager();



            _sensorManagerOrient = (SensorManager)GetSystemService(SensorService);
            _sensorManagerAccel = (SensorManager)GetSystemService(SensorService);
            // _sensorTextView = FindViewById<TextView>(Resource.Id.accelerometer_text);
            _sensorTextView = FindViewById<TextView>(Resource.Id.accelerometer_text);
           
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
    }
}