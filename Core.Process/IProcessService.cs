namespace Core.Process;

public interface IProcessService
{
    IList<string> StartProcess(string command, string args, Action<string> notify = null);
}
