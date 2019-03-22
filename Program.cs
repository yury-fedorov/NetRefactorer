namespace NetRefactorer
{
    class Program
    {
        private const string Root = ".";

        static void Main(string[] args)
        {
            Refactorer.RefactorDirectory(Root).Wait();
        }
    }
}
