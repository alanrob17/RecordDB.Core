USE [RecordDB]
GO
/****** Object:  UserDefinedTableType [dbo].[TrackTableType]    Script Date: 20/08/2026 9:57:33 PM ******/
CREATE TYPE [dbo].[TrackTableType] AS TABLE(
	[DiscId] [int] NULL,
	[TrackNo] [int] NULL,
	[Name] [varchar](255) NULL,
	[TrackLength] [int] NULL,
	[Extended] [varchar](255) NULL
)
GO
/****** Object:  UserDefinedFunction [dbo].[ConvertTimeToHHMMSS]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE function [dbo].[ConvertTimeToHHMMSS]
(
    @time decimal(28,3), 
    @unit varchar(20)
)
returns varchar(20)
as
begin

    declare @seconds decimal(18,3), @minutes int, @hours int;

    if(@unit = 'hour' or @unit = 'hh' )
        set @seconds = @time * 60 * 60;
    else if(@unit = 'minute' or @unit = 'mi' or @unit = 'n')
        set @seconds = @time * 60;
    else if(@unit = 'second' or @unit = 'ss' or @unit = 's')
        set @seconds = @time;
    else set @seconds = 0; -- unknown time units

    set @hours = convert(int, @seconds /60 / 60);
    set @minutes = convert(int, (@seconds / 60) - (@hours * 60 ));
    set @seconds = @seconds % 60;

    return 
        convert(varchar(9), convert(int, @hours)) + ':' +
        right('00' + convert(varchar(2), convert(int, @minutes)), 2) + ':' +
        right('00' + convert(varchar(6), @seconds), 6)

end

-- USAGE:
/*
select dbo.ConvertTimeToHHMMSS(123, 's')
select dbo.ConvertTimeToHHMMSS(96.999, 'mi')
select dbo.ConvertTimeToHHMMSS(35791394.999, 'hh')
*/
GO
/****** Object:  StoredProcedure [dbo].[adm_ArtistInsert]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[adm_ArtistInsert]
(
	@FirstName VARCHAR(50)=NULL,
	@LastName VARCHAR(50),
	@Name VARCHAR(50)=NULL,
	@Biography text=NULL,
	@Result INT OUTPUT
)
AS
BEGIN
    DECLARE @bio VARCHAR(7960)
    SET @bio = CONVERT(VARCHAR(7960), @biography)

    SET @Name = ltrim(isnull(@FirstName, '')+' '+@LastName)
    SET @FirstName = NULLIF(@FirstName, '')
	
    IF NOT EXISTS(SELECT Name FROM artist WHERE Name=@Name)
    BEGIN
        DECLARE @Inserted TABLE (ArtistId INT)

        INSERT INTO Artist (FirstName, LastName, [Name], Biography)
        OUTPUT INSERTED.ArtistId INTO @Inserted
        VALUES (@FirstName, @LastName, @Name, @Biography)

        SET @Result = (SELECT ArtistId FROM @Inserted)

        IF (LEN(@bio) < 7)
        BEGIN
            UPDATE artist SET biography = NULL WHERE ArtistId = @Result
        END
    END
    ELSE
    BEGIN
        SET @Result = -1
    END
END
GO
/****** Object:  StoredProcedure [dbo].[adm_createDisc]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[adm_createDisc]
	@RecordId INT,
	@DiscNo INT	
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO Disc (RecordId, DiscNo) VALUES (@RecordId, @DiscNo)
END
GO
/****** Object:  StoredProcedure [dbo].[adm_GetAllArtists]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Create the stored procedure in the specified schema
CREATE PROCEDURE [dbo].[adm_GetAllArtists]
    @FirstName varchar(150),
    @LastName varchar(150)
AS
    SELECT * FROM Artist WHERE FirstName = @FirstName AND LastName = @LastName
GO
/****** Object:  StoredProcedure [dbo].[adm_getAllRecordIds]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[adm_getAllRecordIds]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT Record.RecordId, Record.Discs, 0 AS DiscNo
	FROM Record
	ORDER BY RecordId
END

GO
/****** Object:  StoredProcedure [dbo].[adm_getArtist]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[adm_getArtist]
	@artistid int
as
Select ArtistId, Name as ArtistName
	from Artist
	where ArtistId = @ArtistId
GO
/****** Object:  StoredProcedure [dbo].[adm_GetArtistAlbums]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[adm_GetArtistAlbums]
AS
BEGIN
    SELECT 
        a.Name AS Artist, r.Name AS Album, r.Recorded, r.Media
    FROM 
        Artist a 
    INNER JOIN
        Record r ON a.ArtistId = r.ArtistId
    ORDER BY 
        a.LastName, a.FirstName, r.Recorded DESC
END
GO
/****** Object:  StoredProcedure [dbo].[adm_GetNumberOfDiscs]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[adm_GetNumberOfDiscs]
	@RecordId INT,
	@DiscNo INT OUTPUT
AS

SELECT @DiscNo = (SELECT count (*) FROM Disc WHERE recordId = @RecordId)
RETURN @DiscNo
GO
/****** Object:  StoredProcedure [dbo].[adm_getRecords]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[adm_getRecords]
	@ArtistId int
as
select ArtistId, name as RecordName, [field], Recorded
	from Record
	where ArtistId = @ArtistId
	Order by Recorded Desc

GO
/****** Object:  StoredProcedure [dbo].[adm_GetSelectedReviews]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[adm_GetSelectedReviews]
AS
SELECT rec.RecordId, rec.Review AS RecordReview, rev.Author, rev.Review
	FROM Record rec INNER JOIN Review rev ON
	rec.RecordId = rev.RecordId
	WHERE rec.Review NOT LIKE '%Pitchfork%'
GO
/****** Object:  StoredProcedure [dbo].[adm_GetTotalCDCount]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[adm_GetTotalCDCount]
AS
BEGIN
	-- I am counting all CD's. 
	-- Where there are CD/DVD or CD/Blu-ray sets with more than 1 disc
	-- I am assuming that one of the discs in each set is a DVD or Blu-ray
	-- so I will subtract these from the Total number of CD's.
	SET NOCOUNT ON;

	DECLARE @Total INT

	-- count CD's only.
	set @Total = (select sum(discs) from record where media = 'CD')

	-- count CD's in DVD and Blu-ray sets.
	SET @Total = @Total + (SELECT SUM(Discs) FROM record
		WHERE (media = 'CD/DVD' OR media = 'CD/Blu-ray')
		AND Discs > 1)

	-- Subtract one disc from each DVD or Blu-ray set.
	SET @Total = @Total - (SELECT Count(*) FROM record
		WHERE (media = 'CD/DVD' OR media = 'CD/Blu-ray')
		AND Discs > 1)

	SELECT @Total AS Total
END
GO
/****** Object:  StoredProcedure [dbo].[adm_InsertSqliteReview]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[adm_InsertSqliteReview]
(
	@ReviewId int,
	@ArtistId int=0,
	@RecordId int=0,
	@Name varchar(200)=null,
	@RecordName varchar(200)=null,
	@Author [nvarchar](100)=null,
	@Published datetime=null,
	@Review text=null
)
AS
INSERT INTO Review (ReviewId, ArtistId, RecordId, Name, RecordName, 
			Author, Published, Review)
VALUES (@ReviewId, @ArtistId, @RecordId, @Name, @RecordName, 
		@Author, @Published, @Review)

RETURN @@IDENTITY
GO
/****** Object:  StoredProcedure [dbo].[adm_RecordInsert]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[adm_RecordInsert]
(
	@ArtistId int,
	@Name varchar(80),
	@Field varchar(50),
	@Recorded int,
	@Label varchar(50),
	@Pressing varchar(50),
	@Rating varchar(4),
	@Discs int,
	@Media varchar(50),
	@Bought datetime=null,
	@Cost money=null,
	@CoverName varchar(50)=null,
	@Review text=null,
	@FreeDBID int=null,
	@Result INT OUTPUT
)
AS

SET NOCOUNT ON;

DECLARE @Inserted TABLE (RecordId INT)

INSERT INTO Record (ArtistId, Name, Field, Recorded,
		Label, Pressing, Rating, Discs, Media, Bought,
		Cost, CoverName, Review, FreeDBID)
OUTPUT INSERTED.RecordId INTO @Inserted
VALUES (@ArtistId, @Name, @Field, @Recorded,
		@Label, @Pressing, @Rating, @Discs, @Media, @Bought,
		@Cost, @CoverName, @Review, @FreeDBID)

SET @Result = (SELECT RecordId FROM @Inserted)

-- Add the disc records
DECLARE @count INT

SET @count = 0

WHILE (@count < @Discs)
BEGIN
	SET @count = @count + 1
	INSERT INTO Disc (RecordId, DiscNo) 
	VALUES (@Result, @count)
	
END
RETURN @Result
GO
/****** Object:  StoredProcedure [dbo].[adm_sdSelect]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[adm_sdSelect]
AS
	SET NOCOUNT ON;

/*
SELECT
	StatusId,
	Status
FROM
	[Statuses]

SELECT
	TypeId,
	Type
FROM
	[Types]
*/
SELECT
	ArtistId,
    [Name]
FROM
    Artist
