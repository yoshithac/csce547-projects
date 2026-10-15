USE master;
GO
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'MusicStoreDb')
BEGIN
    ALTER DATABASE MusicStoreDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE MusicStoreDb;
END
GO
CREATE DATABASE MusicStoreDb;
GO
USE MusicStoreDb;
GO

-- DDL
-- an artist that still has albums cannot be deleted.
CREATE TABLE Artists (            -- admin-only table
    ArtistId    INT IDENTITY(1,1) PRIMARY KEY,
    Name        NVARCHAR(100) NOT NULL UNIQUE,
    Country     NVARCHAR(50)  NOT NULL,
    ActiveSince INT NOT NULL
);

CREATE TABLE Albums (             -- search target
    AlbumId     INT IDENTITY(1,1) PRIMARY KEY,
    Title       NVARCHAR(150) NOT NULL,
    Genre       NVARCHAR(50)  NOT NULL,
    ReleaseYear INT NOT NULL,
    Price       DECIMAL(8,2) NOT NULL,
    Stock       INT NOT NULL,
    ArtistId    INT NOT NULL,
    CONSTRAINT FK_Albums_Artists FOREIGN KEY (ArtistId) REFERENCES Artists(ArtistId)
);

CREATE TABLE Customers (
    CustomerId INT IDENTITY(1,1) PRIMARY KEY,
    FirstName  NVARCHAR(50)  NOT NULL,
    LastName   NVARCHAR(50)  NOT NULL,
    Email      NVARCHAR(100) NOT NULL,
    Phone      NVARCHAR(20)  NULL,
    City       NVARCHAR(50)  NULL
);
GO

-- DML
-- Note: artists 31 (Sade) and 32 (Joni Mitchell) have no albums, so they can be deleted in Admin Mode;
--       every other artist has albums, so deleting them is blocked by the foreign key.
INSERT INTO Artists (Name, Country, ActiveSince) VALUES
(N'Fleetwood Mac', N'UK/USA', 1967),
(N'Daft Punk', N'France', 1993),
(N'Radiohead', N'UK', 1985),
(N'Beyonc' + NCHAR(233), N'USA', 1997),
(N'Tame Impala', N'Australia', 2007),
(N'Bj' + NCHAR(246) + N'rk', N'Iceland', 1977),
(N'Nirvana', N'USA', 1987),
(N'Kendrick Lamar', N'USA', 2003),
(N'Taylor Swift', N'USA', 2004),
(N'Adele', N'UK', 2006),
(N'Arcade Fire', N'Canada', 2001),
(N'Bob Marley & The Wailers', N'Jamaica', 1963),
(N'David Bowie', N'UK', 1962),
(N'Prince', N'USA', 1976),
(N'Michael Jackson', N'USA', 1964),
(N'Pink Floyd', N'UK', 1965),
(N'The Beatles', N'UK', 1960),
(N'Led Zeppelin', N'UK', 1968),
(N'Stevie Wonder', N'USA', 1961),
(N'Amy Winehouse', N'UK', 2003),
(N'Billie Eilish', N'USA', 2015),
(N'Frank Ocean', N'USA', 2009),
(N'Lana Del Rey', N'USA', 2010),
(N'Gorillaz', N'UK', 1998),
(N'The Strokes', N'USA', 1998),
(N'Miles Davis', N'USA', 1944),
(N'Nina Simone', N'USA', 1954),
(N'Fela Kuti', N'Nigeria', 1958),
(N'Rosal' + NCHAR(237) + N'a', N'Spain', 2013),
(N'Bad Bunny', N'Puerto Rico', 2016),
(N'Sade', N'UK', 1982),
(N'Joni Mitchell', N'Canada', 1964);

INSERT INTO Albums (Title, Genre, ReleaseYear, Price, Stock, ArtistId) VALUES
(N'Rumours', N'Rock', 1977, 17.99, 4, 1),
(N'Tango in the Night', N'Rock', 1987, 19.99, 11, 1),
(N'Discovery', N'Electronic', 2001, 19.99, 18, 2),
(N'Random Access Memories', N'Electronic', 2013, 21.99, 25, 2),
(N'OK Computer', N'Alternative', 1997, 21.99, 7, 3),
(N'Kid A', N'Alternative', 2000, 23.99, 14, 3),
(N'In Rainbows', N'Alternative', 2007, 23.99, 21, 3),
(N'Lemonade', N'R&B', 2016, 25.99, 28, 4),
(N'Renaissance', N'Dance', 2022, 25.99, 10, 4),
(N'Currents', N'Psychedelic Pop', 2015, 27.99, 17, 5),
(N'Lonerism', N'Psychedelic Rock', 2012, 27.99, 24, 5),
(N'Homogenic', N'Electronic', 1997, 29.99, 6, 6),
(N'Vespertine', N'Electronic', 2001, 17.99, 13, 6),
(N'Nevermind', N'Grunge', 1991, 19.99, 20, 7),
(N'In Utero', N'Grunge', 1993, 19.99, 27, 7),
(N'good kid, m.A.A.d city', N'Hip-Hop', 2012, 21.99, 9, 8),
(N'To Pimp a Butterfly', N'Hip-Hop', 2015, 21.99, 16, 8),
(N'1989', N'Pop', 2014, 23.99, 23, 9),
(N'Folklore', N'Indie Folk', 2020, 23.99, 5, 9),
(N'21', N'Pop', 2011, 25.99, 12, 10),
(N'30', N'Pop', 2021, 25.99, 19, 10),
(N'Funeral', N'Indie Rock', 2004, 27.99, 26, 11),
(N'Legend', N'Reggae', 1984, 27.99, 8, 12),
(N'Heroes', N'Rock', 1977, 29.99, 15, 13),
(N'Low', N'Rock', 1977, 17.99, 22, 13),
(N'Purple Rain', N'Pop', 1984, 19.99, 4, 14),
(N'Thriller', N'Pop', 1982, 19.99, 11, 15),
(N'The Dark Side of the Moon', N'Progressive Rock', 1973, 21.99, 18, 16),
(N'Abbey Road', N'Rock', 1969, 21.99, 25, 17),
(N'Led Zeppelin IV', N'Rock', 1971, 23.99, 7, 18),
(N'Songs in the Key of Life', N'Soul', 1976, 23.99, 14, 19),
(N'Back to Black', N'Soul', 2006, 25.99, 21, 20),
(N'Happier Than Ever', N'Pop', 2021, 25.99, 28, 21),
(N'Blonde', N'R&B', 2016, 27.99, 10, 22),
(N'Born to Die', N'Pop', 2012, 27.99, 17, 23),
(N'Demon Days', N'Alternative', 2005, 29.99, 24, 24),
(N'Is This It', N'Rock', 2001, 17.99, 6, 25),
(N'Kind of Blue', N'Jazz', 1959, 19.99, 13, 26),
(N'I Put a Spell on You', N'Jazz', 1965, 19.99, 20, 27),
(N'Zombie', N'Afrobeat', 1976, 21.99, 27, 28),
(N'Motomami', N'Pop', 2022, 21.99, 9, 29),
(N'Un Verano Sin Ti', N'Latin', 2022, 23.99, 16, 30);

