using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital_System
{
    class DateCalculation
    {
        private DateTime In;
        private DateTime Out;

        //Constructor for class
        public DateCalculation(DateTime dateIn, DateTime dateout)
        {
            In = dateIn;
            Out = dateout;
        }

        //Calculates inpatient days
        public double days()
        {
           double diff = Convert.ToDouble((Out - In).Days);

           return diff;
        }
    }
}
