CREATE TABLE [dbo].[Sources] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [Url]            NVARCHAR (500) NOT NULL,
    [Name]           NVARCHAR (200) NOT NULL,
    [Description]    NVARCHAR (500) NULL,
    [ComponentType]  NVARCHAR (100) NOT NULL,
    [RequiresSecret] BIT            DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

