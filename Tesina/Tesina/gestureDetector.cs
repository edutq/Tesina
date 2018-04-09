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

        private readonly string gestureDatabase = @"Database\completo.gbd";

        private const string abrir_inico = "abrir_inicio";

        private const string abrir_final = "abrir_final";

        private const string abrirProgress = "abrirProgress";

        private const string aparecer_inicio = "aparecer_incio";

        private const string aparecer_final = "aparecer_final";

        private const string aparecerProgress = "aparecerProgress";
    
        private const string borrego_inicio = "borrego_inicio";

        private const string borrego_final = "borrego_final";

        private const string borregoProgress = "borregoProgress";

        private const string cuadrado_inicio = "cuadrado_inicio";

        private const string cuadrado_mitad = "cuadrado_mitad";

        private const string cuadrado_final = "cuadrado_final";

        private const string cuadradoProgress = "cuadradoProgress";

        private const string harina_inicio = "harina_inicio";

        private const string harina_final = "harina_final";

        private const string harinaProgress = "harinaProgress";

        private const string lunes_inicio = "lunes_inicio";

        private const string lunesProgress = "lunesProgress";

        private const string miercoles_inicio = "miercoles";

        private const string miercolesProgress = "miercolesProgress";

        private const string rapido_inicio = "rapido_inicio";

        private const string rapido_final = "rapido_final";

        private const string rapidoProgress = "rapidoProgress";

        private const string taco_inicio = "taco_inicio";

        private const string taco_final = "taco_final";

        private const string tacoProgress = "tacoProgress";

        private const string yo = "yo";

        private bool abrirIniciado = false;

        private bool abrirFinal = false;

        private float abrirProgreso = 0.0f;

        private bool boolAparecerInicio = false;

        private bool boolAparecerFinal = false;

        private float progresoAparecer = 0.0f;

        private bool boolBorregoInicio = false;

        private bool boolBorregoFinal = false;

        private float progresoBorrego = 0.0f;

        private bool boolCuadradoInicio = false;

        private bool boolCuadradoMitad = false;

        private bool boolCuadradoFinal = false;

        private float progresoCuadrado = 0.0f;

        private bool boolHarinaInicio = false;

        private bool boolHarinaFinal = false;

        private float progresoHarina = 0.0f;

        private bool boolLunes = false;

        private float progresoLunes = 0.0f;

        private bool boolMiercoles = false;

        private float progresoMiercoles = 0.0f;

        private bool boolRapidoInicio = false;

        private bool boolRapidoFinal = false;

        private float progresoRapido = 0.0f;

        private bool boolTacoInicio = false;

        private bool boolTacoFinal = false;

        private float progresoTaco = 0.0f;

        private bool boolYo = false;

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

        private void AllFalse()
        {

            this.abrirIniciado = false;
            this.abrirFinal = false;
            this.boolAparecerFinal = false;
            this.boolAparecerInicio = false;
            this.boolBorregoFinal = false;
            this.boolBorregoInicio = false;
            this.boolCuadradoInicio = false;
            this.boolCuadradoFinal = false;
            this.boolCuadradoMitad = false;
            this.boolHarinaFinal = false;
            this.boolHarinaInicio = false;
            this.boolLunes = false;
            this.boolMiercoles = false;
            this.boolRapidoInicio = false;
            this.boolRapidoFinal = false;
            this.boolTacoFinal = false;
            this.boolTacoInicio = false;
            this.boolYo = false;
            this.progresoAparecer = -1.0f;
            this.abrirProgreso = -1.0f;
            this.progresoBorrego = -1.0f;
            this.progresoCuadrado = -1.0f;
            this.progresoHarina = -1.0f;
            this.progresoLunes = -1.0f;
            this.progresoMiercoles = -1.0f;
            this.progresoRapido = -1.0f;
            this.progresoTaco = -1.0f;

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
                                    switch (gesture.Name)
                                    {
                                        case "relajado":
                                            if (result.Detected)
                                            {
                                                if (result.Confidence > 0.85)
                                                {
                                                    AllFalse();
                                                    continue;
                                                }
                                            }
                                            break;
                                        case abrir_inico:
                                            if (result.Detected)
                                            {
                                                
                                                abrirIniciado = result.Detected;
                                                
                                            }
                                            break;
                                        case abrir_final:
                                            if (result.Detected)
                                            {
                                                
                                                abrirFinal = result.Detected;
                                                
                                            }
                                            break;
                                        case aparecer_inicio:
                                            if (result.Detected)
                                            {
                                                
                                                boolAparecerInicio = result.Detected;
                                                
                                            }
                                            break;
                                        case aparecer_final:
                                            if (result.Detected)
                                            {
                                                
                                                boolAparecerFinal = result.Detected;
                                                
                                            }
                                            break;
                                        case borrego_inicio:
                                            if (result.Detected)
                                            {

                                                boolBorregoInicio = result.Detected;

                                            }
                                            break;
                                        case borrego_final:
                                            if (result.Detected)
                                            {

                                                boolBorregoFinal = result.Detected;

                                            }
                                            break;
                                        case cuadrado_inicio:
                                            if (result.Detected)
                                            {

                                                boolCuadradoInicio = result.Detected;

                                            }
                                            break;
                                        case cuadrado_mitad:
                                            if (result.Detected)
                                            {

                                                boolCuadradoMitad = result.Detected;

                                            }
                                            break;
                                        case cuadrado_final:
                                            if (result.Detected)
                                            {

                                                boolCuadradoFinal = result.Detected;

                                            }
                                            break;
                                        case harina_inicio:
                                            if (result.Detected)
                                            {

                                                boolHarinaInicio = result.Detected;

                                            }
                                            break;
                                        case harina_final:
                                            if (result.Detected)
                                            {

                                                boolHarinaFinal = result.Detected;

                                            }
                                            break;
                                        case lunes_inicio:
                                            if (result.Detected)
                                            {

                                                boolLunes = result.Detected;

                                            }
                                            break;
                                        case miercoles_inicio:
                                            if (result.Detected)
                                            {

                                                boolMiercoles = result.Detected;

                                            }
                                            break;
                                        case rapido_inicio:
                                            if (result.Detected)
                                            {

                                                boolRapidoInicio = result.Detected;

                                            }
                                            break;
                                        case rapido_final:
                                            if (result.Detected)
                                            {

                                                boolRapidoFinal = result.Detected;

                                            }
                                            break;
                                        case taco_inicio:
                                            if (result.Detected)
                                            {

                                                boolTacoInicio = result.Detected;

                                            }
                                            break;
                                        case taco_final:
                                            if (result.Detected)
                                            {

                                                boolTacoFinal = result.Detected;

                                            }
                                            break;
                                        case yo:
                                            if (result.Detected)
                                            {
                                                
                                                boolYo = result.Detected;
                                                
                                            }
                                            break;
                                    }
                                    
                                    
                       
                                }
                            }

                            if (continuousResults != null)
                            {
                                
                                if (gesture.GestureType == GestureType.Continuous)
                                {
                                    
                                    ContinuousGestureResult result = null;
                                    continuousResults.TryGetValue(gesture, out result);

                                    if (result != null)
                                    {
                                        switch (gesture.Name)
                                        {
                                            case abrirProgress:
                                                abrirProgreso = result.Progress;
                                                break;
                                            case aparecerProgress:
                                                progresoAparecer = result.Progress;
                                                break;
                                            case borregoProgress:
                                                progresoBorrego = result.Progress;
                                                break;
                                            case cuadradoProgress:
                                                progresoCuadrado = result.Progress;
                                                break;
                                            case harinaProgress:
                                                progresoHarina = result.Progress;
                                                break;
                                            case lunesProgress:
                                                progresoLunes = result.Progress;
                                                break;
                                            case miercolesProgress:
                                                progresoMiercoles = result.Progress;
                                                break;
                                            case rapidoProgress:
                                                progresoRapido = result.Progress;
                                                break;
                                            case tacoProgress:
                                                progresoTaco = result.Progress;
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                        if (abrirIniciado && abrirFinal && abrirProgreso > 0.8f)
                        {
                            this.word.Text = "ABRIR";
                        }
                        else if (boolAparecerInicio && boolAparecerFinal && progresoAparecer > 0.8f)
                        {
                            this.word.Text = "APARECER";
                        }

                        else if (boolBorregoInicio && boolBorregoFinal && progresoBorrego > 0.8f)
                        {
                            this.word.Text = "BORREGO";
                        }
                        else if (boolCuadradoInicio && boolCuadradoMitad && boolCuadradoFinal && progresoCuadrado > 0.8f)
                        {
                            this.word.Text = "CUADRADO";
                        }
                        else if (boolHarinaInicio && boolHarinaFinal && progresoHarina > 0.8f)
                        {
                            this.word.Text = "HARINA";
                        }
                        else if (boolLunes && progresoLunes > 0.8f)
                        {
                            this.word.Text = "LUNES";
                        }
                        else if (boolMiercoles && progresoMiercoles > 0.8f)
                        {
                            this.word.Text = "MIERCOLES";
                        }
                        else if (boolRapidoInicio && boolRapidoFinal && progresoRapido > 0.8f)
                        {
                            this.word.Text = "RAPIDO";
                        }
                        else if (boolTacoInicio && boolTacoFinal && progresoTaco > 0.8f)
                        {
                            this.word.Text = "TACO";
                        }
                        else if (boolYo)
                        {
                            this.word.Text = "YO";
                        }
                        else
                        {
                            this.word.Text = "ESPERANDO";
                        }
                    }
                }
            }
        }

    }
}
