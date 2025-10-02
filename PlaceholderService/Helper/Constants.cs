namespace PlaceholderService.Helper;

using SdkProject.Api.Sync;

public class Constants
{
    public static FileAddResponse FILE_ADD_RESPONSE = new FileAddResponse()
    {
        FileName = new[] { "dev", "no dev", "no test" },
        Size = Random.Shared.Next(),
    };
}
