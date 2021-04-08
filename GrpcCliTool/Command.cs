namespace GrpcClient
{
    public interface Command
    {
        string Name { get; }
        string Description { get; }
        void Execute(JukeClient client, CommandOutput output, string[] arguments);
    }

}