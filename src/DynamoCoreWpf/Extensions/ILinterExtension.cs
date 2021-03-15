using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynamo.Engine.Linting;

namespace Dynamo.Wpf.Extensions
{
    public interface ILinterExtension
    {
        /// <summary>
        /// A unique id for this extension instance. 
        /// 
        /// The id will be equivalent to the extension that implements this interface id.
        /// </summary>
        string UniqueId { get; }

        /// <summary>
        /// A name for the Extension.
        /// 
        /// The name will be equivalent to the extension that implements this interface name.
        /// </summary>
        string Name { get; }

        ILinterRuleSet RegisterRuleSet();
    }
}
