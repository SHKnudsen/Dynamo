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
    /// Interaction logic for OpenDiffTableDialog.xaml
    /// </summary>
    public partial class OpenDiffTableDialog : Window
    {
        PythonMigrationViewModel ViewModel { get; set; }

        public OpenDiffTableDialog(PythonMigrationViewModel viewModel)
        {
            this.ViewModel = viewModel;
            InitializeComponent();
        }

        private void OnAcceptBtnClicked(object sender, RoutedEventArgs e)
        {
            CodeDiffWindow codeDiff = new CodeDiffWindow(ViewModel);
            this.Close();
            codeDiff.Show();
        }

        private void OnDeclineBtnClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
