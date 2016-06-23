CREATE TABLE [dbo].[News] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Title]       VARCHAR (50)   NOT NULL,
    [Body]        VARCHAR (MAX)  NOT NULL,
    [CreatedDate] DATETIME       NOT NULL,
    [UpdatedDate] DATETIME       NULL,
    [AuthorId]    NVARCHAR (128) NOT NULL,
    CONSTRAINT [PK_News] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_News_ToUser] FOREIGN KEY ([AuthorId]) REFERENCES [dbo].[AspNetUsers] ([Id])
);

