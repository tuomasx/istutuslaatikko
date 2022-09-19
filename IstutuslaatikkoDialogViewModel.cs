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
            Thickness = 100;
            Width = 300;

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
        public double Thickness { get; set; }
        /// <summary>
        /// Leveys
        /// </summary>
        public double Width { get; set; }
        #endregion

        public double LengthX { get; set; }
        public double LengthY { get; set; }
    }

}
