CREATE DATABASE IF NOT EXISTS AutoAdminDb;

USE AutoAdminDb;

CREATE TABLE IF NOT EXISTS `User` (
  `Id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `CreatedAt` DATETIME NOT NULL,
  `CreatedBy` varchar(255) NOT NULL,
  `UpdatedAt` DATETIME NULL,
  `UpdatedBy` varchar(255) DEFAULT NULL,  
  `FullName` varchar(255) NOT NULL,
  `UserName` varchar(255) NOT NULL,
  `BirthDate` DATE NOT NULL,
  `PasswordHash` varchar(255) NOT NULL,
  `PasswordSalt` varchar(255) NOT NULL,
  `Phone` varchar(255) DEFAULT NULL,
  `Email` varchar(255) NOT NULL, 
  `IsVerified` TINYINT NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `UserName_UNIQUE` (`UserName` ASC)
) ENGINE=InnoDB DEFAULT CHARSET=UTF8MB4;

DELETE 
FROM `User`;

INSERT INTO `User`
	(`Id`,`CreatedAt`,`CreatedBy`,`UpdatedAt`,`UpdatedBy`,`FullName`,`UserName`,`BirthDate`,`PasswordHash`,`PasswordSalt`,`Phone`,`Email`,`IsVerified`)
VALUES
(1, NOW(), "Anonymous", Null, Null, "Isia Thomas", "isiah", "1970-06-15", "", "", "+1 218-904-4574", "isia.thomas@gmail.com", 1),
(2, NOW(), "Anonymous", Null, Null, "Micheal Jordan", "jordan", "1970-04-15", "", "", "+1 209-536-1984", "jordan@gmail.com", 0),
(3, NOW(), "Anonymous", Null, Null, "Scotti Peppen", "scotti", "2000-04-01", "", "", "+1 228-632-2697", "scotti@gmail.com", 1),
(4, NOW(), "Anonymous", Null, Null, "Dennis Rodman", "theking", "1974-12-18", "", "", "+1 302-661-8376", "dennis@gmail.com", 0),
(5, NOW(), "Anonymous", Null, Null, "Patrick Ewing", "patrick", "1990-07-02", "", "", "+1 220-526-9580", "patrick@gmail.com", 1),
(6, NOW(), "Anonymous", Null, Null, "Hakeem Ulajuwan", "hakeem", "1986-02-15", "", "", "+1 201-460-1814", "hakeem@gmail.com", 0),
(7, NOW(), "Anonymous", Null, Null, "Karim Abduljabbar", "karim", "1988-07-08", "", "", "+1 210-668-7585", "karim@gmail.com", 1),
(8, NOW(), "Anonymous", Null, Null, "Toni Kukosh", "toni", "2004-01-02", "", "", "+1 818-308-9662", "toni@gmail.com", 1);

CREATE TABLE IF NOT EXISTS `Photo` (
  `Id` BINARY(16) PRIMARY KEY,
  `CreatedAt` DATETIME NOT NULL,
  `CreatedBy` varchar(255) NOT NULL,
  `UpdatedAt` DATETIME NULL,
  `UpdatedBy` varchar(255) DEFAULT NULL,
  `Name` varchar(255) NOT NULL,
  `FileId`  BINARY(16) NOT NULL  
) ENGINE=InnoDB DEFAULT CHARSET=UTF8MB4;

DELETE
FROM `Photo`;

INSERT INTO `Photo`
	(`Id`,`CreatedAt`,`CreatedBy`,`UpdatedAt`,`UpdatedBy`,`Name`,`FileId`)
VALUES
  (UUID_TO_BIN("a8a7cefb-651d-11eb-837c-7cd30a813cc1"), NOW(), "Anonymous", Null, Null, "alp", UUID_TO_BIN(UUID())),
  (UUID_TO_BIN("a8a7cfd2-651d-11eb-837c-7cd30a813cc1"), NOW(), "Anonymous", Null, Null, "amazon", UUID_TO_BIN(UUID())),
  (UUID_TO_BIN("a8a7d033-651d-11eb-837c-7cd30a813cc1"), NOW(), "Anonymous", Null, Null, "arizona", UUID_TO_BIN(UUID())),
  (UUID_TO_BIN("a8a7d08b-651d-11eb-837c-7cd30a813cc1"), NOW(), "Anonymous", Null, Null, "niagara", UUID_TO_BIN(UUID()));