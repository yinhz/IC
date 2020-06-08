using Newtonsoft.Json;
using System;

namespace IC.Core
{
    /// <summary>
    /// Command 描述特性
    /// </summary>
    public class CommandProcessorDescription : Attribute
    {
        public CommandProcessorDescription(string CommandID)
        {
            this.CommandID = CommandID;
        }

        /// <summary>
        /// Command ID
        /// </summary>
        public string CommandID { get; set; }
    }
}