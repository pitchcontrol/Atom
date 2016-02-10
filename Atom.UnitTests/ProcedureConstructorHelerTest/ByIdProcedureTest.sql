--Чтение записи по pkid из таблицу MainTable
--Пареметры
--@id - pkid
CREATE PROCEDURE [dbo].[usp_MainTableById]
	@id int
AS
	SELECT 
		t.pkid id, 
		t.fl_del, 
		t.Field1,
		t.Field2,
		df.prim,
		df.usr,
		df.format,
		df.typ,
		df.nam,
		df.datadd,
		df.path,
		t.Field3,
		ter_NP_dic.nam from MainTable t
	left join vw_ut_files df on df.pkid = t.idfile
	left join ufn_Universal_dic('ter_NP_dic',1) ter_NP_dic on ter_NP_dic.id = t.Field3
	where t.pkid = @id