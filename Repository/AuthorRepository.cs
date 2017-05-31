using Dapper;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WebApplication1.Model;

namespace WebApplication1.Repository
{
    /// <summary>
    /// Class to handle all database operations for author
    /// </summary>
    public class AuthorRepository:BaseRepository
    {
        public AuthorRepository(IConfiguration configuration):base(configuration)
        {
        }

        /// <summary>
        /// Adds a new author
        /// </summary>
        /// <param name="author">the author to add</param>
        /// <returns>the id of the newly added author</returns>
        public int Add(Author author)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<int>("INSERT INTO author (fname,lname) VALUES (@FName,@LName) RETURNING authorid", author).Single();
            }
        }

        /// <summary>
        /// Gets all the authors
        /// </summary>
        /// <returns>a list of authors or an empty list</returns>
        public IEnumerable<Author> FindAll()
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                var authors = dbConnection.Query<Author>("SELECT * FROM author");
                foreach(var author in authors)
                {
                    author.Books = new List<Book>(dbConnection.Query<Book>("SELECT * FROM book,bookauthor WHERE bookauthor.authorid = " + author.AuthorId + " and bookauthor.bookid = book.bookid"));
                    //Getting the libraries that this author's book is availible in. Only show libraries where its not checked out
                    foreach(var book in author.Books)
                    {
                        book.Libraries = new List<Library>(dbConnection.Query<Library>("SELECT * FROM library, librarybook WHERE librarybook.bookid = " +
                            book.BookId + " and librarybook.libraryid = library.libraryid and librarybook.checkedout = false"));
                    }
                }
                return authors;
            }
        }

        /// <summary>
        /// Gets an author by a given id
        /// </summary>
        /// <param name="id">the id of the author to get</param>
        /// <returns>an author object or nothing</returns>
        public Author FindByID(int id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                Author author = dbConnection.Query<Author>("SELECT * FROM author WHERE authorid = @AuthorId", new { AuthorId = id }).FirstOrDefault();
                if (author != null)
                {
                    author.Books = new List<Book>(dbConnection.Query<Book>("SELECT * FROM book,bookauthor WHERE bookauthor.authorid = " + author.AuthorId + " and bookauthor.bookid = book.bookid"));
                    //Getting the libraries that this author's book is availible in. Only show libraries where its not checked out  
                    foreach (var book in author.Books)
                    {
                        book.Libraries = new List<Library>(dbConnection.Query<Library>("SELECT * FROM library, librarybook WHERE librarybook.bookid = " + 
                            book.BookId + " and librarybook.libraryid = library.libraryid and librarybook.checkedout = false"));
                    }
                }
                return author;
            }
        }

        /// <summary>
        /// Removes a Author from the database
        /// </summary>
        /// <param name="id">the id of the author to remove</param>
        /// <returns>a string detailing success or failure</returns>
        public string Remove(int id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                bool success = dbConnection.Execute("DELETE FROM author WHERE authorid=@Id", new { Id = id }) != 0;
                if (success)
                {
                    return "SUCCESS: Author with id: " + id + " deleted";
                }
                return "FAIL: Author with id: " + id + " not deleted";
            }
        }

        /// <summary>
        /// Updates an existing Author
        /// </summary>
        /// <param name="author">the author to update</param>
        /// <returns>a string detailing success or failure</returns>
        public string Update(Author author)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                bool success = dbConnection.Query("UPDATE author SET fname = @FName, lname = @LName WHERE authorid = @AuthorId RETURNING authorid", author).Count() != 0;
                if (success)
                {
                    return "SUCCESS: Author with id: " + author.AuthorId + " successfully updated";
                }
                return "FAIL: Author with id: " + author.AuthorId + " not updated";
            }
        }

        /// <summary>
        /// Adds a book to the author's list of books and adds the author to the book's list of authors
        /// </summary>
        /// <param name="bookAuthor">the identifying information for the book and the author</param>
        /// <returns>a string detailing success or failure</returns>
        public string AddBook(BookAuthor bookAuthor)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                //Checking to see if both the book and the author exist
                bool bookFound = dbConnection.Query("SELECT * FROM book WHERE bookid = " + bookAuthor.BookId).Count() != 0;
                bool authorFound = dbConnection.Query("SELECT * FROM author WHERE authorid = " + bookAuthor.AuthorId).Count() != 0;
                if (bookFound && authorFound)
                {
                    bool success = dbConnection.Execute("INSERT INTO bookauthor (bookid,authorid) VALUES (@BookId, @AuthorId)", bookAuthor) != 0;
                    if (success)
                    {
                        return "SUCCESS: Book with id: " + bookAuthor.BookId + " successfully added to Author with id: " + bookAuthor.AuthorId;
                    }
                }
                return "FAIL: Book with id: " + bookAuthor.BookId + " not added to Author with id: " + bookAuthor.AuthorId;
            }
        }

        /// <summary>
        /// Removes a book from the author list of books
        /// </summary>
        /// <param name="bookAuthor">the bookauthor to remove</param>
        /// <returns>a string detailing success or failure</returns>
        public string RemoveBook(BookAuthor bookAuthor)
        {
            using(IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                bool success = dbConnection.Execute("DELETE FROM bookauthor WHERE bookid=@BookId and authorid=@AuthorId", bookAuthor) != 0;
                if (success)
                {
                    return "SUCCESS: Book with id: " + bookAuthor.BookId + " successfully removed from the Author with id: " + bookAuthor.AuthorId;
                }
                return "FAIL: Book with id: " + bookAuthor.BookId + " not removed from the Author with id: " + bookAuthor.AuthorId;
            }
        }
    }
}
