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
        private ViewLoadedParams ViewLoadedParams { get; set; }
        private DynamoViewModel DynamoViewModel { get; set; }
        private PythonMigrationView MigrationView { get; set; }
        private PythonNodeViewCustomization PythonNodeView { get; set; }
        private PythonNode PythonNode { get; set; }

        public string Code { get; set; }
        public string AnalysisResult { get; set; }

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
            DynamoViewModel.AnalyzePythonCode += OnAnalyzeCode;
            DynamoView.CloseExtension += OnCloseExtension;
        }

        private void OnAnalyzeCode(Wpf.VariableInputNodeViewCustomization obj)
        {
            var pythonNodeView = obj as PythonNodeViewCustomization;
            if (pythonNodeView == null)
                throw new ArgumentNullException(nameof(pythonNodeView));

            this.PythonNodeView = pythonNodeView;
            this.PythonNode = pythonNodeView.pythonNodeModel;
            //AddCodeToWindow(this.PythonNode);
            this.ViewLoadedParams?.AddToExtensionsSideBar(ViewExtension, this.MigrationView);
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
            string twoTo3 = @"C:\Users\SylvesterKnudsen\Miniconda3\Tools\scripts\2to3.py";
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = @"C:\Users\SylvesterKnudsen\Miniconda3\envs\py3warning\python.exe";
            start.Arguments = string.Format("{0} {1}", twoTo3, FILE_PATH);
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    AnalysisResult = reader.ReadToEnd();
                    RaisePropertyChanged(nameof(AnalysisResult));
                }
            }
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

        private void ConvertCode()
        {
            string twoTo3 = @"C:\Users\SylvesterKnudsen\Miniconda3\Tools\scripts\2to3.py";
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = @"C:\Users\SylvesterKnudsen\Miniconda3\envs\py3warning\python.exe";
            start.Arguments = string.Format("{0} {1} {2}", twoTo3, "-w", FILE_PATH);
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    reader.ReadToEnd();
                }
            }

            string convertedPythonCode = System.IO.File.ReadAllText(FILE_PATH);
            this.PythonNode.Script = convertedPythonCode;
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
