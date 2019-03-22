using NetRefactorer.Interface;
using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace NetRefactorer
{
    // composer based on MEF (Managed Extensibility Framework)
    public class Refactorer
    {
        private IEnumerable<IRefactorer> refactoringModuleList;

        public Refactorer()
        {
            // https://blog.softwarepotential.com/porting-to-net-standard-2-0-part-2-porting-mef-1-0-to-mef-2-0-on-net-core/
            var directory = AppDomain.CurrentDomain.BaseDirectory;
            var assemblies = Directory.GetFiles(directory, "*.dll")
                .Select(AssemblyLoadContext.Default.LoadFromAssemblyPath);

            var configuration = new ContainerConfiguration().WithAssemblies(assemblies);
            var container = configuration.CreateContainer();

            refactoringModuleList = container.GetExports<IRefactorer>();
        }

        public static async Task RefactorDirectory(string root)
        {
            var refactorer = new Refactorer();
            var fileProcessor = new FileProcessor(refactorer.RefactorFile);
            await fileProcessor.Process(root);
        }

        public async Task RefactorFile(string file)
        {
            var fileText = File.ReadAllText(file);
            var refactored = Refactor(file, fileText);
            if (fileText == refactored)
            {
                refactored = null;
            }

            if (refactored!=null)
            {
                File.WriteAllText(file, refactored);
            }
        }

        public string Refactor(string filename, string code)
        {
            foreach (var refactoringModule in refactoringModuleList)
            {
                if (refactoringModule.ToRefactor(filename))
                {
                    code = refactoringModule.Refactor(code);
                }
            }
            return code;
        }
    }
}
