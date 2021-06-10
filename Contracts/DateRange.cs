using System;

namespace AAG.Global.Contracts
{
    public struct DateRange
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        public DateRange(
              DateTime startDate
            , DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}