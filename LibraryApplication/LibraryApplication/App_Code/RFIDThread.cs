using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LibraryApplication.SensorStuff;

namespace LibraryApplication.App_Code
{
    public class RFIDThread
    {

        public void run()
        {
            TestProgram test = new TestProgram();
            Console.WriteLine("asd23");
            test.start();
        }
    }
}