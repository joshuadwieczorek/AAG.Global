using System;

namespace AAG.Global.Data.Contracts
{
    public abstract class BaseTableFull : BaseTable
    {
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}