
DROP TABLE IF EXISTS Products;
DROP TABLE IF EXISTS AccountAdmins;
DROP TABLE IF EXISTS Employees;
DROP TABLE IF EXISTS CashierProducts;
DROP TABLE IF EXISTS CashierAdminMapping;

CREATE TABLE Employees(
	eId INT PRIMARY KEY IDENTITY(1,1),
	eName VARCHAR(225),
	eImage VARBINARY(MAX),
	eGender VARCHAR(25),
	ePosition VARCHAR(225),
	ePhoneNumber VARCHAR(100),
	eDateStartWork DATE,
	eSalary DECIMAL(10,2),
	eAddress VARCHAR(225),
	eStatus VARCHAR(50)
);
CREATE TABLE AccountAdmins(
	adminID INT PRIMARY KEY IDENTITY(1,1),
	adminName VARCHAR(225),
	adminPassword VARCHAR(225),
	adminPosition VARCHAR(25),
	adminStatus VARCHAR(25)
);
CREATE TABLE CashierAdminMapping(
    eId INT,
    adminID INT,
    PRIMARY KEY(eId, adminID),
    FOREIGN KEY(eId) REFERENCES Employees(eId),
    FOREIGN KEY(adminID) REFERENCES AccountAdmins(adminID)
);
CREATE TABLE Products(
	barCode INT PRIMARY KEY IDENTITY(1111,1),
	pName VARCHAR(225),
	pImage VARBINARY(MAX),
	pPrice DECIMAL(10,2),
	importDate DATE,
	pStatus VARCHAR(25)
);
CREATE TABLE CashierProducts (
    cashierProductId INT PRIMARY KEY IDENTITY(1,1),
    eId INT NOT NULL,
    barCode INT NOT NULL,
    assignDate DATE,

    FOREIGN KEY (eId) REFERENCES Employees(eId),
    FOREIGN KEY (barCode) REFERENCES Products(barCode)
);
INSERT INTO AccountAdmins(adminName, adminPassword, adminPosition, adminStatus)
	VALUES ('vanra','123','Manager','enable');
