CREATE TABLE book (
  bookid SERIAL PRIMARY KEY,
  title VARCHAR(30),
  publisheddate date
);

CREATE TABLE author (
  authorid SERIAL PRIMARY KEY,
  fname VARCHAR(30),
  lname VARCHAR(30)
);

CREATE TABLE bookauthor(
  bookauthorid SERIAL PRIMARY KEY,
  bookid INTEGER REFERENCES book(bookid) ON DELETE CASCADE,
  authorid INTEGER REFERENCES author(authorid)ON DELETE CASCADE
);

CREATE TABLE library(
  libraryid SERIAL PRIMARY KEY,
  name VARCHAR(30)
);

CREATE TABLE librarybook(
  librarybookid SERIAL PRIMARY KEY,
  libraryid INTEGER REFERENCES library(libraryid) ON DELETE CASCADE,
  bookid INTEGER REFERENCES book(bookid) ON DELETE CASCADE,
  checkedout BOOLEAN DEFAULT FALSE
);

CREATE TABLE patron(
  patronid SERIAL PRIMARY KEY,
  fname VARCHAR(30),
  lname VARCHAR(30)
);

CREATE TABLE patronbook(
  patronbookid SERIAL PRIMARY KEY,
  patronid INTEGER REFERENCES patron(patronid) ON DELETE CASCADE,
  libraryid INTEGER REFERENCES library(libraryid) ON DELETE CASCADE,
  bookid INTEGER REFERENCES book(bookid) ON DELETE CASCADE
);