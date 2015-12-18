using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LibraryApplication.Models;
using LibraryApplication.App_Code;
using System.Threading;
using System.Diagnostics;
using System.Collections;

namespace LibraryApplication.Controllers
{
    public class BooksController : Controller
    {
        private LibraryEntities db = new LibraryEntities();
        private RFIDThread rfid = new RFIDThread();

        // GET: /Books/
        public ActionResult Index()
        {
            return View(db.tbl_Books.ToList());
        }


        // GET: /Books/
        public ActionResult BookMap()
        {
  
            DatabaseConnection dc = new DatabaseConnection();
            List<string> listOfBooks = new List<string>();
            listOfBooks = dc.getAllInBook();
            ViewData["show"] = false;
            ViewData["159699"] = "http://i.imgur.com/kdLyRnJ.png";
            ViewData["shelf2"] = "http://i.imgur.com/kdLyRnJ.png";
            ViewData["309428"] = "http://i.imgur.com/kdLyRnJ.png";
            ViewData["book2"] = "";
            ViewData["book3"] = "http://i.imgur.com/tMj7XRR.png";
            ViewData["pic"] = "http://i.imgur.com/RFoCaRr.png";

                
            
            for (int i = 0; i < listOfBooks.Count; i++)
            {

                    ViewBag.test = "listbook is: " + listOfBooks[i];

                    if (listOfBooks[i] == "The Bible")
                    {

                        ViewData["book1"] = "";
                    }
                    else if (listOfBooks[i] == "Ubiquitous for Dummies")
                    {
                        ViewData["book2"] = "";
                    }
                    else if (listOfBooks[i] == "Harry Potter")
                    {
                        ViewData["book3"] = "";
                    }
                

            }


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // GET: /Books/
        public ActionResult BookMap(String bookString)
        {
            var books = from s in db.tbl_Books select s;
            books = books.Where(s => s.Title.ToUpper().Contains(bookString));

            ViewData["show"] = false;
            ViewData["159699"] = "http://i.imgur.com/kdLyRnJ.png";
            ViewData["shelf2"] = "http://i.imgur.com/kdLyRnJ.png";
            ViewData["309428"] = "http://i.imgur.com/kdLyRnJ.png";
            ViewData["book1"] = "http://i.imgur.com/tMj7XRR.png";
            ViewData["book2"] = "";
            ViewData["book3"] = "http://i.imgur.com/no0R6Wa.png";
            ViewData["pic"] = "http://i.imgur.com/RFoCaRr.png";

            DatabaseConnection dc = new DatabaseConnection();
            List<string> listOfBooks = new List<string>();
            listOfBooks = dc.getAllInBook();
            for (int i = 0; i < listOfBooks.Count; i++)
            {


                if (listOfBooks[i] == "The Bible")
                {
                    
                    ViewData["book1"] = "";
                }
                else if (listOfBooks[i] == "Ubiquitous for Dummies")
                {
                    ViewData["book2"] = "";
                }
                else if (listOfBooks[i] == "Harry Potter")
                {
                    ViewData["book3"] = "";
                }

            }



            if(!String.IsNullOrEmpty(bookString)){
                List<String> bookList = dc.getBookByName(bookString);
                if (bookList.Count != 0) {
                    if (bookList[0] == "-1")
                    {
                        ViewBag.book = " Book is not in any shelf";
                    }
                    else
                    {
                        ViewData["show"] = true;
                    

                        if (bookList[0] == "309428")
                        {
                            ViewData["book3"] = "http://i.imgur.com/aJu9ZBj.png";
                            ViewData["pic"] = "http://i.imgur.com/68qQGfh.png";
                        }

                        else if (bookList[0] == "159699")
                        {
                            ViewData["book1"] = "http://i.imgur.com/2pzSWMg.png";
                            ViewData["pic"] = "http://i.imgur.com/Qne8kJa.png";
                        }
                       
                    }
                    
                }
                
            }
            

            return View();
        }


        // GET: /Books/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Books tbl_books = db.tbl_Books.Find(id);
            if (tbl_books == null)
            {
                return HttpNotFound();
            }
            return View(tbl_books);
        }

        // GET: /Books/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Books/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,Title,Author,CurrentShelf,CorrectShelf,TagId")] tbl_Books tbl_books)
        {
            if (ModelState.IsValid)
            {
                db.tbl_Books.Add(tbl_books);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tbl_books);
        }

        // GET: /Books/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Books tbl_books = db.tbl_Books.Find(id);
            if (tbl_books == null)
            {
                return HttpNotFound();
            }
            return View(tbl_books);
        }

        // POST: /Books/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Title,Author,CurrentShelf,CorrectShelf,TagId")] tbl_Books tbl_books)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_books).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tbl_books);
        }

        // GET: /Books/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Books tbl_books = db.tbl_Books.Find(id);
            if (tbl_books == null)
            {
                return HttpNotFound();
            }
            return View(tbl_books);
        }

        // POST: /Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_Books tbl_books = db.tbl_Books.Find(id);
            db.tbl_Books.Remove(tbl_books);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
