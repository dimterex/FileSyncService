using System.Diagnostics;

using Core.Process;

using NLog;

public class ProcessService : IProcessService
{
    private readonly ILogger _logger;
    
    public ProcessService()
    {
        _logger = LogManager.GetLogger(nameof(ProcessService));
    }
    #region Methods

    public IList<string> StartProcess(string command, string args, Action<string> notify = null)
    {
        _logger.Debug(() => $"{command} {args}");
        var process = new Process
        {
            StartInfo =
            {
                FileName = command,
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }
        };
        process.Start();
        var result = new List<string>();
        while (!process.HasExited)
        {
            string s = process.StandardOutput.ReadToEnd();
            notify?.Invoke(s);
            _logger.Debug(() => s);
            result.Add(s);
        }

        return result;
    }

    #endregion
}
