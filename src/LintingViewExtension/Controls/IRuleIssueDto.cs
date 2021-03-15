using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dynamo.Engine.Linting;
using Dynamo.Engine.Linting.Rules;

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