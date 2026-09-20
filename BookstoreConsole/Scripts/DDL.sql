-- 1. Create the database from scratch
USE master;
GO

IF EXISTS (SELECT * FROM sys.databases WHERE name = 'BookstoreDB')
BEGIN
    ALTER DATABASE BookstoreDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE BookstoreDB;
END
GO

CREATE DATABASE BookstoreDB;
GO

USE BookstoreDB;
GO

-- 2. Create the tables with primary and foreign keys
CREATE TABLE Authors (
    AuthorID INT IDENTITY(1,1) PRIMARY KEY,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    Country VARCHAR(50),
    BirthYear INT
);

CREATE TABLE Books (
    BookID INT IDENTITY(1,1) PRIMARY KEY,
    Title VARCHAR(150) NOT NULL,
    AuthorID INT NOT NULL,
    Genre VARCHAR(50),
    PublishedYear INT,
    Price DECIMAL(6,2) NOT NULL,
    FOREIGN KEY (AuthorID) REFERENCES Authors(AuthorID)
);
GO