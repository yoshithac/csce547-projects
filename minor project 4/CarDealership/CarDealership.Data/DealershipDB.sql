
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'DealershipDB')
    CREATE DATABASE DealershipDB;
GO

USE DealershipDB;
GO

-- 1. DROP EXISTING TABLES IN FK DEPENDENCY ORDER
IF OBJECT_ID('dbo.Vehicles', 'U') IS NOT NULL DROP TABLE dbo.Vehicles;
IF OBJECT_ID('dbo.Salespeople', 'U') IS NOT NULL DROP TABLE dbo.Salespeople;
IF OBJECT_ID('dbo.Dealerships', 'U') IS NOT NULL DROP TABLE dbo.Dealerships;
GO

-- 2. DDL: CREATE TABLES
CREATE TABLE Dealerships (
    DealershipId INT IDENTITY(1,1) PRIMARY KEY,
    BranchName NVARCHAR(100) NOT NULL,
    City NVARCHAR(50) NOT NULL,
    State NVARCHAR(10) NOT NULL,
    Phone NVARCHAR(20) NOT NULL
);

CREATE TABLE Salespeople (
    SalespersonId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    CommissionRate DECIMAL(5,2) NOT NULL,
    DealershipId INT NOT NULL FOREIGN KEY REFERENCES Dealerships(DealershipId) ON DELETE CASCADE
);

CREATE TABLE Vehicles (
    VehicleId INT IDENTITY(1,1) PRIMARY KEY,
    VIN NVARCHAR(17) NOT NULL,
    Make NVARCHAR(50) NOT NULL,
    Model NVARCHAR(50) NOT NULL,
    Year INT NOT NULL,
    Price DECIMAL(12,2) NOT NULL,
    Color NVARCHAR(30) NOT NULL,
    Mileage INT NOT NULL,
    DealershipId INT NOT NULL FOREIGN KEY REFERENCES Dealerships(DealershipId) ON DELETE CASCADE
);
GO

-- 3. DML: SEED DEALERSHIPS (30 RECORDS)
INSERT INTO Dealerships (BranchName, City, State, Phone) VALUES
('Metro Toyota & Lexus', 'Columbia', 'SC', '803-555-0101'),
('Apex Performance Auto', 'Charlotte', 'NC', '704-555-0102'),
('Harbor Motors', 'Charleston', 'SC', '843-555-0103'),
('Midtown Motorcars', 'Atlanta', 'GA', '404-555-0104'),
('Piedmont Auto Group', 'Greenville', 'SC', '864-555-0105'),
('Coastal Ford & Lincoln', 'Savannah', 'GA', '912-555-0106'),
('Palmetto Luxury Imports', 'Lexington', 'SC', '803-555-0107'),
('Queen City Pre-Owned', 'Raleigh', 'NC', '919-555-0108'),
('Lowcountry Auto Mall', 'Mount Pleasant', 'SC', '843-555-0109'),
('Blue Ridge Subaru', 'Asheville', 'NC', '828-555-0110'),
('Triangle European Motors', 'Durham', 'NC', '919-555-0111'),
('Peach State Chevrolet', 'Macon', 'GA', '478-555-0112'),
('Sun City Honda', 'Augusta', 'GA', '706-555-0113'),
('Grand Strand Auto Group', 'Myrtle Beach', 'SC', '843-555-0114'),
('Spartan City Motors', 'Spartanburg', 'SC', '864-555-0115'),
('Smoky Mountain CDJR', 'Knoxville', 'TN', '865-555-0116'),
('Riverfront Motors', 'Chattanooga', 'TN', '423-555-0117'),
('Capital City Nissan', 'Columbia', 'SC', '803-555-0118'),
('Sandhills Ford', 'Florence', 'SC', '843-555-0119'),
('Cape Fear Auto Gallery', 'Wilmington', 'NC', '910-555-0120'),
('Triad Fleet & Commercial', 'Greensboro', 'NC', '336-555-0121'),
('Winston Select Auto', 'Winston-Salem', 'NC', '336-555-0122'),
('Athens Classic Motors', 'Athens', 'GA', '706-555-0123'),
('Catawba Valley Motors', 'Rock Hill', 'SC', '803-555-0124'),
('Golden Isles Auto Group', 'Brunswick', 'GA', '912-555-0125'),
('Cherokee Ridge Toyota', 'Canton', 'GA', '770-555-0126'),
('Foothills Mazda', 'Anderson', 'SC', '864-555-0127'),
('Old Town Auto Exchange', 'Aiken', 'SC', '803-555-0128'),
('Tarheel Hyundai', 'Chapel Hill', 'NC', '919-555-0129'),
('Lake Norman Elite Auto', 'Mooresville', 'NC', '704-555-0130');
GO

