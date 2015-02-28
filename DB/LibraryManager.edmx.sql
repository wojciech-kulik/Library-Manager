
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 02/28/2015 20:38:01
-- Generated from EDMX file: D:\GitHub\Library-Manager\DB\LibraryManager.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [LibraryManager];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_ClientLending]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Lendings] DROP CONSTRAINT [FK_ClientLending];
GO
IF OBJECT_ID(N'[dbo].[FK_BookBookCategory_Book]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BookBookCategory] DROP CONSTRAINT [FK_BookBookCategory_Book];
GO
IF OBJECT_ID(N'[dbo].[FK_BookBookCategory_BookCategory]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BookBookCategory] DROP CONSTRAINT [FK_BookBookCategory_BookCategory];
GO
IF OBJECT_ID(N'[dbo].[FK_LendingEmployee]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Lendings] DROP CONSTRAINT [FK_LendingEmployee];
GO
IF OBJECT_ID(N'[dbo].[FK_LentBookBook]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[LentBooks] DROP CONSTRAINT [FK_LentBookBook];
GO
IF OBJECT_ID(N'[dbo].[FK_LentBookLending]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[LentBooks] DROP CONSTRAINT [FK_LentBookLending];
GO
IF OBJECT_ID(N'[dbo].[FK_LentBookEmployee]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[LentBooks] DROP CONSTRAINT [FK_LentBookEmployee];
GO
IF OBJECT_ID(N'[dbo].[FK_PublisherBook]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Books] DROP CONSTRAINT [FK_PublisherBook];
GO
IF OBJECT_ID(N'[dbo].[FK_AuthorBook_Author]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AuthorBook] DROP CONSTRAINT [FK_AuthorBook_Author];
GO
IF OBJECT_ID(N'[dbo].[FK_AuthorBook_Book]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AuthorBook] DROP CONSTRAINT [FK_AuthorBook_Book];
GO
IF OBJECT_ID(N'[dbo].[FK_Client_inherits_Person]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Persons_Client] DROP CONSTRAINT [FK_Client_inherits_Person];
GO
IF OBJECT_ID(N'[dbo].[FK_Employee_inherits_Person]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Persons_Employee] DROP CONSTRAINT [FK_Employee_inherits_Person];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Books]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Books];
GO
IF OBJECT_ID(N'[dbo].[Persons]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Persons];
GO
IF OBJECT_ID(N'[dbo].[Lendings]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Lendings];
GO
IF OBJECT_ID(N'[dbo].[BookCategories]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BookCategories];
GO
IF OBJECT_ID(N'[dbo].[LentBooks]', 'U') IS NOT NULL
    DROP TABLE [dbo].[LentBooks];
GO
IF OBJECT_ID(N'[dbo].[Publishers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Publishers];
GO
IF OBJECT_ID(N'[dbo].[Authors]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Authors];
GO
IF OBJECT_ID(N'[dbo].[Persons_Client]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Persons_Client];
GO
IF OBJECT_ID(N'[dbo].[Persons_Employee]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Persons_Employee];
GO
IF OBJECT_ID(N'[dbo].[BookBookCategory]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BookBookCategory];
GO
IF OBJECT_ID(N'[dbo].[AuthorBook]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AuthorBook];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Books'
CREATE TABLE [dbo].[Books] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Title] nvarchar(max)  NOT NULL,
    [ISBN] nvarchar(max)  NULL,
    [HardCover] bit  NOT NULL,
    [PublishDate] datetime  NULL,
    [Quantity] smallint  NOT NULL,
    [Location] nvarchar(max)  NULL,
    [PublisherId] int  NULL,
    [AdditionalInfo] nvarchar(max)  NULL,
    [Removed] bit  NOT NULL
);
GO

-- Creating table 'Persons'
CREATE TABLE [dbo].[Persons] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FirstName] nvarchar(max)  NOT NULL,
    [LastName] nvarchar(max)  NOT NULL,
    [Phone] nvarchar(max)  NULL,
    [Email] nvarchar(max)  NULL,
    [IdNumber] nvarchar(max)  NULL,
    [Address_Street] nvarchar(max)  NULL,
    [Address_HouseNumber] nvarchar(max)  NULL,
    [Address_ApartmentNumber] nvarchar(max)  NULL,
    [Address_PostalCode] nvarchar(max)  NULL,
    [Address_City] nvarchar(max)  NULL
);
GO

-- Creating table 'Lendings'
CREATE TABLE [dbo].[Lendings] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [EndDate] datetime  NOT NULL,
    [LendingDate] datetime  NOT NULL,
    [ReturnDate] datetime  NULL,
    [ClientId] int  NOT NULL,
    [LendingEmployeeId] int  NOT NULL
);
GO

-- Creating table 'BookCategories'
CREATE TABLE [dbo].[BookCategories] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'LentBooks'
CREATE TABLE [dbo].[LentBooks] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ReturnDate] datetime  NULL,
    [EndDate] datetime  NOT NULL,
    [BookId] int  NOT NULL,
    [LendingId] int  NOT NULL,
    [ReturnEmployeeId] int  NULL
);
GO

-- Creating table 'Publishers'
CREATE TABLE [dbo].[Publishers] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Authors'
CREATE TABLE [dbo].[Authors] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Persons_Client'
CREATE TABLE [dbo].[Persons_Client] (
    [CardNumber] nvarchar(max)  NULL,
    [AdditionalInfo] nvarchar(max)  NULL,
    [Id] int  NOT NULL
);
GO

-- Creating table 'Persons_Employee'
CREATE TABLE [dbo].[Persons_Employee] (
    [Role] tinyint  NOT NULL,
    [Username] nvarchar(max)  NOT NULL,
    [Password] nvarchar(max)  NOT NULL,
    [Removed] bit  NOT NULL,
    [Id] int  NOT NULL
);
GO

-- Creating table 'BookBookCategory'
CREATE TABLE [dbo].[BookBookCategory] (
    [BookBookCategory_BookCategory_Id] int  NOT NULL,
    [BookCategories_Id] int  NOT NULL
);
GO

-- Creating table 'AuthorBook'
CREATE TABLE [dbo].[AuthorBook] (
    [Authors_Id] int  NOT NULL,
    [AuthorBook_Author_Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Books'
ALTER TABLE [dbo].[Books]
ADD CONSTRAINT [PK_Books]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Persons'
ALTER TABLE [dbo].[Persons]
ADD CONSTRAINT [PK_Persons]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Lendings'
ALTER TABLE [dbo].[Lendings]
ADD CONSTRAINT [PK_Lendings]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'BookCategories'
ALTER TABLE [dbo].[BookCategories]
ADD CONSTRAINT [PK_BookCategories]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'LentBooks'
ALTER TABLE [dbo].[LentBooks]
ADD CONSTRAINT [PK_LentBooks]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Publishers'
ALTER TABLE [dbo].[Publishers]
ADD CONSTRAINT [PK_Publishers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Authors'
ALTER TABLE [dbo].[Authors]
ADD CONSTRAINT [PK_Authors]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Persons_Client'
ALTER TABLE [dbo].[Persons_Client]
ADD CONSTRAINT [PK_Persons_Client]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Persons_Employee'
ALTER TABLE [dbo].[Persons_Employee]
ADD CONSTRAINT [PK_Persons_Employee]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [BookBookCategory_BookCategory_Id], [BookCategories_Id] in table 'BookBookCategory'
ALTER TABLE [dbo].[BookBookCategory]
ADD CONSTRAINT [PK_BookBookCategory]
    PRIMARY KEY CLUSTERED ([BookBookCategory_BookCategory_Id], [BookCategories_Id] ASC);
GO

-- Creating primary key on [Authors_Id], [AuthorBook_Author_Id] in table 'AuthorBook'
ALTER TABLE [dbo].[AuthorBook]
ADD CONSTRAINT [PK_AuthorBook]
    PRIMARY KEY CLUSTERED ([Authors_Id], [AuthorBook_Author_Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [ClientId] in table 'Lendings'
ALTER TABLE [dbo].[Lendings]
ADD CONSTRAINT [FK_ClientLending]
    FOREIGN KEY ([ClientId])
    REFERENCES [dbo].[Persons_Client]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ClientLending'
CREATE INDEX [IX_FK_ClientLending]
ON [dbo].[Lendings]
    ([ClientId]);
GO

-- Creating foreign key on [BookBookCategory_BookCategory_Id] in table 'BookBookCategory'
ALTER TABLE [dbo].[BookBookCategory]
ADD CONSTRAINT [FK_BookBookCategory_Book]
    FOREIGN KEY ([BookBookCategory_BookCategory_Id])
    REFERENCES [dbo].[Books]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [BookCategories_Id] in table 'BookBookCategory'
ALTER TABLE [dbo].[BookBookCategory]
ADD CONSTRAINT [FK_BookBookCategory_BookCategory]
    FOREIGN KEY ([BookCategories_Id])
    REFERENCES [dbo].[BookCategories]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_BookBookCategory_BookCategory'
CREATE INDEX [IX_FK_BookBookCategory_BookCategory]
ON [dbo].[BookBookCategory]
    ([BookCategories_Id]);
GO

-- Creating foreign key on [LendingEmployeeId] in table 'Lendings'
ALTER TABLE [dbo].[Lendings]
ADD CONSTRAINT [FK_LendingEmployee]
    FOREIGN KEY ([LendingEmployeeId])
    REFERENCES [dbo].[Persons_Employee]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_LendingEmployee'
CREATE INDEX [IX_FK_LendingEmployee]
ON [dbo].[Lendings]
    ([LendingEmployeeId]);
GO

-- Creating foreign key on [BookId] in table 'LentBooks'
ALTER TABLE [dbo].[LentBooks]
ADD CONSTRAINT [FK_LentBookBook]
    FOREIGN KEY ([BookId])
    REFERENCES [dbo].[Books]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_LentBookBook'
CREATE INDEX [IX_FK_LentBookBook]
ON [dbo].[LentBooks]
    ([BookId]);
GO

-- Creating foreign key on [LendingId] in table 'LentBooks'
ALTER TABLE [dbo].[LentBooks]
ADD CONSTRAINT [FK_LentBookLending]
    FOREIGN KEY ([LendingId])
    REFERENCES [dbo].[Lendings]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_LentBookLending'
CREATE INDEX [IX_FK_LentBookLending]
ON [dbo].[LentBooks]
    ([LendingId]);
GO

-- Creating foreign key on [ReturnEmployeeId] in table 'LentBooks'
ALTER TABLE [dbo].[LentBooks]
ADD CONSTRAINT [FK_LentBookEmployee]
    FOREIGN KEY ([ReturnEmployeeId])
    REFERENCES [dbo].[Persons_Employee]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_LentBookEmployee'
CREATE INDEX [IX_FK_LentBookEmployee]
ON [dbo].[LentBooks]
    ([ReturnEmployeeId]);
GO

-- Creating foreign key on [PublisherId] in table 'Books'
ALTER TABLE [dbo].[Books]
ADD CONSTRAINT [FK_PublisherBook]
    FOREIGN KEY ([PublisherId])
    REFERENCES [dbo].[Publishers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PublisherBook'
CREATE INDEX [IX_FK_PublisherBook]
ON [dbo].[Books]
    ([PublisherId]);
GO

-- Creating foreign key on [Authors_Id] in table 'AuthorBook'
ALTER TABLE [dbo].[AuthorBook]
ADD CONSTRAINT [FK_AuthorBook_Author]
    FOREIGN KEY ([Authors_Id])
    REFERENCES [dbo].[Authors]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [AuthorBook_Author_Id] in table 'AuthorBook'
ALTER TABLE [dbo].[AuthorBook]
ADD CONSTRAINT [FK_AuthorBook_Book]
    FOREIGN KEY ([AuthorBook_Author_Id])
    REFERENCES [dbo].[Books]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AuthorBook_Book'
CREATE INDEX [IX_FK_AuthorBook_Book]
ON [dbo].[AuthorBook]
    ([AuthorBook_Author_Id]);
GO

-- Creating foreign key on [Id] in table 'Persons_Client'
ALTER TABLE [dbo].[Persons_Client]
ADD CONSTRAINT [FK_Client_inherits_Person]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Persons]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'Persons_Employee'
ALTER TABLE [dbo].[Persons_Employee]
ADD CONSTRAINT [FK_Employee_inherits_Person]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Persons]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------