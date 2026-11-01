-- Copy of the DealershipDB script reused from a previous project.
-- Run 01_CreateDatabase.sql first. This script drops and re-creates the three tables, then reloads the seed data.
USE DealershipDB;
GO

-- =============================================
-- DDL: Table Definitions & Foreign Key Constraints
-- =============================================

IF OBJECT_ID('dbo.Vehicle', 'U') IS NOT NULL DROP TABLE dbo.Vehicle;
IF OBJECT_ID('dbo.Salesperson', 'U') IS NOT NULL DROP TABLE dbo.Salesperson;
IF OBJECT_ID('dbo.Dealership', 'U') IS NOT NULL DROP TABLE dbo.Dealership;

CREATE TABLE Dealership (
    DealershipId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    City NVARCHAR(50) NOT NULL
);

CREATE TABLE Salesperson (
    SalespersonId INT IDENTITY(1,1) PRIMARY KEY,
    DealershipId INT NOT NULL FOREIGN KEY REFERENCES Dealership(DealershipId) ON DELETE CASCADE,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL
);

CREATE TABLE Vehicle (
    VehicleId INT IDENTITY(1,1) PRIMARY KEY,
    DealershipId INT NOT NULL FOREIGN KEY REFERENCES Dealership(DealershipId) ON DELETE CASCADE,
    Vin NVARCHAR(50) NOT NULL,
    Make NVARCHAR(50) NOT NULL,
    Model NVARCHAR(50) NOT NULL,
    Year INT NOT NULL,
    Price DECIMAL(10,2) NOT NULL
);

-- =============================================
-- DML: Seed Records (50+ Rows Per Table)
-- =============================================

-- 1. DEALERSHIP (50 Rows)
INSERT INTO Dealership (Name, City) VALUES 
('Apex Motors', 'Charlotte'),
('Metro Auto Mall', 'Raleigh'),
('Summit Imports', 'Columbia'),
('Palmetto State Auto', 'Charleston'),
('Queen City Car Hub', 'Charlotte'),
('Piedmont Motor Group', 'Greensboro'),
('Triangle Luxury Auto', 'Durham'),
('Lowcountry Auto Sales', 'Mount Pleasant'),
('Blue Ridge Fleet', 'Asheville'),
('Carolina Coast Motors', 'Wilmington'),
('Greenville Speed Shop', 'Greenville'),
('Midlands Auto Park', 'Columbia'),
('Cape Fear Auto Outlet', 'Wilmington'),
('Forsyth Pre-Owned', 'Winston-Salem'),
('Spartanburg City Motors', 'Spartanburg'),
('Rock Hill Car Co', 'Rock Hill'),
('High Point Car Gallery', 'High Point'),
('Concord Speedway Autos', 'Concord'),
('Gastonia Express Auto', 'Gastonia'),
('Cary Elite Motors', 'Cary'),
('Chapel Hill Automotive', 'Chapel Hill'),
('Apex Auto Exchange', 'Apex'),
('Sumter Drive Center', 'Sumter'),
('Florence Auto Plaza', 'Florence'),
('Myrtle Beach Car Barn', 'Myrtle Beach'),
('Hickory Ridge Motors', 'Hickory'),
('Burlington Auto Line', 'Burlington'),
('Kannapolis Pre-Owned', 'Kannapolis'),
('Mooresville Lake Auto', 'Mooresville'),
('Matthews Auto Group', 'Matthews'),
('Huntersville Car Source', 'Huntersville'),
('Monroe Motor Depot', 'Monroe'),
('Salisbury Express Cars', 'Salisbury'),
('New Bern Marina Motors', 'New Bern'),
('Goldsboro Fleet Center', 'Goldsboro'),
('Wilson City Auto', 'Wilson'),
('Rocky Mount Motors', 'Rocky Mount'),
('Greenville East Auto', 'Greenville'),
('Jacksonville Coast Cars', 'Jacksonville'),
('Fayetteville Patriot Auto', 'Fayetteville'),
('Lumberton Highway Sales', 'Lumberton'),
('Aiken Horsepower Auto', 'Aiken'),
('Anderson Premier Motors', 'Anderson'),
('Greer Car Exchange', 'Greer'),
('Mauldin Motor Works', 'Mauldin'),
('Goose Creek Automotive', 'Goose Creek'),
('Hanahan Auto Mall', 'Hanahan'),
('Clemson Valley Auto', 'Clemson'),
('Boone High Country Cars', 'Boone'),
('Southern Pines Motor Co', 'Southern Pines');

