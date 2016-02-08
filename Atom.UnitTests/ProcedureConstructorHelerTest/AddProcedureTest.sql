--Добавление записи в таблицу MainTable
--Пареметры
--@idreq - ИД заявки
--@idul - ИД родительской таблицы
--@Field1 - Поле 1
--@Field2 - Поле 2
CREATE PROCEDURE [dbo].[usp_MainTableAdd]
	@idreq int,
	@idul int,
	@Field1 int,
	@Field2 varchar(max)
AS
	INSERT INTO MainTable_id (fl_del)
    VALUES (1)
    DECLARE @id int = SCOPE_IDENTITY();

	INSERT INTO MainTable (idul, datf, dats, fl_del, idRecord, idreq, Field1, Field2)
    VALUES (@idul, '30000101', '30000101', 1, @id, @idreq, @Field1, @Field2)

	DECLARE @idcp int=scope_identity();

	update MainTable set idcp=@idcp where pkid=@idcp;
	select @idcp id