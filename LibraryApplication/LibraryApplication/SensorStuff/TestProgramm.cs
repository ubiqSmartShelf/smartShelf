using System;
using System.Collections.Generic;
using System.Text;
using Phidgets; //Needed for the RFID class and the PhidgetException class
using Phidgets.Events; //Needed for the phidget event handling classes
using LibraryApplication.App_Code;

namespace LibraryApplication.SensorStuff
{
    public class TestProgram
    {

        public static TextLCD tLCD;
        static Dictionary<String, Shelf> shelves = new Dictionary<String, Shelf>();
        private static InterfaceKit ifk;
        private static Boolean personClose = false;
        private static String currentPersonTag;
        private static RFID identificationReader;
        private static DatabaseConnection database = new DatabaseConnection();
        private static readonly object displayLock = new object();

        public void start()
        {

            Shelf shelf = new Shelf();
            shelf = new Shelf();
            shelf.Serial = 309428; //harry potter
            shelf.BookId = "0101a855fd";
            shelves.Add("309428", shelf);
            shelf = new Shelf();
            shelf.Serial = 159699; //bible
            shelf.BookId = "0101a824ce";
            shelves.Add("159699", shelf);
            
            try
            {
                
                ifk = ifk_init();



                tLCD = tLCD_init();

                RFID shelf1 = RFID_init(); //Declare an RFID object
                RFID shelf2 = RFID_init(); //Declare an RFID object

                identificationReader = RFID_init();

                //Wait for a Phidget RFID to be attached before doing anything with 
                //the object
                Console.WriteLine("waiting for attachment of shelf 1...");
                shelf1.waitForAttachment();

                Console.WriteLine("waiting for attachment of shelf 2...");
                shelf2.waitForAttachment();

                identificationReader.waitForAttachment();

                shelves[shelf1.SerialNumber.ToString()].Rfid = shelf1;
                shelves[shelf2.SerialNumber.ToString()].Rfid = shelf2;


                //We have to wait to make sure that a TextLCD is plugged in before
                //trying to communicate with it
                if (!tLCD.Attached)
                {
                    Console.WriteLine("Waiting for TextLCD to be attached....");
                    tLCD.waitForAttachment();
                }

                while (!tLCD.Attached)
                {

                }

                tLCD.screens[1].ScreenSize = TextLCD.ScreenSizes._4x20;
                tLCD.screens[0].ScreenSize = TextLCD.ScreenSizes._2x20;

                //turn on the antenna and the led to show everything is working
                shelf1.Antenna = true;
                shelf1.LED = true;
                
                shelf2.Antenna = true;
                shelf2.LED = true;

                identificationReader.Antenna = true;
                identificationReader.LED = true;

                tLCD.screens[0].initialize();
                tLCD.screens[1].initialize();
                //keep waiting and outputting events until keyboard input is entered
                Console.WriteLine("Press any key to end...");
                Console.Read();

                //turn off the led
                shelf1.LED = false;

                shelf2.LED = false;

                identificationReader.LED = false;

               
                //close the phidget and dispose of the object
                shelf1.close();
                shelf1 = null;

                shelf2.close();
                shelf2 = null;

                identificationReader.close();
                identificationReader = null;

                ifk.close();
                ifk = null;

                tLCD.close();
                tLCD = null; 

                Console.WriteLine("ok");
            }
            catch (PhidgetException ex)
            {
                Console.WriteLine(ex.Description);
            }
        }

        static void ifk_Attach(object sender, AttachEventArgs e)
        {
            InterfaceKit attached = (InterfaceKit)sender;
            Console.WriteLine("interface attached {0}", attached.ID);
        }

        //detach event handler.... we will display the device attach status and clear all the other fields
        static void ifk_Detach(object sender, DetachEventArgs e)
        {
            InterfaceKit detached = (InterfaceKit)sender;
            Console.WriteLine("interface detached {0}", detached.ID);
        }

        static void ifk_InputChange(object sender, InputChangeEventArgs e)
        {
            if(currentPersonTag != null)
            {
                try
                {
                    database.setBookPersonReturnDate(currentPersonTag);
                    if (tLCD.Attached)
                    {
                        lock(displayLock)
                        {
                            tLCD.screens[0].rows[0].DisplayString = "OK\0";
                        }
                    }
                }
                catch (Exception ex)
                {
                    lock(displayLock)
                    {
                        tLCD.screens[0].rows[0].DisplayString = "Error occured\0";
                    }
                }
            }
        }

        static void ifKit_SensorChange(object sender, SensorChangeEventArgs e)
        {
            if (personClose && e.Value < 150)
            {
                //Clearing displays when person moves away
                personClose = false;
                lock(displayLock)
                {
                    tLCD.screens[1].Backlight = false;
                    tLCD.screens[0].Backlight = false;

                }
                currentPersonTag = null;
            } 
            else if (!personClose && e.Value > 100)
            {
                personClose = true;
                //Console.WriteLine("person is close");
            }
        }