-- 2. SALESPERSON (50 Rows)
INSERT INTO Salesperson (DealershipId, FullName, Email) VALUES 
(1, 'Jordan Reed', 'jordan.reed@apexmotors.com'),
(1, 'Sarah Chen', 'sarah.chen@apexmotors.com'),
(2, 'Sam Taylor', 'sam.taylor@metroautomall.com'),
(2, 'David Miller', 'david.miller@metroautomall.com'),
(3, 'Morgan Vance', 'morgan.vance@summitimports.com'),
(3, 'Elena Rostova', 'elena.rostova@summitimports.com'),
(4, 'Marcus Holloway', 'marcus.h@palmettoauto.com'),
(5, 'Chloe Bennett', 'cbennett@queencitycars.com'),
(6, 'Trevor Philips', 'tphilips@piedmontmotorgroup.com'),
(7, 'Rachel Green', 'rgreen@triangleluxury.com'),
(8, 'Liam Gallagher', 'liam.g@lowcountryautos.com'),
(9, 'Noah Calhoun', 'noah.c@blueridgefleet.com'),
(10, 'Hannah Abbott', 'hannah.a@carolinacoast.com'),
(11, 'Arthur Pendelton', 'arthur.p@greenvillespeed.com'),
(12, 'Lucas Scott', 'lscott@midlandsautopark.com'),
(13, 'Brooke Davis', 'bdavis@capefearauto.com'),
(14, 'Haley James', 'hjames@forsythpreowned.com'),
(15, 'Nathan Scott', 'nscott@spartanburgmotors.com'),
(16, 'Peyton Sawyer', 'psawyer@rockhillcars.com'),
(17, 'Julian Baker', 'jbaker@highpointgallery.com'),
(18, 'Clay Evans', 'cevans@concordspeedway.com'),
(19, 'Quinn James', 'qjames@gastoniaexpress.com'),
(20, 'Alex Dupre', 'adupre@caryelitemotors.com'),
(21, 'Mia Catalano', 'mcatalano@chapelhillauto.com'),
(22, 'Chase Adams', 'cadams@apexautoexchange.com'),
(23, 'Millicent Huxtable', 'mhuxtable@sumterdrive.com'),
(24, 'Mouth McFadden', 'mmcfadden@florenceplaza.com'),
(25, 'Skills Taylor', 'staylor@myrtlebeachcars.com'),
(26, 'Bevin Mirskey', 'bmirskey@hickoryridge.com'),
(27, 'Tim Smith', 'tsmith@burlingtonline.com'),
(28, 'Jake Jagielski', 'jjagielski@kannapolisauto.com'),
(29, 'Keith Scott', 'kscott@mooresvillelake.com'),
(30, 'Karen Roe', 'kroe@matthewsauto.com'),
(31, 'Andy Hargrove', 'ahargrove@huntersvillecars.com'),
(32, 'Deb Scott', 'dscott@monroedepot.com'),
(33, 'Dan Scott', 'danscott@salisburycars.com'),
(34, 'Whitey Durham', 'wdurham@newbernmarina.com'),
(35, 'Chris Keller', 'ckeller@goldsborofleet.com'),
(36, 'Rachel Gatina', 'rgatina@wilsoncityauto.com'),
(37, 'Cooper Lee', 'clee@rockymountmotors.com'),
(38, 'Shelley Simon', 'ssimon@greenvilleeast.com'),
(39, 'Gigi Silveri', 'gsilveri@jacksonvillecoast.com'),
(40, 'Ian Banks', 'ibanks@fayettevillepatriot.com'),
(41, 'Victoria Davis', 'vdavis@lumbertonhighway.com'),
(42, 'Xavier Daniels', 'xdaniels@aikenhorse.com'),
(43, 'Miranda Stone', 'mstone@andersonpremier.com'),
(44, 'Owen Morello', 'omorello@greerexchange.com'),
(45, 'Tara Kelly', 'tkelly@mauldinworks.com'),
(46, 'Logan Evans', 'levans@goosecreekauto.com'),
(47, 'Chuck Scolnick', 'cscolnick@hanahanmall.com');

