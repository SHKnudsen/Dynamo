using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Dynamo.Controls;
using Dynamo.Core;
using Dynamo.Graph.Workspaces;
using Dynamo.PythonMigration.Controls;
using Dynamo.ViewModels;
using Dynamo.Wpf;
using Dynamo.Wpf.Extensions;
using PythonNodeModels;
using PythonNodeModelsWpf;

namespace Dynamo.PythonMigration
{
    public class PythonMigrationViewModel : NotificationObject
    {
        private const string FILE_PATH = @"C:\Users\SylvesterKnudsen\test.py";

        private PythonMigrationViewExtension ViewExtension { get; set; }
        private DynamoViewModel DynamoViewModel { get; set; }
        private PythonMigrationView MigrationView { get; set; }
        private PythonNodeViewCustomization PythonNodeView { get; set; }
        private PythonNode PythonNode { get; set; }

        public ViewLoadedParams ViewLoadedParams { get; set; }
        public string Code { get; set; }
        public string RefactoredCode { get; set; }

        public PythonMigrationViewModel(PythonMigrationViewExtension viewExtension, ViewLoadedParams vlp, DynamoViewModel dynamoViewModel)
        {
            this.ViewExtension = viewExtension;
            this.ViewLoadedParams = vlp;
            this.DynamoViewModel = dynamoViewModel;

            MigrationView = new PythonMigrationView
            {
                DataContext = this,
            };
            SubscribToMigrationViewEvents();
            SubscribeToDynamoEvents();
        }

        private void SubscribeToDynamoEvents()
        {
            DynamoViewModel.CPythonEngineSelected += OnCPythonEngineSelected;
            DynamoView.CloseExtension += OnCloseExtension;
        }

        private void OnCPythonEngineSelected(Wpf.VariableInputNodeViewCustomization obj)
        {
            var pythonNodeView = obj as PythonNodeViewCustomization;
            if (pythonNodeView == null)
                throw new ArgumentNullException(nameof(pythonNodeView));

            this.PythonNodeView = pythonNodeView;
            this.PythonNode = pythonNodeView.pythonNodeModel;
            CreatePyFile(PythonNode.Script);

            if (RefactoredCode == PythonNode.Script)
                return;

            var openDiffDialog = new OpenDiffTableDialog(this);

            openDiffDialog.Show();
            openDiffDialog.Owner = ViewLoadedParams.DynamoWindow;
        }

        //private void AddCodeToWindow(PythonNode pythonNode)
        //{
        //    string code = pythonNode.Script;
        //    Code += code;
        //    RaisePropertyChanged(nameof(Code));
        //    CreatePyFile(code);
        //}

        private void CreatePyFile(string code)
        {
            System.IO.File.WriteAllText(FILE_PATH, code);
            AnalyzeCode();
        }

        private void AnalyzeCode()
        {
            string twoTo3 = @"C:\Users\SylvesterKnudsen\Documents\PythonPlayground\2to3_string.py";
            // Set working directory and create process
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = @"C:\Users\SylvesterKnudsen\Miniconda3\envs\py3warning\python.exe",
                    Arguments = string.Format("{0} {1}", twoTo3, FILE_PATH),
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    WorkingDirectory = @"C:\Users\SylvesterKnudsen\Documents\PythonPlayground",
                    CreateNoWindow = true
                }
            };

            process.Start();

            using (StreamReader reader = process.StandardOutput)
            {
                RefactoredCode = reader.ReadToEnd();
                RaisePropertyChanged(nameof(RefactoredCode));
            }

            process.WaitForExit();
        }

        private void SubscribToMigrationViewEvents()
        {
            MigrationView.convertButton.Click += OnConvertCode;
            MigrationView.revertButton.Click += OnRevertCode;
        }

        private void OnRevertCode(object sender, RoutedEventArgs e)
        {
            RevertCode();
        }

        private void OnConvertCode(object sender, System.Windows.RoutedEventArgs e)
        {
            ConvertCode();
        }

        internal void ConvertCode()
        {
            this.PythonNode.Script = RefactoredCode;
            this.PythonNodeView.UpdateScriptContent();
        }

        private void RevertCode()
        {
            string oldPythonCode = System.IO.File.ReadAllText(string.Format("{0}{1}",FILE_PATH,".BAK"));
            this.PythonNode.Script = oldPythonCode;
            this.PythonNodeView.UpdateScriptContent();
            CreatePyFile(oldPythonCode);
        }

        private void OnCloseExtension(String extensionTabName)
        {
            if (extensionTabName.Equals(ViewExtension.Name))
            {
                //this.pythonMigrationMenuItem.IsChecked = false;
            }
        }

    }
}
