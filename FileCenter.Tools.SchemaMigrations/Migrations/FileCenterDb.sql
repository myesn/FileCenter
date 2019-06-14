CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(95) NOT NULL,
    `ProductVersion` varchar(32) NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
);

CREATE TABLE `Files` (
    `Id` char(36) NOT NULL,
    `UserId` char(36) NOT NULL,
    `FileName` longtext NULL,
    `Extensions` longtext NULL,
    `PhysicalPath` longtext NULL,
    `ContentType` longtext NULL,
    `Length` bigint NOT NULL,
    `DownloadTimes` bigint NOT NULL,
    `IsPrivate` bit NOT NULL,
    `IsDeleted` bit NOT NULL,
    `UploadTime` datetime(6) NOT NULL,
    `DeleteTime` datetime(6) NOT NULL,
    CONSTRAINT `PK_Files` PRIMARY KEY (`Id`)
);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20190614165029_InitialFileCenterDbContextMigration', '2.2.4-servicing-10062');

