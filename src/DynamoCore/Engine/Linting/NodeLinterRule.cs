using System;

namespace Dynamo.Engine.Linting
{
    public abstract class NodeLinterRule : ILinterRule<NodeChangedEventArgs, NodeRuleEvaluationResult>
    {
        public string Id { get; private set; }
        public string SeverityCode { get; private set; }
        public string Description { get; private set; }
        public string CallToAction { get; private set; }

        protected NodeLinterRule(string id, string severityCode, string description, string callToAction)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException($"'{nameof(id)}' cannot be null or empty", nameof(id));
            }

            if (string.IsNullOrEmpty(severityCode))
            {
                throw new ArgumentException($"'{nameof(severityCode)}' cannot be null or empty", nameof(severityCode));
            }

            if (string.IsNullOrEmpty(description))
            {
                throw new ArgumentException($"'{nameof(description)}' cannot be null or empty", nameof(description));
            }

            if (string.IsNullOrEmpty(callToAction))
            {
                throw new ArgumentException($"'{nameof(callToAction)}' cannot be null or empty", nameof(callToAction));
            }

            Id = id;
            SeverityCode = severityCode;
            Description = description;
            CallToAction = callToAction;
        }

        public abstract NodeRuleEvaluationResult Evaluate(NodeChangedEventArgs e);
    }
}
