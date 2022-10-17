using Enterprixe.ValosUITools.Features;
using Epx.BIM;
using Epx.BIM.Models;
using Epx.BIM.Models.Concrete;
using Epx.BIM.Models.Geometry;
using Epx.BIM.Models.Steel;
using Epx.BIM.Models.Timber;
using Epx.BIM.Plugins;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Valos.Ifc.IfcClasses;

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
            Istutuslaatikko istutuslaatikko = IsInEditMode ? PluginDataNode as Istutuslaatikko : null;
            inputPointIndex = 0;
            // luodaan dialogin tarvitsema view model luokka
            _viewModel = new IstutuslaatikkoDialogViewModel();
            // jos edit tila, niin asetetaan arvot tietomalli luokalta
            if (istutuslaatikko != null)
            {
                _viewModel.Name = istutuslaatikko.Name;
                _viewModel.BoxXLength = istutuslaatikko.BoxXLength;
                _viewModel.BoxYLength = istutuslaatikko.BoxYLength;
                _startPoint = istutuslaatikko.Origin;
                _endPoint = _startPoint + istutuslaatikko.XAxis;
                _viewModel.TimberXLength = istutuslaatikko.TimberXLength;
                _viewModel.TimberYLength = istutuslaatikko.TimberYLength;
                _viewModel.TimberZLength = istutuslaatikko.TimberZLength;
                _viewModel.TimberStackAmount = istutuslaatikko.TimberStackAmount;
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
            IstutuslaatikkoPluginDialog dlg = new IstutuslaatikkoPluginDialog();
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
                    //_startPoint = oldOrigin;-
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
        private Istutuslaatikko CreateOrUpdateModel(IstutuslaatikkoDialogViewModel parametrit,Istutuslaatikko oldIstutuslaatikko = null)
        {
            Istutuslaatikko retVal;

            if (oldIstutuslaatikko == null)
            {
                retVal = new Istutuslaatikko();

            }
            else
            {
                retVal = oldIstutuslaatikko;
                retVal.RemoveAllChildren(false);
            }

            // attribuutit talteen
            retVal.Name = parametrit.Name;
            retVal.BoxXLength = parametrit.BoxXLength;
            retVal.BoxYLength = parametrit.BoxYLength;
            retVal.TimberXLength = parametrit.TimberXLength;
            retVal.TimberYLength = parametrit.TimberYLength;
            retVal.TimberZLength = parametrit.TimberZLength;
            retVal.TimberStackAmount = parametrit.TimberStackAmount;
            retVal.Origin = _startPoint;

            //normalisoidaan suuntausvektori
            Vector3D xVec = _endPoint - _startPoint;
            xVec.Normalize();

            retVal.XAxis = xVec;

            retVal.YAxis = new Vector3D(0, 1, 0);

            retVal.ZAxis = new Vector3D(0, 0, 1);

            //lasketaan x- & y-suunnassa olevien maarat
            int TimberXCons = (int)Math.Ceiling(retVal.BoxXLength / retVal.TimberXLength);
            int TimberYCons = (int)Math.Ceiling(retVal.BoxYLength / retVal.TimberXLength);

            Member3D[] TimberByX = new Member3D[TimberXCons];

            for (int i = 0; i<TimberByX.Length; i++) 
            {
                Member3D timb = new();

                timb.Name = "xSuuntainenPalkki" + i;
                timb.Origin = new Point3D(retVal.TimberXLength *i, -retVal.TimberYLength/2, 0);
                timb.XAxis = new Vector3D(1, 0, 0);
                timb.YAxis = new Vector3D(0, 1, 0);
                timb.ZAxis = new Vector3D(0, 0, 1);
                
                timb.SizeY = parametrit.TimberYLength;
                timb.SizeZ = parametrit.TimberZLength;

                if (i == TimberByX.Length-1)
                {
                    timb.Length = (retVal.BoxXLength - parametrit.TimberXLength * (i + 1));
                } 
                else
                {
                    timb.Length = parametrit.TimberXLength;
                }

                retVal.AddChild(timb);

                if (i == 0)
                {
                    PlaneCut cut = new PlaneCut();
                    cut.Position = new Point3D(0, retVal.TimberYLength/2, retVal.TimberZLength / 2);
                    //cut.Position = timb.Origin;
                    //cut.
                    cut.PlaneNormal = new Vector3D(-1, -1, 0);
                    timb.AddChild(cut);
                }

                if (i == TimberByX.Length - 1)
                {
                    PlaneCut cut = new PlaneCut();
                    cut.Position = new Point3D(timb.Length-timb.SizeY/2,retVal.TimberYLength/2,retVal.TimberZLength/2);
                    cut.PlaneNormal = new Vector3D(1, -1, 0);
                    timb.AddChild(cut);

                }

            }

            for (int i = 0; i < TimberYCons; i++)
            {

                Epx.BIM.Models.Member3D timb = new();

                timb.Name = "ySuuntainenPalkki" + i;
                timb.Origin = new Point3D(-retVal.TimberYLength/2, retVal.TimberXLength * i, 0);
                timb.XAxis = new Vector3D(0, 1, 0);
                timb.YAxis = new Vector3D(1, 0, 0);
                timb.ZAxis = new Vector3D(0, 0, 1);

                timb.SizeY = parametrit.TimberYLength;
                timb.SizeZ = parametrit.TimberZLength;

                if (i == TimberYCons-1)
                {
                    timb.Length = (retVal.BoxYLength - parametrit.TimberXLength * (i + 1));
                }
                else
                {
                    timb.Length = parametrit.TimberXLength;
                }

                retVal.AddChild(timb);

                if (i == 0)
                {
                    PlaneCut cut = new PlaneCut();
                    cut.Position = new Point3D(retVal.TimberYLength/2, 0, retVal.TimberZLength / 2);
                    cut.PlaneNormal = new Vector3D(-1, -1, 0);
                    timb.AddChild(cut);
                }

                if (i == TimberYCons - 1)
                {
                    PlaneCut cut = new PlaneCut();
                    cut.Position = new Point3D(retVal.TimberYLength / 2, timb.Length-timb.SizeY/2, retVal.TimberZLength / 2);
                    cut.PlaneNormal = new Vector3D(1, -1, 0);
                    timb.AddChild(cut);
                }
            }

            return retVal;
        }
    }
}
