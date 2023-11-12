using Microsoft.Build.Framework;

namespace TailwindDotnet.Hosting.Build.Tasks;

public class SetupExecutableTask : Microsoft.Build.Utilities.Task
{
    public override bool Execute()
    {
        Log.LogMessage(MessageImportance.High, "Hello World from Setup!!");

        return true;
    }
}
