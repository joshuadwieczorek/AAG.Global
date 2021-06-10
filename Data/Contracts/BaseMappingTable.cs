using System;

namespace AAG.Global.Data.Contracts
{
    public abstract class BaseMappingTable
    {
        public string MappedBy { get; set; }
        public DateTime? MappedAt { get; set; }
    }
}