INSERT INTO Customers (FirstName, LastName, Email, Phone, City) VALUES
(N'Maya', N'Johnson', N'maya.johnson@example.com', N'803-555-0100', N'Columbia'),
(N'Liam', N'Carter', N'liam.carter@example.com', N'864-555-0101', N'Lexington'),
(N'Sofia', N'Nguyen', N'sofia.nguyen@example.com', N'843-555-0102', N'Charleston'),
(N'Ethan', N'Brooks', N'ethan.brooks@example.com', N'803-555-0103', N'Greenville'),
(N'Ava', N'Patel', N'ava.patel@example.com', NULL, N'Irmo'),
(N'Noah', N'Rivera', N'noah.rivera@example.com', N'843-555-0105', N'Cayce'),
(N'Isabella', N'Kim', N'isabella.kim@example.com', N'803-555-0106', N'Sumter'),
(N'Mason', N'Davis', N'mason.davis@example.com', N'864-555-0107', N'Spartanburg'),
(N'Olivia', N'Turner', N'olivia.turner@example.com', N'843-555-0108', N'Rock Hill'),
(N'Lucas', N'Reed', N'lucas.reed@example.com', N'803-555-0109', N'Florence'),
(N'Emma', N'Foster', N'emma.foster@example.com', N'864-555-0110', N'Columbia'),
(N'Jacob', N'Hughes', N'jacob.hughes@example.com', N'843-555-0111', N'Lexington'),
(N'Mia', N'Bailey', N'mia.bailey@example.com', N'803-555-0112', N'Charleston'),
(N'Logan', N'Cooper', N'logan.cooper@example.com', NULL, N'Greenville'),
(N'Harper', N'Diaz', N'harper.diaz@example.com', N'843-555-0114', N'Irmo'),
(N'Elijah', N'Evans', N'elijah.evans@example.com', N'803-555-0115', N'Cayce'),
(N'Amelia', N'Flores', N'amelia.flores@example.com', N'864-555-0116', N'Sumter'),
(N'James', N'Garcia', N'james.garcia@example.com', N'843-555-0117', N'Spartanburg'),
(N'Evelyn', N'Hayes', N'evelyn.hayes@example.com', N'803-555-0118', N'Rock Hill'),
(N'Henry', N'Ingram', N'henry.ingram@example.com', N'864-555-0119', N'Florence'),
(N'Abigail', N'Jenkins', N'abigail.jenkins@example.com', N'843-555-0120', N'Columbia'),
(N'Jack', N'Knight', N'jack.knight@example.com', N'803-555-0121', N'Lexington'),
(N'Ella', N'Lopez', N'ella.lopez@example.com', NULL, N'Charleston'),
(N'Owen', N'Mitchell', N'owen.mitchell@example.com', N'843-555-0123', N'Greenville'),
(N'Scarlett', N'Nelson', N'scarlett.nelson@example.com', N'803-555-0124', N'Irmo'),
(N'Wyatt', N'Ortiz', N'wyatt.ortiz@example.com', N'864-555-0125', N'Cayce'),
(N'Grace', N'Perry', N'grace.perry@example.com', N'843-555-0126', N'Sumter'),
(N'Caleb', N'Quinn', N'caleb.quinn@example.com', N'803-555-0127', N'Spartanburg'),
(N'Chloe', N'Ross', N'chloe.ross@example.com', N'864-555-0128', N'Rock Hill'),
(N'Isaac', N'Sanders', N'isaac.sanders@example.com', N'843-555-0129', N'Florence'),
(N'Lily', N'Thomas', N'lily.thomas@example.com', N'803-555-0130', N'Columbia'),
(N'Nathan', N'Underwood', N'nathan.underwood@example.com', NULL, N'Lexington'),
(N'Zoe', N'Vargas', N'zoe.vargas@example.com', N'843-555-0132', N'Charleston'),
(N'Andre', N'Walker', N'andre.walker@example.com', N'803-555-0133', N'Greenville'),
(N'Priya', N'Shah', N'priya.shah@example.com', N'864-555-0134', N'Irmo'),
(N'Marcus', N'Young', N'marcus.young@example.com', N'843-555-0135', N'Cayce');
GO