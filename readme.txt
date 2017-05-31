Nick Marchionda - API specification and startup

Before starting:
	-Run startup.sql on your database, this sets up all the tables and relationships but doesn't insert any data
	-In appsettings.json, modify the DBInfo so it points to your database:
	"DBInfo": {
        	"Name": "{Your Database name}",
        	"ConnectionString": "User ID={Your username};Password={Your Password};
				     Host=localhost;Port={Your database port};Database={Your database name};Pooling=true;"
    	} 

All API endpoints are prepended with: {hostname}:{portnumber}/api
	ex: localhost:51423/api/Author

Author endpoints:
GET .../Author
	description: Gets all the authors in the database and displays their information. If there are no authors then it returns an empty list
	return: a list of Author objects or an empty list

GET .../Author/{Some Author id}
	description: Gets an author with a given ID, if the Author isn't found returns nothing
	return: an Author object or nothing

POST .../Author
	description: Creates a new author on the database
	headers: Content-Type : application/json
	body: { "FName" : "{A name 30 characters or less",
		"LName" : "{A name 30 characters or less}"}
	return: the id of the added author

PUT .../Author/{Some Author id}
	description: Updates an existing author with new data, if it can't be found then does nothing
	headers: Content-Type: application/json
	body: { "FName" : "{A name 30 characters or less",
		"LName" : "{A name 30 characters or less}"}
	return: a string detailing success or failure

DELETE .../Author/{Some Author id}
	description: Deletes an existing author, if the author can't be found then does nothing
	return: a string detailing success or failure

PUT .../Author/addbook
	description: adds a book to an author's list of books (only adds a book if both the author and book exist)
		     (This operation works both ways, also adds onto the book's author list)
	headers: Content-Type: application/json
	body: { "AuthorId" : "{Some author id}",
		"BookId" : "{Some book id}"}
	return: a string detailing success or failure

DELETE .../Author/removebook
	description: removes a book from an author's list of books (only if both the author and book exist)
	headers: Content-Type: application/json
	body: { "AuthorId" : "{Some author id}",
		"BookId" : "{Some book id}"}
	return: a string detailing success or failure



Book endpoints:
GET .../Book
	description: Gets all the books in the database and displays their information. If there are no books then it returns an empty list
		(Will show the libraries that the book is available at, the libraries it exists in and is not checked out)
	return: a list of books objects or an empty list

GET .../Book/{Some Book id}
	description: Gets a Book with a given ID, if the Book isn't found returns nothing
		     (Will show the libraries that the book is available at, the libraries it exists in and is not checked out)
	return: a Book object or nothing

POST .../Book
	description: Creates a new book on the database
	headers: Content-Type : application/json
	body: { "Title" : "{A name 30 characters or less",
		"PublishedDate" : "{a Date in M/D/YYYY format}"}
	return: the id of the added book

PUT .../Book/{Some Book id}
	description: Updates an existing book with new data, if it can't be found then does nothing
	headers: Content-Type: application/json
	body: { "Title" : "{A name 30 characters or less",
		"PublishedDate" : "{a Date in M/D/YYYY format}"}
	return: a string detailing success or failure

DELETE .../Book/{Some Book id}
	description: Deletes an existing Book, if the Book can't be found then does nothing
	return: a string detailing success or failure

PUT .../Book/addauthor
	description: adds an author to a book's list of authors (only adds an author if both the author and book exist)
		     (This operation works both ways, also adds onto the author's book list)
	headers: Content-Type: application/json
	body: { "AuthorId" : "{Some author id}",
		"BookId" : "{Some book id}"}
	return: a string detailing success or failure

DELETE .../Book/removeauthor
	description: removes an author from a book's list of authors (only if both the author and book exist)
	headers: Content-Type: application/json
	body: { "AuthorId" : "{Some author id}",
		"BookId" : "{Some book id}"}
	return: a string detailing success or failure



Library endpoints:
GET .../Library
	description: Gets all the libraries in the database. If there are no libraries then it returns an empty list
		     (Will also show the books in the library that are not checked out)
	return: a list of library objects or an empty list

GET .../Library/{Some Library id}
	description: Gets a Library with a given ID, if the Library isn't found returns nothing
		     (Will also show the books in the library that are not checked out)
	return: a library object or nothing

POST .../Library
	description: Creates a new library on the database
	headers: Content-Type : application/json
	body: { "Name" : "{A name 30 characters or less}"}
	return: the id of the added library

PUT .../Library/{Some library id}
	description: Updates a library on the database, if it can't be found then does nothing
	headers: Content-Type : application/json
	body: { "Name" : "{A name 30 characters or less}"}
	return: a string detailing success or failure

DELETE .../Library/{Some library id}
	description: Deletes an existing Library, if the Library can't be found then does nothing
	return: a string detailing success or failure

PUT .../Library/addbook
	description: adds a book to a library(only adds if both the book and the library exist)
		     (This operation works both ways, adds to both the library's book list and the book's library list)
	return: a string detailing success or failure

DELETE .../Library/removebook
	description: removes a book from a library (only if both the library and book exist)
	headers: Content-Type : application/json
	body: { "LibraryId" : "{A library id}",
		"BookId" : "{A book id}"}



Patron endpoints:
GET .../Patron
	description: Gets all the Patrons in the database. If there are no patrons then it returns an empty list
	return: a list of library objects or an empty list

GET .../Patron/{Some patron id}
	description: Gets a Patron with a given ID, if the Patron isn't found returns nothing
	return: a Patron object or nothing

POST .../Patron
	description: Creates a new Patron on the database
	headers: Content-Type: application/json
	body: {"FName" : "{A name 30 characters or less}",
		"LName" : "{A name 30 characters or less}"}
	return: the id of the added patron

PUT .../Patron/{Some patron id}
	description: Updates an existing Patron on the database, if it can't be found then does nothing
	headers: Content-Type : application/json
	body: {"FName" : "{A name 30 characters or less}",
		"LName" : "{A name 30 characters or less}"}
	return: a string detailing success or failure
	
DELETE .../Patron/{Some patron id}
	description: Deletes an existing Patron, if the Patron can't be found then does nothing
	return: a string detailing success or failure

PUT .../Patron/checkout
	description: "Checks out" a book in a library. Marks the book as checked out so other patrons can't check it out.
		     (Only will check it out if the book exists in the given library not checked out and the patron exists)
	headers: Content-Type : application/json
	body: {"PatronId": "{the id of a patron}",
	       "BookId": "{the id of a book}",
	       "LibraryId":"{the id of a library}"}
	return: a string detailing success or failure

DELETE .../Patron/checkin
	description: "Checks in" a book to a library. Marks the book as checked in so other patrons can check it out.
	headers: Content-Type : application/json
	body: {"PatronId": "{the id of a patron}",
	       "BookId": "{the id of a book}",
	       "LibraryId":"{the id of a library}"}
	return: a string detailing success or failure	
