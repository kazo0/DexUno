namespace Dexter.Presentation;

public partial record AboutModel
{
    public string Title => "About";

    public string AppVersion { get; } = ResolveVersion();

    private static string ResolveVersion()
    {
        try
        {
            var version = Windows.ApplicationModel.Package.Current.Id.Version;
            return $"{version.Major}.{version.Minor}.{version.Build}";
        }
        catch
        {
            return typeof(AboutModel).Assembly.GetName().Version?.ToString(3) ?? "1.0";
        }
    }
}
