using AAG.Global.ExtensionMethods;
using System;

namespace AAG.Global.Data.Extractors
{
    public class JobNumberExtractor : IDataExtractor
    {
        /// <summary>
        /// Extract job number from campaign.
        /// </summary>
        /// <param name="campaign"></param>
        /// <returns></returns>
        public object Extract(string campaign)
        {
            var jobNumber = string.Empty;
            if (campaign.Contains("-"))
            {
                int lastIndex = campaign.IndexOf("-");
                jobNumber = campaign.Substring(0, lastIndex);
            }

            if (!jobNumber.HasValue() || !int.TryParse(jobNumber[0].ToString(), out int oInt))
                return DBNull.Value;

            return jobNumber;
        }
    }
}
