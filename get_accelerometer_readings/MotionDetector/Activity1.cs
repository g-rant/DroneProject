using Android.App;
using Android.Hardware;
using Android.OS;
using Android.Widget;

namespace MotionDetector
{
    [Activity(Label = "MotionDetector", MainLauncher = true, Icon = "@drawable/icon")]
    public class Activity1 : Activity, ISensorEventListener
    {
        static readonly object _syncLock = new object();
        SensorManager _sensorManagerOrient;
        SensorManager _sensorManagerAccel;
        TextView _sensorTextView;

        private float x, y, z, pitch, roll, azimuth;

        //Activity1()
        //{
        //    x = 0f;
        //    y = 0f;
        //    z = 0f;
        //    pitch = 0f;
        //    roll = 0f;
        //    azimuth = 0f;
        //}

        public void OnAccuracyChanged(Sensor sensor, SensorStatus accuracy)
        {
            // We don't want to do anything here. 
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
            _sensorManagerOrient = (SensorManager)GetSystemService(SensorService);
            _sensorManagerAccel = (SensorManager)GetSystemService(SensorService);
            // _sensorTextView = FindViewById<TextView>(Resource.Id.accelerometer_text);
            _sensorTextView = FindViewById<TextView>(Resource.Id.accelerometer_text);
        }

        protected override void OnResume()
        {
            base.OnResume();
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
            _sensorManagerOrient.UnregisterListener(this);
            _sensorManagerAccel.UnregisterListener(this);
        }
    }
}