
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 04/07/2018 17:21:05
-- Generated from EDMX file: C:\Users\useradmin\Source\Repos\tfevents\TFSServices\TFSServicesDBLib\TFSServicesDB.edmx
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
IF OBJECT_ID(N'[dbo].[FK_RulesRunHistory]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RunHistorySet] DROP CONSTRAINT [FK_RulesRunHistory];
GO
IF OBJECT_ID(N'[dbo].[FK_RulesRulesRevisions]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RevisionsSet] DROP CONSTRAINT [FK_RulesRulesRevisions];
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
IF OBJECT_ID(N'[dbo].[RunHistorySet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RunHistorySet];
GO
IF OBJECT_ID(N'[dbo].[RevisionsSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RevisionsSet];
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
    [Revision] int  NOT NULL,
    [IsDeleted] bit  NOT NULL
);
GO

-- Creating table 'RuleTypeSet'
CREATE TABLE [dbo].[RuleTypeSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NULL
);
GO

-- Creating table 'RunHistorySet'
CREATE TABLE [dbo].[RunHistorySet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Date] datetime  NOT NULL,
    [Result] nvarchar(max)  NOT NULL,
    [RulesId] int  NOT NULL,
    [RuleRevision] int  NOT NULL,
    [Message] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'RevisionsSet'
CREATE TABLE [dbo].[RevisionsSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Date] datetime  NOT NULL,
    [UserName] nvarchar(max)  NOT NULL,
    [TriggerScript] nvarchar(max)  NOT NULL,
    [ProcessScript] nvarchar(max)  NOT NULL,
    [RulesId] int  NOT NULL,
    [Revision] int  NOT NULL,
    [Operation] nvarchar(10)  NOT NULL
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

-- Creating primary key on [Id] in table 'RunHistorySet'
ALTER TABLE [dbo].[RunHistorySet]
ADD CONSTRAINT [PK_RunHistorySet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'RevisionsSet'
ALTER TABLE [dbo].[RevisionsSet]
ADD CONSTRAINT [PK_RevisionsSet]
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

-- Creating foreign key on [RulesId] in table 'RunHistorySet'
ALTER TABLE [dbo].[RunHistorySet]
ADD CONSTRAINT [FK_RulesRunHistory]
    FOREIGN KEY ([RulesId])
    REFERENCES [dbo].[RulesSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RulesRunHistory'
CREATE INDEX [IX_FK_RulesRunHistory]
ON [dbo].[RunHistorySet]
    ([RulesId]);
GO

-- Creating foreign key on [RulesId] in table 'RevisionsSet'
ALTER TABLE [dbo].[RevisionsSet]
ADD CONSTRAINT [FK_RulesRulesRevisions]
    FOREIGN KEY ([RulesId])
    REFERENCES [dbo].[RulesSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RulesRulesRevisions'
CREATE INDEX [IX_FK_RulesRulesRevisions]
ON [dbo].[RevisionsSet]
    ([RulesId]);
GO

INSERT INTO [dbo].[RuleTypeSet]
           ([Name]
           ,[Description])
     VALUES
           ('WorkItemUpdated',
           'Rules for changing of work item')
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------