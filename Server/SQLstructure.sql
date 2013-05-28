
CREATE TABLE [levelupBackend].[Alphabet](
[ID] BIGINT IDENTITY (1, 1) NOT NULL,
[Logo] nvarchar(140) NOT NULL,
PRIMARY KEY CLUSTERED ([id] ASC) 
);

CREATE TABLE [levelupBackend].[AlphabetLocalization](
[ID] BIGINT IDENTITY (1, 1) NOT NULL,
[AlphabetID] BIGINT NOT NULL,
[LanguageID] nvarchar(8) NOT NULL,
[LanguageName] nvarchar(40) NOT NULL,
[Description] nvarchar(240) NULL,
PRIMARY KEY CLUSTERED ([ID] ASC) 
);


CREATE TABLE [levelupBackend].[Award] (
  [ID] BIGINT IDENTITY (1, 1) NOT NULL,
  [LogoPath] nvarchar(140) NOT NULL, 
  [Rate] INT NOT NULL,
  PRIMARY KEY CLUSTERED ([ID] ASC) );


CREATE TABLE [levelupBackend].[AwardLocalization](
[ID] BIGINT IDENTITY (1, 1) NOT NULL,
[AlphabetID] BIGINT NOT NULL,
[LanguageID] nvarchar(8) NOT NULL,
[AwardName] nvarchar(40) ,
[AwardDescription] nvarchar(240),
  PRIMARY KEY CLUSTERED ([ID] ASC) );


CREATE TABLE [levelupBackend].[User](
[ID] BIGINT IDENTITY (1, 1) NOT NULL,
[Name] nvarchar(140) ,
[Hash] nvarchar(40) ,
[Avatar] nvarchar(140),
PRIMARY KEY CLUSTERED ([ID] ASC) );


CREATE TABLE [levelupBackend].[UserAward] (
  [ID] BIGINT IDENTITY (1, 1) NOT NULL,
  [UserID] BIGINT, 
  [AwardID] BIGINT,
  PRIMARY KEY CLUSTERED ([ID] ASC) );




