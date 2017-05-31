using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Model;

namespace WebApplication1.Repository
{
    /// <summary>
    /// Class to handle all database operations involving Patrons
    /// </summary>
    public class PatronRepository:BaseRepository
    {
        public PatronRepository(IConfiguration configuration) : base(configuration) {}

        /// <summary>
        /// Adds a new patron
        /// </summary>
        /// <param name="patron">the patron to add goten from the body of the request</param>
        /// <returns>the id of the newly added patron</returns>
        public int Add(Patron patron)
        {
            using(IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<int>("INSERT INTO patron (fname,lname) VALUES (@FName,@LName) RETURNING patronid", patron).Single();
            }
        }

        /// <summary>
        /// Gets all the patrons
        /// </summary>
        /// <returns>List of patrons or empty list</returns>
        public IEnumerable<Patron> FindAll()
        {
            using(IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                var patrons = dbConnection.Query<Patron>("SELECT * FROM patron");
                foreach(var patron in patrons)
                {
                    patron.Books = new List<Book>(dbConnection.Query<Book>("SELECT * FROM book, patronbook WHERE patronbook.patronid = " + patron.PatronId + " and patronbook.bookid = book.bookid"));
                    foreach(var book in patron.Books)
                    {
                        book.Authors = new List<Author>(dbConnection.Query<Author>("SELECT * FROM author, bookauthor WHERE bookauthor.bookid = " + book.BookId + " and bookauthor.authorid = author.authorid"));
                    }
                }
                return patrons;
            }
        }

        /// <summary>
        /// Gets a patron that has a given id
        /// </summary>
        /// <param name="id">the id of the patron to get</param>
        /// <returns>the patron object or nothing</returns>
        public Patron FindById(int id)
        {
            using(IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                Patron patron = dbConnection.Query<Patron>("SELECT * FROM patron WHERE patronid = @PatronId", new { PatronId = id }).FirstOrDefault();
                if(patron != null)
                {
                    patron.Books = new List<Book>(dbConnection.Query<Book>("SELECT * FROM book, patronbook WHERE patronbook.patronid = " + patron.PatronId + " and patronbook.bookid = book.bookid"));
                    foreach (var book in patron.Books)
                    {
                        book.Authors = new List<Author>(dbConnection.Query<Author>("SELECT * FROM author, bookauthor WHERE bookauthor.bookid = " + book.BookId + " and bookauthor.authorid = author.authorid"));
                    }
                }
                return patron;
            }
        }

        /// <summary>
        /// Removes a Patron
        /// </summary>
        /// <param name="id">the id of the patron to remove</param>
        /// <returns>a string detailing success or failure</returns>
        public string Remove(int id)
        {
            using(IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                //First return all of this patron's books back to their respective libraries
                var patronBooks = new List<PatronBook>(dbConnection.Query<PatronBook>("SELECT * FROM patronbook,book WHERE patronbook.patronid = " + id + " and patronbook.bookid = book.bookid"));
                foreach(var book in patronBooks)
                {
                    dbConnection.Query("UPDATE librarybook SET checkedout = false WHERE bookid = " + book.BookId + " and libraryid = " + book.LibraryId);
                }
                bool success = dbConnection.Execute("DELETE FROM patron WHERE patronid=@Id", new { Id = id }) != 0;
                if (success)
                {
                    return "SUCCESS: Patron with id: " + id + " deleted";
                }
                return "FAIL: Patron with id: " + id + " not deleted";
            }
        }

        /// <summary>
        /// Updates an already existing Patron
        /// </summary>
        /// <param name="patron">the patron to update, gotten from the body of the request</param>
        /// <returns>a string detailing success or failure</returns>
        public string Update(Patron patron)
        {
            using(IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                bool changed = dbConnection.Query("UPDATE patron SET fname = @FName, lname = @LName WHERE patronid = @PatronId RETURNING patronid", patron).Count() != 0;
                if (changed)
                {
                    return "SUCCESS: Patron with id: " + patron.PatronId + " successfully updated";
                }
                return "FAIL: Patron with id: " + patron.PatronId + " not updated";
            }
        }

        /// <summary>
        /// Checks out a book for a given patron. If it can't checkout the given book at the given library for the given patron does nothing
        /// </summary>
        /// <param name="patronBook">the identifying info for the book to check out</param>
        /// <returns>a string detailing success or failure</returns>
        public string CheckOutBook(PatronBook patronBook)
        {
            using(IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                //Making sure we can find the given book in the given library
                bool foundBook = dbConnection.Query("SELECT * FROM librarybook WHERE libraryid = " + patronBook.LibraryId + " and bookid = " + patronBook.BookId + " and checkedout=false").Count() != 0;
                //Making sure the patron exists
                bool foundPatron = dbConnection.Query("SELECT * FROM patron WHERE patronid = " + patronBook.PatronId).Count() != 0;
                if(foundBook && foundPatron)
                {
                    bool success = dbConnection.Execute("INSERT INTO patronbook (patronid,bookid,libraryid) VALUES (@PatronId,@BookId,@LibraryId)", patronBook) != 0;
                    if (success)
                    {
                        //setting the book's status to checked out
                        dbConnection.Query("UPDATE librarybook SET checkedout = true WHERE bookid = " + patronBook.BookId + " and libraryid = " + patronBook.LibraryId);
                        return "SUCCESS: Patron with id: " + patronBook.PatronId + " checked out Book with id: " + patronBook.BookId + " from Library with id: " + patronBook.LibraryId;
                    }
                }
                return "FAIL: Patron with id: " + patronBook.PatronId + " failed to check out Book with id: " + patronBook.BookId + " from Library with id: " + patronBook.LibraryId;
            }
        }

        /// <summary>
        /// Checks in a book for a given patron. If it can't checkin the given book at the given library for the given patron does nothing
        /// </summary>
        /// <param name="patronBook">the identifying info for the book to check in</param>
        /// <returns>a string detailing success or failure</returns>
        public string CheckInBook(PatronBook patronBook)
        {
            using(IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                bool success = dbConnection.Execute("DELETE FROM patronbook WHERE patronid=@PatronId and bookid=@BookId and libraryid=@LibraryId", patronBook) != 0;
                if (success)
                {
                    //checking the book back in
                    dbConnection.Query("UPDATE librarybook SET checkedout = false WHERE bookid = " + patronBook.BookId + " and libraryid = " + patronBook.LibraryId);
                    return "SUCCESS: Patron with id: " + patronBook.PatronId + " checked in Book with id: " + patronBook.BookId + " to Library with id: " + patronBook.LibraryId;
                }
                return "FAIL: Patron with id: " + patronBook.PatronId + " failed to check in Book with id: " + patronBook.BookId + " to Library with id: " + patronBook.LibraryId;
            }
        }
    }
}
