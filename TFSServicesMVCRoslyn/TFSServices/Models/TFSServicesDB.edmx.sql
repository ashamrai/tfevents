
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 04/01/2018 19:06:10
-- Generated from EDMX file: C:\Users\useradmin\Source\Repos\tfevents\TFSServicesMVCRoslyn\TFSServices\Models\TFSServicesDB.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [TFSServicesDB];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_RuleTypeRules]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RulesSet] DROP CONSTRAINT [FK_RuleTypeRules];
GO
IF OBJECT_ID(N'[dbo].[FK_WebMethodRules]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RulesSet] DROP CONSTRAINT [FK_WebMethodRules];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[RulesSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RulesSet];
GO
IF OBJECT_ID(N'[dbo].[RuleTypeSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RuleTypeSet];
GO
IF OBJECT_ID(N'[dbo].[WebMethodSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WebMethodSet];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'RulesSet'
CREATE TABLE [dbo].[RulesSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [IsActive] bit  NOT NULL,
    [Title] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [TriggerScript] nvarchar(max)  NOT NULL,
    [ProcessScript] nvarchar(max)  NOT NULL,
    [RuleTypeId] int  NOT NULL,
    [WebMethodId] int  NOT NULL
);
GO

-- Creating table 'RuleTypeSet'
CREATE TABLE [dbo].[RuleTypeSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NULL
);
GO

-- Creating table 'WebMethodSet'
CREATE TABLE [dbo].[WebMethodSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'RulesSet'
ALTER TABLE [dbo].[RulesSet]
ADD CONSTRAINT [PK_RulesSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'RuleTypeSet'
ALTER TABLE [dbo].[RuleTypeSet]
ADD CONSTRAINT [PK_RuleTypeSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WebMethodSet'
ALTER TABLE [dbo].[WebMethodSet]
ADD CONSTRAINT [PK_WebMethodSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [RuleTypeId] in table 'RulesSet'
ALTER TABLE [dbo].[RulesSet]
ADD CONSTRAINT [FK_RuleTypeRules]
    FOREIGN KEY ([RuleTypeId])
    REFERENCES [dbo].[RuleTypeSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RuleTypeRules'
CREATE INDEX [IX_FK_RuleTypeRules]
ON [dbo].[RulesSet]
    ([RuleTypeId]);
GO

-- Creating foreign key on [WebMethodId] in table 'RulesSet'
ALTER TABLE [dbo].[RulesSet]
ADD CONSTRAINT [FK_WebMethodRules]
    FOREIGN KEY ([WebMethodId])
    REFERENCES [dbo].[WebMethodSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_WebMethodRules'
CREATE INDEX [IX_FK_WebMethodRules]
ON [dbo].[RulesSet]
    ([WebMethodId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------