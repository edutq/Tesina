
namespace Tesina
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;
    using Microsoft.Kinect;
    using System.Diagnostics;

    /// <summary>
    /// Interaction logic for the MainWindow
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensor kinectSensor = null;

        private MultiSourceFrameReader frameReader = null;

        private Body[] bodies = null;

        private int activeBodyIndex = 0;

        private BodyFrameReader bodyFrameReader = null;

        private gestureDetector GD = null;

        public MainWindow()
        {
            this.InitializeComponent();

            this.kinectSensor = KinectSensor.GetDefault();

            this.kinectSensor.Open();

            this.bodyFrameReader = this.kinectSensor.BodyFrameSource.OpenReader();

            this.frameReader = this.kinectSensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Body);
            frameReader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;

            this.GD = new gestureDetector(kinectSensor, this.Word);

        }

        void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            // Get a reference to the multi-frame
            var reference = e.FrameReference.AcquireFrame();
            bool dataReceived = false;
            // Open color frame
            using (var frame = reference.ColorFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    Camera.Source = ToBitmap(frame);
                }
            }

            // Body
            using (var frame = reference.BodyFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    //this.Word.Text = "Veo el cuerpo";
                    using (var bodyFrame = this.bodyFrameReader.AcquireLatestFrame())
                    {
                        if (bodyFrame != null)
                        {
                            if (this.GD.IsPaused)
                            {
                                this.Word.Text = "NO HAY NADIE";
                            }
                            if (bodyFrame != null)
                            {
                                if (this.bodies == null)
                                {
                                    // creates an array of 6 bodies, which is the max number of bodies that Kinect can track simultaneously
                                    this.bodies = new Body[bodyFrame.BodyCount];
                                }

                                // The first time GetAndRefreshBodyData is called, Kinect will allocate each Body in the array.
                                // As long as those body objects are not disposed and not set to null in the array,
                                // those body objects will be re-used.
                                bodyFrame.GetAndRefreshBodyData(this.bodies);

                                if (!this.bodies[this.activeBodyIndex].IsTracked)
                                {
                                    // we lost tracking of the active body, so update to the first tracked body in the array
                                    int bodyIndex = this.GetActiveBodyIndex();

                                    if (bodyIndex > 0)
                                    {
                                        this.activeBodyIndex = bodyIndex;
                                    }
                                }

                                dataReceived = true;
                            }

                            if (dataReceived)
                            {
                                Body activeBody = this.bodies[this.activeBodyIndex];
  
                                // visualize the new gesture data
                                if (activeBody.TrackingId != this.GD.TrackingId)
                                {
                                    // if the tracking ID changed, update the detector with the new value
                                    this.GD.TrackingId = activeBody.TrackingId;
                                }

                                if (this.GD.TrackingId == 0)
                                {
                                    // the active body is not tracked, pause the detector and update the UI
                                    this.GD.IsPaused = true;
                                }
                                else
                                {
                                    // the active body is tracked, unpause the detector
                                    this.GD.IsPaused = false;

                                    // steering gestures are only valid when the active body's hand state is 'closed'
                                    // update the detector with the latest hand state


                                    // get the latest gesture frame from the sensor and updates the UI with the results
                                    this.GD.UpdateGestureData();
                                }
                            }
                        }
                    }
                        
                }
            }

        }

        private int GetActiveBodyIndex()
        {
            int activeBodyIndex = -1;
            int maxBodies = this.kinectSensor.BodyFrameSource.BodyCount;

            for (int i = 0; i < maxBodies; ++i)
            {
                // find the first tracked body and verify it has hands tracking enabled (by default, Kinect will only track handstate for 2 people)
                if (this.bodies[i].IsTracked && (this.bodies[i].HandRightState != HandState.NotTracked || this.bodies[i].HandLeftState != HandState.NotTracked))
                {
                    activeBodyIndex = i;
                    break;
                }
            }

            return activeBodyIndex;
        }

        private ImageSource ToBitmap(ColorFrame frame)
        {
            int width = frame.FrameDescription.Width;
            int height = frame.FrameDescription.Height;
            PixelFormat format = PixelFormats.Bgr32;

            byte[] pixels = new byte[width * height * ((PixelFormats.Bgr32.BitsPerPixel + 7) / 8)];

            if (frame.RawColorImageFormat == ColorImageFormat.Bgra)
            {
                frame.CopyRawFrameDataToArray(pixels);
            }
            else
            {
                frame.CopyConvertedFrameDataToArray(pixels, ColorImageFormat.Bgra);
            }
            int stride = width * format.BitsPerPixel / 8;

            return BitmapSource.Create(width, height, 96, 96, format, null, pixels, stride);
        }

    }
}
