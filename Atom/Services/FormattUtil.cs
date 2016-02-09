using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atom.Services
{
    public class FormattUtil
    {
        public static string Format(string sql)
        {
            var tokenizer = new PoorMansTSqlFormatterLib.Tokenizers.TSqlStandardTokenizer();
            var parser = new PoorMansTSqlFormatterLib.Parsers.TSqlStandardParser();

            var tokenizedSql = tokenizer.TokenizeSQL(sql);
            
            var parsedSql = parser.ParseSQL(tokenizedSql);

            var innerFormatter = new PoorMansTSqlFormatterLib.Formatters.TSqlStandardFormatter();
            
            //webBrowser_OutputSql.SetHTML(_formatter.FormatSQLTree(parsedSql));
            return innerFormatter.FormatSQLTree(parsedSql);
        }
    }
}
