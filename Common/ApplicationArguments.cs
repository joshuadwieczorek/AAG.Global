using System.Linq;
using System.Collections.Generic;

namespace AAG.Global.Common
{
    public class ApplicationArguments
    {
        private string[] _arguments;


        /// <summary>
        /// Arguments.
        /// </summary>
        public List<string> Arguments
            => _arguments.ToList();


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="arguments"></param>
        public ApplicationArguments(string[] arguments)
            => _arguments = arguments;
    }
}