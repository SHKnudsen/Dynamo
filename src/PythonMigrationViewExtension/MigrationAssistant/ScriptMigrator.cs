using Python.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Dynamo.PythonMigration.MigrationAssistant
{
    internal static class ScriptMigrator
    {
        internal static string MigrateCode(List<string> inputNames, List<object> inputValues, string returnName)
        {
            Python.Included.Installer.SetupPython().Wait();

            if (!PythonEngine.IsInitialized)
            {
                PythonEngine.Initialize();
                PythonEngine.BeginAllowThreads();
            }

            IntPtr gs = PythonEngine.AcquireLock();

            try
            {
                using (Py.GIL())
                {

                    using (PyScope scope = Py.CreateScope())
                    {
                        int amt = Math.Min(inputNames.Count, inputValues.Count);

                        for (int i = 0; i < amt; i++)
                        {
                            scope.Set(inputNames[i], inputValues[i].ToPython());
                        }

                        try
                        {
                            scope.Exec(GetPythonMigrationScript());
                            var result = scope.Contains(returnName) ? scope.Get(returnName) : null;

                            return result.ToString();
                        }
                        catch (Exception e)
                        {
                            throw;
                        }
                    }
                }
            }

            catch (PythonException pe)
            {
                throw;
            }
            finally
            {
                PythonEngine.ReleaseLock(gs);
            }
        }

        private static string GetPythonMigrationScript()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            var reader = new StreamReader(asm.GetManifestResourceStream("Dynamo.PythonMigration.MigrationAssistant.migrate_2to3.py"));
            return reader.ReadToEnd();
        }
    }
}
