namespace GrpcClient
{
    public interface CommandOutput
    {
        void WriteMessage(string msg);
        void WriteError(string msg);
    }
}