-- 3. VEHICLE (50 Rows)
INSERT INTO Vehicle (DealershipId, Vin, Make, Model, Year, Price) VALUES 
(1, '1HGCR2F83HA100001', 'Honda', 'Accord', 2021, 24500.00),
(1, '1N4AL3AP8JC100002', 'Nissan', 'Altima', 2020, 19800.00),
(2, '4T1B11HK5JU100003', 'Toyota', 'Camry', 2022, 26800.00),
(2, 'JM3KFBCM9L1100004', 'Mazda', 'CX-5', 2023, 29500.00),
(3, '1FTEW1EP8KK100005', 'Ford', 'F-150', 2020, 32000.00),
(3, '3C6UR5DL7MG100006', 'Ram', '1500', 2021, 34500.00),
(4, 'WAUZZZ8V8KA100007', 'Audi', 'A4', 2022, 38900.00),
(5, 'WBA33AY08NP100008', 'BMW', '330i', 2023, 42500.00),
(6, '5YJ3E1EB8NF100009', 'Tesla', 'Model 3', 2022, 36000.00),
(7, 'KM8K33A45NU100010', 'Hyundai', 'Tucson', 2021, 23500.00),
(8, 'KNDPB3AC4N7100011', 'Kia', 'Sportage', 2023, 27400.00),
(9, '2T3P1RFV7NC100012', 'Toyota', 'RAV4', 2022, 28900.00),
(10, '1G1ZE5ST8NF100013', 'Chevrolet', 'Malibu', 2020, 18200.00),
(11, '1GC4YNEY6NF100014', 'Chevrolet', 'Silverado 1500', 2021, 37500.00),
(12, '1FM5K8GC8MG100015', 'Ford', 'Explorer', 2021, 33200.00),
(13, 'JN1CV7AP8NM100016', 'Infiniti', 'Q50', 2022, 31000.00),
(14, 'SALWR2V45NA100017', 'Land Rover', 'Defender', 2023, 58000.00),
(15, 'WDDSJ4EB3NF100018', 'Mercedes-Benz', 'C300', 2022, 41200.00),
(16, 'YV4A22PK8N1100019', 'Volvo', 'XC60', 2023, 44800.00),
(17, 'JH4TC1H58NA100020', 'Acura', 'RDX', 2022, 35600.00),
(18, '3FA6P0H78HR100021', 'Ford', 'Fusion', 2019, 15800.00),
(19, '2T1BURHE7KC100022', 'Toyota', 'Corolla', 2020, 19200.00),
(20, '1HGCP2F84NA100023', 'Honda', 'Civic', 2022, 22900.00),
(21, '3VW2B7AJ9NM100024', 'Volkswagen', 'Jetta', 2021, 20400.00),
(22, '3VV3B7AX5NE100025', 'Volkswagen', 'Tiguan', 2022, 25800.00),
(23, '4S4WAAFC6N3100026', 'Subaru', 'Outback', 2022, 28700.00),
(24, 'JF1VMAC63NH100027', 'Subaru', 'Forester', 2021, 26100.00),
(25, '1C4PJLCB8ND100028', 'Jeep', 'Cherokee', 2020, 21900.00),
(26, '1C4HJXDG9NW100029', 'Jeep', 'Wrangler', 2022, 39500.00),
(27, '1C4RJFLT4NC100030', 'Jeep', 'Grand Cherokee', 2022, 40200.00),
(28, '5J8TC1H36NL100031', 'Acura', 'MDX', 2022, 45100.00),
(29, 'WA1L2AFY5N2100032', 'Audi', 'Q5', 2023, 43700.00),
(30, 'WBA5R1C56N8100033', 'BMW', 'X3', 2022, 42100.00),
(31, '1G1FH1R71N0100034', 'Chevrolet', 'Camaro', 2021, 31800.00),
(32, '1FA6P8CF9N5100035', 'Ford', 'Mustang', 2022, 33500.00),
(33, '2C3CDXBG7NH100036', 'Dodge', 'Charger', 2021, 29400.00),
(34, '2C3CDZAG5NH100037', 'Dodge', 'Challenger', 2022, 32100.00),
(35, '1GKS1GKD8NR100038', 'GMC', 'Yukon', 2021, 56000.00),
(36, '1GT49TEY3NF100039', 'GMC', 'Sierra 1500', 2022, 41900.00),
(37, 'KM8K22AA5NU100040', 'Hyundai', 'Elantra', 2023, 21200.00),
(38, 'KM8J33A42NU100041', 'Hyundai', 'Santa Fe', 2022, 28300.00),
(39, 'KNDCE3LG8N5100042', 'Kia', 'K5', 2022, 24100.00),
(40, 'KNDR25A24N7100043', 'Kia', 'Telluride', 2023, 41500.00),
(41, 'JTJHY7AX8N4100044', 'Lexus', 'RX 350', 2022, 46800.00),
(42, 'JTHBK1GG4N2100045', 'Lexus', 'ES 350', 2023, 43900.00),
(43, 'JM1BPACL4N1100046', 'Mazda', 'Mazda3', 2022, 23700.00),
(44, 'JM3TBAA75N0100047', 'Mazda', 'CX-9', 2021, 31400.00),
(45, 'W1K3G5FB3NF100048', 'Mercedes-Benz', 'GLC 300', 2022, 45200.00),
(46, '1N4BL4BV7NC100049', 'Nissan', 'Sentra', 2022, 19500.00),
(47, '5N1DL1NB6NC100050', 'Nissan', 'Rogue', 2022, 26900.00),
(48, '3FA6P0H79HR100051', 'Nissan', 'Frontier', 2021, 28500.00),
(49, '1GNSCKKD9NR100052', 'Chevrolet', 'Tahoe', 2022, 54900.00),
(50, '1FMSK8DH7NGA00053', 'Ford', 'Edge', 2022, 31200.00);

-- test
SELECT 
    (SELECT COUNT(*) FROM Dealership) AS DealershipCount,
    (SELECT COUNT(*) FROM Salesperson) AS SalespersonCount,
    (SELECT COUNT(*) FROM Vehicle) AS VehicleCount;