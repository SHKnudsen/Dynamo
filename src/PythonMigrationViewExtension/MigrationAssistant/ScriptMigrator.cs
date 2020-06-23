using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Python.Runtime;

namespace Dynamo.PythonMigration.MigrationAssistant
{
    internal static class ScriptMigrator
    {
        internal static string MigrateCode(string code, List<string> inputNames, List<object> inputValues, string returnName)
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
                            scope.Exec(code);
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
    }
}
