using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA1_WPF_XAML
{
    public class Ward
    {
        private string text;
        private int value;

        public int Number { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        public List<Patient> Patients { get; set; } = new List<Patient>();
        public object NumberOfPatients { get; internal set; }

        public Ward(string name, int capacity, List<Patient> patients)
        {
            Name = name;
            Capacity = capacity;
            Patients = patients;
            foreach (Patient patient in Patients)
            {
                patient.Ward = this;
            }
        }

        public Ward(string text, int value)
        {
            this.text = text;
            this.value = value;
        }

        public List<Patient> GetSortedPatients()
        {
            List<Patient> sortedPatients = Patients.OrderBy(p => p.Name).ToList();
            return sortedPatients;
        }

        public override string ToString()
        {
            return Name + " (Limit: " + Capacity + ")";
        }

        public bool IsFull()
        {
            return Patients.Count >= Capacity;
        }

        public bool AddPatient(Patient patient)
        {
            if (!IsFull())
            {
                Patients.Add(patient);
                return true;
            }
            else
            {
                return false;
            }
        }

       
        public bool RemovePatient(Patient patient)
        {
            if (Patients.Contains(patient))
            {
                patient.Ward = null;
                Patients.Remove(patient);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
