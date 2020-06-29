using Dynamo.PythonMigration.MigrationAssistant;
using Dynamo.ViewModels;
using PythonNodeModels;
using PythonNodeModelsWpf;
using System.Collections.Generic;
using System.IO;

namespace Dynamo.PythonMigration
{
    internal class PythonMigrationAssistantViewModel
    {
        public string OldCode { get; set; }
        public string NewCode { get; set; }
        public ScriptEditorWindow OwnerWindow { get; set; }
        private PythonNode PythonNode { get; set; }
        public DynamoViewModel DynamoViewModel { get; set; }

        public PythonMigrationAssistantViewModel(PythonNode pythonNode, ScriptEditorWindow parentWindow, DynamoViewModel dynamoViewModel)
        {
            PythonNode = pythonNode;
            OwnerWindow = parentWindow;
            DynamoViewModel = dynamoViewModel;
            OldCode = pythonNode.Script;
            MigrateCode();
        }

        private void MigrateCode()
        {
            var migrationInputs = new List<string>()
            {
                "code"
            };
            var migrationInputValues = new List<object>()
            {
                OldCode
            };
            var migrationOutputVar = "output";

            NewCode = ScriptMigrator.MigrateCode(migrationInputs, migrationInputValues, migrationOutputVar);
        }

        public void ChangeCode()
        {
            SavePythonMigrationBackup();
            PythonNode.MigrateCode(NewCode);
        }

        private void SavePythonMigrationBackup()
        {
            var path = GetPythonMigrationBackupPath();
            if (File.Exists(path))
                return;

            DynamoViewModel.SaveAs(path, true);
        }

        private string GetPythonMigrationBackupPath()
        {
            var workspaceName = DynamoViewModel.CurrentSpace.Name;
            var backupDirectory = DynamoViewModel.Model.PathManager.BackupDirectory;
            return Path.Combine(backupDirectory, workspaceName) + string.Concat(".", Properties.Resources.PythonMigrationBackupExtension, ".dyn");
        }
    }
}
