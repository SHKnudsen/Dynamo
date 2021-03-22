using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynamo.Wpf.Linting.Rules;

namespace Dynamo.Wpf.Linting
{
    public interface ILinterRuleSet
    {
        /// <summary>
        /// Unique id for this rule set
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Name of this rule set
        /// </summary>
        string Name { get; }

        List<LinterRule> LinterRules { get; }
    }
}