        //attach event handler...display the serial number of the attached RFID phidget
        static void rfid_Attach(object sender, AttachEventArgs e)
        {
        }

        //detach event handler...display the serial number of the detached RFID phidget
        static void rfid_Detach(object sender, DetachEventArgs e)
        {
        }

        //Error event handler...display the error description string
        static void rfid_Error(object sender, ErrorEventArgs e)
        {
        }

        //attach event handler, we'll output the name and serial number of the TextLCD
        //that was attached
        static void tLCD_Attach(object sender, AttachEventArgs e)
        {
            TextLCD attached = (TextLCD)sender;
            string name = attached.Name;
            string serialNo = attached.SerialNumber.ToString();
        }

        //Detach event handler, we'll output the name and serial of the phidget that is
        //detached
        static void tLCD_Detach(object sender, DetachEventArgs e)
        {
            TextLCD detached = (TextLCD)sender;
            string name = detached.Name;
            string serialNo = detached.SerialNumber.ToString();
        }

        //TextLCD error event handler, we'll just output any error data to the console
        static void tLCD_Error(object sender, ErrorEventArgs e)
        {
        }

        //Print the tag code of the scanned tag
        static void rfid_Tag(object sender, TagEventArgs e)
        {
            if(((RFID)sender).Attached)
            { 
                String serial = ((RFID)sender).SerialNumber.ToString();
                if (identificationReader != null && identificationReader.Attached && identificationReader.SerialNumber.ToString().Equals(serial))
                {
                    string name = database.getPersonByTag(e.Tag);
                    if(name != null)
                    {
                        currentPersonTag = e.Tag;
                        List<String> books = database.getPersonBooks(e.Tag);
                        if (books.Count != 0)
                        {
                            lock(displayLock)
                            {
                                for (int i = 0; i < books.Count && i < 4; i++)
                                {
                                    tLCD.screens[1].rows[i].DisplayString = books[i] + "\0";
                                }
                                tLCD.screens[0].rows[0].DisplayString = "Borrowing books\0";
                                tLCD.screens[0].rows[1].DisplayString = "Press button for ok\0";
                                if (ifk.sensors[0].Value > 150)
                                {
                                    tLCD.screens[0].Backlight = true;
                                    tLCD.screens[1].Backlight = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("A book is not a person!");
                    }
                }
                else
                {
                    Book book = database.getBookByTag(e.Tag);
                    if (book != null && shelves.ContainsKey(serial) && shelves[serial].Rfid != null)
                    {
                        database.setShelfByTag(book.TagId, serial);
                        if (book.CorrectShelf.Equals(serial))
                        {
                            shelves[serial].Rfid.outputs[1] = false;
                        }
                        else
                        {
                            shelves[serial].Rfid.outputs[1] = true;
                        }
                    }
                    else
                    {
                        String name = database.getPersonByTag(e.Tag);
                        if(name != null)
                        {
                            database.setBookPerson(e.Tag, shelves[serial].BookId);
                        }
                    }
                }
            }
        }

        //print the tag code for the tag that was just lost
        static void rfid_TagLost(object sender, TagEventArgs e)
        {
            String serial = ((RFID)sender).SerialNumber.ToString();
            if (shelves.ContainsKey(serial)) { 
                shelves[serial].Rfid.outputs[1] = false;
                Book book = database.getBookByTag(e.Tag);
                if (book != null && shelves.ContainsKey(serial))
                {
                    database.setShelfByTag(e.Tag, null);
                }
            }
        }

        private RFID RFID_init()
        {
            RFID rfid = new RFID();
            rfid.Attach += new AttachEventHandler(rfid_Attach);
            rfid.Detach += new DetachEventHandler(rfid_Detach);
            rfid.Error += new ErrorEventHandler(rfid_Error);

            rfid.Tag += new TagEventHandler(rfid_Tag);
            rfid.TagLost += new TagEventHandler(rfid_TagLost);
            rfid.open();
            return rfid;
        }

        private InterfaceKit ifk_init()
        {
            InterfaceKit ifk = new InterfaceKit();
            ifk.Attach += new AttachEventHandler(ifk_Attach);
            ifk.Detach += new DetachEventHandler(ifk_Detach);
            ifk.InputChange += new InputChangeEventHandler(ifk_InputChange);
            ifk.SensorChange += new SensorChangeEventHandler(ifKit_SensorChange);
            ifk.open();
            return ifk;
        }

        private TextLCD tLCD_init()
        {
            TextLCD tLCD = new TextLCD();
            tLCD.Attach += new AttachEventHandler(tLCD_Attach);
            tLCD.Detach += new DetachEventHandler(tLCD_Detach);
            tLCD.Error += new ErrorEventHandler(tLCD_Error);

            tLCD.open();
            return tLCD;
        }
    }
}