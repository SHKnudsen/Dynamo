using Dynamo.PythonMigration.MigrationAssistant;
using PythonNodeModels;
using PythonNodeModelsWpf;
using System.Collections.Generic;

namespace Dynamo.PythonMigration
{
    internal class PythonMigrationAssistantViewModel
    {
        public string OldCode { get; set; }
        public string NewCode { get; set; }
        public ScriptEditorWindow OwnerWindow { get; set; }
        private PythonNode PythonNode { get; set; }

        public PythonMigrationAssistantViewModel(PythonNode pythonNode, ScriptEditorWindow parentWindow)
        {
            PythonNode = pythonNode;
            OwnerWindow = parentWindow;
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
            PythonNode.MigrateCode(NewCode);
        }
    }
}
