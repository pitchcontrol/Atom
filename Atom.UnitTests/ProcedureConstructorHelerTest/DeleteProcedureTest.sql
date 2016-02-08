--Удаление записи из таблицы MainTable
--Пареметры
--@id - pkid записи
--@idreq - ИД заявки
--@dat - ТА
CREATE PROCEDURE [dbo].[usp_MainTableDelete]
	@id int,
	@idreq int,
	@dat datetime
AS
	declare @idt int = (select idRecord from MainTable where pkid=@id and fl_del in (1, 2, 3) and idreq=@idreq)
	delete from MainTable
	where pkid=@id and fl_del in (1, 2, 3) and idreq=@idreq;

	delete MainTable_id where pkid = @idt

	insert into MainTable (dats, datf, fl_del, idreq, idcp, idul, idRecord, Field1, Field2)
	select cast('3000-01-01' as datetime),cast('3000-01-01' as datetime), 3, @idreq, pkid, idul, idRecord, Field1, Field2
	from MainTable where pkid = @id and @dat>=dats and @dat<datf and fl_del=0

	DECLARE @iddel int = SCOPE_IDENTITY();
	update MainTable_id set fl_del = 3 where pkid = @iddel