-- 4. DML: SEED SALESPEOPLE (30 RECORDS)
INSERT INTO Salespeople (FullName, Email, Phone, CommissionRate, DealershipId) VALUES
('Marcus Rivera', 'mrivera@metroauto.com', '803-555-0201', 4.50, 1),
('Sarah Jenkins', 'sjenkins@metroauto.com', '803-555-0202', 5.00, 1),
('David Zhao', 'dzhao@apexauto.com', '704-555-0203', 6.00, 2),
('Elena Rostova', 'erostova@apexauto.com', '704-555-0204', 5.50, 2),
('Christopher Vance', 'cvance@harbormotors.com', '843-555-0205', 4.75, 3),
('Hannah Abbott', 'habbott@harbormotors.com', '843-555-0206', 5.25, 3),
('Tariq Jordan', 'tjordan@midtowncars.com', '404-555-0207', 5.00, 4),
('Megan Keller', 'mkeller@midtowncars.com', '404-555-0208', 4.50, 4),
('Bradley Cooper', 'bcooper@piedmontauto.com', '864-555-0209', 5.50, 5),
('Alicia Patel', 'apatel@piedmontauto.com', '864-555-0210', 6.25, 5),
('Garrett Hayes', 'ghayes@coastalauto.com', '912-555-0211', 4.25, 6),
('Whitney Scott', 'wscott@coastalauto.com', '912-555-0212', 5.00, 6),
('Nathaniel Drake', 'ndrake@palmettoauto.com', '803-555-0213', 5.75, 7),
('Claire Redfield', 'credfield@queencity.com', '919-555-0214', 4.80, 8),
('Leon Kennedy', 'lkennedy@lowcountry.com', '843-555-0215', 5.20, 9),
('Jill Valentine', 'jvalentine@blueridge.com', '828-555-0216', 5.10, 10),
('Carlos Oliveira', 'coliveira@triangle.com', '919-555-0217', 4.90, 11),
('Rebecca Chambers', 'rchambers@peachstate.com', '478-555-0218', 4.60, 12),
('Chris Redfield', 'credfield2@suncity.com', '706-555-0219', 5.30, 13),
('Barry Burton', 'bburton@grandstrand.com', '843-555-0220', 4.75, 14),
('Sherry Birkin', 'sbirkin@spartancity.com', '864-555-0221', 4.40, 15),
('Ada Wong', 'awong@smokymtn.com', '865-555-0222', 6.50, 16),
('Albert Wesker', 'awesker@riverfront.com', '423-555-0223', 6.00, 17),
('Sheva Alomar', 'salomar@capitalcity.com', '803-555-0224', 5.00, 18),
('Josh Stone', 'jstone@sandhills.com', '843-555-0225', 4.50, 19),
('Piers Nivans', 'pnivans@capefear.com', '910-555-0226', 4.80, 20),
('Helena Harper', 'hharper@triadfleet.com', '336-555-0227', 5.20, 21),
('Jake Muller', 'jmuller@winstonselect.com', '336-555-0228', 5.40, 22),
('Finn Macauley', 'fmacauley@athensclassic.com', '706-555-0229', 4.30, 23),
('Mia Winters', 'mwinters@catawbavalley.com', '803-555-0230', 5.15, 24);
GO

