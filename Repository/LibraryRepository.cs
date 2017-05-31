using Dapper;
using Microsoft.Extensions.Configuration;
using WebApplication1.Model;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace WebApplication1.Repository
{
    /// <summary>
    /// Class to handle all database operations for Library
    /// </summary>
    public class LibraryRepository : BaseRepository
    {
        public LibraryRepository(IConfiguration configuration):base(configuration)
        {
        }

        /// <summary>
        /// Adds a new library to the database
        /// </summary>
        /// <param name="library">the library to add</param>
        /// <returns>the id of the new library</returns>
        public int Add(Library library)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<int>("INSERT INTO library (name) VALUES (@Name) RETURNING libraryid", library).Single();
            }
        }

        /// <summary>
        /// Gets all the libraries
        /// </summary>
        /// <returns>A list of library objects or an empty lsit</returns>
        public IEnumerable<Library> FindAll()
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                var libraries = dbConnection.Query<Library>("SELECT * FROM library");
                foreach(var library in libraries)
                {
                    //Only showing books in library that are not checked out
                    library.Books = new List<Book>(dbConnection.Query<Book>("SELECT * FROM book, librarybook WHERE librarybook.libraryid = " + library.LibraryId + " and librarybook.bookid = book.bookid and librarybook.checkedout = false"));
                    //Go one step deeper to get the authors of each book in each library
                    foreach(var book in library.Books)
                    {
                        book.Authors = new List<Author>(dbConnection.Query<Author>("SELECT * FROM author,bookauthor WHERE bookauthor.bookid = " + book.BookId + " and bookauthor.authorid = author.authorid"));
                    }
                }
                return libraries;
            }
        }

        /// <summary>
        /// Gets a library by ID
        /// </summary>
        /// <param name="id">the id of the library to get</param>
        /// <returns>the library object or nothing</returns>
        public Library FindById(int id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                Library library = dbConnection.Query<Library>("SELECT * FROM library WHERE libraryid = @LibraryId", new { LibraryId = id }).FirstOrDefault();
                if (library != null)
                {
                    library.Books = new List<Book>(dbConnection.Query<Book>("SELECT * FROM book, librarybook WHERE librarybook.libraryid = " + library.LibraryId + " and librarybook.bookid = book.bookid and librarybook.checkedout = false"));
                    //Getting the authors for each book in the library
                    foreach (var book in library.Books)
                    {
                        book.Authors = new List<Author>(dbConnection.Query<Author>("SELECT * FROM author, bookauthor WHERE bookauthor.bookid = " + book.BookId + " and bookauthor.authorid = author.authorid"));
                    }
                }
                return library;
            }
        }

        /// <summary>
        /// Removes a library
        /// </summary>
        /// <param name="id">the id of the library to update</param>
        /// <returns>a string detailing success or failure</returns>
        public string Remove(int id)
        {
            using(IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                bool success = dbConnection.Execute("DELETE FROM library WHERE libraryid=@Id", new { Id = id }) != 0;
                if (success)
                {
                    return "SUCCESS: Library with id: " + id + " deleted";
                }
                return "FAIL: Library with id: " + id + " not deleted";
            }
        }

        /// <summary>
        /// Updates an existing library
        /// </summary>
        /// <param name="library">the library to update</param>
        /// <returns>a string detailing success or failure</returns>
        public string Update(Library library)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                bool changed = dbConnection.Query("UPDATE library SET name = @Name WHERE libraryid = @LibraryId RETURNING libraryid", library).Count() != 0;
                if (changed)
                {
                    return "SUCCESS: Library with id: " + library.LibraryId + " successfully updated";
                }
                return "FAIL: Library with id: " + library.LibraryId + " not updated";
            }
        }

        /// <summary>
        /// Adds a book to a library
        /// </summary>
        /// <param name="libraryBook">the library book to add gotten from the body of the request</param>
        /// <returns>a string detailing success or failure</returns>
        public string AddBook(LibraryBook libraryBook)
        {
            using(IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                //Before doing anything, check to see if the book and library exist
                bool bookFound = dbConnection.Query("SELECT * FROM book WHERE bookid = " + libraryBook.BookId).Count() != 0;
                bool libraryFound = dbConnection.Query("SELECT * FROM library WHERE libraryid = " + libraryBook.LibraryId).Count() != 0;
                if (bookFound && libraryFound)
                {
                    bool success = dbConnection.Execute("INSERT INTO librarybook (libraryid,bookid) VALUES (@LibraryId,@BookId)", libraryBook) != 0;
                    if (success)
                    {
                        return "SUCCESS: Book with id: " + libraryBook.BookId + " successfully added to Library with id: " + libraryBook.LibraryId;
                    }
                }
                return "FAIL: Book with id: " + libraryBook.BookId + " not added to Library with id: " + libraryBook.LibraryId;
            }
        }

        /// <summary>
        /// Removes a book from a library
        /// </summary>
        /// <param name="libraryBook">the library book to remove, gotten from the Body of the request</param>
        /// <returns>a string detailing success or failure</returns>
        public string RemoveBook(LibraryBook libraryBook)
        {
            using(IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                dbConnection.Execute("DELETE FROM patronbook WHERE libraryid= " + libraryBook.LibraryId + " and bookid= " + libraryBook.BookId);
                bool success = dbConnection.Execute("DELETE FROM librarybook WHERE bookid= " + libraryBook.BookId + " and libraryid= " + libraryBook.LibraryId) != 0;
                if (success)
                {
                    return "SUCCESS: Book with id: " + libraryBook.BookId + " successfully removed from the Library with id: " + libraryBook.LibraryId;
                }
                return "FAIL: Book with id: " + libraryBook.BookId + " not removed from the Library with id: " + libraryBook.LibraryId;
            }
        }
    }


}
