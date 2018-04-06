
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

    /// <summary>
    /// Interaction logic for the MainWindow
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensor kinectSensor = null;

        private MultiSourceFrameReader frameReader = null;


        public MainWindow()
        {
            this.InitializeComponent();

            this.kinectSensor = KinectSensor.GetDefault();

            this.kinectSensor.Open();

            this.frameReader = this.kinectSensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Body);
            frameReader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;


        }

        void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            // Get a reference to the multi-frame
            var reference = e.FrameReference.AcquireFrame();

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
                    
                }
            }

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
