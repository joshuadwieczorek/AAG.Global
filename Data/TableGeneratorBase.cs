using System;
using System.Data;
using System.Collections.Generic;

namespace AAG.Global.Data
{
    public abstract class TableGeneratorBase<T> : IDisposable
    {
        public DataTable Table { get; }
        protected abstract void Construct();
        public abstract void Populate(IEnumerable<T> rows);
        protected abstract void Populate(T row);


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="tableName"></param>
        public TableGeneratorBase(string tableName)
        {
            Table = new DataTable();
            Table.TableName = tableName;
            Construct();
        }


        /// <summary>
        /// Garbage cleanup.
        /// </summary>
        public void Dispose()
        {
            Table?.Dispose();
        }
    }
}