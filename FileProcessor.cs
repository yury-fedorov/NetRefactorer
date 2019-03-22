using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace NetRefactorer
{
    public class FileProcessor
    {
        // asynchronous processing
        private readonly Func<string,Task> _processFile;

        public FileProcessor(Func<string,Task> processFile)
        {
            _processFile = processFile;
        }

        public async Task ProcessFile(string filename)
        {
            await _processFile(filename);
        }

        public async Task Process(string directory)
        {
            var info = new DirectoryInfo(directory);
            var tasks = new List<Task>();
            foreach (var entry in info.EnumerateFileSystemInfos())
            {
                var processor = entry.Attributes.HasFlag(FileAttributes.Directory) ? (Func<string, Task>) Process : ProcessFile;
                tasks.Add(processor(entry.FullName));
            }
            await Task.WhenAll(tasks);
        }
    }
}
