using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using Microsoft.Kinect.VisualGestureBuilder;
using System.Diagnostics;
using System.Windows.Controls;

namespace Tesina
{
    public class gestureDetector
    {

        private readonly string gestureDatabase = @"Database\continuo.gbd";

        private readonly string abrir_inico = "abrir_inicio";

        private readonly string abrir_final = "abrir_final";

        private readonly string abrirProgress = "abrirProgress";

        private bool abrirIniciado = false;

        private bool abrirFinal = false;

        private float abrirProgreso = 0.0f;

        private TextBlock word;
        /// <summary> Gesture frame source which should be tied to a body tracking ID </summary>
        private VisualGestureBuilderFrameSource vgbFrameSource = null;

        /// <summary> Gesture frame reader which will handle gesture events coming from the sensor </summary>
        private VisualGestureBuilderFrameReader vgbFrameReader = null;

        public gestureDetector(KinectSensor kinectSensor, TextBlock palabra)
        {
            if (kinectSensor == null)
            {
                throw new ArgumentNullException("kinectSensor");
            }

            this.word = palabra;

            // create the vgb source. The associated body tracking ID will be set when a valid body frame arrives from the sensor.
            this.vgbFrameSource = new VisualGestureBuilderFrameSource(kinectSensor, 0);

            // open the reader for the vgb frames
            this.vgbFrameReader = this.vgbFrameSource.OpenReader();
            if (this.vgbFrameReader != null)
            {
                this.vgbFrameReader.IsPaused = true;
            }

            // load all gestures from the gesture database
            using (var database = new VisualGestureBuilderDatabase(this.gestureDatabase))
            {
                this.vgbFrameSource.AddGestures(database.AvailableGestures);
            }

            // disable the set of gestures which determine the 'keep straight' behavior, we will use hand state instead

        }

        public ulong TrackingId
        {
            get
            {
                return this.vgbFrameSource.TrackingId;
            }

            set
            {
                if (this.vgbFrameSource.TrackingId != value)
                {
                    this.vgbFrameSource.TrackingId = value;
                }
            }
        }

        public bool IsPaused
        {
            get
            {
                return this.vgbFrameReader.IsPaused;
            }

            set
            {
                if (this.vgbFrameReader.IsPaused != value)
                {
                    this.vgbFrameReader.IsPaused = value;
                }
            }
        }

        public void UpdateGestureData()
        {
            using (var frame = this.vgbFrameReader.CalculateAndAcquireLatestFrame())
            {
                
                if (frame != null)
                {
                    //this.word.Text = "AQUI ANDO";
                    // get all discrete and continuous gesture results that arrived with the latest frame
                    var discreteResults = frame.DiscreteGestureResults;
                    var continuousResults = frame.ContinuousGestureResults;

                    if (discreteResults != null)
                    {
                        foreach (var gesture in this.vgbFrameSource.Gestures)
                        {
                            
                            if (gesture.GestureType == GestureType.Discrete)
                            {
                                DiscreteGestureResult result = null;
                                discreteResults.TryGetValue(gesture, out result);

                                if (result != null)
                                {
                                    if (gesture.Name.Equals(abrir_inico))
                                    {
                                        if(result.Detected)
                                        {
                                            abrirIniciado = result.Detected;
                                        }
                                        
                                    }
                                    else if (gesture.Name.Equals(abrir_final))
                                    {
                                        if (result.Detected)
                                        {
                                            abrirFinal = result.Detected;
                                        }
                                    }
                                }
                            }

                            if (continuousResults != null)
                            {
                                
                                if (gesture.Name.Equals(this.abrirProgress) && gesture.GestureType == GestureType.Continuous)
                                {
                                    
                                    ContinuousGestureResult result = null;
                                    continuousResults.TryGetValue(gesture, out result);

                                    if (result != null)
                                    {
                                        abrirProgreso = result.Progress;
                                    }
                                }
                            }
                        }
                        if (abrirIniciado && abrirFinal && abrirProgreso > 0.8f)
                        {
                            this.word.Text = "DIJISTE ABRIR";
                        }
                        else
                        {
                            this.word.Text = "probando";
                        }
                    }
                }
            }
        }

    }
}
