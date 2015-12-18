using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using LibraryApplication.Models;
using System.Data.Entity;
using System.Collections;
using System.Data.OleDb;
using System.Diagnostics;
using LibraryApplication.SensorStuff;

namespace LibraryApplication.App_Code
{
    public class DatabaseConnection
    {

        private SqlConnection sqlCn = new SqlConnection();

        public DatabaseConnection() {
            sqlCn.ConnectionString = ConfigurationManager.ConnectionStrings["sqlConnectionDatabase"].ConnectionString;
        }

        public int setShelf(int newint, String bookName)
        {

            int rowsaffected = 0;
            sqlCn.ConnectionString = ConfigurationManager.ConnectionStrings["sqlConnectionDatabase"].ConnectionString;

            string sqlString = "UPDATE tbl_Books SET bo_shelf = @setshelf WHERE bo_Title = @bookName";
            SqlCommand myCmd = new SqlCommand(sqlString, sqlCn);
            myCmd.Parameters.Add("@setshelf", SqlDbType.BigInt);
            myCmd.Parameters.Add("@bookName", SqlDbType.NChar);


            try
            {
                sqlCn.Open();
                myCmd.Parameters["@setshelf"].Value = newint;
                myCmd.Parameters["@bookName"].Value = bookName;
                rowsaffected = myCmd.ExecuteNonQuery();

                sqlCn.Close();

            }
            catch (Exception e)
            {

                Console.WriteLine(e.StackTrace);

            }
            return rowsaffected;

        }

        public int setShelfByTag(String bookTag, String shelfTag)
        {

            int rowsaffected = 0;

            string sqlString = "UPDATE tbl_Books SET CurrentShelf = @shelfTag WHERE TagId = @bookTag";
            SqlCommand myCmd = new SqlCommand(sqlString, sqlCn);
            myCmd.Parameters.AddWithValue("@shelfTag", shelfTag != null ? (Object)shelfTag : (Object)DBNull.Value);
            myCmd.Parameters.AddWithValue("@bookTag", bookTag);

            try
            {
                sqlCn.Open();
                rowsaffected = myCmd.ExecuteNonQuery();

                sqlCn.Close();

            }
            catch (Exception e)
            {

                Console.WriteLine(e.StackTrace);

            }
            return rowsaffected;

        }

        public int getShelfOfBook(String bookName)
        {
            int bookShelf = 0;
            int rowsaffected = 0;
            sqlCn.ConnectionString = ConfigurationManager.ConnectionStrings["sqlConnectionDatabase"].ConnectionString;

            string sqlString = "SELECT bo_shelf FROM tbl_Books WHERE bo_Title = @bookName";
            SqlCommand myCmd = new SqlCommand(sqlString, sqlCn);
            myCmd.Parameters.Add("@bookName", SqlDbType.NChar);


            try
            {
                sqlCn.Open();
                myCmd.Parameters["@bookName"].Value = bookName;

                SqlDataReader myReader;
                myReader = myCmd.ExecuteReader();

                while (myReader.Read())
                {
                    bookShelf = myReader.GetInt32(0);
                }


                sqlCn.Close();

            }
            catch (Exception e)
            {

                Console.WriteLine(e.StackTrace);

            }
            return bookShelf;

        }


        public List<String> getPersonBooks(string cardId)
        {
            List<String> list = new List<String>();
            int rowsaffected = 0;

            string sqlString = "SELECT Title FROM tbl_Borrowing JOIN tbl_Books ON tbl_Borrowing.BookTag = tbl_Books.TagId WHERE PersonTag = @cardId";
            SqlCommand myCmd = new SqlCommand(sqlString, sqlCn);
            myCmd.Parameters.AddWithValue("@cardId", cardId);


            try
            {
                sqlCn.Open();

                SqlDataReader myReader;
                myReader = myCmd.ExecuteReader();

                while (myReader.Read())
                {
                    list.Add(myReader.GetString(0));

                }

                myReader.Close();

                sqlCn.Close();

            }
            catch (Exception e)
            {

                Console.WriteLine(e.StackTrace);

            }


            return list;
        }

        public int setBookPersonReturnDate(String personTag)
        {

            int rowsaffected = 0;
            sqlCn.ConnectionString = ConfigurationManager.ConnectionStrings["sqlConnectionDatabase"].ConnectionString;

            string sqlString = "UPDATE tbl_Borrowing SET ReturnDate = @returnDate WHERE personTag = @personTag AND ReturnDate IS NULL";
            SqlCommand myCmd = new SqlCommand(sqlString, sqlCn);
            myCmd.Parameters.AddWithValue("@personTag", personTag);
            myCmd.Parameters.AddWithValue("@returnDate", 15);


            try
            {
                sqlCn.Open();
                if (!myCmd.ExecuteReader().HasRows)
                {
                    rowsaffected = myCmd.ExecuteNonQuery();
                }

                sqlCn.Close();

            }
            catch (Exception e)
            {

                Console.WriteLine(e.StackTrace);

            }
            return rowsaffected;

        }
        
