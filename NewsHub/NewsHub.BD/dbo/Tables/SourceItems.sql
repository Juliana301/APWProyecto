CREATE TABLE [dbo].[SourceItems] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [SourceId]  INT            NOT NULL,
    [Json]      NVARCHAR (MAX) NOT NULL,
    [CreatedAt] DATETIME       DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SourceItems_Sources] FOREIGN KEY ([SourceId]) REFERENCES [dbo].[Sources] ([Id])
);

