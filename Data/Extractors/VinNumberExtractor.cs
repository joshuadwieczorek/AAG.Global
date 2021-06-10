using AAG.Global.ExtensionMethods;
using System;
using System.Text.RegularExpressions;

namespace AAG.Global.Data.Extractors
{
    public class VinNumberExtractor : IDataExtractor
    {
        /// <summary>
        /// Extract vin number.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public object Extract(string url)
        {
            // If url is empty or bail.
            if (!url.HasValue() || !url.Trim('/').HasValue())
                return DBNull.Value;

            url = url.Trim('/');

            // Check url for vin regex match.
            Regex regex = new Regex(@"(-|=)[A-Za-z0-9]{12,20}", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant);
            MatchCollection matches = regex.Matches(url);

            // Check matches are empty then return DBNull.Value for data table.
            if (matches.Count.Equals(0) 
                || matches?[0].Groups[0] is null 
                || !matches[0].Groups[0].Value.HasValue())
                return DBNull.Value;

            return matches[0].Groups[0].Value.TrimStart('-');
        }
    }
}