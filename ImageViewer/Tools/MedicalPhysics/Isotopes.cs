using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClearCanvas.ImageViewer.Tools.MedicalPhysics
{
    public class Isotope
    {
        #region privateFields
        private string _elementName;
        private string _elementAbbreviation;
        private TimeSpan _halfLife;
        private bool _metastable;
        private int _atomicMass;
        #endregion

        public Isotope(string name, string elementAbbreviation, TimeSpan halfLife, bool metastable, int atomicMass)
        {
            _elementName = name;
            _elementAbbreviation = elementAbbreviation;
            _halfLife = halfLife;
            _metastable = metastable;
            _atomicMass = atomicMass;
        }

        public string ElementName
        {
            get { return _elementName; }
        }

        public string ElementAbbrevitation
        {
            get { return _elementAbbreviation; }
        }

        public TimeSpan HalfLife
        {
            get { return _halfLife; }
        }

        public bool Metastable
        {
            get { return _metastable; }
        }

        public int AtomicMass
        {
            get { return _atomicMass; }
        }

        public string FullName
        {
            get
            {
                if (Metastable)
                {
                    return _elementName + "-" + AtomicMass.ToString() + "m";
                }
                else
                {
                    return _elementName + "-" + AtomicMass.ToString("N0");
                }
            }
        }

        public double GetActivity(Double referenceActivity, DateTime referenceDate, DateTime targetDate)
        {
            var timeDelay = targetDate - referenceDate;

            double targetActivity = referenceActivity * Math.Exp((Math.Log(2) / (HalfLife.TotalHours)) * -1 * timeDelay.TotalHours);
            return targetActivity;
        }

        public double GetCurrentActivity(Double referenceActivity, DateTime referenceDate)
        {
            return GetActivity(referenceActivity, referenceDate, DateTime.Now);
        }

        #region staticMembers
        private static List<Isotope> _isotopes;

        public static IEnumerable<Isotope> Isotopes
        {
            get
            {
                if (_isotopes is null)
                    buildIsotopes();
                return _isotopes;
            }
        }

        private static void buildIsotopes()
        {
            _isotopes = new List<Isotope>();

            _isotopes.Add(new Isotope("Cobalt", "Co", new TimeSpan(271, 17, 45, 0), false, 57));
            _isotopes.Add(new Isotope("Caesium", "Cs", new TimeSpan(10979,4,48,0), false, 137));
            _isotopes.Add(new Isotope("Technetium", "Tc", new TimeSpan(6, 0, 0), true, 99));
            
        }
        #endregion
    }
}
