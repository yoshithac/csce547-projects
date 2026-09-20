USE BookstoreDB;
GO

-- 3. Populate Sample Data
INSERT INTO Authors (FirstName, LastName, Country, BirthYear) VALUES
('George', 'Orwell', 'United Kingdom', 1903),
('Jane', 'Austen', 'United Kingdom', 1775),
('Gabriel', 'Garcia Marquez', 'Colombia', 1927),
('Toni', 'Morrison', 'United States', 1931),
('Haruki', 'Murakami', 'Japan', 1949),
('Chinua', 'Achebe', 'Nigeria', 1930),
('Margaret', 'Atwood', 'Canada', 1939),
('Isaac', 'Asimov', 'United States', 1920),
('Agatha', 'Christie', 'United Kingdom', 1890),
('Octavia', 'Butler', 'United States', 1947);

INSERT INTO Books (Title, AuthorID, Genre, PublishedYear, Price) VALUES
('1984', 1, 'Dystopian', 1949, 14.99),
('Animal Farm', 1, 'Satire', 1945, 9.99),
('Homage to Catalonia', 1, 'Memoir', 1938, 13.50),
('Pride and Prejudice', 2, 'Romance', 1813, 8.99),
('Emma', 2, 'Romance', 1815, 9.49),
('Sense and Sensibility', 2, 'Romance', 1811, 9.49),
('One Hundred Years of Solitude', 3, 'Magical Realism', 1967, 16.00),
('Love in the Time of Cholera', 3, 'Romance', 1985, 15.50),
('Beloved', 4, 'Historical Fiction', 1987, 15.99),
('Song of Solomon', 4, 'Literary Fiction', 1977, 14.99),
('The Bluest Eye', 4, 'Literary Fiction', 1970, 13.99),
('Norwegian Wood', 5, 'Literary Fiction', 1987, 15.00),
('Kafka on the Shore', 5, 'Magical Realism', 2002, 16.50),
('The Wind-Up Bird Chronicle', 5, 'Magical Realism', 1994, 17.00),
('Things Fall Apart', 6, 'Historical Fiction', 1958, 12.99),
('Arrow of God', 6, 'Historical Fiction', 1964, 12.99),
('The Handmaid''s Tale', 7, 'Dystopian', 1985, 15.95),
('Oryx and Crake', 7, 'Science Fiction', 2003, 16.00),
('Alias Grace', 7, 'Historical Fiction', 1996, 15.00),
('Foundation', 8, 'Science Fiction', 1951, 10.99),
('I, Robot', 8, 'Science Fiction', 1950, 10.99),
('The Caves of Steel', 8, 'Science Fiction', 1954, 9.99),
('Murder on the Orient Express', 9, 'Mystery', 1934, 9.99),
('And Then There Were None', 9, 'Mystery', 1939, 9.99),
('Kindred', 10, 'Science Fiction', 1979, 14.99);
GO

-- 4. Verify data creation
SELECT 'Authors Loaded' AS Status, COUNT(*) AS Total FROM Authors;
SELECT 'Books Loaded' AS Status, COUNT(*) AS Total FROM Books;
GO