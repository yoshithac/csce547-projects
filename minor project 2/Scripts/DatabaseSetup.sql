USE master;
GO

IF EXISTS (SELECT * FROM sys.databases WHERE name = 'WarehouseDB')
BEGIN
    ALTER DATABASE WarehouseDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE WarehouseDB;
END
GO

CREATE DATABASE WarehouseDB;
GO

USE WarehouseDB;
GO

-- NOTE: No ON DELETE CASCADE on purpose: deleting a Supplier or Product that is still referenced is blocked by the FK.
-- 1. Suppliers Table (Admin-Managed)
CREATE TABLE Suppliers (
    SupplierID INT IDENTITY(1,1) PRIMARY KEY,
    SupplierName NVARCHAR(100) NOT NULL,
    ContactEmail NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    City NVARCHAR(50) NOT NULL
);
GO

-- 2. Products Table (Search Target)
CREATE TABLE Products (
    ProductID INT IDENTITY(1,1) PRIMARY KEY,
    SKU NVARCHAR(30) NOT NULL UNIQUE,
    ProductName NVARCHAR(100) NOT NULL,
    Category NVARCHAR(50) NOT NULL,
    UnitPrice DECIMAL(10,2) NOT NULL,
    StockQuantity INT NOT NULL,
    SupplierID INT NOT NULL,
    CONSTRAINT FK_Products_Suppliers FOREIGN KEY (SupplierID) 
        REFERENCES Suppliers(SupplierID)
);
GO

-- 3. Orders Table
CREATE TABLE Orders (
    OrderID INT IDENTITY(1,1) PRIMARY KEY,
    CustomerName NVARCHAR(100) NOT NULL,
    OrderDate DATETIME NOT NULL DEFAULT GETDATE(),
    ProductID INT NOT NULL,
    Quantity INT NOT NULL,
    TotalAmount DECIMAL(10,2) NOT NULL,
    CONSTRAINT FK_Orders_Products FOREIGN KEY (ProductID) 
        REFERENCES Products(ProductID)
);
GO

-- Seed Data
INSERT INTO Suppliers (SupplierName, ContactEmail, Phone, City) VALUES
('Apex Global Logistics', 'contact@apexlogistics.com', '555-0101', 'Atlanta'),
('Pacific Tech Components', 'sales@pacifictech.com', '555-0102', 'Seattle'),
('Midwest Industrial Supply', 'orders@midwestind.com', '555-0103', 'Chicago');

INSERT INTO Products (SKU, ProductName, Category, UnitPrice, StockQuantity, SupplierID) VALUES
('ELEC-001', 'Wireless Optical Mouse', 'Electronics', 24.99, 120, 2),
('ELEC-002', 'Mechanical Gaming Keyboard', 'Electronics', 89.99, 45, 2),
('OFFC-101', 'Ergonomic Desk Chair', 'Furniture', 199.50, 18, 3),
('OFFC-102', 'Adjustable Standing Desk', 'Furniture', 349.00, 10, 3),
('IND-201', 'Heavy Duty Pallet Straps', 'Warehouse', 14.50, 250, 1),
('IND-202', 'Hydraulic Hand Truck', 'Warehouse', 285.00, 8, 1);

INSERT INTO Orders (CustomerName, OrderDate, ProductID, Quantity, TotalAmount) VALUES
('Acme Corp', '2026-09-10', 1, 5, 124.95),
('Nexus Systems', '2026-09-15', 2, 2, 179.98),
('Summit Health', '2026-09-20', 3, 4, 798.00),
('Vanguard Partners', '2026-09-25', 5, 20, 290.00);
GO