        public int setBookPerson(String personTag, String bookTag)
        {

            int rowsaffected = 0;
            sqlCn.ConnectionString = ConfigurationManager.ConnectionStrings["sqlConnectionDatabase"].ConnectionString;

            string sqlString = "SELECT * FROM Borrowing WHERE personTag = @personTag AND bookTag = @bookTag AND ReturnDate IS NULL";
            string sqlString2 = "INSERT INTO tbl_Borrowing (BookTag, PersonTag) VALUES (@bookTag, @personTag)";
            SqlCommand myCmd = new SqlCommand(sqlString, sqlCn);
            SqlCommand myCmd2 = new SqlCommand(sqlString2, sqlCn);
            myCmd.Parameters.AddWithValue("@personTag", personTag);
            myCmd.Parameters.AddWithValue("@bookTag", bookTag);
            myCmd2.Parameters.AddWithValue("@personTag", personTag);
            myCmd2.Parameters.AddWithValue("@bookTag", bookTag);


            try
            {
                sqlCn.Open();
                if (!myCmd.ExecuteReader().HasRows)
                {
                    rowsaffected = myCmd2.ExecuteNonQuery();
                }

                sqlCn.Close();

            }
            catch (Exception e)
            {

                Console.WriteLine(e.StackTrace);

            }
            return rowsaffected;

        }


        public String getPersonByTag(String tag)
        {

            sqlCn.ConnectionString = ConfigurationManager.ConnectionStrings["sqlConnectionDatabase"].ConnectionString;
            String name = null;

            string sqlString = "SELECT Name FROM tbl_Person WHERE PersonTagId = @tag";
            SqlCommand myCmd = new SqlCommand(sqlString, sqlCn);
            myCmd.Parameters.AddWithValue("@tag", tag);

            try
            {
                sqlCn.Open();

                SqlDataReader myReader;
                myReader = myCmd.ExecuteReader();
                if(myReader.HasRows)
                {
                    myReader.Read();
                    name = myReader.GetString(0);
                }

                myReader.Close();

                sqlCn.Close();

            }
            catch (Exception e)
            {

                Console.WriteLine(e.StackTrace);

            }
            return name;

        }

        public Book getBookByTag(String tag)
        {

            Book book = null;

            string sqlString = "SELECT * FROM tbl_Books WHERE TagId = @tag";
            SqlCommand myCmd = new SqlCommand(sqlString, sqlCn);
            myCmd.Parameters.AddWithValue("@tag", tag);

            try
            {
                sqlCn.Open();
                SqlDataReader myReader;
                myReader = myCmd.ExecuteReader();
                if (myReader.HasRows)
                {
                    myReader.Read();
                    book = new Book();
                    book.Title = myReader["Title"].ToString();
                    book.Author = myReader["Author"].ToString();
                    book.CorrectShelf = myReader["CorrectShelf"].ToString();
                    book.CurrentShelf = myReader["CurrentShelf"].ToString();
                    book.TagId = myReader["TagId"].ToString();
                }
                myReader.Close();

                sqlCn.Close();

            }
            catch (Exception e)
            {

                Console.WriteLine(e.StackTrace);

            }
            return book;

        }

              public List<string> getAllInBook()
              {
                  sqlCn.ConnectionString = ConfigurationManager.ConnectionStrings["sqlConnectionDatabase"].ConnectionString;
                  List<string> booklist = new List<string>();
                  
                  int rowsaffected = 0;

                  string sqlString = "SELECT Title, CurrentShelf FROM tbl_Books WHERE CurrentShelf IS NULL";
                  SqlCommand myCmd = new SqlCommand(sqlString, sqlCn);


                  try
                  {
                      sqlCn.Open();

                      SqlDataReader myReader;
                      myReader = myCmd.ExecuteReader();

                      while (myReader.Read())
                      {
                          booklist.Add(myReader.GetString(0));
                      }

                      myReader.Close();

                      sqlCn.Close();
                      

                  }
                  catch (Exception e)
                  {

                      Console.WriteLine(e.StackTrace);

                  }


                  return booklist;
              }
              

        public List<String> getBookByName(string bookName)
        {
            List<String> bookinfo;
            bookinfo = new List<String>();
            int rowsaffected = 0;
            sqlCn.ConnectionString = ConfigurationManager.ConnectionStrings["sqlConnectionDatabase"].ConnectionString;

            string sqlString = "SELECT CurrentShelf, CorrectShelf FROM tbl_Books WHERE Title = @bookName";
            SqlCommand myCmd = new SqlCommand(sqlString, sqlCn);
            myCmd.Parameters.Add("@bookName", SqlDbType.NChar);


            try
            {
                sqlCn.Open();
                myCmd.Parameters["@bookName"].Value = bookName;
                SqlDataReader myReader;
                myReader = myCmd.ExecuteReader();
                
                while (myReader.Read())
                {
                    String tempstring;
                    String tempstring1;

                    try
                    {
                        tempstring = myReader.GetString(0);
                        
                    }
                    catch(Exception e)
                    {
                        tempstring = "-1";

                    }

                        tempstring1 = myReader.GetString(1);

                    bookinfo.Add(tempstring);
                    bookinfo.Add(tempstring1);
                }

                myReader.Close();

                sqlCn.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);

            }


            return bookinfo;
        }


    }
}