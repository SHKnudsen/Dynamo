using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Dynamo.Wpf.Linting;
using Dynamo.Wpf.Linting.Rules;

namespace Dynamo.LintingViewExtension.Converters
{
    [ValueConversion(typeof(CollectionViewGroup), typeof(LinterRule))]
    public class RuleFromRuleIdConverter : DependencyObject, IValueConverter
    {
        // The property used as a parameter
        public ILinterRuleSet CurrentLinter
        {
            get { return (ILinterRuleSet)GetValue(CurrentLinterProperty); }
            set { SetValue(CurrentLinterProperty, value); }
        }

        // The dependency property to allow the property to be used from XAML.
        public static readonly DependencyProperty CurrentLinterProperty =
            DependencyProperty.Register(
            nameof(CurrentLinter),
            typeof(ILinterRuleSet),
            typeof(RuleFromRuleIdConverter));


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is CollectionViewGroup cvg) || !(cvg.Name is string ruleId))
                return null;

            var linterRule = GetLinterRuleById(ruleId, CurrentLinter);
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
