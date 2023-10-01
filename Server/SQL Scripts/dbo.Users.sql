CREATE TABLE [dbo].[Users] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Username] VARCHAR (25) NOT NULL,
    [Password] VARCHAR (50) NOT NULL,
    [UserType] INT          NOT NULL
);

