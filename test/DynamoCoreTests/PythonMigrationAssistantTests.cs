using Dynamo.PythonMigration.MigrationAssistant;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamo.Tests
{
    public class PythonMigrationAssistantTests
    {
        [Test]
        public void CanRunPython3CodeFromMigrationAssistant()
        {
            string code = System.IO.File.ReadAllText(@"C:\Users\SylvesterKnudsen\Documents\PythonPlayground\codeEvaluationTest.py");
            var inputNames = new List<string>()
            {
                "name",
                "value"
            };
            var inputValues = new List<object>()
            {
                "From .NET!!",
                100
            };

            ScriptMigrator.MigrateCode(code, inputNames, inputValues, "output");
        }

        [Test]
        public void CanMigratePyton2CodeToPython3Code()
        {
            var expectedNewCode = System.IO.File.ReadAllText(@"C:\Users\SylvesterKnudsen\Desktop\new");
            var oldCode = System.IO.File.ReadAllText(@"C:\Users\SylvesterKnudsen\Desktop\old");
            //var oldCode = @"import urllib

            //print ""Hello world""

            //try:  
            //    x = urllib.request.urlopen(""http://dynamobim.org"").read()
            //    print x
            //except NameError, err:  
            //    print err, 'Error Caused'";

            string pythonScript = System.IO.File.ReadAllText(@"C:\Users\SylvesterKnudsen\Documents\PythonPlayground\migrate_2to3.py");
            var inputNames = new List<string>()
            {
                "code"
            };
            var inputValues = new List<object>()
            {
                oldCode
            };

            var migratedScript = ScriptMigrator.MigrateCode(pythonScript, inputNames, inputValues, "output");
            Assert.AreEqual(expectedNewCode, migratedScript);
        }
    }
}
