using System.Collections.ObjectModel;
using Dynamo.Wpf.Linting;
using Dynamo.Wpf.Linting.Rules;

namespace Dynamo.LintingViewExtension.Controls
{
    public interface IRuleIssueDto<TRuleArgType, TResultType> where TResultType : IRuleEvaluationResult
    {
        string Id { get; }
        ObservableCollection<TResultType> Results { get; }
        LinterRule Rule { get; }

        void AddResult(TResultType result);
    }
}