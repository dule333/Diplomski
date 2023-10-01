CREATE TABLE [dbo].[Outages] (
    [OutageId]        INT       IDENTITY (1,1) PRIMARY KEY,
    [OutagePoints]    VARCHAR (300) NOT NULL,
    [OutageStartTime] VARCHAR (150) NOT NULL,
    [OutageEndTime]   VARCHAR (150) NOT NULL,
    [OutageType]      INT           NOT NULL
);