WHERE [Name] is not null
GO
/****** Object:  StoredProcedure [dbo].[adm_SelectAllDiscs]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[adm_SelectAllDiscs]
AS
SELECT RecordId, DiscId, DiscNo, FreeDbId, FreeDbDiscId, [Length]
	FROM Disc
GO
/****** Object:  StoredProcedure [dbo].[adm_SelectAllFreeDBItems]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[adm_SelectAllFreeDBItems]
AS
SELECT Id, Artist, RecordId, Record, DiscId, FreeDbId, 
		OtherFreeDbId, Genre, Revision, Review
	FROM FreeDB
GO
/****** Object:  StoredProcedure [dbo].[adm_SelectAllTracks]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[adm_SelectAllTracks]
AS
BEGIN

	SET NOCOUNT ON;

	SELECT a.Name AS ArtistName, 
       r.RecordId, 
       r.Name AS Name,
       d.DiscId,  
       d.DiscNo, 
       d.FreeDbDiscId,
       d.FreeDbId, 
       d.Length,
       t.TrackId, 
       t.TrackNo, 
       t.Name AS TrackName, 
       t.TrackLength,
       t.Extended
    FROM Artist a
        INNER JOIN Record r ON a.ArtistId = r.ArtistId
        INNER JOIN Disc d ON r.RecordId = d.RecordId
        LEFT JOIN Track t ON d.DiscId = t.DiscId  -- Shows all discs, even without tracks
    WHERE t.TrackId IS NOT NULL -- This removes empty track records
    ORDER BY a.LastName, a.FirstName, r.Recorded, d.DiscNo, t.TrackNo
END
GO
/****** Object:  StoredProcedure [dbo].[adm_UpdateRecord]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[adm_UpdateRecord]
(
    @RecordId int,
	@ArtistId int,
    @Name varchar(80),
    @Field varchar(50),
    @Recorded int,
    @Label varchar(50),
    @Pressing varchar(50),
    @Rating varchar(4),
    @Discs int,
    @Media varchar(50),
    @Bought datetime = null,
    @Cost money = null,
    @CoverName varchar(50) = null,
    @Review nvarchar(MAX) = null, -- Changed from TEXT to nvarchar(MAX)
	@RowsAffected int=0 OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Record
    SET [Name] = @Name, Field = @Field, Recorded = @Recorded, Label = @Label, 
		Pressing = @Pressing, Rating = @Rating, Discs = @Discs, Media = @Media, 
		Bought = @Bought, Cost = @Cost, CoverName = @CoverName, Review = @Review
    WHERE RecordId = @RecordId;
    
    -- Set the output parameter to the actual number of rows affected
  if (@@ROWCOUNT=1)
   BEGIN
      select @RowsAffected = @@ROWCOUNT
   END
END
GO
/****** Object:  StoredProcedure [dbo].[adm_UpdateRecordReview]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[adm_UpdateRecordReview]
		@RecordId int,
		@Review NVARCHAR(MAX)
AS
UPDATE Record
	SET Review=@Review
   	WHERE RecordId=@RecordId

   	if (@@rowcount=0)
	    select 0 'RecordId', 0 'Status', 'update record failed - please try again' 'StatusStr'
	else
	    select @@identity 'RecordId', 1 'Status', 'Ok' 'StatusStr'
GO
/****** Object:  StoredProcedure [dbo].[json_ArtistSelectAll]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[json_ArtistSelectAll]
AS
SELECT
    a.ArtistId, a.FirstName, a.LastName, a.Name, a.Biography
FROM Artist AS a
ORDER BY a.LastName, a.FirstName
GO
/****** Object:  StoredProcedure [dbo].[json_InternationalRecordSelectAll]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[json_InternationalRecordSelectAll]
AS
SELECT r.ArtistId, r.RecordId, r.Name, r.Field, r.Recorded, r.Label, 
	r.Pressing, r.Rating, r.Discs, r.Media, FORMAT (r.Bought, 'MM-dd-yyyy') AS Bought, 
	r.Cost, r.CoverName, r.Review, r.FreeDbId
FROM Artist a INNER JOIN Record r 
	ON a.ArtistId = r.ArtistId
ORDER BY a.LastName, a.FirstName, r.recorded, r.Name
GO
/****** Object:  StoredProcedure [dbo].[json_RecordSelectAll]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[json_RecordSelectAll]
AS
SELECT r.ArtistId, r.RecordId, r.Name, r.Field, r.Recorded, r.Label, 
	r.Pressing, r.Rating, r.Discs, r.Media, FORMAT (r.Bought, 'dd-MM-yyyy') AS Bought, 
	r.Cost, r.Review
FROM Artist a INNER JOIN Record r 
	ON a.ArtistId = r.ArtistId
ORDER BY a.LastName, a.FirstName, r.recorded, r.Name
GO
/****** Object:  StoredProcedure [dbo].[sp_2001]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_2001] AS

SELECT Artist.FirstName, Artist.LastName, Record.Name, Record.Field, Record.Recorded
FROM  Artist INNER JOIN
               Record ON Artist.ArtistId = Record.ArtistId
WHERE (Record.Bought > '12/31/2000')
ORDER BY Record.Bought DESC
GO
/****** Object:  StoredProcedure [dbo].[sp_addArtist]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_addArtist]
	@FirstName varchar(50)=null,
	@LastName varchar(50),
	@biography varchar(4000)=null
As
	if  not exists(select FirstName,LastName from artist where FirstName=@FirstName and LastName=@LastName)
	begin
	insert into artist(FirstName,LastName,biography)
	select @FirstName,@LastName,@biography
	end

	-- Check for ok
	if (@@rowcount=0)
	begin
		select 0 'ArtistId', 0 'Status', 'Record not added - please try again' 'StatusStr'
	end else
	begin
		select @@identity 'ArtistId', 1 'Status', 'Ok' 'StatusStr'
	end
Return @@identity
GO
/****** Object:  StoredProcedure [dbo].[sp_AddNewArtist]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_AddNewArtist]
	@FirstName varchar(50)=null,
	@LastName varchar(50),
	@Name varchar(100),
	@biography varchar(4000)=null,
	@ArtistId int OUTPUT
As
	if  not exists(select FirstName,LastName from artist where FirstName=@FirstName and LastName=@LastName)
	begin
	insert into artist(FirstName, LastName, name, biography)
	VALUES (@FirstName, @LastName, @Name, @biography)
	end
	
SELECT @ArtistId = @@IDENTITY

GO
/****** Object:  StoredProcedure [dbo].[sp_AddNewRecord]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  Procedure [dbo].[sp_AddNewRecord]
@ArtistID Integer,
@Name  VarChar(50),
@Field VarChar(50),
@Recorded Integer,
@Label VarChar(50),
@Pressing VarChar(50),
@Rating VarChar(10),
@Discs Integer,
@Media VarChar(50),
@Bought SmallDateTime,
@Cost Money,
@CoverName VarChar(50) = NULL,
@Review VarChar(400) = NULL,
@FreeDbId int = NULL

As
INSERT INTO Record
    VALUES (@ArtistID, @Name, @Field, @Recorded, @Label, @Pressing, @Rating, @Discs, @Media, @Bought, @Cost, @CoverName, @Review, @FreeDbId)

GO
/****** Object:  StoredProcedure [dbo].[sp_All]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_All] AS
SELECT *, Artist.ArtistId AS [Artist ID], Artist.FirstName AS [First Name],
   Artist.LastName AS [Last Name], Artist.Biography AS Bio,
   Record.RecordId AS [Record ID], Record.[Name] AS Title,
   Record.Field AS Field, Record.Recorded AS Recorded,
   Record.Label AS Label, Record.Pressing AS Pressing,
   Record.Rating AS Rating,
   Record.CoverName AS [Cover Name],
   Record.Review AS Review, Record.Discs AS [No. of Disks],
   Record.Media AS Media, Record.Bought AS Bought,
   Record.Cost AS Cost
FROM Artist INNER JOIN
   Record ON Artist.ArtistId = Record.ArtistId
ORDER BY  Artist.LastName,  Artist.FirstName,  Record.Recorded
GO
/****** Object:  StoredProcedure [dbo].[sp_CombineNames]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_CombineNames] AS
SELECT ArtistId, ltrim(ISNULL(FirstName, '') + ' ' + LastName) AS Name
FROM  Artist
ORDER BY LastName, FirstName
GO
/****** Object:  StoredProcedure [dbo].[sp_getArtistsListandNone]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_getArtistsListandNone]
as
create table #temp
(
   ArtistId int,
   LastName varchar(50),
   FirstName varchar(50),
   [Name] varchar(100)
)

insert #temp(ArtistId, LastName, FirstName, [name]) values ('0',null,null,'#select an Artist to view')

-- get Artists names
insert #temp
select ArtistId, LastName, isnull(FirstName, ''),  LastName+', '+isnull(FirstName, '') as [Name]
FROM Artist
where FirstName is not null

-- get Group names
insert #temp
select ArtistId, LastName, FirstName as FirstName, LastName as [Name]
FROM Artist
where FirstName is null and LastName is not Null

create table #temp2
(
   ArtistId int,
   [Name] varchar(100)
)

insert #temp2
select ArtistId, [Name]
FROM #temp
order by [Name]

if (select count(1) from #temp2) > 0
begin
  select ArtistId,[name] from #temp2
end
else
begin
  select 0 ID, 'none' Name
end

GO
/****** Object:  StoredProcedure [dbo].[sp_getList]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_getList]
as
select RecordId, name, field
from record order by RecordId

GO
/****** Object:  StoredProcedure [dbo].[sp_GetRecords]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetRecords]
	@ArtistId int
as
select RecordId, [name], [field], recorded, rating, media
from Record
where ArtistId = @ArtistId
order by recorded

GO
/****** Object:  StoredProcedure [dbo].[sp_getRecordsListandNone]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[sp_getRecordsListandNone]
	@ArtistId int=null
as

SELECT [RecordId], [name]+' ('+rtrim(Media)+')' as [Name]
into #temp
FROM  Record
WHERE (ArtistId = @ArtistId)
ORDER BY Recorded

if (select count(1) from #temp) > 0
begin
	select [RecordId], [Name] from #temp
end
else
begin
	select 0 [RecordId], 'none' Name
end
GO
/****** Object:  StoredProcedure [dbo].[sp_getSingleArtist]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_getSingleArtist]
   @Artistid INT
AS

SET NOCOUNT ON;

SELECT ArtistId, LastName, FirstName, biography
FROM Artist
WHERE Artist.ArtistId = @ArtistId

GO
/****** Object:  StoredProcedure [dbo].[sp_getSingleRecord]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_getSingleRecord]
   @Recordid int
As
select [RecordId],[Name], [Field], Recorded, Label, Pressing, Rating, Discs, Bought, Media, Cost, Review
from Record
where [RecordId]=@Recordid
GO
/****** Object:  StoredProcedure [dbo].[sp_getTotalCostForEachArtist]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_getTotalCostForEachArtist]
AS
	SELECT Record.ArtistId, SUM(Record.Cost) AS TotalCost
	INTO #temp
	FROM Artist INNER JOIN
	     Record ON Artist.ArtistId = Record.ArtistId
	GROUP BY Record.ArtistId
	SELECT ltrim(ISNULL(a.FirstName, '') + ' ' + a.LastName) AS Name, t.TotalCost
	FROM #temp t INNER JOIN Artist a ON
		t.ArtistId = a.ArtistId
	WHERE t.TotalCost > 0.00
	ORDER BY t.TotalCost Desc 
GO
/****** Object:  StoredProcedure [dbo].[sp_getTotalDiscsForEachArtist]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_getTotalDiscsForEachArtist]
AS
SELECT a.ArtistId,
	ltrim(ISNULL(a.FirstName, '') + ' ' + a.LastName) AS Name,
	TotalDiscs
FROM Artist a,
(SELECT r.ArtistId, SUM(Discs) AS TotalDiscs
	FROM Record r
	GROUP BY ArtistId) AS SubQuery
	WHERE a.ArtistId = SubQuery.ArtistId 
ORDER BY SubQuery.TotalDiscs DESC
GO
/****** Object:  StoredProcedure [dbo].[sp_getTotalsForEachArtist]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_getTotalsForEachArtist]
AS
SELECT a.ArtistId,
	ltrim(ISNULL(a.FirstName, '') + ' ' + a.LastName) AS Name,
	TotalDiscs,
	TotalCost
FROM Artist a,
(SELECT r.ArtistId, SUM(Discs) AS TotalDiscs,
	SUM(Cost) AS TotalCost
	FROM Record r
	GROUP BY ArtistId) AS SubQuery
	WHERE a.ArtistId = SubQuery.ArtistId 
ORDER BY SubQuery.TotalCost DESC
GO
/****** Object:  StoredProcedure [dbo].[sp_getTracks]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_getTracks]
 @RecordId INT
AS

SET NOCOUNT ON;

BEGIN
    SELECT d.DiscID, d.Length AS DiscLength, d.DiscNo, t.TrackId, t.TrackNo AS TrackNo,
           t.Name AS TrackName, t.Extended, t.TrackLength AS TrackLength
    FROM  [Disc] d LEFT OUTER JOIN
          Track t ON d.DiscId = t.DiscId
    WHERE d.RecordId = @RecordId
    ORDER BY d.DiscNo, t.TrackNo
END
GO
/****** Object:  StoredProcedure [dbo].[sp_NumBoughtIn2000]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- calculate the number of CD's bought in 2000
CREATE PROCEDURE [dbo].[sp_NumBoughtIn2000]
AS
SELECT SUM(discs) as 'Number of CD''s bought in 2000'
FROM record
where Record.Media = 'CD' and Record.bought > '31/12/1999' and Record.bought < '01/01/2001'

GO
/****** Object:  StoredProcedure [dbo].[sp_NumBoughtIn2001]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_NumBoughtIn2001] AS
-- calculate the number of CD's bought in 2001
SELECT SUM(discs) as 'Number of CD''s bought in 2001'
FROM record
where Record.Media = 'CD' and Record.bought > '01/01/2001'

GO
/****** Object:  StoredProcedure [dbo].[sp_Titles]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Titles]
As
	/* set nocount on */
Select * from artist
inner join record on artist.ArtistId = record.ArtistId
	return

GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateArtist]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_UpdateArtist]
		@ArtistId int,
		@FirstName varchar(50)=null,
		@LastName varchar(50)=null,
		@Biography text=null
As
   update Artist
	set FirstName=@FirstName, LastName=@LastName, Biography=@biography
   	where ArtistId=@ArtistId

    	if (@@rowcount=0)
	    select 0 'ArtistId', 0 'Status', 'update record failed - please try again' 'StatusStr'
	else
	    select @@identity 'ArtistId', 1 'Status', 'Ok' 'StatusStr'

