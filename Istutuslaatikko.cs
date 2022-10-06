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
    public class Istutuslaatikko : Epx.BIM.Models.Model3DNode, Epx.BIM.Plugins.IPluginNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Esimerkkirakenne"/> class.
        /// </summary>
        public Istutuslaatikko() : this("EsimerkkiRakenne")
        {
        }
        #region älä tee muutoksia
        [Epx.BIM.NodeData]
        public string AssemblyName { get; set; }
        [Epx.BIM.NodeData]
        public string FullClassName { get; set; }
        #endregion
        /// <summary>
        /// Initializes a new instance of the <see cref="Istutuslaatikko"/> class.
        /// Tässä konstruktorissa alustetaan oletuarvot parametreille
        /// </summary>
        /// <param name="name">The name.</param>
        public Istutuslaatikko(string name) : base(name)
        {
            BoxXLength = 1000;
            BoxYLength = 1000;
            TimberXLength = 100;
            TimberYLength = 100;
            TimberZLength = 100;
            TimberStackAmount = 1;
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
        public double BoxXLength { get; set; }
        /// <summary>
        /// Laatikon x-suuntainen pituus
        /// </summary>
        [Epx.BIM.NodeData]
        public double BoxYLength { get; set; }
        //Laatikon y-suuntainen pituus
        [Epx.BIM.NodeData]

        public double TimberXLength { get; set; }
        //puutavaran x-suuntainen pituus
        [Epx.BIM.NodeData]
        public double TimberYLength { get; set; }
        [Epx.BIM.NodeData]
        public double TimberZLength { get; set; }
        [Epx.BIM.NodeData]
        public int TimberStackAmount { get; set; }
        #endregion
    }
}
