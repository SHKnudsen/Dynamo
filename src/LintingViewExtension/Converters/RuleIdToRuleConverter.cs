using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Dynamo.Wpf.Linting;
using Dynamo.Wpf.Linting.Rules;

namespace Dynamo.LintingViewExtension.Converters
{
    [ValueConversion(typeof(CollectionViewGroup), typeof(LinterRule))]
    public class RuleIdToRuleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is CollectionViewGroup cvg) || !(cvg.Name is string ruleId))
                return null;

            if (!(parameter is LinterManager linter))
                return null;

            var linterRule = GetLinterRuleById(ruleId, linter.ActiveRuleSet);
            return linterRule;
            
        }
        private LinterRule GetLinterRuleById(string ruleId, ILinterRuleSet linter)
        {
            var linterRule = linter.LinterRules.Where(x => x.Id == ruleId).FirstOrDefault();
            return linterRule;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Boolean boolean))
            {
                return false;
            }

            return !boolean;
        }
    }
}



//public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
//{
//    if (!(value[0] is CollectionViewGroup cvg) || !(cvg.Name is string ruleId) || !(value[1] is LinterManager linter))
//        return null;


//    var linterRule = GetLinterRuleById(ruleId, linter.CurrentLinter);
//    return linterRule;

//}
//private ILinterRule<EventArgs, IRuleEvaluationResult> GetLinterRuleById(string ruleId, ILinterRuleSet linter)
//{
//    var linterRule = linter.LinterRules.Where(x => x.Id == ruleId).FirstOrDefault();
//    return linterRule;
//}

//public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
//{
//    return null;
//}
