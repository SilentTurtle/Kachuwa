DECLARE @Sql NVARCHAR(500) DECLARE @Cursor CURSOR

SET @Cursor = CURSOR FAST_FORWARD FOR
SELECT DISTINCT sql = 'ALTER TABLE [' + tc2.TABLE_NAME + '] DROP [' + rc1.CONSTRAINT_NAME + ']'
FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS rc1
LEFT JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc2 ON tc2.CONSTRAINT_NAME =rc1.CONSTRAINT_NAME

OPEN @Cursor FETCH NEXT FROM @Cursor INTO @Sql

WHILE (@@FETCH_STATUS = 0)
BEGIN
Exec sp_executesql @Sql
FETCH NEXT FROM @Cursor INTO @Sql
END

CLOSE @Cursor DEALLOCATE @Cursor
GO

EXEC sp_MSforeachtable 'DROP TABLE ?'


--declare @procName varchar(500)
--declare cur cursor 

--for select [name] from sys.objects where type = 'p'
--open cur
--fetch next from cur into @procName
--while @@fetch_status = 0
--begin
--    exec('drop procedure [' + @procName + ']')
--    fetch next from cur into @procName
--end
--close cur
--deallocate cur

 /* Drop all non-system stored procs */
DECLARE @SPname VARCHAR(128)
DECLARE @SQL VARCHAR(254)
SELECT @SPname = (SELECT TOP 1 [name] FROM sysobjects WHERE [type] = 'P' AND category = 0 ORDER BY [name])
WHILE @SPname is not null
BEGIN
    SELECT @SQL = 'DROP PROCEDURE [dbo].[' + RTRIM(@SPname ) +']'
    EXEC (@SQL)
    PRINT 'Dropped Procedure: ' + @SPname
    SELECT @SPname = (SELECT TOP 1 [name] FROM sysobjects WHERE [type] = 'P' AND category = 0 AND [name] > @SPname ORDER BY [name])
END
GO

/* Drop all views */
DECLARE @VWname VARCHAR(128)
DECLARE @SQL VARCHAR(254)
SELECT @VWname = (SELECT TOP 1 [name] FROM sysobjects WHERE [type] = 'V' AND category = 0 ORDER BY [name])
WHILE @VWname IS NOT NULL
BEGIN
    SELECT @SQL = 'DROP VIEW [dbo].[' + RTRIM(@VWname ) +']'
    EXEC (@SQL)
    PRINT 'Dropped View: ' + @VWname
    SELECT @VWname = (SELECT TOP 1 [name] FROM sysobjects WHERE [type] = 'V' AND category = 0 AND [name] > @VWname ORDER BY [name])
END
GO


/* Drop all functions */
DECLARE @FNname VARCHAR(128)
DECLARE @SQL VARCHAR(254)
SELECT @FNname = (SELECT TOP 1 [name] FROM sysobjects WHERE [type] IN (N'FN', N'IF', N'TF', N'FS', N'FT') AND category = 0 ORDER BY [name])
WHILE @FNname IS NOT NULL
BEGIN
    SELECT @SQL = 'DROP FUNCTION [dbo].[' + RTRIM(@FNname ) +']'
    EXEC (@SQL)
   -- PRINT 'Dropped Function: ' + @FNname
    SELECT @FNname = (SELECT TOP 1 [name] FROM sysobjects WHERE [type] IN (N'FN', N'IF', N'TF', N'FS', N'FT') AND category = 0 AND [name] > @FNname ORDER BY [name])
END
GO 


GO