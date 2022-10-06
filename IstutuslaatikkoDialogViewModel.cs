using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIMKurssi
{
    public class IstutuslaatikkoDialogViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Konstruktori, aseta oletusarvot muuttujille
        /// </summary>
        public IstutuslaatikkoDialogViewModel()
        {
            Name = "Istutuslaatikko";
            BoxXLength = 1000;
            BoxYLength = 1000;
            TimberXLength = 100;
            TimberYLength = 100;
            TimberZLength = 100;
            TimberStackAmount = 1;

        }
        #region älä tee muutoksia
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
        #endregion
        #region tähän tarvittavat parametrit, ovat samat kuin EsimerkkiRakenteessa
        /// <summary>
        /// Rakenteen nimi
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Paksuus
        /// </summary>
        public double BoxXLength { get; set; }
        /// <summary>
        /// Leveys
        /// </summary>
        public double BoxYLength { get; set; }
        #endregion

        public double TimberXLength { get; set; }
        public double TimberYLength { get; set; }

        public double TimberZLength { get; set; }

        public int TimberStackAmount { get; set; }


    }

}