GO
/****** Object:  StoredProcedure [dbo].[spGetAlbum]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spGetAlbum]
	@name varchar (60)
as
	Select Artist.LastName+', '+Artist.FirstName as Artist, record.name as name
		from Artist inner join Record on
		Artist.ArtistId = record.ArtistId
		where Record.name = @name

GO
/****** Object:  StoredProcedure [dbo].[up_AddNewArtist]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_AddNewArtist]
(
    @FirstName VARCHAR(50) = NULL,
    @LastName VARCHAR(50),
    @Name VARCHAR(80),
    @Biography TEXT = NULL,
    @ArtistId INT = NULL OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT FirstName, LastName FROM Artist WHERE FirstName = @FirstName AND LastName = @LastName)
    BEGIN
        -- Check for empty biography
        DECLARE @bio VARCHAR(7960)
        SET @bio = CONVERT(VARCHAR(7960), @Biography)

        -- Set null if there is no value in @FirstName
        SET @FirstName = NULLIF(@FirstName, '')

        INSERT INTO Artist (FirstName, LastName, [Name], Biography)
        VALUES (@FirstName, @LastName, @Name, @Biography)

        SET @ArtistId = SCOPE_IDENTITY()

        -- Set biography to null if blank
        IF (LEN(@bio) < 7)
        BEGIN
            UPDATE Artist
            SET Biography = NULL
            WHERE ArtistId = @ArtistId
        END

        -- Return the new ArtistId
        SELECT @ArtistId AS NewArtistId
    END
    ELSE
    BEGIN
        -- Artist already exists
        SET @ArtistId = -1
        SELECT @ArtistId AS NewArtistId
    END
END
GO
/****** Object:  StoredProcedure [dbo].[up_AddNewRecord]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_AddNewRecord]
(
	@ArtistId int,
	@Name varchar(60),
	@Field varchar(50),
	@Recorded int,
	@Label varchar(50),
	@Pressing varchar(50),
	@Rating varchar(4),
	@Discs int,
	@Media varchar(50),
	@Bought smalldatetime=null,
	@Cost money=null,
	@CoverName varchar(50)=null,
	@Review text=null,
	@RecordId int OUTPUT
)
As
  	-- set null if there is no value in @CoverName
	set @Covername = nullif(@CoverName, '')

	insert into record
		(ArtistId, [Name], Field, Recorded, Label, Pressing, Rating, Discs,
		 Media, Bought, Cost, CoverName, Review)
	values
		 (@ArtistId, @Name, @Field, @Recorded, @Label, @Pressing, @Rating, @Discs,
			@Media, @Bought, @Cost,	@CoverName,	@Review)
    select
      @RecordId = @@Identity
GO
/****** Object:  StoredProcedure [dbo].[up_ArtistByFirstLastName]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_ArtistByFirstLastName]
   @FirstName VARCHAR(50)=null,
   @LastName VARCHAR(50)
As
select ArtistId, FirstName, LastName, [name]
from Artist
where FirstName = @FirstName and LastName = @Lastname;
GO
/****** Object:  StoredProcedure [dbo].[up_ArtistDelete]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_ArtistDelete]
    @ArtistId INT
AS
BEGIN
    DELETE FROM Artist WHERE ArtistId = @ArtistId
	RETURN @@ROWCOUNT
END
GO
/****** Object:  StoredProcedure [dbo].[up_ArtistDeleteByName]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_ArtistDeleteByName]
    @Name NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
        DELETE FROM Artist WHERE Name = @Name;
        
        SELECT CASE WHEN @@ROWCOUNT > 0 THEN 1 ELSE 0 END AS Deleted;
END
GO
/****** Object:  StoredProcedure [dbo].[up_ArtistGetList]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_ArtistGetList]
AS
    create table #temp
(
	[Name] varchar(200),
	ArtistId int
)

insert #temp

select  name, ArtistId
from Artist
order by LastName, FirstName

if (select count(1) from #temp) > 0
begin
    insert #temp(name, ArtistId) values (' Select an Artist...', '0')
  	select ArtistId, [name] from #temp
	order by [name]
end
else
begin
	select  0 ID, 'none' Name
end
GO
/****** Object:  StoredProcedure [dbo].[up_ArtistsBandTitles]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_ArtistsBandTitles]
AS

SET NOCOUNT ON;

SELECT
    a.ArtistId, a.FirstName, a.LastName, a.Name, a.Biography
FROM Artist AS a
WHERE a.FirstName = 'The'
ORDER BY LastName
GO
/****** Object:  StoredProcedure [dbo].[up_ArtistSelectAll]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_ArtistSelectAll]
AS
SELECT
    a.ArtistId, a.FirstName, a.LastName, a.Name, a.Biography
FROM Artist AS a
ORDER BY LastName, FirstName

GO
/****** Object:  StoredProcedure [dbo].[up_ArtistSelectById]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_ArtistSelectById]
   @ArtistId INT
AS

SET NOCOUNT ON;

SELECT ArtistId, LastName, FirstName, [name], biography
FROM Artist
WHERE Artist.ArtistId = @ArtistId
GO
/****** Object:  StoredProcedure [dbo].[up_ArtistSelectByRecordId]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_ArtistSelectByRecordId]
   @RecordId int
AS
DECLARE @ArtistId INT

SET @ArtistId = (SELECT ArtistId FROM Record WHERE RecordId = @RecordId) 

IF @ArtistId > 0
BEGIN
	SELECT ArtistId, LastName, FirstName, [name], biography
		FROM Artist
	WHERE ArtistId = @ArtistId
END
GO
/****** Object:  StoredProcedure [dbo].[up_ArtistSelectFull]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_ArtistSelectFull]
AS
SELECT
    a.ArtistId, a.FirstName AS FirstName, a.LastName As LastName, a.Name, a.Biography
FROM Artist AS a
ORDER BY LastName, FirstName

GO
/****** Object:  StoredProcedure [dbo].[up_CheckArtistExists]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_CheckArtistExists]
    @Name NVARCHAR(150)
AS
BEGIN
	SET NOCOUNT ON;
    
	IF EXISTS (SELECT 1 FROM Artist WHERE Name = @Name)
        SELECT 1 AS [Exists]
    ELSE
        SELECT 0 AS [Exists]
END
GO
/****** Object:  StoredProcedure [dbo].[up_CheckForTracks]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[up_CheckForTracks]
    @DiscId INT,
    @TrackCount INT=0 OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT @TrackCount = COUNT(*)
    FROM Track
    WHERE DiscId = @DiscId;
END
GO
/****** Object:  StoredProcedure [dbo].[up_CountDiscs]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_CountDiscs]
	@show varchar(20)
AS
if @show = 'all'
begin
	SELECT SUM(discs) AS discs
	FROM Record AS r
	INNER JOIN Artist as a on
		a.ArtistId = r.ArtistId
end
else if @show = 'cd'
begin
	SELECT SUM(discs) AS discs
	FROM Record AS r
	INNER JOIN Artist as a on
		a.ArtistId = r.ArtistId
	WHERE r.media = 'CD' OR r.media = 'CD/DVD' OR r.media = 'CD/Blu-ray' OR r.media = 'Blu-ray'
end
else if @show = 'records'
begin
	SELECT SUM(discs) AS discs
	FROM Record AS r
	INNER JOIN Artist as a on
		a.ArtistId = r.ArtistId
	WHERE r.media = 'R'
end
else if @show = 'dvds'
begin
	SELECT SUM(discs) AS discs
	FROM Record AS r
	INNER JOIN Artist as a on
		a.ArtistId = r.ArtistId
	WHERE r.media = 'DVD' or r.media = 'CD/DVD' or r.media = 'Blu-ray' or r.media = 'CD/Blu-ray'
end
else if @show = '2014'
begin
	SELECT SUM(discs) AS discs
	FROM Record AS r
	INNER JOIN Artist as a on
		a.ArtistId = r.ArtistId
	WHERE (YEAR(r.bought) > 2013) AND (YEAR(r.bought) < 2015)
end
else if @show = '2013'
begin
	SELECT SUM(discs) AS discs
	FROM Record AS r
	INNER JOIN Artist as a on
		a.ArtistId = r.ArtistId
	WHERE (YEAR(r.bought) > 2012) AND (YEAR(r.bought) < 2014)
end
else if @show = '2012'
begin
	SELECT SUM(discs) AS discs
	FROM Record AS r
	INNER JOIN Artist as a on
		a.ArtistId = r.ArtistId
	WHERE (YEAR(r.bought) > 2011) AND (YEAR(r.bought) < 2013)
end
else if @show = '2011'
begin
	SELECT SUM(discs) AS discs
	FROM Record AS r
	INNER JOIN Artist as a on
		a.ArtistId = r.ArtistId
	WHERE (YEAR(r.bought) > 2010) AND (YEAR(r.bought) < 2012)
end
else if @show = '2010'
begin
	SELECT SUM(discs) AS discs
	FROM Record AS r
	INNER JOIN Artist as a on
		a.ArtistId = r.ArtistId
	WHERE (YEAR(r.bought) > 2009) AND (YEAR(r.bought) < 2011)
end
else if @show = '2009'
begin
	SELECT SUM(discs) AS discs
	FROM Record AS r
	INNER JOIN Artist as a on
		a.ArtistId = r.ArtistId
	WHERE (YEAR(r.bought) > 2008) AND (YEAR(r.bought) < 2010)
end
else if @show = '2008'
begin
	SELECT SUM(discs) AS discs
	FROM Record AS r
	INNER JOIN Artist as a on
		a.ArtistId = r.ArtistId
	WHERE (YEAR(r.bought) > 2007) AND (YEAR(r.bought) < 2009)
end
else if @show = '2007'
begin
	SELECT SUM(discs) AS discs
	FROM Record AS r
	INNER JOIN Artist as a on
		a.ArtistId = r.ArtistId
	WHERE (YEAR(r.bought) > 2006) AND (YEAR(r.bought) < 2008)
end
else if @show = '2006'
begin
	SELECT SUM(discs) AS discs
	FROM Record AS r
	INNER JOIN Artist as a on
		a.ArtistId = r.ArtistId
	WHERE (YEAR(r.bought) > 2005) AND (YEAR(r.bought) < 2007)
end
else if @show = '2005'
begin
	SELECT SUM(discs) AS discs
	FROM Record AS r
	INNER JOIN Artist as a on
		a.ArtistId = r.ArtistId
	WHERE (YEAR(r.bought) > 2004) AND (YEAR(r.bought) < 2006)
end
else if @show = '2004'
begin
	SELECT SUM(discs) AS discs
	FROM Record AS r
	INNER JOIN Artist as a on
		a.ArtistId = r.ArtistId
	WHERE (YEAR(r.bought) > 2003) AND (YEAR(r.bought) < 2005)
end
else if @show = '2003'
begin
	SELECT SUM(discs) AS discs
	FROM Record AS r
	INNER JOIN Artist as a on
		a.ArtistId = r.ArtistId
	WHERE (YEAR(r.bought) > 2002) AND (YEAR(r.bought) < 2004)
end
else if @show = '2002'
begin
	SELECT SUM(discs) AS discs
	FROM Record AS r
	INNER JOIN Artist as a on
		a.ArtistId = r.ArtistId
	WHERE (YEAR(r.bought) > 2001) AND (YEAR(r.bought) < 2003)
end
else if @show = '2001'
begin
	SELECT SUM(discs) AS discs
	FROM Record AS r
	INNER JOIN Artist as a on
		a.ArtistId = r.ArtistId
	WHERE (YEAR(r.bought) > 2000) AND (YEAR(r.bought) < 2002)
end
else if @show = '2000'
begin
	SELECT SUM(discs) AS discs
	FROM Record AS r
	INNER JOIN Artist as a on
		a.ArtistId = r.ArtistId
	WHERE (YEAR(r.bought) > 1999) AND (YEAR(r.bought) < 2001)
end
else if @show = '****'
begin
	SELECT SUM(discs) AS discs
	FROM Record AS r
	INNER JOIN Artist as a on
		a.ArtistId = r.ArtistId
	WHERE r.rating = '****'
end
else if @show = 'Rock'
begin
	SELECT SUM(discs) AS discs
	FROM Record AS r
	INNER JOIN Artist as a on
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Rock'
end
else if @show = 'Blues'
begin
	SELECT SUM(discs) AS discs
	FROM Record AS r
	INNER JOIN Artist as a on
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Blues'
end
else if @show = 'Jazz'
begin
	SELECT SUM(discs) AS discs
	FROM Record AS r
	INNER JOIN Artist as a on
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Jazz' OR r.field='Fusion'
end
else if @show = 'Classical'
begin
	SELECT SUM(discs) AS discs
	FROM Record AS r
	INNER JOIN Artist as a on
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Classical'
end
else if @show = 'Soundtrack'
begin
	SELECT SUM(discs) AS discs
	FROM Record AS r
	INNER JOIN Artist as a on
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Soundtrack'
end
else if @show = 'Country'
begin
	SELECT SUM(discs) AS discs
	FROM Record AS r
	INNER JOIN Artist as a on
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Country'
end
else if @show = 'Rockdesc'
begin
	SELECT SUM(discs) AS discs
	FROM Record AS r
	INNER JOIN Artist as a on
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Rock'
end
else if @show = 'Bluesdesc'
begin
	SELECT SUM(discs) AS discs
	FROM Record AS r
	INNER JOIN Artist as a on
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Blues'
end
else if @show = 'Jazzdesc'
begin
	SELECT SUM(discs) AS discs
	FROM Record AS r
	INNER JOIN Artist as a on
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Jazz' OR r.field = 'Fusion'
end
else if @show = 'Classicaldesc'
begin
	SELECT SUM(discs) AS discs
	FROM Record AS r
	INNER JOIN Artist as a on
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Classical'
end
else if @show = 'Soundtrackdesc'
begin
	SELECT SUM(discs) AS discs
	FROM Record AS r
	INNER JOIN Artist as a on
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Soundtrack'
end
else if @show = 'Countrydesc'
begin
	SELECT SUM(discs) AS discs
	FROM Record AS r
	INNER JOIN Artist as a on
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Country'
end

GO
/****** Object:  StoredProcedure [dbo].[up_deleteArtist]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_deleteArtist]
	@artistid int
as

-- TODO: This sp needs to be totally rewritten to delete records from all tables
DELETE from Record
	where ArtistId = @ArtistId

DELETE from Artist
	where ArtistId = @ArtistId

GO
/****** Object:  StoredProcedure [dbo].[up_DeleteRecord]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_DeleteRecord]
	@RecordId INT
AS
DELETE FROM Disc
	WHERE RecordID = @RecordId

DELETE FROM Record
	WHERE RecordId = @RecordId
RETURN @@ROWCOUNT
GO
/****** Object:  StoredProcedure [dbo].[up_deleteTrack]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[up_deleteTrack]
	@TrackId INT
AS

	SET NOCOUNT ON;

	DELETE FROM Track
		WHERE TrackId = @TrackId

	RETURN @@ROWCOUNT
GO
/****** Object:  StoredProcedure [dbo].[up_DiscDelete]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_DiscDelete]
    @DiscId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM Disc WHERE DiscId = @DiscId
	RETURN @@ROWCOUNT
END
GO
/****** Object:  StoredProcedure [dbo].[up_getAlbumName]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_getAlbumName]
	@RecordId int
as
	select record.name
	from record
	where record.RecordId = @RecordId

GO
/****** Object:  StoredProcedure [dbo].[up_GetAllArtistsAndRecords]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetAllArtistsAndRecords]
AS
  -- Create a query string to show all records done by an artist
  SELECT a.[name] AS [ArtistName], r.[Name], r.Field, r.Label,
  r.Recorded, r.Rating, r.Bought, r.Discs, r.Pressing, r.FreeDbId,
  r.Cost, r.Media, r.RecordId, r.ArtistId, r.CoverName, r.Review
  FROM Artist a INNER JOIN Record r ON
  a.ArtistId = r.ArtistId
  ORDER By a.LastName, a.FirstName, r.Recorded DESC
GO
/****** Object:  StoredProcedure [dbo].[up_GetAllBlurays]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetAllBlurays]
as
   -- Create a query string to show all Blu-rays's
   SELECT a.[name] as [Name],
  r.[Name] as [Title], r.[Field] as [Field],
  r.[Recorded] as [Recorded], r.[Rating] as [Rating],
  r.[Bought] as [Bought], r.[Discs] as [Discs],
  r.[Cost] as [Cost], r.[Media] as [Media]
     from Artist a INNER JOIN Record r ON
        a.[ArtistId] = r.[ArtistId] WHERE r.[media] = 'Blu-ray' or r.[media] = 'CD/Blu-ray'
        order by r.[Bought] Desc
GO
/****** Object:  StoredProcedure [dbo].[up_GetAllDVDs]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetAllDVDs]
as
   -- Create a query string to show all DVD's
   SELECT a.[name] as [Name],
  r.[Name] as [Title], r.[Field] as [Field],
  r.[Recorded] as [Recorded], r.[Rating] as [Rating],
  r.[Bought] as [Bought], r.[Discs] as [Discs],
  r.[Cost] as [Cost], r.[Media] as [Media]
     from Artist a INNER JOIN Record r ON
        a.[ArtistId] = r.[ArtistId] WHERE r.[media] = 'DVD' or r.[media] = 'CD/DVD' or r.[media] = 'Blu-ray' or r.[media] = 'CD/Blu-ray'
        order by r.[Bought] Desc

GO
/****** Object:  StoredProcedure [dbo].[up_getAlphaArtistList]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_getAlphaArtistList]
as
create table #temp
(
   [ArtistId] int,
   LastName varchar(50),
   FirstName varchar(50),
   [Name] varchar(100)
)

create table #temp3
(
   [ArtistId] int,
   LastName varchar(50),
   FirstName varchar(50),
   [Name] varchar(100)
)

-- get Artists names
insert #temp
select ArtistId, LastName, isnull(FirstName, ''),  LastName+', '+isnull(FirstName, '') as [Name]
FROM Artist
where FirstName is not null and FirstName <> ''

-- get Group names
insert #temp3
select ArtistId, LastName, FirstName as FirstName, LastName as [Name]
FROM Artist
where FirstName is null or FirstName = '' 

create table #temp2
(
   ArtistId int,
   [Name] varchar(100)
)

insert #temp2
select ArtistId, [Name]
FROM #temp
order by [Name]

insert #temp2
select ArtistId, [Name]
FROM #temp3
order by [Name]

if (select count(1) from #temp2) > 0
begin
  select ArtistId,[name] from #temp2
  order by [name]
end
else
begin
  select 0 ArtistId, 'none' Name
  order by [name]
end

GO
/****** Object:  StoredProcedure [dbo].[up_GetArtistAndNumberOfRecords]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetArtistAndNumberOfRecords] 
	@ArtistId INT
AS

create table #temp
(
   ArtistId INT,
   [Name] varchar(100),
   DiscCount INT
)

DECLARE @DiscNo AS INT
DECLARE @ArtistName AS VARCHAR(100)

SET @DiscNo = (SELECT SUM(Discs) FROM Record WHERE artistId = @artistId)

SET @ArtistName = (SELECT [Name] FROM  Artist WHERE ArtistId = @ArtistId)

INSERT #temp(ArtistId, [Name], DiscCount) values (@ArtistId, @ArtistName, @DiscNo)

SELECT ArtistId, [Name], DiscCount FROM #temp
GO
/****** Object:  StoredProcedure [dbo].[up_GetArtistByName]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetArtistByName]
	@Name VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT ArtistId, FirstName, LastName FROM Artist WHERE Name = @Name;
END
GO
/****** Object:  StoredProcedure [dbo].[up_GetArtistCount]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetArtistCount]
AS

SET NOCOUNT ON;

SELECT COUNT(*) FROM Artist;
GO
/****** Object:  StoredProcedure [dbo].[up_getArtistID]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_getArtistID]
  @FirstName varchar(50)=null,
  @LastName varchar(50)
as
if Len(@FirstName) < 2
	set @FirstName = null

if @FirstName is null or @FirstName = ''
begin
  select top 1 ArtistId
   from artist
   where LastName like @LastName+'%'
end
else
begin
  select top 1 ArtistId
   from artist
   where FirstName like @FirstName+'%' and LastName like @LastName+'%'
end
GO
/****** Object:  StoredProcedure [dbo].[up_getArtistIdByName]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_getArtistIdByName]
  @FirstName varchar(50),
  @LastName varchar(50)
AS
BEGIN
    IF LEN(@FirstName) < 2
        SET @FirstName = null
    
    IF @FirstName IS NULL OR @FirstName = ''
    BEGIN
        SELECT ArtistId
        FROM Artist
        WHERE LastName like @LastName+'%'
    END 
    ELSE
    BEGIN
        SELECT ArtistId 
        FROM Artist
        WHERE FirstName like @FirstName+'%' AND LastName like @LastName+'%'
    END
END

GO
/****** Object:  StoredProcedure [dbo].[up_getArtistIdFromRecord]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_getArtistIdFromRecord]
  @RecordId INT,
  @ArtistId INT OUTPUT
AS
BEGIN
  SET NOCOUNT ON;

  SELECT @ArtistId = ArtistId
  FROM Record
  WHERE RecordId = @RecordId;
END
GO
/****** Object:  StoredProcedure [dbo].[up_getArtistIdFromRecordId]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create PROCEDURE [dbo].[up_getArtistIdFromRecordId]
  @recordId int
as
  select ArtistId
   from Record
   where recordId = @recordId

GO
/****** Object:  StoredProcedure [dbo].[up_getArtistList]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_getArtistList]
as
SELECT ArtistId, Name
	FROM Artist
	ORDER BY LastName, FirstName

GO
/****** Object:  StoredProcedure [dbo].[up_getArtistListandNone]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_getArtistListandNone]
as
create table #temp
(
   [ArtistId] int,
   LastName varchar(50),
   FirstName varchar(50),
   [Name] varchar(100)
)
insert #temp(ArtistId, LastName, FirstName, [name]) values ('0',null,null,'#select an Artist to view')

create table #temp3
(
   [ArtistId] int,
   LastName varchar(50),
   FirstName varchar(50),
   [Name] varchar(100)
)

-- get Artists names
insert #temp
select ArtistId, LastName, isnull(FirstName, ''),  LastName+', '+isnull(FirstName, '') as [Name]
FROM Artist
where FirstName is not null and FirstName <> ''

-- get Group names
insert #temp3
select ArtistId, LastName, FirstName as FirstName, LastName as [Name]
FROM Artist
where FirstName is null or FirstName = '' 

create table #temp2
(
   ArtistId int,
   [Name] varchar(100)
)

insert #temp2
select ArtistId, [Name]
FROM #temp
order by [Name]

insert #temp2
select ArtistId, [Name]
FROM #temp3
order by [Name]

if (select count(1) from #temp2) > 0
begin
  select ArtistId,[name] from #temp2
  order by [name]
end
else
begin
  select 0 ArtistId, 'none' Name
  order by [name]
end

GO
/****** Object:  StoredProcedure [dbo].[up_GetArtistNameByArtistId]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetArtistNameByArtistId]
  @ArtistId INT
AS

SET NOCOUNT ON;

SELECT Artist.[Name]
FROM Artist 
WHERE ArtistId = @ArtistId
GO
/****** Object:  StoredProcedure [dbo].[up_GetArtistNameByRecordId]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetArtistNameByRecordId]
  @recordid INT
AS

SET NOCOUNT ON;

SELECT a.[Name]
FROM Artist a INNER JOIN
               Record r ON a.ArtistId = r.ArtistId 
WHERE r.RecordId = @RecordId
GO
/****** Object:  StoredProcedure [dbo].[up_GetArtistNumberOfRecords]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetArtistNumberOfRecords]
  @ArtistId int
as
  select sum(discs)
    from record where ArtistId = @ArtistId

GO
/****** Object:  StoredProcedure [dbo].[up_GetArtistRecordByRecordId]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetArtistRecordByRecordId] 
	@RecordId INT
AS
SELECT a.ArtistId, a.FirstName,
   a.LastName, a.[Name] AS ArtistName,
   r.RecordId, r.[Name],
   r.Field, r.Recorded,
   r.Label, r.Pressing,
   r.Rating, r.CoverName,
   r.Discs, r.Media, 
   r.Bought, r.Cost AS Cost
FROM Artist a INNER JOIN
   Record r ON a.ArtistId = r.ArtistId
WHERE r.RecordId = @recordId
ORDER BY  a.LastName,  a.FirstName,  r.Recorded
GO
/****** Object:  StoredProcedure [dbo].[up_GetArtistRecordEntity]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetArtistRecordEntity]
	@RecordId INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT 
	    r.*,
	    a.ArtistId as ArtistId,
	    a.FirstName as FirstName,
	    a.LastName as LastName,
	    a.Name as Artist
	FROM Record r
	JOIN Artist a ON a.ArtistId = r.ArtistId
	WHERE r.RecordId = 2196;
END
GO
/****** Object:  StoredProcedure [dbo].[up_GetArtistRecords]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[up_GetArtistRecords]
    @ArtistId INT
AS
    -- Columns are ordered deliberately for Dapper multi-mapping (splitOn: "ArtistId"):
    --   • Everything before ArtistId  → mapped to Record
    --   • ArtistId and onwards        → mapped to Artist
    SELECT
        -- ── Record columns ──────────────────────────────────────────────────────
        r.RecordId,
        r.[Name],
        r.Field,
        r.Label,
        r.Recorded,
        r.Rating,
        r.Bought,
        r.Discs,
        r.Pressing,
        r.Cost,
        r.Media,
        r.CoverName,
        r.Review,
        a.[Name]    AS ArtistName,   -- populates Record.ArtistName
        -- ── Dapper split point ──────────────────────────────────────────────────
        -- ArtistId must appear here; everything from this column onwards
        -- is mapped to Artist. Record.ArtistId is set in the C# mapping lambda.
        a.ArtistId,
        -- ── Artist columns ──────────────────────────────────────────────────────
        a.FirstName,
        a.LastName,
        a.[Name],
        a.Biography
    FROM Artist a
    INNER JOIN Record r ON a.ArtistId = r.ArtistId
    WHERE a.ArtistId = @ArtistId
    ORDER BY r.Recorded DESC;
GO
/****** Object:  StoredProcedure [dbo].[up_GetArtistRecordsWithNoTracks]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetArtistRecordsWithNoTracks]
	@Name VARCHAR(80)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT 
		a.ArtistId, 
		a.Name AS ArtistName, 
		r.RecordId, 
		r.Name AS Name, 
		r.Recorded,
		d.DiscId, 
		d.DiscNo
	FROM Artist a
	INNER JOIN Record r ON a.ArtistId = r.ArtistId
	LEFT JOIN Disc d ON r.RecordId = d.RecordId
	WHERE NOT EXISTS (
		SELECT 1 
		FROM Track t 
		WHERE t.DiscId = d.DiscId
	) AND a.Name = @Name
	ORDER BY a.LastName, a.FirstName, r.Recorded, d.DiscNo
END
GO
/****** Object:  StoredProcedure [dbo].[up_GetArtistRecordTracks]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetArtistRecordTracks]
    @Name NVARCHAR(200)
AS
SET NOCOUNT ON;
BEGIN
    DECLARE @RecordId INT
    SET @RecordId = (
        SELECT TOP 1 RecordId
        FROM Record
        WHERE Name = @Name
        ORDER BY RecordId
    )
    -- Get the record and all its tracks
    SELECT
        a.ArtistId, a.FirstName, a.LastName, a.Name AS ArtistName, a.Biography,
        r.RecordId, r.Name, r.Field, r.Recorded, r.Label, r.Pressing, r.Rating,
        r.Discs, r.Media, r.Bought, r.Cost, r.Review, d.DiscId, d.DiscNo, d.Length,
        t.TrackId, t.TrackNo, t.Name AS TrackName, t.TrackLength
    FROM Record r
        INNER JOIN Artist a ON r.ArtistId = a.ArtistId
        INNER JOIN Disc d ON r.RecordId = d.RecordId
        INNER JOIN Track t ON d.DiscId = t.DiscId
    WHERE r.RecordId = @RecordId
    ORDER BY d.DiscNo, t.TrackNo
END
GO
/****** Object:  StoredProcedure [dbo].[up_GetArtists]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetArtists]
as
SELECT ArtistId, Name
	FROM Artist
	ORDER BY LastName, FirstName

GO
/****** Object:  StoredProcedure [dbo].[up_GetArtistsByPartialName]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[up_GetArtistsByPartialName]
  @Name varchar(100)
AS
	SET NOCOUNT ON

BEGIN
  SELECT ArtistId, FirstName, LastName, Name, Biography
	FROM Artist
	WHERE Name like '%'+@Name+'%'
	ORDER BY LastName, FirstName
END
GO
/****** Object:  StoredProcedure [dbo].[up_GetArtistsRecords]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[up_GetArtistsRecords]
  @ArtistId int
as
  -- Create a query string to show all records done by an artist
  SELECT a.[name] as [ArtistName], r.[Name], r.Field, r.Label,
  r.Recorded, r.Rating, r.Bought, r.Discs, r.Pressing, r.FreeDbId,
  r.Cost, r.Media, r.RecordId, r.ArtistId, r.CoverName, r.Review
  from Artist a INNER JOIN Record r ON
  a.ArtistId = r.ArtistId
  WHERE a.artistid = @artistid
  order by r.Recorded desc
GO
/****** Object:  StoredProcedure [dbo].[up_getArtistTitle]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_getArtistTitle]
AS
SELECT Name, ArtistId 
FROM Record
ORDER BY Recorded
GO
/****** Object:  StoredProcedure [dbo].[up_getBiography]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_getBiography]
	@RecordId int
AS
DECLARE @ArtistId int

SET @ArtistId = (Select ArtistId FROM Record WHERE RecordId=@RecordId)

SELECT ISNULL(Biography, '') 
FROM Artist
WHERE ArtistId = @ArtistId

GO
/****** Object:  StoredProcedure [dbo].[up_GetBoughtDiscCountForYear]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetBoughtDiscCountForYear]
	@year VARCHAR(20)
AS
SELECT SUM(Discs) 
	FROM Record 
	WHERE Bought LIKE '%'+@year+'%';
GO
/****** Object:  StoredProcedure [dbo].[up_GetDiscRecordsByRecordName]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetDiscRecordsByRecordName]
	@Name VARCHAR(80)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT r.RecordId, d.DiscId, a.Name AS ArtistName, r.Name, 
		d.DiscNo, d.FreeDbId, d.FreeDbDiscId, d.Length
	FROM Record r INNER JOIN
		Artist a ON r.ArtistId = a.ArtistId INNER JOIN
		Disc d ON r.RecordId = d.RecordId
	WHERE r.Name LIKE '%'+@Name+'%'
	ORDER BY a.LastName, a.FirstName, r.Recorded, DiscNo
END
GO
/****** Object:  StoredProcedure [dbo].[up_GetFaultyArtists]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetFaultyArtists]
AS

SET NOCOUNT ON;

SELECT
    a.ArtistId, a.FirstName, a.LastName, a.Name, a.Biography
FROM Artist AS a
WHERE a.Name Like 'The The%'
ORDER BY LastName
GO
/****** Object:  StoredProcedure [dbo].[up_getField]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_getField]
  @field varchar(48)
as
   -- create a query string to show all records
if @field = 'Jazz'
begin
   SELECT a.[name] as [Name], r.[Name] as Title, r.Field,
  r.Recorded, r.Rating, r.Bought, r.Discs,
  r.Cost, r.Media
   FROM Artist a INNER JOIN Record r ON
     a.ArtistId = r.ArtistId
   WHERE r.Field = @field or r.Field = 'Fusion'
   order by a.LastName, a.FirstName, r.Recorded
end
else
begin
   SELECT a.[name] as [Name], r.[Name] as Title, r.Field,
  r.Recorded, r.Rating, r.Bought, r.Discs,
  r.Cost, r.Media
   FROM Artist a INNER JOIN Record r ON
     a.ArtistId = r.ArtistId
   WHERE r.Field = @field
   order by a.LastName, a.FirstName, r.Recorded
end

GO
/****** Object:  StoredProcedure [dbo].[up_GetFieldNumber]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- this sp gets the number of records for a particular field type
CREATE PROCEDURE [dbo].[up_GetFieldNumber]
  @field varchar(48)
as
select sum(discs) from record where field=@field

GO
/****** Object:  StoredProcedure [dbo].[up_getFieldOrdered]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[up_getFieldOrdered]
  @field varchar(48)
as
   -- create a query string to show all records
if @field = 'Jazz'
begin
  SELECT a.[name] as [Name], r.[Name] as Title, r.Field,
  r.Recorded, r.Rating, r.Bought, r.Discs,
  r.Cost, r.Media
   FROM Artist a INNER JOIN Record r ON
     a.ArtistId = r.ArtistId
   WHERE r.Field = @field or r.Field = 'Fusion'
   order by r.Recorded desc
end
else
begin
   SELECT a.[name] as [Name], r.[Name] as Title, r.Field,
  r.Recorded, r.Rating, r.Bought, r.Discs,
  r.Cost, r.Media
   FROM Artist a INNER JOIN Record r ON
     a.ArtistId = r.ArtistId
   WHERE r.Field = @field
   order by r.Recorded desc
end

GO
/****** Object:  StoredProcedure [dbo].[up_GetFullArtistByName]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetFullArtistByName]
	@Name VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT ArtistId, FirstName, LastName, Name, Biography FROM Artist WHERE Name = @Name;
END
GO
/****** Object:  StoredProcedure [dbo].[up_getFullArtistList]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_getFullArtistList]
as
select ArtistId, isnull(FirstName, ''), LastName, [Name], Biography
FROM Artist
ORDER BY LastName, FirstName

GO
/****** Object:  StoredProcedure [dbo].[up_GetMediaCountByType]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetMediaCountByType]
	@MediaType INT
AS
BEGIN
	SET NOCOUNT ON;

	IF @MediaType = 0
	BEGIN
		SELECT SUM(Discs) 
			FROM Record 
			WHERE @MediaType = 0
	END
	ELSE IF @MediaType = 1
	BEGIN
		SELECT SUM(Discs) 
			FROM Record 
			WHERE Media = 'DVD' OR Media = 'CD/DVD' OR Media = 'Blu-ray' OR Media = 'CD/Blu-ray'
	END
	ELSE IF @MediaType = 2
	BEGIN
		SELECT SUM(Discs) 
			FROM Record 
			WHERE Media = 'CD'
	END
	ELSE IF @MediaType = 3
	BEGIN
		SELECT SUM(Discs) 
			FROM Record 
			WHERE Media = 'R'
	END
END
GO
/****** Object:  StoredProcedure [dbo].[up_GetNoRecordReview]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[up_GetNoRecordReview]
AS
	SET NOCOUNT ON;

SELECT a.ArtistId, A.Name AS ArtistName, r.RecordId, r.Name AS RecordName, r.Recorded
	FROM Artist a INNER JOIN
	Record r ON a.ArtistId = r.ArtistId
	WHERE r.Review IS NULL OR len(Convert(Varchar(8000), r.Review)) < 5
GO
/****** Object:  StoredProcedure [dbo].[up_GetNoRecordReviewCount]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[up_GetNoRecordReviewCount]
AS
	SET NOCOUNT ON;

SELECT Sum(1)
	FROM Record
	WHERE Review IS NULL OR len(Convert(Varchar(8000), Review)) < 5

GO
/****** Object:  StoredProcedure [dbo].[up_GetNumberOfAlbums]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[up_GetNumberOfAlbums]
  @RecordId int
as
select discs
from  Record
where Record.RecordId = @RecordId

GO
/****** Object:  StoredProcedure [dbo].[up_GetNumberOfRecordsForYear]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- this sp gets the number of records for a particular recorded year
CREATE PROCEDURE [dbo].[up_GetNumberOfRecordsForYear]
  @year INT
AS
	SET NOCOUNT ON;

	SELECT SUM(discs) AS Count FROM Record WHERE Recorded = @year
GO
/****** Object:  StoredProcedure [dbo].[up_GetNumberOfTracks]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[up_GetNumberOfTracks]
  @recordid int
as
select count (1)
from  Artist inner join
               Record on Artist.ArtistId = Record.ArtistId INNER JOIN
               Disc on Record.RecordId = Disc.RecordId INNER JOIN
               Track on [Disc].DiscId = Track.DiscId
where Record.RecordId = @RecordId

GO
/****** Object:  StoredProcedure [dbo].[up_GetRating]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetRating]
  @rating varchar(4)
as
  -- Create a query string to show all records
  SELECT a.[name] as [Name], r.[Name] as Title, r.Field,
  r.Recorded, r.Rating, r.Bought, r.Discs,
  r.Cost, r.Media
  FROM Artist a INNER JOIN Record r ON
    a.ArtistId = r.ArtistId WHERE r.rating = @rating
  ORDER BY a.LastName, a.FirstName, r.Recorded

GO
/****** Object:  StoredProcedure [dbo].[up_GetRatingNumber]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- this sp gets the number of records with a rating of four stars
CREATE PROCEDURE [dbo].[up_GetRatingNumber]
as
select count(Rating) from record where Rating='****'

GO
/****** Object:  StoredProcedure [dbo].[up_GetRecordById]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetRecordById]
	@RecordId int
AS

SET NOCOUNT ON;

SELECT
    r.ArtistId, r.RecordId, r.Name, r.Field, r.Recorded,
    r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.Bought,
    r.Cost, r.CoverName, r.Review
FROM Record AS r 
WHERE r.RecordId = @RecordId

GO
/****** Object:  StoredProcedure [dbo].[up_GetRecordByName]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetRecordByName]
	@Name VARCHAR(60)
AS
	SELECT Record.*
		FROM Record
		WHERE Record.Name LIKE '%' + @Name + '%' 
GO
/****** Object:  StoredProcedure [dbo].[up_GetRecordByPartialName]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetRecordByPartialName]
	@Name NVARCHAR(250)
AS
SELECT
    a.ArtistId, a.name AS ArtistName, r.RecordId, r.Name, r.Field, r.Recorded,
    r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.Bought,
    r.Cost, r.CoverName, r.Review, r.FreeDBID
FROM Record AS r INNER JOIN
	Artist AS a ON r.ArtistId = a.ArtistId
	WHERE r.Name LIKE '%'+@Name+'%'
GO
/****** Object:  StoredProcedure [dbo].[up_GetRecordCountForYear]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- this sp gets the number of records for a particular recorded year
CREATE PROCEDURE [dbo].[up_GetRecordCountForYear]
  @year INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Count INT = 0;

    SELECT @Count = ISNULL(SUM(discs), 0) FROM Record WHERE Recorded = @year;

    SELECT @Count AS RecordCount;
END


GO
/****** Object:  StoredProcedure [dbo].[up_GetRecordedYearNumber]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- this sp gets the number of records for a particular recorded year
CREATE PROCEDURE [dbo].[up_GetRecordedYearNumber]
  @year INT
AS
SELECT SUM(discs) FROM Record WHERE Recorded = @year

GO
/****** Object:  StoredProcedure [dbo].[up_GetRecordList]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetRecordList]
AS
SELECT ArtistId, RecordId, [Name], Field,
  Recorded, Rating, Bought, Discs,
  Cost, Media
  FROM Record
  ORDER BY ArtistId, Recorded

GO
/****** Object:  StoredProcedure [dbo].[up_getRecordListandNone]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_getRecordListandNone]
    @ArtistId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    CREATE TABLE #temp
    (
        RecordId INT,
        Name VARCHAR(200),
        Recorded INT,
        SortOrder INT  -- Added sort order column
    )

    -- Always insert the "Select a record" option first with SortOrder = 0
    INSERT INTO #temp (RecordId, Name, Recorded, SortOrder)
    VALUES (0, '#Select a record', 0, 0)
    
    -- Insert artist's records with SortOrder = 1
    INSERT INTO #temp (RecordId, Name, Recorded, SortOrder)
    SELECT RecordId, [Name] + ' (' + RTRIM(Media) + ')', Recorded, 1
    FROM Record
    WHERE (ArtistId = @ArtistId OR @ArtistId IS NULL)

    -- Return results based on count
    IF (SELECT COUNT(1) FROM #temp WHERE SortOrder = 1) > 0
    BEGIN
        -- Return both the "Select" option and records, ordered properly
        SELECT [RecordId], [Name] 
        FROM #temp
        ORDER BY SortOrder, Recorded DESC
    END
    ELSE
    BEGIN
        -- Just return the "none" option if no records found
        SELECT 0 AS RecordId, 'none' AS Name
    END
END
GO
/****** Object:  StoredProcedure [dbo].[up_GetRecordsByArtistId]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetRecordsByArtistId]
	@ArtistId INT
AS
BEGIN
	SELECT Record.* 
	FROM Record
	WHERE ArtistId = @ArtistId
	ORDER BY Record.Recorded DESC
END
GO
/****** Object:  StoredProcedure [dbo].[up_GetRecordsByArtistName]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetRecordsByArtistName]
    @ArtistName VARCHAR(100)
AS
    SET NOCOUNT ON
BEGIN
    SELECT
        r.RecordId,
        r.[Name],
        r.Field,
        r.Label,
        r.Recorded,
        r.Rating,
        r.Bought,
        r.Discs,
        r.Pressing,
        r.Cost,
        r.Media,
        r.CoverName,
        r.Review,
        a.ArtistId,
        a.FirstName,
        a.LastName,
        a.[Name] AS ArtistName,
        a.Biography
    FROM Artist a
        INNER JOIN Record r ON a.ArtistId = r.ArtistId
    WHERE a.Name = @ArtistName
    ORDER BY r.Recorded DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[up_GetRecordsByYear]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[up_GetRecordsByYear]
	@Recorded INT
AS
	SET NOCOUNT ON

BEGIN
	SELECT
		a.ArtistId, a.Name AS artistName, r.RecordId, r.Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, Bought, r.Cost, r.CoverName, r.Review
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.recorded = @Recorded
	ORDER BY a.LastName, a.FirstName, r.Bought
END
GO
/****** Object:  StoredProcedure [dbo].[up_GetRecordsWithNoTracks]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[up_GetRecordsWithNoTracks]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT 
		a.ArtistId, 
		a.Name AS ArtistName, 
		r.RecordId, 
		r.Name AS Name, 
		r.Recorded,
		d.DiscId, 
		d.DiscNo
	FROM Artist a
	INNER JOIN Record r ON a.ArtistId = r.ArtistId
	LEFT JOIN Disc d ON r.RecordId = d.RecordId
	WHERE NOT EXISTS (
		SELECT 1 
		FROM Track t 
		WHERE t.DiscId = d.DiscId
	)
	ORDER BY a.LastName, a.FirstName, r.Recorded, d.DiscNo
END
GO
/****** Object:  StoredProcedure [dbo].[up_GetSingleArtist]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetSingleArtist]
   @Artistid int
As
select ArtistId, FirstName, LastName, [name], biography
from Artist
where Artist.ArtistId=@ArtistId

GO
/****** Object:  StoredProcedure [dbo].[up_getSingleArtistAndRecord]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_getSingleArtistAndRecord]
   @Recordid int
As
select r.RecordId, r.ArtistId, a.Name AS ArtistName, r.[Name], r.[Field], r.Recorded, r.Label, r.Pressing, r.Rating, r.Discs, r.Bought, r.Media, r.Cost, r.CoverName, r.Review
	from Artist a INNER JOIN 
	Record r ON a.ArtistId = r.ArtistId
where r.RecordId = @RecordId
GO
/****** Object:  StoredProcedure [dbo].[up_getSingleRecord]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_getSingleRecord]
   @Recordid int
As
select RecordId, artistId, [Name], [Field], Recorded, Label, Pressing, Rating, Discs, Bought, Media, Cost, coverName, Review
from Record
where RecordId = @RecordId

GO
/****** Object:  StoredProcedure [dbo].[up_GetTotalCostOfAllCDs]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetTotalCostOfAllCDs]
AS
SET NOCOUNT ON;

SELECT SUM(cost) FROM Record WHERE Media = 'CD'
GO
/****** Object:  StoredProcedure [dbo].[up_GetTotalNumberOfAllBlurays]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetTotalNumberOfAllBlurays]
as
select sum(discs) from record where media='Blu-ray'  or media='CD/Blu-ray'

GO
/****** Object:  StoredProcedure [dbo].[up_GetTotalNumberOfAllCDs]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- TODO: This SP needs to be updated.
CREATE PROCEDURE [dbo].[up_GetTotalNumberOfAllCDs]
as
select sum(discs) from record where media='CD' or media='CD/DVD'

/*
DECLARE Total INT;

    -- count CD's only.
    SET Total = (SELECT SUM(Discs) FROM record WHERE media = 'CD');

    -- count CD's in DVD and Blu-ray sets.
    SET Total = Total + (SELECT SUM(Discs) FROM record
                        WHERE (media = 'CD/DVD' OR media = 'CD/Blu-ray')
                        AND Discs > 1);

    -- Subtract one disc from each DVD or Blu-ray set.
    SET Total = Total - (SELECT COUNT(*) FROM record
                        WHERE (media = 'CD/DVD' OR media = 'CD/Blu-ray')
                        AND Discs > 1);

    SELECT Total;
*/

