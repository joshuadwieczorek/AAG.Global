using System;

namespace AAG.Global.Data.Contracts
{
    public abstract class BaseTable
    {
        public string CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}