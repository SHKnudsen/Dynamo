using PythonNodeModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamo.PythonMigration
{
    internal class PythonMigrationAssistantViewModel
    {
        private string OldCode { get; set; }
        private string NewCode { get; set; }

        public PythonMigrationAssistantViewModel(PythonNode pythonNode)
        {
            OldCode = pythonNode.Script;

        }
    }
}