GO
/****** Object:  StoredProcedure [dbo].[up_GetTotalNumberOfAllDVDs]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetTotalNumberOfAllDVDs]
as
select sum(discs) from record where media='DVD' or media='CD/DVD' or media='Blu-ray'  or media='CD/Blu-ray'

GO
/****** Object:  StoredProcedure [dbo].[up_GetTotalNumberOfAllRecords]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetTotalNumberOfAllRecords]
as
select sum(discs) from record

GO
/****** Object:  StoredProcedure [dbo].[up_GetTotalNumberOfArtists]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[up_GetTotalNumberOfArtists]
as
select sum(1) from artist

GO
/****** Object:  StoredProcedure [dbo].[up_GetTotalNumberOfRecords]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetTotalNumberOfRecords]
as
select sum(discs) from record where media = 'R'

GO
/****** Object:  StoredProcedure [dbo].[up_GetTotalYearCost]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetTotalYearCost]
  @Year INT
AS
SET NOCOUNT ON;
DECLARE @Datestr1 VARCHAR(20)
DECLARE @Datestr2 VARCHAR(20)
SET @Datestr1=CONVERT(VARCHAR(4), @Year-1)+'/12/31 0:0:0'
SET @Datestr2=CONVERT(VARCHAR(4), @Year+1)+'/1/1 0:0:0'
DECLARE @Start DATETIME
DECLARE @Finish DATETIME
SET @Start=CONVERT(DATETIME, @Datestr1, 101)
SET @Finish=CONVERT(DATETIME, @Datestr2, 101)
SELECT SUM(Cost) FROM Record WHERE Bought > @Start AND Bought < @Finish
GO
/****** Object:  StoredProcedure [dbo].[up_GetTotalYearNumber]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- this sp gets the number of records bought for a particular year
CREATE PROCEDURE [dbo].[up_GetTotalYearNumber]
  @year int
as
declare @datestr1 varchar(20)
declare @datestr2 varchar(20)
set @datestr1=Convert(varchar(4), @year-1)+'/12/31 0:0:0'
set @datestr2=Convert(varchar(4), @year+1)+'/1/1 0:0:0'
declare @start datetime
declare @finish datetime
set @start=Convert(datetime, @datestr1, 101)
set @finish=Convert(datetime, @datestr2, 101)
select sum(discs) from record where bought > @start and bought < @finish

GO
/****** Object:  StoredProcedure [dbo].[up_GetTrackListing]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetTrackListing]
  @recordid INT
AS
BEGIN
    SET NOCOUNT ON;

	SELECT a.Name AS ArtistName, 
       r.RecordId, 
       r.Name AS Name,
       d.DiscId,  
       d.DiscNo, 
       d.FreeDbDiscId,
       d.FreeDbId, 
       d.Length,
       t.TrackId, 
       t.TrackNo, 
       t.Name AS TrackName, 
       t.TrackLength,
       t.Extended
	FROM Artist a
		INNER JOIN Record r ON a.ArtistId = r.ArtistId 
		INNER JOIN Disc d ON r.RecordId = d.RecordId 
		LEFT JOIN Track t ON d.DiscId = t.DiscId
	WHERE r.RecordId = @RecordId
	ORDER BY d.DiscNo, t.TrackNo
END
GO
/****** Object:  StoredProcedure [dbo].[up_GetYear2000]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetYear2000]
as
  --Create a query string to show all records bought in 2000
  SELECT a.[name] as [Name], r.[Name] as Title, r.Field,
  r.Recorded, r.Rating, r.Bought, r.Discs,
  r.Cost, r.Media
  from Artist a INNER JOIN Record r ON
	a.ArtistId = r.ArtistId
   WHERE (YEAR(r.Bought) > 1999) AND (YEAR(r.Bought) < 2001) order by r.bought desc

GO
/****** Object:  StoredProcedure [dbo].[up_GetYear2001]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetYear2001]
as
   -- Create a query string to show all records bought in 2001
  SELECT a.[name] as [Name], r.[Name] as Title, r.Field,
  r.Recorded, r.Rating, r.Bought, r.Discs,
  r.Cost, r.Media
   from Artist a INNER JOIN Record r ON
     a.ArtistId = r.ArtistId
   WHERE (YEAR(r.Bought) > 2000) AND (YEAR(r.Bought) < 2002) order by r.Bought desc

GO
/****** Object:  StoredProcedure [dbo].[up_Getyear2002]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_Getyear2002]
as
    -- Create a query string to show all records bought in 2002
  select a.[name] as [Name], r.[Name] as Title, r.Field,
  r.Recorded, r.Rating, r.Bought, r.Discs,
  r.Cost, r.Media
    from Artist a INNER JOIN Record r ON
      a.ArtistId = r.ArtistId
    WHERE (YEAR(r.Bought) > 2001) AND (YEAR(r.Bought) < 2003) order by r.bought desc

GO
/****** Object:  StoredProcedure [dbo].[up_GetYear2003]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetYear2003]
as
   -- Create a query string to show all records bought in 2003
  SELECT a.[name] as [Name], r.[Name] as Title, r.Field,
  r.Recorded, r.Rating, r.Bought, r.Discs,
  r.Cost, r.Media
   from Artist a INNER JOIN Record r ON
   a.ArtistId = r.ArtistId
   WHERE (YEAR(r.Bought) > 2002) AND (YEAR(r.Bought) < 2004) order by r.Bought desc

GO
/****** Object:  StoredProcedure [dbo].[up_GetYear2004]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetYear2004]
as
  -- Create a query string to show all records bought in 2004
  SELECT a.[name] as [Name], r.[Name] as Title, r.Field,
  r.Recorded, r.Rating, r.Bought, r.Discs,
  r.Cost, r.Media
  FROM Artist a INNER JOIN Record r ON
  a.ArtistId = r.ArtistId
  WHERE (YEAR(r.Bought) > 2003) AND (YEAR(r.Bought) < 2005) order by r.bought desc

GO
/****** Object:  StoredProcedure [dbo].[up_GetYear2005]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetYear2005]
as
  -- Create a query string to show all records bought in 2005
  SELECT a.[name] as [Name], r.[Name] as Title, r.Field,
  r.Recorded, r.Rating,
  r.Bought as Bought, r.Discs as Discs,
  r.Cost as Cost, r.Media as Media
  from Artist a INNER JOIN Record r ON
  a.ArtistId = r.ArtistId
  WHERE (YEAR(r.Bought) > 2004) AND (YEAR(r.Bought) < 2006) order by r.bought desc

GO
/****** Object:  StoredProcedure [dbo].[up_GetYear2006]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetYear2006]
AS
  -- Create a query string to show all records bought in 2006
  SELECT a.[name] as [Name], r.[Name] as Title, r.Field,
  r.Recorded, r.Rating, r.Bought, r.Discs,
  r.Cost, r.Media
  from Artist a INNER JOIN record r ON
  a.ArtistId = r.ArtistId
  WHERE (YEAR(r.Bought) > 2005) AND (YEAR(r.Bought) < 2007) order by r.Bought desc

GO
/****** Object:  StoredProcedure [dbo].[up_GetYear2007]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetYear2007]
as
  -- Create a query string to show all records bought in 2007
  SELECT a.[name] as [Name], r.[Name] as Title, r.Field,
  r.Recorded, r.Rating, r.Bought, r.Discs,
  r.Cost, r.Media
  from Artist a INNER JOIN Record r ON
  a.ArtistId = r.ArtistId
  WHERE (YEAR(r.Bought) > 2006) AND (YEAR(r.Bought) < 2008) order by r.Bought desc

GO
/****** Object:  StoredProcedure [dbo].[up_GetYear2008]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetYear2008]
as
  -- Create a query string to show all records bought in 2008
  SELECT a.[name] as [Name],
  r.[Name] as Title, r.Field, r.Recorded, r.Rating,
  r.Bought, r.Discs, r.Cost, r.Media
  from Artist a INNER JOIN Record r ON
  a.ArtistId = r.ArtistId
  WHERE (YEAR(r.Bought) > 2007) AND (YEAR(r.Bought) < 2009) order by r.bought desc

GO
/****** Object:  StoredProcedure [dbo].[up_GetYear2009]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetYear2009]
as
  -- Create a query string to show all records bought in 2009
  SELECT a.[name] AS [Name], r.[Name] as Title, r.Field,
  r.Recorded, r.Rating, r.Bought, r.Discs,
  r.Cost, r.Media
  from Artist a INNER JOIN Record r ON
  a.ArtistId = r.ArtistId
  WHERE (YEAR(r.Bought) > 2008) AND (YEAR(r.Bought) < 2010) order by r.bought desc

GO
/****** Object:  StoredProcedure [dbo].[up_GetYear2010]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetYear2010]
as
  -- Create a query string to show all records bought in 2010
  SELECT a.[name] as [Name],
  r.[Name] as Title, r.Field as Field,
  r.Recorded, r.Rating,
  r.Bought, r.Discs,
  r.Cost, r.Media
  from Artist a INNER JOIN Record r ON
  a.ArtistId = r.ArtistId
  WHERE (YEAR(r.Bought) > 2009) AND (YEAR(r.Bought) < 2011) order by r.bought desc

GO
/****** Object:  StoredProcedure [dbo].[up_GetYear2011]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetYear2011]
as
  -- Create a query string to show all records bought in 2010
  SELECT a.[name],
  r.[Name] as [Title], r.Field,
  r.Recorded, r.Rating,
  r.Bought, r.Discs,
  r.Cost, r.Media
  from Artist a INNER JOIN Record r ON
  a.ArtistId = r.ArtistId
  WHERE (YEAR(r.Bought) > 2010) AND (YEAR(r.Bought) < 2012) order by r.bought desc

GO
/****** Object:  StoredProcedure [dbo].[up_GetYear2012]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetYear2012]
as
  -- Create a query string to show all records bought in 2010
  SELECT a.[name],
  r.[Name] as [Title], r.Field,
  r.Recorded, r.Rating,
  r.Bought, r.Discs,
  r.Cost, r.Media
  from Artist a INNER JOIN Record r ON
  a.ArtistId = r.ArtistId
  WHERE (YEAR(r.Bought) > 2011) AND (YEAR(r.Bought) < 2013) order by r.bought desc

GO
/****** Object:  StoredProcedure [dbo].[up_GetYear2013]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetYear2013]
AS
  -- Create a query string to show all records bought in 2010
  SELECT a.[name],
  r.[Name] as [Title], r.Field,
  r.Recorded, r.Rating,
  r.Bought, r.Discs,
  r.Cost, r.Media
  from Artist a INNER JOIN Record r ON
  a.ArtistId = r.ArtistId
  WHERE (YEAR(r.Bought) > 2012) AND (YEAR(r.Bought) < 2014) order by r.bought desc

GO
/****** Object:  StoredProcedure [dbo].[up_GetYear2014]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_GetYear2014]
AS
  -- Create a query string to show all records bought in 2014
  SELECT a.[name],
  r.[Name] as [Title], r.Field,
  r.Recorded, r.Rating,
  r.Bought, r.Discs,
  r.Cost, r.Media
  from Artist a INNER JOIN Record r ON
  a.ArtistId = r.ArtistId
  WHERE (YEAR(r.Bought) > 2013) AND (YEAR(r.Bought) < 2015) order by r.bought desc

GO
/****** Object:  StoredProcedure [dbo].[up_InsertDisc]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_InsertDisc]
	@RecordId INT,
	@DiscNo INT, 
	@FreeDbDiscId INT = NULL, 
	@FreeDbId VARCHAR(10) = NULL, 
	@Length INT = NULL,
	@Result INT OUTPUT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @Inserted TABLE (DiscId INT)

	IF EXISTS(SELECT @RecordId FROM Record WHERE RecordId=@RecordId)
	BEGIN
		INSERT INTO Disc (RecordId, DiscNo, FreeDbDiscId, FreeDbId, Length) 
			OUTPUT INSERTED.DiscId INTO @Inserted
			VALUES (@RecordId, @DiscNo,	@FreeDbDiscId, @FreeDbId, @Length)

		SET @Result = (SELECT DiscId FROM @Inserted)
	END
	ELSE
	BEGIN
		SET @Result = -1
	END
END
GO
/****** Object:  StoredProcedure [dbo].[up_InsertTrack]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[up_InsertTrack]
    @DiscId INT,
	@TrackNo INT NULL,
	@Name VARCHAR(255) NULL,
	@TrackLength INT NULL,
	@Extended VARCHAR(255) NULL,
    @Result INT OUTPUT
AS
BEGIN

    SET NOCOUNT ON;

    DECLARE @Inserted TABLE (TrackId INT)

    INSERT INTO Track (DiscId, TrackNo, Name, TrackLength, Extended)
    OUTPUT INSERTED.TrackId INTO @Inserted
    VALUES (@DiscId, @TrackNo, @Name, @TrackLength, @Extended)

    SET @Result = (SELECT TrackId FROM @Inserted)
END
GO
/****** Object:  StoredProcedure [dbo].[up_InsertTracks]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_InsertTracks]
        @Tracks dbo.TrackTableType READONLY
 AS
 BEGIN
     INSERT INTO Track (DiscId, TrackNo, Name, TrackLength, Extended)
     SELECT DiscId, TrackNo, Name, TrackLength, Extended
     FROM @Tracks;
 END	
GO
/****** Object:  StoredProcedure [dbo].[up_MissingRecordReview]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[up_MissingRecordReview]
AS
	SET NOCOUNT ON;

SELECT a.ArtistId, A.Name AS Artist, r.RecordId, r.Name, r.Recorded, r.Discs, r.Rating, r.Media
	FROM Artist a INNER JOIN
	Record r ON a.ArtistId = r.ArtistId
	WHERE r.Review IS NULL OR len(Convert(Varchar(8000), r.Review)) < 5

GO
/****** Object:  StoredProcedure [dbo].[up_NoBioCount]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- calculate the number of null Biographies
CREATE PROCEDURE [dbo].[up_NoBioCount]
AS
SELECT COUNT(*) FROM Artist WHERE Biography IS NULL;
GO
/****** Object:  StoredProcedure [dbo].[up_NoBiographyCount]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_NoBiographyCount]
	@Count INT OUTPUT
AS
SELECT @Count = COUNT(*) FROM Artist WHERE Biography IS NULL;
GO
/****** Object:  StoredProcedure [dbo].[up_NoRecordReviews]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[up_NoRecordReviews]
AS
SELECT a.Name, r.Name AS Record, r.RecordId 
	FROM Artist a INNER JOIN
	Record r ON a.ArtistId = r.ArtistId
	WHERE r.Review IS NULL OR len(Convert(Varchar(8000), r.Review)) < 5

GO
/****** Object:  StoredProcedure [dbo].[up_RecordDBBackup]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_RecordDBBackup]
as
declare @today datetime
declare @month varchar(48)
declare @year varchar(48)
declare @day varchar(48)
declare @message varchar(100)

set @year = convert(varchar(48),datepart(yy,getdate()))
set @month = convert(varchar(48),datepart(mm,getdate()))
set @day = convert(varchar(48),datepart(dd,getdate()))
set @today = convert(datetime, @year + '/' + @month + '/' + @day + ' 00:00:00')
set @message = 'RecordDB: '+cast(@today as varchar(48))

backup database RecordDB
to RecordDBDevice
   with
     init,
     Description = @message

-- create a backup device
/*
exec sp_addumpdevice 'DISK', 'RecordDBDevice',
	'c:\temp\RecordDB_BU.BAK'
*/

