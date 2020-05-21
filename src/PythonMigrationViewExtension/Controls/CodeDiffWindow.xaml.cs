using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dynamo.PythonMigration.Controls
{
    /// <summary>
    /// Interaction logic for CodeDiffWindow.xaml
    /// </summary>
    public partial class CodeDiffWindow : Window
    {
        private PythonMigrationViewModel ViewModel { get; set; }
        public CodeDiffWindow(PythonMigrationViewModel viewModel)
        {
            InitializeComponent();
            this.diffHtml.Navigate(@"C:\Users\SylvesterKnudsen\Documents\PythonPlayground\diff_file.html");
            this.ViewModel = viewModel;
            this.Owner = this.ViewModel.ViewLoadedParams.DynamoWindow;
        }

        private void OnAcceptChangesBtnClicked(object sender, RoutedEventArgs e)
        {
            ViewModel.ConvertCode();
            this.Close();
        }

        private void OnContinueWithOriginalBtnClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
