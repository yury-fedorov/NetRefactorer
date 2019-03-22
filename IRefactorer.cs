namespace NetRefactorer.Interface
{
    public interface IRefactorer
    {
        string Refactor(string code);
        bool ToRefactor(string filename);
    }
}
