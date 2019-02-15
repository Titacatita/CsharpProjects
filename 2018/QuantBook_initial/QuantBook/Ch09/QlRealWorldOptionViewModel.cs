using System;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using QuantBook.Models.Options;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using QuantLib;

namespace QuantBook.Ch09
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class QlRealWorldOptionViewModel : Screen
    {
        
        [ImportingConstructor]
        public QlRealWorldOptionViewModel()
        {
            DisplayName = "09. QL: Real-World";
        }

        private DataTable optionTable;
        public DataTable OptionTable
        {
            get { return optionTable; }
            set
            {
                optionTable = value;
                NotifyOfPropertyChange(() => OptionTable);
            }
        }

        private IEnumerable<AmericanEngineType> engineType;
        public IEnumerable<AmericanEngineType> EngineType
        {
            get { return Enum.GetValues(typeof(AmericanEngineType)).Cast<AmericanEngineType>(); }
            set
            {
                engineType = value;
                NotifyOfPropertyChange(() => EngineType);
            }
        }

        private AmericanEngineType selectedEngineType;
        public AmericanEngineType SelectedEngineType
        {
            get { return selectedEngineType; }
            set
            {
                selectedEngineType = value;
                NotifyOfPropertyChange(() => SelectedEngineType);
            }
        }

        public void CalculatePrice()
        {
            // Pricing INTC Calls expiring on Feb 21, 2014

            string optionType = "C";
            Date evalDate = new Date(15, Month.November, 2013);
            Date maturity = new Date(21, Month.February, 2014);
            Date exDivDate = new Date(5, Month.February, 2014);

            double spot = 24.52;
            double[] strikes = new double[] { 22.0, 23.0, 24.0, 25.0, 26.0, 27.0, 28.0 };
            double dividend = 0.22;
            int dividendFrequency = 3; // Dividend paid quanterly
            double[] rates = new double[] { 0.001049, 0.0012925, 0.001675, 0.00207, 0.002381, 0.003514, 0.005841 };
            double[] vols = new double[]{0.23362, 0.21374, 0.20661, 0.20132, 0.19921, 0.19983, 0.20122};

            OptionTable = QuantLibHelper.AmericanOptionRealWorld(optionType, evalDate, maturity, spot, strikes, vols, rates, dividend, dividendFrequency, exDivDate, SelectedEngineType, 500);
        }
    }
}
