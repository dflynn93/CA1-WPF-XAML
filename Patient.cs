using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA1_WPF_XAML
{
    public class Patient
    {       
        // Properties
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }

        public int Age { get { return CalculateAge(); } }

        public BloodType BloodType { get; set; }
        public Ward Ward { get; set; }


        //Constructor
        public Patient(string name, DateTime dateOfBirth, BloodType bloodType, Ward ward)
        {
            Name = name;
            DateOfBirth = dateOfBirth;
            BloodType = bloodType;
            Ward = ward;
        }

        //Methods

        private int CalculateAge()
        {
            int age = DateTime.Now.Year - DateOfBirth.Year;

            if (DateTime.Now.Month < DateOfBirth.Month || (DateTime.Now.Month == DateOfBirth.Month && DateTime.Now.Day < DateOfBirth.Day))
            {
                age--;
            }

            return age;
        }
        
        
        private int GetAge(DateTime birthDate)
        {
            DateTime now = DateTime.Now;
            int age = now.Year - birthDate.Year;
            if (now.Month < birthDate.Month || (now.Month == birthDate.Month && now.Day < birthDate.Day))
            {
                age--;
            }
            return age;
        }

        public int GetAge()
        {
            DateTime now = DateTime.Now;
            int age = now.Year - DateOfBirth.Year;
            if (now.Month < DateOfBirth.Month || (now.Month == DateOfBirth.Month && now.Day < DateOfBirth.Day))
            {
                age--;
            }
            return age;
        }

        public override string ToString()

        {
            return $"{Name} ({Age}), {BloodType}";
        }

        public enum BloodType
        {
            A,
            B,
            AB,
            O
        }

    }
}
