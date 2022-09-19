using Enterprixe.ValosUITools.Features;
using Epx.BIM;
using Epx.BIM.Models.Concrete;
using Epx.BIM.Models.Geometry;
using Epx.BIM.Models.Steel;
using Epx.BIM.Models.Timber;
using Epx.BIM.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BIMKurssi
{
    /// <summary>
    /// Esimerkki "pruju" harjoitustyöhön. Vaihda luokkien nimistä Esimerkki oman harjoitustyösi nimeksi
    /// </summary>
    public class IstutuslaatikkoPlugin : PluginTool, IModelViewFeature
    {
        #region ei muutoksia
        protected System.Windows.Media.Media3D.Point3D _startPoint;
        protected System.Windows.Media.Media3D.Point3D _endPoint;
        private int inputPointIndex;
        #endregion
        // luokan EsimerkkiDialogViewModel nimen voi vaihtaa, jätä loppu DialogViewModel
        internal IstutuslaatikkoDialogViewModel _viewModel;
        public IstutuslaatikkoPlugin()
        {
            // alusat haluamasi alkuarvot
            _startPoint = new Point3D();
            _endPoint = new Point3D(5000, 0, 0);
            #region ei muutoksia
            _modelViewNodes.Clear();
            inputPointIndex = 0;
            #endregion
            // muutitko luokan nimen ?
            _viewModel = new IstutuslaatikkoDialogViewModel();
        }
        #region ei muutoksia
        private List<BaseDataNode> _modelViewNodes = new List<BaseDataNode>();
        public IEnumerable<BaseDataNode> ModelViewNodes => _modelViewNodes;
        public override Type AcceptedMasterNodeType => typeof(Epx.BIM.Models.ModelBaseNode);
        public override bool SupportsEditMode => true;        
        #endregion
        // Menussa näkyvän nimi, on syytä muuttaa
        public override string NameForMenu { get { return "Istutuslaatikko"; } }
        /// <summary>
        /// Menun tooltip teksti, on syytä muuttaa
        /// </summary>
        public override object MenuToolTip
        {
            get
            {
                return "Luo Istutuslaatikon";
            }
        }
        #region todennäköisesti ei muutoksia, jos kysytään kaksi pistettä: alku- ja loppupiste
        /// <summary>
        /// Alku- ja loppupisteen kysely, tuskin tarvitsee muuttaa
        /// </summary>
        /// <param name="previousInput"></param>
        /// <returns></returns>
        public override PluginInput GetNextPostDialogInput(PluginInput previousInput)
        {
            if (previousInput != null)
            {
                if (previousInput is PluginKeyInput)
                {
                    if (IsInEditMode && (previousInput as PluginKeyInput).InputKey == System.Windows.Input.Key.Enter)
                    {
                        return null;
                    }
                }
                if (previousInput is PluginPointInput && previousInput.Index == 0)
                {
                    PluginInput pp = new PluginPointInput();
                    pp.Prompt = "Valitse loppupiste";
                    if (IsInEditMode) pp.Prompt += " (enter to skip)";
                    pp.Index = 1;
                    inputPointIndex = 1;
                    return pp;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                PluginInput pp = new PluginPointInput();
                pp.Prompt = "Valitse alkupiste";
                if (IsInEditMode) pp.Prompt += " (enter to skip)";
                inputPointIndex = 0;
                return pp;
            }
        }
        /// <summary>
        /// Pisteiden syötön tarkistus, tuskin tarvitsee muuttaa
        /// </summary>
        /// <param name="pluginInput"></param>
        /// <returns></returns>
        public override bool IsPostDialogInputValid(PluginInput pluginInput)
        {
            if (pluginInput is PluginPointInput pointIn)
            {
                if (pointIn.Index == 0)
                {
                    _startPoint = pointIn.Point;
                }
                else if (pointIn.Index == 1)
                {
                    _endPoint = pointIn.Point;
                }
                return true;
            }
            else if (pluginInput is PluginKeyInput)
            {
                if (IsInEditMode && (pluginInput as PluginKeyInput).InputKey == System.Windows.Input.Key.Enter) return true;
            }
            return false;
        }
        #endregion
        /// <summary>
        /// Pluginin attribuuttien alustus, muokkaus tilassa (edit) otetaan viewmodeliin arvot olemassa olevalta luokan instansilta.
        /// Muokka edit tilaan omat attribuuttisi
        /// </summary>
        /// <param name="initialPlugin"></param>
        public override void InitializePluginParameters(PluginTool initialPlugin)
        {
            Istutuslaatikko esimerkki = IsInEditMode ? PluginDataNode as Istutuslaatikko : null;
            inputPointIndex = 0;
            // luodaan dialogin tarvitsema view model luokka
            _viewModel = new IstutuslaatikkoDialogViewModel();
            // jos edit tila, niin asetetaan arvot tietomalli luokalta
            if (esimerkki != null)
            {
                _viewModel.Name = esimerkki.Name;
                _viewModel.Width = esimerkki.Width;
                _viewModel.Thickness = esimerkki.Thickness;
                _startPoint = esimerkki.Origin;
                _endPoint = _startPoint + esimerkki.XAxis;
                // pidetään pisteet vaakassa, kaltevuus parametrien kautta
                _endPoint.Z = _startPoint.Z;
            }

        }
        #region ei muutoksia
        /// <summary>
        /// Näytetään dialogi
        /// </summary>
        /// <param name="isDialogApplyEnabled"></param>
        /// <returns></returns>
        public override bool ShowDialog(bool isDialogApplyEnabled)
        {
            EsimerkkiPluginDialog dlg = new EsimerkkiPluginDialog();
            dlg.Owner = System.Windows.Application.Current.MainWindow;
            dlg.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            dlg.DataContext = _viewModel;
            _DialogResult = (bool) dlg.ShowDialog();
            if (!_DialogResult && _FeatureEngine != null) _FeatureEngine.PluginDialogClosed(_DialogResult);
            return _DialogResult;
        }
        private bool _DialogResult;
        bool IModelViewFeature.DialogResult { get { return _DialogResult; } set { _DialogResult = value; } }
        IEnumerable<BaseDataNode> IModelViewFeature.HiddenNodes => null;

        IEnumerable<ModelDimensionBase> IModelViewFeature.ModelViewDimensions => null;

        IEnumerable<ModelUIContentElement> IModelViewFeature.ModelViewOverlayContents => null;

        private IFeatureEngine _FeatureEngine;
        IFeatureEngine IModelViewFeature.FeatureEngine
        {
            get
            { return _FeatureEngine; }
            set
            { _FeatureEngine = value; }
        }


        void IModelViewFeature.CancelFeature()
        {
            _modelViewNodes.Clear();
            if (_FeatureEngine != null) _FeatureEngine.PluginUpdate3D(false);
        }

        void IModelViewFeature.MouseMoved(Point3D currentPoint)
        {
            Point3D startP = _startPoint;
            Point3D  endP = _endPoint;
            if (inputPointIndex == 0)
            {
                Vector3D vek = _endPoint - _startPoint;
                if (vek.Length < 1) return;
                _startPoint = currentPoint;
                _endPoint = _startPoint + vek;
            }
            else
            {
                if ((currentPoint - _startPoint).Length < 1) return;
                _endPoint = currentPoint;
            }
            _modelViewNodes.Clear();
            _modelViewNodes.AddRange(Excecute(true, out Action doDelegate, out Action undoDelegate, out List<BaseDataNode> updateNodes));
            if (_FeatureEngine != null) _FeatureEngine.PluginUpdate3D(false);
            _startPoint = startP;
            _endPoint = endP;
        }
        /// <summary>
        /// Kelvollisen isännän tarkistus hierarkiassa
        /// </summary>
        /// <param name="targetNode"></param>
        /// <returns></returns>
        public override bool IsValidTarget(BaseDataNode targetNode)
        {
            return targetNode != null;
        }
        /// <summary>
        /// Luodaan tai muokataan rakenne
        /// </summary>
        /// <param name="doDelegate"></param>
        /// <param name="undoDelegate"></param>
        /// <param name="update3DNodes"></param>
        /// <returns></returns>
        public override List<BaseDataNode> Excecute(out Action doDelegate, out Action undoDelegate, out List<BaseDataNode> update3DNodes)
        {
            return Excecute(false, out doDelegate, out undoDelegate, out update3DNodes);
        }
        /// <summary>
        /// Näytetään preview, luodaan tai muokataan rakenne
        /// </summary>
        /// <param name="previewMode"></param>
        /// <param name="doDelegate"></param>
        /// <param name="undoDelegate"></param>
        /// <param name="update3DNodes"></param>
        /// <returns></returns>
        private List<BaseDataNode> Excecute(bool previewMode, out Action doDelegate, out Action undoDelegate, out List<BaseDataNode> update3DNodes)
        {
            doDelegate = null;
            undoDelegate = null;
            update3DNodes = new List<BaseDataNode>();
            // luodaan uutta tai preview piirto
            if (!IsInEditMode || previewMode)
            {
                Istutuslaatikko rakenne = CreateOrUpdateModel(_viewModel);
                 if (previewMode) return new List<BaseDataNode>() { rakenne };
                doDelegate += delegate
                {
                    Target.AddChild(rakenne);
                };
                // undo:ta ei tarvitse implementoida
                undoDelegate += delegate
                {
 //                   Target.RemoveChild(rakenne);
                };
                PluginDataNode = rakenne;
            }
            // muokataan olemassa olevaa
            else
            {
                Istutuslaatikko rakenne = PluginDataNode as Istutuslaatikko;
                //Point3D oldOrigin = rakenne.Origin;
                //Vector3D oldX = rakenne.XAxis;
                //EsimerkkiDialogViewModel oldParameters = new EsimerkkiDialogViewModel()
                //{
                //    Name = _viewModel.Name,
                //    Thickness = _viewModel.Thickness,
                //    Width = _viewModel.Width
                //};
                //double oldWidth = rakenne.Width;
                //double oldThickness = rakenne.Thickness;
                //string oldName = rakenne.Name;
                doDelegate += delegate
                {
                    rakenne = CreateOrUpdateModel(_viewModel, rakenne);
                    Target.AddChild(rakenne);
                };
                // undo:ta ei tarvitse implementoida
                undoDelegate += delegate
                {
                    //_startPoint = oldOrigin;
                    //_endPoint = oldOrigin + oldX;
                    //rakenne = CreateOrUpdateModel(oldParameters, rakenne);
                };
            }
            return new List<BaseDataNode>(0);
        }
        #endregion
        /// <summary>
        /// Luodaan uusi rakenne tai muokataan olemassa olevaa. Tämä funktio on keskeinen kohta harjoitustyössä. Tässä funktiossa
        /// luodaan tai muokataan harjoitustyössä määritelty rakenne attribuuttien mukaan. Tässä on yksi esimerkki, joka
        /// luo yhden vaakaosan ja yhden pystyosan sekä tekee niihin boolean työstöjä (leikkaa osia pois). Kaikissa harjoitustöissä
        /// ei tarvita boolean työstöjä.
        /// </summary>
        /// <param name="parametrit">Dialog view model luokka, jossa käyttäjän dialogissa antamat arvot</param>
        /// <param name="oldEsimerkki">Create moodissa on null, muokkaus tilassa olemassa olevan luokan instanssi</param>
        /// <returns></returns>
        private Istutuslaatikko CreateOrUpdateModel(IstutuslaatikkoDialogViewModel parametrit, Istutuslaatikko oldEsimerkki=null)
        {
            Istutuslaatikko retVal;
            // onko kysessä uuden luonti vai olemassa olevan muokkaaminen
            if (oldEsimerkki == null)
            {
                // luodaan uusi
                retVal = new Istutuslaatikko();
            } else
            {
                // muokataan vanhaa
                retVal = oldEsimerkki;
                // poisteaan aliosta, luodaan aina uudet
                retVal.RemoveAllChildren(false);
            }
            // asetetaan pääluokan attribuutit talteen, jotta arvot säilyvät editointiin
            retVal.Name = parametrit.Name;
            retVal.Thickness = parametrit.Thickness;
            retVal.Width = parametrit.Width;
            retVal.Origin = _startPoint;
            Vector3D xVec = _endPoint - _startPoint;
            retVal.XAxis = xVec;
            // y-akseli suoraan ylös
            retVal.YAxis = new Vector3D(0, 0, 1);
            // luodaan aliosa, sijainti on isännän koordinaatistossa
            Epx.BIM.Models.Member3D mem = new Epx.BIM.Models.Member3D();
            mem.Name = "Vaakassa";
            mem.Origin = new Point3D(0, 0, 0);
            mem.XAxis = new Vector3D(xVec.Length, 0, 0);
            mem.YAxis = new Vector3D(0, 1, 0);
            mem.SizeY = parametrit.Width;
            mem.SizeZ = parametrit.Thickness;
             retVal.AddChild(mem);
           // luodaan boolean leikkaus osalle, aliosa memberille, sijaainti member koordinaatistossa
            // RectangularCuboid on suorakulmainen suuntaissärmiö
            RectangularCuboid subtract = new RectangularCuboid();
            subtract.Origin = new Point3D(0.5 * xVec.Length, 0.5 * parametrit.Width, -1000);
            subtract.XAxis = new Vector3D(1, 0, 0); // x-akselin suunta osan x-akselin suuntaan
            subtract.YAxis = new Vector3D(0, 1, 0); // y-akseli osan y-akselin suuntaan
            // z-akseli lasketaan automaattisesti ristitulolla ja on osan z-akselin suuntaan
            subtract.XSize = 500; // 500 leveä kolo
            subtract.YSize = 100;  // 100 "paksu" kolo, koska origo (keskipiste) osan yläreunassa, leikkaa vain puolet
            subtract.ZSize = 2000; // tämä mitta riittävä, jotta "puhkaisee" koko osan
            subtract.BooleanOperationType = ModelGeometry3D.BooleanOperationTypeEnum.SubtractionDynamic; // dynaaminen leikkaus
            mem.AddChild(subtract);
            // luodaan toinen kolo vinosti
            subtract = new RectangularCuboid();
            subtract.XAxis = new Vector3D(1, 1, 0); // x-akselin vinosti
            subtract.YAxis = Vector3D.CrossProduct(new Vector3D(0, 1, 1),subtract.XAxis); // y-akseli ristitulolla myös vinoon
            // z-akseli lasketaan automaattisesti ristitulolla ja on osan z-akselin suuntaan
            // asetetaan origo niin, että leikkaa osan yläreunaa
            Vector3D zaxis = subtract.ZAxis;
            zaxis.Normalize();
            subtract.Origin = new Point3D(0.2 * xVec.Length, 0.5 * parametrit.Width, 0) - zaxis * 1000;
            subtract.XSize = 200; // 200 leveä kolo
            subtract.YSize = 100;  // 100 "paksu" kolo, koska origo (keskipiste) osan yläreunassa, leikkaa vain puolet
            subtract.ZSize = 2000; // tämä mitta riittävä, jotta "puhkaisee" koko osan
            subtract.BooleanOperationType = ModelGeometry3D.BooleanOperationTypeEnum.SubtractionDynamic; // dynaaminen leikkaus
            mem.AddChild(subtract);
            // toinen aliosa
            mem = new Epx.BIM.Models.Member3D();
            mem.Name = "Pystyssä";
            mem.Origin = new Point3D(xVec.Length, 0, 0);
            mem.XAxis = new Vector3D(0, xVec.Length, 0);
            mem.YAxis = new Vector3D(1, 0, 0);
            mem.SizeY = parametrit.Width;
            mem.SizeZ = parametrit.Thickness;
            retVal.AddChild(mem);
            // leikataan osa vinosti poikki
            PlaneCut cut = new PlaneCut();
            cut.Position = new Point3D(0.8*xVec.Length, 0, 0);
            cut.PlaneNormal = new Vector3D(1, 1, 0);
            mem.AddChild(cut);
            // pyöreä pilari
            ConcreteColumn col = new ConcreteColumn();
            col.Round = true;
            col.Diameter = 400;
            col.Origin = new Point3D(0, 500, 0);
            col.XAxis = new Vector3D(0, 1, 0);
            col.YAxis = new Vector3D(1, 0, 0);
            col.Length = 3000;
            retVal.AddChild(col);
            return retVal;
        }
    }
}
