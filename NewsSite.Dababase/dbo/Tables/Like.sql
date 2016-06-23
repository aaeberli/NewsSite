CREATE TABLE [dbo].[Like] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [ArticleId]   INT            NOT NULL,
    [UserId]      NVARCHAR (128) NOT NULL,
    [CreatedDate] DATETIME       NOT NULL,
    CONSTRAINT [PK_Like] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Like_ToArticles] FOREIGN KEY ([ArticleId]) REFERENCES [dbo].[Articles] ([Id]),
    CONSTRAINT [FK_Like_ToUser] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]),
    CONSTRAINT [AK_Like] UNIQUE NONCLUSTERED ([ArticleId] ASC, [UserId] ASC)
);

