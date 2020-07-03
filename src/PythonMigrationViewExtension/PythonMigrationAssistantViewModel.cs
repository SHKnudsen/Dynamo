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
        internal ScriptEditorWindow OwnerWindow { get; private set; }
        private PythonNode PythonNode { get; set; }

        public PythonMigrationAssistantViewModel(PythonNode pythonNode, ScriptEditorWindow parentWindow = null)
        {
            PythonNode = pythonNode;
            OldCode = pythonNode.Script;
            MigrateCode();
            if (parentWindow is null)
                return;

            SetOwnerWindow(parentWindow);

        }

        private void MigrateCode()
        {
            NewCode = ScriptMigrator.MigrateCode(OldCode);
        }

        public void ChangeCode()
        {
            PythonNode.MigrateCode(NewCode);
        }

        private void SetOwnerWindow(ScriptEditorWindow parentWindow)
        {
            OwnerWindow = parentWindow;
        }
    }
}
