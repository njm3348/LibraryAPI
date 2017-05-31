using System.Collections.Generic;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Dapper;
using WebApplication1.Model;


namespace WebApplication1.Repository
{
    /// <summary>
    /// Class to handle all database operations for Book
    /// </summary>
    public class BookRepository:BaseRepository
    {
        public BookRepository(IConfiguration configuration):base(configuration)
        {
        }

        /// <summary>
        /// Adds a new book to the database
        /// </summary>
        /// <param name="book">the book to add</param>
        /// <returns>the id of the newly added book</returns>
        public int Add(Book book)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<int>("INSERT INTO book (title,publisheddate) VALUES (@Title,@PublishedDate) RETURNING bookid", book).Single();

            }
        }

        /// <summary>
        /// Gets all the books in the database
        /// </summary>
        /// <returns>a list of books or an empty list</returns>
        public IEnumerable<Book> FindAll()
        {
            using(IDbConnection dbConnection = Connection){
                dbConnection.Open();
                var books = dbConnection.Query<Book>("SELECT * FROM book");
                foreach(var book in books)
                {
                    book.Authors = new List<Author>(dbConnection.Query<Author>("SELECT * FROM author,bookauthor WHERE bookauthor.bookid = " + book.BookId + " and bookauthor.authorid = author.authorid"));
                    //Getting which libraries the book is availible in. Only display instances where it isn't checked out
                    book.Libraries = new List<Library>(dbConnection.Query<Library>("SELECT * FROM library, librarybook WHERE librarybook.bookid = " + book.BookId + " and librarybook.libraryid = library.libraryid and librarybook.checkedout = false"));
                }
                return books;
            }
        }

        /// <summary>
        /// Gets a book with a given id
        /// </summary>
        /// <param name="id">the id of the book to get</param>
        /// <returns>a book or nothing</returns>
        public Book FindByID(int id)
        {
            using(IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                Book book = dbConnection.Query<Book>("SELECT * FROM book WHERE bookid = @BookId", new { BookId = id }).FirstOrDefault();
                if (book != null)
                {
                    book.Authors = new List<Author>(dbConnection.Query<Author>("SELECT * FROM author,bookauthor WHERE bookauthor.bookid = " + book.BookId + " and bookauthor.authorid = author.authorid"));
                    //Getting which libraries the book is availible in. Only display instances where it isn't checked out
                    book.Libraries = new List<Library>(dbConnection.Query<Library>("SELECT * FROM library, librarybook WHERE librarybook.bookid = " + book.BookId + " and librarybook.libraryid = library.libraryid and librarybook.checkedout = false"));
                }
                return book;
            }
        }

        /// <summary>
        /// Removes an existing book from the database
        /// </summary>
        /// <param name="id">the id of the book to remove</param>
        /// <returns>a string detailing success or failure</returns>
        public string Remove(int id)
        {
            using(IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                bool success = dbConnection.Execute("DELETE FROM book WHERE bookid=@Id", new { Id = id }) != 0;
                if (success)
                {
                    return "SUCCESS: Book with id: " + id + " deleted";
                }
                return "FAIL: Book with id: " + id + " not deleted";
            }
        }

        /// <summary>
        /// Updates an existing book
        /// </summary>
        /// <param name="book">the book to update</param>
        /// <returns>a string detailing success or failure</returns>
        public string Update(Book book)
        {
            using(IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                bool changed = dbConnection.Query("UPDATE book SET title = @Title, publisheddate = @PublishedDate WHERE bookid = @BookId RETURNING bookid", book).Count() != 0;
                if (changed)
                {
                    return "SUCCESS: Book with id: " + book.BookId + " successfully updated";
                }
                return "FAIL: Book with id: " + book.BookId + " not updated";
            }
        }

        /// <summary>
        /// Adds an author to a book
        /// </summary>
        /// <param name="bookAuthor">the bookauthor to add</param>
        /// <returns>a string detailing success or failure</returns>
        public string AddAuthor(BookAuthor bookAuthor)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                bool foundBook = dbConnection.Query("SELECT * FROM book WHERE bookid = " + bookAuthor.BookId).Count() != 0;
                bool foundAuthor = dbConnection.Query("SELECT * FROM author WHERE authorid = " + bookAuthor.AuthorId).Count() != 0;
                if (foundBook && foundAuthor)
                {
                    bool success = dbConnection.Execute("INSERT INTO bookauthor (bookid,authorid) VALUES (@BookId,@AuthorId)", bookAuthor) != 0;
                    if (success)
                    {
                        return "SUCCESS: Author with id: " + bookAuthor.AuthorId + " successfully added to Book with id: " + bookAuthor.BookId;
                    }
                }
                return "FAIL: Author with id: " + bookAuthor.AuthorId + " not added to Book with id: " + bookAuthor.BookId;
            }
        }

        /// <summary>
        /// Removes an author from a book
        /// </summary>
        /// <param name="bookAuthor">the bookauthor to remove</param>
        /// <returns>a string detailing success or failure</returns>
        public string RemoveAuthor(BookAuthor bookAuthor)
        {
            using(IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                bool success = dbConnection.Execute("DELETE FROM bookauthor WHERE bookid=@BookId and authorid=@AuthorId", bookAuthor) != 0;
                if (success)
                {
                    return "SUCCESS: Author with id: " + bookAuthor.AuthorId + " successfully removed from the Book with id: " + bookAuthor.BookId;
                }
                return "FAIL: Author with id: " + bookAuthor.AuthorId + " not removed from the Book with id: " + bookAuthor.BookId;
            }
        }
    }
}
