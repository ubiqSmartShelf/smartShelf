using System;
namespace LibraryApplication.SensorStuff
{
    public class Book
    {
        private String title;

        public String Title
        {
            get { return title; }
            set { title = value; }
        }
        private String author;

        public String Author
        {
            get { return author; }
            set { author = value; }
        }
        private String currentShelf;

        public String CurrentShelf
        {
            get { return currentShelf; }
            set { currentShelf = value; }
        }
        private String correctShelf;  //-1 when book is in no shelf

        public String CorrectShelf
        {
            get { return correctShelf; }
            set { correctShelf = value; }
        }
        private String tagId;

        public String TagId
        {
            get { return tagId; }
            set { tagId = value; }
        }

    }
}