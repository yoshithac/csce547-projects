-- =============================================
-- Creates the DealershipDB database (run once, before 02_DealershipDB.sql).
-- Skips creation if a database with this name already exists.
-- =============================================
IF DB_ID(N'DealershipDB') IS NULL
BEGIN
    CREATE DATABASE DealershipDB;
END
GO
