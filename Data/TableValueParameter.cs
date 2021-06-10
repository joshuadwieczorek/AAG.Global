using System.Data;
using Dapper;
using System.Data.SqlClient;

namespace AAG.Global.Data
{
    public class TableValueParameter<T> : SqlMapper.ICustomQueryParameter
    {
        private readonly TableGeneratorBase<T> _tableGenerator;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="tableGenerator"></param>
        public TableValueParameter(TableGeneratorBase<T> tableGenerator)
        {
            _tableGenerator = tableGenerator;
        }


        /// <summary>
        /// Add parameter.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="name"></param>
        public void AddParameter(
              IDbCommand command
            , string name)
        {
            SqlParameter paramater = (SqlParameter)command.CreateParameter();
            paramater.ParameterName = name;
            paramater.SqlDbType = SqlDbType.Structured;
            paramater.Value = _tableGenerator.Table;
            paramater.TypeName = _tableGenerator.Table.TableName;
            command.Parameters.Add(paramater);
        }
    }
}