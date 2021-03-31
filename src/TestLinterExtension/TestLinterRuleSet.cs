using System.Collections.Generic;
using Dynamo.Wpf.Linting;
using Dynamo.Wpf.Linting.Rules;

namespace Dynamo.TestLinterExtension
{
    public class TestLinterRuleSet : ILinterRuleSet
    {
        public string Id => "c9c0646e-0d3e-4d50-b16a-0f577726217e";

        public string Name => this.GetType().Name;

        public List<LinterRule> LinterRules { get; }

        public TestLinterRuleSet()
        {
            LinterRules = new List<LinterRule>();
        }

        public TestLinterRuleSet(List<LinterRule> linterRules)
        {
            LinterRules = linterRules;
        }
    }
}
