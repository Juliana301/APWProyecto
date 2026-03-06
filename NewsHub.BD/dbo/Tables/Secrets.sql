CREATE TABLE [dbo].[Secrets] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Key]         NVARCHAR (200) NOT NULL,
    [Value]       NVARCHAR (MAX) NOT NULL,
    [Description] NVARCHAR (500) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