GO
/****** Object:  StoredProcedure [dbo].[up_RecordSelectAll]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_RecordSelectAll]
AS
SELECT
    a.name as ArtistName, r.RecordId, r.ArtistId, r.Name, r.Field, r.Recorded,
    r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.Bought,
    r.Cost, r.CoverName, r.Review, r.FreeDBID
FROM Record AS r
INNER JOIN Artist as a on
	a.ArtistId = r.ArtistId
ORDER BY a.LastName, a.FirstName, r.recorded

GO
/****** Object:  StoredProcedure [dbo].[up_RecordSelectById]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_RecordSelectById]
(
	@RecordId int
)
AS
SELECT
    a.ArtistId, '<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS ArtistName, r.RecordId, r.Name, r.Field, r.Recorded,
    r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.Bought,
    r.Cost, r.CoverName, r.Review, r.FreeDBID
FROM Record AS r INNER JOIN
	Artist AS a ON r.ArtistId = a.ArtistId
	WHERE r.RecordId = @RecordId

GO
/****** Object:  StoredProcedure [dbo].[up_RecordSelectByIdCore]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[up_RecordSelectByIdCore]
(
	@RecordId int
)
AS
SELECT
    a.ArtistId, a.FirstName, a.LastName, a.name AS ArtistName, a.Biography, 
	r.RecordId, r.Name, r.Field, r.Recorded, r.Label, r.Pressing, r.Rating, 
	r.Discs, r.Media, r.Bought, r.Cost, r.CoverName, r.Review, r.FreeDBID
FROM Record AS r INNER JOIN
	Artist AS a ON r.ArtistId = a.ArtistId
	WHERE r.RecordId = @RecordId
GO
/****** Object:  StoredProcedure [dbo].[up_RecordSelectShow]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_RecordSelectShow]
	@show VARCHAR(20)
AS
IF @show = 'all'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, 
		'<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, CONVERT(VARCHAR(10), r.Bought, 105)  Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	ORDER BY a.LastName, a.FirstName, r.recorded desc
END
ELSE IF @show = 'cd'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordID, r.ArtistId, 
		'<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, CONVERT(VARCHAR(10), r.Bought, 105)  Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.media = 'CD' OR r.media = 'CD/DVD' OR r.media = 'CD/Blu-ray' OR r.media = 'Blu-ray'
	ORDER BY a.LastName, a.FirstName, r.recorded desc
END
ELSE IF @show = 'records'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, 
		'<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, CONVERT(VARCHAR(10), r.Bought, 105)  Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.media = 'R'
	ORDER BY a.LastName, a.FirstName, r.recorded desc
END
ELSE IF @show = 'dvds'
BEGIN
    -- get dvd's
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, 
		'<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, CONVERT(VARCHAR(10), r.Bought, 105) Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.media = 'DVD' or r.media = 'CD/DVD' or r.media = 'Blu-ray' or r.media = 'CD/Blu-ray'
	ORDER BY r.Bought desc
END
ELSE IF @show = 'blurays'
BEGIN
    -- get blurays
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, 
		'<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, CONVERT(VARCHAR(10), r.Bought, 105) Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.media = 'Blu-ray' or r.media = 'CD/Blu-ray'
	ORDER BY r.Bought desc 
END
ELSE IF @show = '2015'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, 
		'<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, CONVERT(VARCHAR(10), r.Bought, 105) Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.bought > Convert(datetime,'31/12/2014', 103) AND r.bought < Convert(datetime, '01/01/2016', 103)
	ORDER BY r.bought DESC
END
ELSE IF @show = '2014'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, 
		'<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, CONVERT(VARCHAR(10), r.Bought, 105) Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.bought > Convert(datetime,'31/12/2013', 103) AND r.bought < Convert(datetime, '01/01/2015', 103)
	ORDER BY r.bought DESC
END
ELSE IF @show = '2013'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, 
		'<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, CONVERT(VARCHAR(10), r.Bought, 105) Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.bought > Convert(datetime,'31/12/2012', 103) AND r.bought < Convert(datetime, '01/01/2014', 103)
	ORDER BY r.bought DESC
END
ELSE IF @show = '2012'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, '<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, CONVERT(VARCHAR(10), r.Bought, 105) Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.bought > Convert(datetime,'31/12/2011', 103) AND r.bought < Convert(datetime, '01/01/2013', 103)
	ORDER BY r.bought DESC
END
ELSE IF @show = '2011'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, '<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, CONVERT(VARCHAR(10), r.Bought, 105) Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.bought > Convert(datetime,'31/12/2010', 103) AND r.bought < Convert(datetime, '01/01/2012', 103)
	ORDER BY r.bought DESC
END
ELSE IF @show = '2010'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, '<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, CONVERT(VARCHAR(10), r.Bought, 105) Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.bought > Convert(datetime,'31/12/2009', 103) AND r.bought < Convert(datetime, '01/01/2011', 103)
	ORDER BY r.bought DESC
END
ELSE IF @show = '1111'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, '<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, CONVERT(VARCHAR(10), r.Bought, 105) Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.rating = '****'
	ORDER BY a.LastName, a.FirstName, r.recorded
END
ELSE IF @show = 'Rock'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, '<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, CONVERT(VARCHAR(10), r.Bought, 105) Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Rock'
	ORDER BY a.LastName, a.FirstName, r.recorded
END
ELSE IF @show = 'Blues'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, '<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, CONVERT(VARCHAR(10), r.Bought, 105) Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Blues'
	ORDER BY a.LastName, a.FirstName, r.recorded
END
ELSE IF @show = 'Jazz'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, 
		'<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, CONVERT(VARCHAR(10), r.Bought, 105) Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Jazz' OR r.field='Fusion'
	ORDER BY a.LastName, a.FirstName, r.recorded
END
ELSE IF @show = 'Classical'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, 
		'<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, CONVERT(VARCHAR(10), r.Bought, 105) Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Classical'
	ORDER BY a.LastName, a.FirstName, r.recorded
END
ELSE IF @show = 'Soundtrack'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, 
		'<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, CONVERT(VARCHAR(10), r.Bought, 105) Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Soundtrack'
	ORDER BY a.LastName, a.FirstName, r.recorded
END
ELSE IF @show = 'Country'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, 
		'<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, CONVERT(VARCHAR(10), r.Bought, 105) Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Country'
	ORDER BY a.LastName, a.FirstName, r.recorded
END
ELSE IF @show = 'Rockdesc'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, 
		'<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, CONVERT(VARCHAR(10), r.Bought, 105) Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Rock'
	ORDER BY r.recorded DESC
END
ELSE IF @show = 'Bluesdesc'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, 
		'<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media,CONVERT(VARCHAR(10), r.Bought, 105) Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Blues'
	ORDER BY r.recorded DESC
END
ELSE IF @show = 'Jazzdesc'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, 
		'<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, CONVERT(VARCHAR(10), r.Bought, 105) Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Jazz' OR r.field = 'Fusion'
	ORDER BY r.recorded DESC
END
ELSE IF @show = 'Classicaldesc'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, 
		'<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, CONVERT(VARCHAR(10), r.Bought, 105) Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Classical'
	ORDER BY r.recorded DESC
END
ELSE IF @show = 'Soundtrackdesc'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, 
		'<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, CONVERT(VARCHAR(10), r.Bought, 105) Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Soundtrack'
	ORDER BY r.recorded DESC
END
ELSE IF @show = 'Countrydesc'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, 
		'<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, CONVERT(VARCHAR(10), r.Bought, 105) Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Country'
	ORDER BY r.recorded DESC
END
ELSE IF LEN(@show) = 5 AND SUBSTRING(@show, 1, 1) = 'r'
BEGIN
	DECLARE @strShow VARCHAR(20)
	DECLARE @intShow int

	SET @strShow = SUBSTRING(@show, 2, 4)
	SET @intShow = CAST(@strShow AS int)

    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, 
		'<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, CONVERT(VARCHAR(10), r.Bought, 105) Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE (r.recorded > @intShow - 1) AND (r.recorded < @intShow + 1)
	ORDER BY r.bought DESC
END
ELSE IF SUBSTRING(@show, 1, 3) = 'aid'
BEGIN
	DECLARE @strId VARCHAR(20)
	DECLARE @artistId int
	
	SET @strId = REPLACE(@show, 'aid', '')
	SET @artistId = CAST(@strId AS int)

    -- get an artist's records
	SELECT
		a.name AS artistName, r.RecordId, r.ArtistId, 
		'<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, 
		r.Field, r.Recorded, Label, r.Pressing, r.Rating, r.Discs, r.Media, 
		CONVERT(VARCHAR(10), r.Bought, 105) Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE a.ArtistId = @artistId
	ORDER BY r.Recorded DESC
END

GO
/****** Object:  StoredProcedure [dbo].[up_RecordSelectShowCore]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[up_RecordSelectShowCore]
	@show VARCHAR(20)
AS
IF @show = 'all'
BEGIN
    -- get records
	SELECT
		a.name AS artistName, r.RecordId, r.ArtistId, r.Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	ORDER BY a.LastName, a.FirstName, r.recorded desc
END
ELSE IF @show = 'cd'
BEGIN
    -- get records
	SELECT
		a.name AS artistName, r.RecordId, r.ArtistId, r.Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.media = 'CD' OR r.media = 'CD/DVD' OR r.media = 'CD/Blu-ray' OR r.media = 'Blu-ray'
	ORDER BY a.LastName, a.FirstName, r.recorded desc
END
ELSE IF @show = 'records'
BEGIN
    -- get records
	SELECT
		a.name AS artistName, r.RecordId, r.ArtistId, r.Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.media = 'R'
	ORDER BY a.LastName, a.FirstName, r.recorded desc
END
ELSE IF @show = 'dvds'
BEGIN
    -- get dvd's
	SELECT
		a.name AS artistName, r.RecordId, r.ArtistId, r.Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.media = 'DVD' or r.media = 'CD/DVD' or r.media = 'Blu-ray' or r.media = 'CD/Blu-ray'
	ORDER BY r.Bought desc
END
ELSE IF @show = 'blurays'
BEGIN
    -- get blurays
	SELECT
		a.name AS artistName, r.RecordId, r.ArtistId, r.Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.media = 'Blu-ray' or r.media = 'CD/Blu-ray'
	ORDER BY r.Bought desc 
END
ELSE IF @show = '2022'
BEGIN
    -- get records
	SELECT
		a.name AS artistName, r.RecordId, r.ArtistId, r.Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.bought > Convert(datetime,'31/12/2021', 103) AND r.bought < Convert(datetime, '01/01/2023', 103)
	ORDER BY r.bought DESC
END
ELSE IF @show = '2021'
BEGIN
    -- get records
	SELECT
		a.name AS artistName, r.RecordId, r.ArtistId, r.Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.bought > Convert(datetime,'31/12/2020', 103) AND r.bought < Convert(datetime, '01/01/2022', 103)
	ORDER BY r.bought DESC
END
ELSE IF @show = '2020'
BEGIN
    -- get records
	SELECT
		a.name AS artistName, r.RecordId, r.ArtistId, r.Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.bought > Convert(datetime,'31/12/2019', 103) AND r.bought < Convert(datetime, '01/01/2021', 103)
	ORDER BY r.bought DESC
END
ELSE IF @show = '2019'
BEGIN
    -- get records
	SELECT
		a.name AS artistName, r.RecordId, r.ArtistId, r.Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.bought > Convert(datetime,'31/12/2018', 103) AND r.bought < Convert(datetime, '01/01/2020', 103)
	ORDER BY r.bought DESC
END
ELSE IF @show = '2018'
BEGIN
    -- get records
	SELECT
		a.name AS artistName, r.RecordId, r.ArtistId, r.Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.bought > Convert(datetime,'31/12/2017', 103) AND r.bought < Convert(datetime, '01/01/2019', 103)
	ORDER BY r.bought DESC
END
ELSE IF @show = '2017'
BEGIN
    -- get records
	SELECT
		a.name AS artistName, r.RecordId, r.ArtistId, r.Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.bought > Convert(datetime,'31/12/2016', 103) AND r.bought < Convert(datetime, '01/01/2018', 103)
	ORDER BY r.bought DESC
END
ELSE IF @show = '1111'
BEGIN
    -- get records
	SELECT
		a.name AS artistName, r.RecordId, r.ArtistId, r.Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.rating = '****'
	ORDER BY a.LastName, a.FirstName, r.recorded
END
ELSE IF @show = 'Rock'
BEGIN
    -- get records
	SELECT
		a.name AS artistName, r.RecordId, r.ArtistId, r.Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Rock'
	ORDER BY a.LastName, a.FirstName, r.recorded
END
ELSE IF @show = 'Blues'
BEGIN
    -- get records
	SELECT
		a.name AS artistName, r.RecordId, r.ArtistId, r.Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Blues'
	ORDER BY a.LastName, a.FirstName, r.recorded
END
ELSE IF @show = 'Jazz'
BEGIN
    -- get records
	SELECT
		a.name AS artistName, r.RecordId, r.ArtistId, r.Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Jazz' OR r.field='Fusion'
	ORDER BY a.LastName, a.FirstName, r.recorded
END
ELSE IF @show = 'Classical'
BEGIN
    -- get records
	SELECT
		a.name AS artistName, r.RecordId, r.ArtistId, r.Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Classical'
	ORDER BY a.LastName, a.FirstName, r.recorded
END
ELSE IF @show = 'Soundtrack'
BEGIN
    -- get records
	SELECT
		a.name AS artistName, r.RecordId, r.ArtistId, r.Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Soundtrack'
	ORDER BY a.LastName, a.FirstName, r.recorded
END
ELSE IF @show = 'Country'
BEGIN
    -- get records
	SELECT
		a.name AS artistName, r.RecordId, r.ArtistId, r.Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Country'
	ORDER BY a.LastName, a.FirstName, r.recorded
END
ELSE IF @show = 'Rockdesc'
BEGIN
    -- get records
	SELECT
		a.name AS artistName, r.RecordId, r.ArtistId, r.Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Rock'
	ORDER BY r.recorded DESC
END
ELSE IF @show = 'Bluesdesc'
BEGIN
    -- get records
	SELECT
		a.name AS artistName, r.RecordId, r.ArtistId, r.Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Blues'
	ORDER BY r.recorded DESC
END
ELSE IF @show = 'Jazzdesc'
BEGIN
    -- get records
	SELECT
		a.name AS artistName, r.RecordId, r.ArtistId, r.Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Jazz' OR r.field = 'Fusion'
	ORDER BY r.recorded DESC
END
ELSE IF @show = 'Classicaldesc'
BEGIN
    -- get records
	SELECT
		a.name AS artistName, r.RecordId, r.ArtistId, r.Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Classical'
	ORDER BY r.recorded DESC
END
ELSE IF @show = 'Soundtrackdesc'
BEGIN
    -- get records
	SELECT
		a.name AS artistName, r.RecordId, r.ArtistId, r.Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Soundtrack'
	ORDER BY r.recorded DESC
END
ELSE IF @show = 'Countrydesc'
BEGIN
    -- get records
	SELECT
		a.name AS artistName, r.RecordId, r.ArtistId, r.Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Country'
	ORDER BY r.recorded DESC
END
ELSE IF LEN(@show) = 5 AND SUBSTRING(@show, 1, 1) = 'r'
BEGIN
	DECLARE @strShow VARCHAR(20)
	DECLARE @intShow int

	SET @strShow = SUBSTRING(@show, 2, 4)
	SET @intShow = CAST(@strShow AS int)

    -- get records
	SELECT
		a.name AS artistName, r.RecordId, r.ArtistId, r.Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE (r.recorded > @intShow - 1) AND (r.recorded < @intShow + 1)
	ORDER BY a.LastName, a.FirstName
END
ELSE IF SUBSTRING(@show, 1, 3) = 'aid'
BEGIN
	DECLARE @strId VARCHAR(20)
	DECLARE @artistId int
	
	SET @strId = REPLACE(@show, 'aid', '')
	SET @artistId = CAST(@strId AS int)

    -- get an artist's records
	SELECT
		a.name AS artistName, r.RecordId, r.ArtistId, r.Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE a.ArtistId = @artistId
	ORDER BY r.Recorded DESC
END
GO
/****** Object:  StoredProcedure [dbo].[up_RecordSelectShowNew]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_RecordSelectShowNew]
	@show VARCHAR(20)
AS
IF @show = 'all'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, 
		'<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	ORDER BY a.LastName, a.FirstName, r.recorded desc
END
ELSE IF @show = 'cd'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordID, r.ArtistId, 
		'<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.media = 'CD' OR r.media = 'CD/DVD' OR r.media = 'CD/Blu-ray' OR r.media = 'Blu-ray'
	ORDER BY a.LastName, a.FirstName, r.recorded desc
END
ELSE IF @show = 'records'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, 
		'<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.media = 'R'
	ORDER BY a.LastName, a.FirstName, r.recorded desc
END
ELSE IF @show = 'dvds'
BEGIN
    -- get dvd's
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, 
		'<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.media = 'DVD' or r.media = 'CD/DVD' or r.media = 'Blu-ray' or r.media = 'CD/Blu-ray'
	ORDER BY r.Bought desc
END
ELSE IF @show = 'blurays'
BEGIN
    -- get blurays
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, 
		'<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.media = 'Blu-ray' or r.media = 'CD/Blu-ray'
	ORDER BY r.Bought desc 
END
ELSE IF @show = '2022'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, 
		'<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.bought > Convert(datetime,'31/12/2021', 103) AND r.bought < Convert(datetime, '01/01/2023', 103)
	ORDER BY r.bought DESC
END
ELSE IF @show = '2021'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, 
		'<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.bought > Convert(datetime,'31/12/2020', 103) AND r.bought < Convert(datetime, '01/01/2022', 103)
	ORDER BY r.bought DESC
END
ELSE IF @show = '2020'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, 
		'<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.bought > Convert(datetime,'31/12/2019', 103) AND r.bought < Convert(datetime, '01/01/2021', 103)
	ORDER BY r.bought DESC
END
ELSE IF @show = '2019'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, 
		'<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.bought > Convert(datetime,'31/12/2018', 103) AND r.bought < Convert(datetime, '01/01/2020', 103)
	ORDER BY r.bought DESC
END
ELSE IF @show = '2018'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, '<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.bought > Convert(datetime,'31/12/2017', 103) AND r.bought < Convert(datetime, '01/01/2019', 103)
	ORDER BY r.bought DESC
END
ELSE IF @show = '2017'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, 
		'<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.bought > Convert(datetime,'31/12/2016', 103) AND r.bought < Convert(datetime, '01/01/2018', 103)
	ORDER BY r.bought DESC
END
ELSE IF @show = '1111'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, '<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.rating = '****'
	ORDER BY a.LastName, a.FirstName, r.recorded
END
ELSE IF @show = 'Rock'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, '<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Rock'
	ORDER BY a.LastName, a.FirstName, r.recorded
END
ELSE IF @show = 'Blues'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, '<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Blues'
	ORDER BY a.LastName, a.FirstName, r.recorded
END
ELSE IF @show = 'Jazz'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, 
		'<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Jazz' OR r.field='Fusion'
	ORDER BY a.LastName, a.FirstName, r.recorded
END
ELSE IF @show = 'Classical'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, 
		'<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Classical'
	ORDER BY a.LastName, a.FirstName, r.recorded
END
ELSE IF @show = 'Soundtrack'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, 
		'<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Soundtrack'
	ORDER BY a.LastName, a.FirstName, r.recorded
END
ELSE IF @show = 'Country'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, 
		'<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Country'
	ORDER BY a.LastName, a.FirstName, r.recorded
END
ELSE IF @show = 'Rockdesc'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, 
		'<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Rock'
	ORDER BY r.recorded DESC
END
ELSE IF @show = 'Bluesdesc'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, 
		'<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media,r.Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Blues'
	ORDER BY r.recorded DESC
END
ELSE IF @show = 'Jazzdesc'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, 
		'<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Jazz' OR r.field = 'Fusion'
	ORDER BY r.recorded DESC
END
ELSE IF @show = 'Classicaldesc'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, 
		'<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Classical'
	ORDER BY r.recorded DESC
END
ELSE IF @show = 'Soundtrackdesc'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, 
		'<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Soundtrack'
	ORDER BY r.recorded DESC
END
ELSE IF @show = 'Countrydesc'
BEGIN
    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, 
		'<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE r.Field = 'Country'
	ORDER BY r.recorded DESC
END
ELSE IF LEN(@show) = 5 AND SUBSTRING(@show, 1, 1) = 'r'
BEGIN
	DECLARE @strShow VARCHAR(20)
	DECLARE @intShow int

	SET @strShow = SUBSTRING(@show, 2, 4)
	SET @intShow = CAST(@strShow AS int)

    -- get records
	SELECT
		'<a href="../GetArtist/aid'+Convert(varchar(10), r.ArtistId)+'">'+a.name+'</a>' AS artistName, r.RecordId, r.ArtistId, 
		'<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, r.Field, r.Recorded,
		r.Label, r.Pressing, r.Rating, r.Discs, r.Media, r.Bought,
		r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE (r.recorded > @intShow - 1) AND (r.recorded < @intShow + 1)
	ORDER BY r.bought DESC
END
ELSE IF SUBSTRING(@show, 1, 3) = 'aid'
BEGIN
	DECLARE @strId VARCHAR(20)
	DECLARE @artistId int
	
	SET @strId = REPLACE(@show, 'aid', '')
	SET @artistId = CAST(@strId AS int)

    -- get an artist's records
	SELECT
		a.name AS artistName, r.RecordId, r.ArtistId, 
		'<a href="../GetRecord/'+Convert(varchar(10), r.RecordId)+'">'+r.Name+'</a>' AS Name, 
		r.Field, r.Recorded, Label, r.Pressing, r.Rating, r.Discs, r.Media, 
		r.Bought, r.Cost, r.CoverName, r.Review, r.FreeDBID
	FROM Record AS r
	INNER JOIN Artist AS a ON
		a.ArtistId = r.ArtistId
	WHERE a.ArtistId = @artistId
	ORDER BY r.Recorded DESC
END
GO
/****** Object:  StoredProcedure [dbo].[up_SelectAllCDs]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--Create a query string to show all CD records
CREATE PROCEDURE [dbo].[up_SelectAllCDs]
as
SELECT a.[name], r.[Name] as [Title], r.Field,
  r.Recorded, r.Rating, r.Bought, r.Discs, r.Cost, r.Media
from Artist a INNER JOIN Record r ON
		a.ArtistId = r.ArtistId WHERE r.media = 'CD'
                           or r.media = 'CD/DVD'
		order by a.LastName, a.FirstName, r.Recorded

GO
/****** Object:  StoredProcedure [dbo].[up_SelectAllDiscEntities]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_SelectAllDiscEntities]
AS
BEGIN
	SET NOCOUNT ON;
	SELECT r.RecordId, d.DiscId, a.Name AS ArtistName, r.Name, 
		d.DiscNo, d.FreeDbId, d.FreeDbDiscId, d.Length
	FROM Record r INNER JOIN
		Artist a ON r.ArtistId = a.ArtistId INNER JOIN
		Disc d ON r.RecordId = d.RecordId
	ORDER BY a.LastName, a.FirstName, r.Recorded, DiscNo
END
GO
/****** Object:  StoredProcedure [dbo].[up_SelectAllDiscs]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_SelectAllDiscs]
AS
SELECT r.RecordId, d.DiscId, a.Name AS ArtistName, r.Name, d.DiscNo, d.FreeDbId, d.FreeDbDiscId, d.Length
	FROM Record r INNER JOIN
	Artist a ON r.ArtistId = a.ArtistId INNER JOIN
	Disc d ON r.RecordId = d.RecordId
	WHERE Media = 'CD' and d.FreeDbDiscId is null
ORDER BY a.LastName, a.FirstName, r.Recorded, DiscNo

GO
/****** Object:  StoredProcedure [dbo].[up_SelectAllRecords]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ALTER  a query string to show all records
CREATE PROCEDURE [dbo].[up_SelectAllRecords]
as
SELECT a.[name] as [Name], r.[Name] as Title, r.Field,
  r.Recorded, r.Rating, r.Bought, r.Discs,
  r.Cost, r.Media
  from Artist a INNER JOIN Record r ON
  a.ArtistId = r.ArtistId
  order by a.LastName, a.FirstName, r.Recorded

GO
/****** Object:  StoredProcedure [dbo].[up_SelectAllVinyl]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_SelectAllVinyl]
as
SELECT a.[name] as [Name],
  r.[Name] as Title, r.Field,
  r.Recorded, r.Rating,
  r.Bought, r.Discs,
  r.Cost, r.Media
from Artist a INNER JOIN Record r ON
	a.ArtistId = r.ArtistId WHERE r.media = 'R'
order by a.LastName, a.FirstName, r.Recorded

GO
/****** Object:  StoredProcedure [dbo].[up_selectArtistsWithNoBio]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_selectArtistsWithNoBio]
AS
SELECT
    a.ArtistId, a.FirstName, a.LastName, a.Name, a.Biography
FROM Artist AS a
WHERE a.biography is null or a.biography like ''
ORDER BY a.LastName, a.FirstName

GO
/****** Object:  StoredProcedure [dbo].[up_SelectRecordReviews]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_SelectRecordReviews]
as
SELECT a.[name] as [Name], r.[Name] as Title, r.Review
  from Artist a INNER JOIN Record r ON
  a.ArtistId = r.ArtistId
  order by a.LastName, a.FirstName, r.Recorded

GO
/****** Object:  StoredProcedure [dbo].[up_SelectRecordReviews2]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_SelectRecordReviews2]
AS
BEGIN
SELECT a.[name] as [ArtistName], r.[Name], r.Review
  FROM Artist a INNER JOIN Record r ON
  a.ArtistId = r.ArtistId
  order by a.LastName, a.FirstName, r.Recorded
END
GO
/****** Object:  StoredProcedure [dbo].[up_SelectRecordReviewsCore]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[up_SelectRecordReviewsCore]
AS
BEGIN
SELECT a.[name] as [Name], r.[Name] AS Title, r.Review
  FROM Artist a INNER JOIN Record r ON
  a.ArtistId = r.ArtistId
  order by a.LastName, a.FirstName, r.Recorded
END
GO
/****** Object:  StoredProcedure [dbo].[up_SelectRecordTracks]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_SelectRecordTracks]
    @Name NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT a.Name AS ArtistName,
           r.RecordId,
           r.Name AS Name,
           d.DiscId,
           d.DiscNo,
           d.FreeDbDiscId,
           d.FreeDbId,
           d.Length,
           t.TrackId,
           t.TrackNo,
           t.Name AS TrackName,
           t.TrackLength,
           t.Extended
    FROM Artist a
        INNER JOIN Record r ON a.ArtistId = r.ArtistId
        INNER JOIN Disc d ON r.RecordId = d.RecordId
        LEFT JOIN Track t ON d.DiscId = t.DiscId
    WHERE r.RecordId = (
        SELECT TOP 1 RecordId
        FROM Record
        WHERE Name = @Name
        ORDER BY RecordId
    )
    ORDER BY r.RecordId, d.DiscNo, t.TrackNo
END
GO
/****** Object:  StoredProcedure [dbo].[up_SelectSingleDisc]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_SelectSingleDisc]
	@DiscId INT
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT r.RecordId, d.DiscId, a.Name AS ArtistName, r.Name, 
			d.DiscNo, d.FreeDbId, d.FreeDbDiscId, d.Length
	FROM Record r INNER JOIN
		Artist a ON r.ArtistId = a.ArtistId INNER JOIN
		Disc d ON r.RecordId = d.RecordId
	 WHERE DiscId = @DiscId
END
GO
/****** Object:  StoredProcedure [dbo].[up_SelectSingleTrack]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[up_SelectSingleTrack]
	@TrackId INT
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT a.Name AS ArtistName, r.RecordId, r.Name, 
		   d.DiscId, d.DiscNo, d.Length, t.TrackId, t.TrackNo, 
		   t.Name AS TrackName, t.Extended, t.TrackLength
	FROM Record r 
		INNER JOIN Artist a ON r.ArtistId = a.ArtistId 
		INNER JOIN Disc d ON r.RecordId = d.RecordId
		INNER JOIN Track t ON d.DiscId = t.DiscId
	 WHERE TrackId = @TrackId
END
GO
/****** Object:  StoredProcedure [dbo].[up_SelectYearRecorded]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ALTER  a query string to show all records recorded for a particular year
CREATE PROCEDURE [dbo].[up_SelectYearRecorded]
  @year INT
AS
  SELECT a.[name],
  r.[Name] as Title, r.Field,
  r.Recorded, r.Rating,
  r.Bought, r.Discs,
  r.[Cost], r.Media
  from Artist a INNER JOIN Record r ON
	a.ArtistId = r.ArtistId
  WHERE r.Recorded = @year
  ORDER BY a.LastName, a.FirstName

GO
/****** Object:  StoredProcedure [dbo].[up_UpdateArtist]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_UpdateArtist]
   @ArtistId int,
   @FirstName varchar(50)=null,
   @LastName varchar(50),
   @Name varchar(80)=null,
   @Biography text=null,
   @result int=0 output
As
   update Artist
     set FirstName=@FirstName, LastName=@LastName, [Name]=@Name, Biography=@Biography
     where ArtistId=@ArtistId
	 
   if (@@rowcount=1)
   begin
      set @result = @artistId
   end
  
  return @result
GO
/****** Object:  StoredProcedure [dbo].[up_UpdateArtistNames]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_UpdateArtistNames]
AS
-- update artist name where there is a FirstName and LastName
update artist
set name = FirstName+' '+LastName
FROM Artist
where FirstName is not null and Name is null

-- update artist name where there is only a LastName
update artist
set name = LastName
where FirstName is null and Name is null

GO
/****** Object:  StoredProcedure [dbo].[up_UpdateDisc]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_UpdateDisc]
	@DiscId INT,
	@DiscNo INT,
	@FreeDbDiscId INT = NULL, 
	@FreeDbId VARCHAR(10) = NULL, 
	@Length INT = NULL,
	@Result INT OUTPUT
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE Disc
	SET 
		DiscNo = @DiscNo,
		FreeDbDiscId = @FreeDbDiscId,
		FreeDbId = @FreeDbId,
		Length = @Length
	WHERE DiscId = @DiscId

	IF @@ROWCOUNT > 0
	BEGIN
		SET @Result = @DiscId
	END
	ELSE
	BEGIN
		SET @Result = 0
	END
END
GO
/****** Object:  StoredProcedure [dbo].[up_UpdateDiscLength]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[up_UpdateDiscLength]
	@DiscId INT,
	@Length INT NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	UPDATE Disc
		SET [Length] = @Length
	WHERE DiscId = @DiscId
END
GO
/****** Object:  StoredProcedure [dbo].[up_UpdateDiscRecord]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_UpdateDiscRecord]
	@DiscId INT,
	@FreeDbDiscId INT = NULL
AS

UPDATE Disc
	SET FreeDbDiscId = @FreeDbDiscId
	WHERE DiscId = @DiscId

--PRINT @FreeDbDiscId

-- Now update the FreeDbId
DECLARE @FreeDbId varchar(10)

SET @FreeDbId = (Select FreeDbId from FreeDB where DiscId = @FreeDbDiscId)

--PRINT '***'+@FreeDbId+'***'

IF @FreeDbId IS NOT NULL 
BEGIN
	UPDATE Disc
		SET FreeDbId = @FreeDbId
		WHERE DiscId = @DiscId
END
--else
--begin
--print 'No FreeDbId'
--end



GO
/****** Object:  StoredProcedure [dbo].[up_UpdateRecord]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[up_UpdateRecord]
(
	@RecordId int,
	@ArtistId int,
	@Name varchar(80),
	@Field varchar(50),
	@Recorded int,
	@Label varchar(50),
	@Pressing varchar(50),
	@Rating varchar(4),
	@Discs int,
	@Media varchar(50),
	@Bought datetime=null,
	@Cost money=null,
	@CoverName varchar(50)=null,
	@Review text=null,
    @Result int=0 OUTPUT
)
As
   update Record
     set artistid=@ArtistId, [name]=@Name, field=@Field,
	 recorded=@Recorded, label=@Label, pressing=@Pressing, rating=@Rating,
    	 discs=@Discs, media=@Media, bought=@Bought, cost=@Cost, covername=@CoverName,
   	 review=@Review
     where RecordId=@RecordId

   if (@@rowcount=1)
      set @result = @RecordId
   else
      select @result = 0
GO
/****** Object:  StoredProcedure [dbo].[up_UpdateTrack]    Script Date: 20/08/2026 9:57:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[up_UpdateTrack]
	@TrackId INT,
	@TrackNo INT,
	@Name VARCHAR(255),
	@TrackLength INT,
	@Extended VARCHAR(255),
	@Result INT OUTPUT
AS

SET NOCOUNT ON;

BEGIN
	UPDATE Track 
	SET Name = @Name, TrackNo = @TrackNo, TrackLength = @TrackLength,	
		Extended = @Extended
	WHERE TrackId = @TrackId

	IF (@@ROWCOUNT=1)
	BEGIN
		set @Result = @TrackId
	END
  
	RETURN @Result
END
GO
