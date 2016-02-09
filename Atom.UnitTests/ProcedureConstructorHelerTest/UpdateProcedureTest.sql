--Обнавление записи в таблицу MainTable
--Пареметры
--@id - pkid записи
--@idreq - ИД заявки
--@dat - ТА
--@Field1 - Поле 1
--@Field2 - Поле 2
CREATE PROCEDURE [dbo].[usp_MainTableUpdate]
	@id int,
	@idreq int,
	@dat datetime,
	@Field1 int,
	@Field2 varchar(max)
	
AS
if(exists(select * from [MainTable] where fl_del <>0 and idreq = @idreq and pkid = @id))
begin
	update MainTable 
		SET 
		Field1 = @Field1,
		Field2 = @Field2,
		fl_del = (case when [fl_del] = 3 then 2 else [fl_del] end)
	where pkid = @id
end
else
begin

	insert into [MainTable]
		([dats], [datf], [idRecord], [idreq], [idcp], [fl_del], [idul],
		[Field1], [Field2])
	select cast('3000-01-01' as datetime), cast('3000-01-01' as datetime), idRecord, @idreq, pkid, 2, idul, @Field1, @Field2
	from	
		MainTable
	where pkid = @id
end
