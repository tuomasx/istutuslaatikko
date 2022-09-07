using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows;

namespace BIMKurssi
{
    /// <summary>
    /// Tietomalliluokka joka tallentuu kantaan. Vaihda Esimerkki oman harjoitustyösi nimeksi
    /// </summary>
    public  class EsimerkkiRakenne : Epx.BIM.Models.Model3DNode, Epx.BIM.Plugins.IPluginNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Esimerkkirakenne"/> class.
        /// </summary>
        public EsimerkkiRakenne() : this("EsimerkkiRakenne")
        {
        }
        #region älä tee muutoksia
        [Epx.BIM.NodeData]
        public string AssemblyName { get; set; }
        [Epx.BIM.NodeData]
        public string FullClassName { get; set; }
        #endregion
        /// <summary>
        /// Initializes a new instance of the <see cref="EsimerkkiRakenne"/> class.
        /// Tässä konstruktorissa alustetaan oletuarvot parametreille
        /// </summary>
        /// <param name="name">The name.</param>
        public EsimerkkiRakenne(string name) : base(name)
        {
            Thickness = 50;
            Width = 150;
            Origin = new Point3D();
            XAxis = new Vector3D(1, 0, 0);
            YAxis = new Vector3D(0, 1, 0);
            // plugin system will set next two values automatically
            AssemblyName = "";
            FullClassName = "";
        }
        #region Rakenteen attribuutit, nämä muokataan kuhunkin harjoitustyöhön sopivaksi
        /// <summary>
        /// Paksuus
        /// </summary>
        [Epx.BIM.NodeData]
        public double Thickness { get;set; }
        /// <summary>
        /// Leveys
        /// </summary>
        [Epx.BIM.NodeData]
        public double Width { get; set; }
        #endregion
    }
}
