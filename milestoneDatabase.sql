CREATE TABLE [dbo].[users] (
    [Id]        INT           IDENTITY (1, 1) NOT NULL,
    [firstName] NVARCHAR (50) NOT NULL,
    [lastName]  NVARCHAR (50) NOT NULL,
    [gender]    NVARCHAR (50) NULL,
    [age]       INT           NULL,
    [state]     NVARCHAR (50) NULL,
    [email]     NVARCHAR (50) NOT NULL,
    [username]  NVARCHAR (50) NOT NULL,
    [password]  NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    UNIQUE NONCLUSTERED ([username] ASC)
);

CREATE TABLE [dbo].[games] (
    [Id]         INT           IDENTITY (1, 1) NOT NULL,
    [gameString] TEXT          NULL,
    [userID]     INT           NULL,
    [datePlayed] DATETIME      NULL,
    [level]      NVARCHAR (50) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