-- 5. DML: SEED VEHICLES (30 RECORDS)
INSERT INTO Vehicles (VIN, Make, Model, Year, Price, Color, Mileage, DealershipId) VALUES
('1HGCR2F83HA100001', 'Honda', 'Accord EX-L', 2021, 24500.00, 'Modern Steel', 31200, 1),
('4T1B11HK5MU100002', 'Toyota', 'Camry SE', 2022, 26800.00, 'Celestial Silver', 19800, 1),
('JTJHY7AX8M4100003', 'Lexus', 'RX 350', 2023, 48900.00, 'Eminent White', 11400, 1),
('WP0AB2A98NS100004', 'Porsche', '911 Carrera', 2022, 112000.00, 'Guards Red', 8500, 2),
('WBA33AY08NF100005', 'BMW', 'M3 Competition', 2023, 84500.00, 'Isle of Man Green', 6200, 2),
('1FTEW1E48PK100006', 'Ford', 'F-150 Lariat', 2023, 56000.00, 'Agate Black', 14500, 3),
('1G1YY2D75P5100007', 'Chevrolet', 'Corvette Stingray', 2023, 76500.00, 'Rapid Blue', 4100, 3),
('WAUZZZF27NA100008', 'Audi', 'A6 Premium Plus', 2022, 43900.00, 'Mythos Black', 22100, 4),
('JM3KFBDL3N0100009', 'Mazda', 'CX-5 Carbon', 2022, 28400.00, 'Polymetal Gray', 18300, 4),
('KM8K33A45NU100010', 'Hyundai', 'Tucson Limited', 2022, 29700.00, 'Amazon Gray', 25600, 4),
('5NPE34AF8PH100011', 'Hyundai', 'Sonata N-Line', 2023, 33200.00, 'Portofino Gray', 9200, 5),
('3FA6P0G79LR100012', 'Ford', 'Fusion Titanium', 2020, 18900.00, 'Oxford White', 52400, 5),
('1FTFW1ED4N1100013', 'Ford', 'F-150 Lightning', 2023, 62000.00, 'Antimatter Blue', 10100, 6),
('2C3CDXHG7NH100014', 'Dodge', 'Charger Scat Pack', 2022, 47800.00, 'Pitch Black', 15800, 6),
('3VW2B7AJ9NM100015', 'Volkswagen', 'Jetta GLI', 2022, 27500.00, 'Tornado Red', 21400, 6),
('5YJ3E1EB8NF100016', 'Tesla', 'Model 3 Long Range', 2022, 36900.00, 'Midnight Silver', 24100, 7),
('1G11C5SL8NF100017', 'Chevrolet', 'Malibu LT', 2022, 21800.00, 'Summit White', 34200, 8),
('JN1AZ4EH9NM100018', 'Nissan', 'Rogue SV', 2022, 25400.00, 'Gun Metallic', 29000, 9),
('4S4WAAFC6N3100019', 'Subaru', 'Outback Premium', 2022, 29900.00, 'Autumn Green', 17800, 10),
('WAUDG4F47PA100020', 'Audi', 'Q5 S-Line', 2023, 44200.00, 'Navarra Blue', 12300, 11),
('3GCUKREC5NG100021', 'Chevrolet', 'Silverado 1500 RST', 2022, 46500.00, 'Cherry Red', 26400, 12),
('2HKRW2H87NH100022', 'Honda', 'CR-V Touring', 2022, 33400.00, 'Platinum White', 22100, 13),
('1C4PJLCB3PD100023', 'Jeep', 'Grand Cherokee L', 2023, 51200.00, 'Baltic Gray', 14900, 14),
('1FA6P8CF5N5100024', 'Ford', 'Mustang GT', 2022, 41800.00, 'Velocity Blue', 16700, 15),
('1C4HJXDG9PW100025', 'Jeep', 'Wrangler Rubicon', 2023, 53900.00, 'Sarge Green', 11300, 16),
('WBA0E3C09P5100026', 'BMW', 'X5 xDrive40i', 2023, 61500.00, 'Phytonic Blue', 13800, 17),
('1N4BL4BV9NN100027', 'Nissan', 'Altima SR', 2022, 23700.00, 'Super Black', 31000, 18),
('1FDOW4HT6NE100028', 'Ford', 'Explorer Timberline', 2022, 42100.00, 'Forged Green', 28400, 19),
('5N1DL0MN8NC100029', 'Infiniti', 'QX60 Luxe', 2022, 45800.00, 'Mineral Black', 19600, 20),
('KNDCD3LD7N7100030', 'Kia', 'K5 GT-Line', 2022, 26400.00, 'Glacier White', 23200, 21);
GO

-- 6. VERIFICATION QUERY (Run this to verify counts in SSMS)
SELECT 'Dealerships' AS TableName, COUNT(*) AS TotalCount FROM Dealerships
UNION ALL
SELECT 'Salespeople' AS TableName, COUNT(*) AS TotalCount FROM Salespeople
UNION ALL
SELECT 'Vehicles' AS TableName, COUNT(*) AS TotalCount FROM Vehicles;