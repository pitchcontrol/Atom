--Чтение записей из таблицы MainTable
--Пареметры
--@idul - ИД родительской таблицы
CREATE FUNCTION [dbo].[ufn_MainTableView]
	(@dat datetime, @idul int)
RETURNS TABLE 
AS
RETURN 
(
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

	where t.dats<=@dat and t.datf>@dat and  t.fl_del=0 and  t.idul = @idul and t.idreq is null

)