using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Phidgets;

namespace LibraryApplication.SensorStuff
{
    class Shelf
    {
        private RFID rfid;

        private int serial;

        public int Serial
        {
            get
            {
                return serial;
            }
            set
            {
                serial = value;
            }
        }
        private string bookId;
    
        public string BookId
        {
            get
            {
                return bookId;
            }
            set
            {
                bookId = value;
            }
        }

        public RFID Rfid
        {
            get
            {
                return rfid;
            }
            set
            {
                rfid = value;
            }
        }

    }
}
