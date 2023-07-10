using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Docker;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.Octopus;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using System;
using System.Net.Http;
using System.Web;

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main() => Execute<Build>(x => x.Default);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution(GenerateProjects = true)] public readonly Solution Solution;
    [GitRepository] public readonly GitRepository GitRepository;
    [GitVersion(Framework = "net7.0")][Optional] public readonly GitVersion GitVersion;
    AbsolutePath SourceDirectory => RootDirectory / "src";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore());
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
        });

    Target DockerPRBuild => _ => _
        .Before(Test)
        .Executes(() =>
        {
            DockerTasks.DockerBuild(x => x
                .SetPath(".")
                .SetFile(Solution.Website_Api.Directory / "Dockerfile")
                .SetTag("website-api")
                .SetProcessWorkingDirectory(SourceDirectory));
        });

    [Parameter("Github Token - PAT or Pipeline for writing to ghrc")]
    public readonly string GithubToken = string.Empty;

    [Parameter("Image Name")]
    public readonly string ImageName = "website-api";

    const string Repository = "ghcr.io";
    public string WebsiteApiImageTag => $"{Repository}/arcticgizmo/{ImageName}:{GitVersion.SemVer}";

    Target DockerProductionBuild => _ => _
        .After(Test)
        .Executes(() =>
        {
            DockerTasks.DockerBuild(x => x
                .SetPath(".")
                .SetFile(Solution.Website_Api.Directory / "Dockerfile")
                .AddTag(WebsiteApiImageTag)
                .AddLabel($"org.opencontainers.image.revision:{GitRepository.Commit}")
                .AddLabel($"org.opencontainers.image.source:https://github.com/{GitRepository.Identifier.ToLowerInvariant()}")
                .SetProcessWorkingDirectory(SourceDirectory));
        });


    Target DockerPush => _ => _
        .DependsOn(DockerProductionBuild)
        .Requires(() => !string.IsNullOrWhiteSpace(GithubToken))
        .Executes(() =>
        {
            DockerTasks.DockerLogin(x => x
                .SetServer(Repository)
                .SetUsername("USERNAME")
                .SetPassword(GithubToken));

            DockerTasks.DockerPush(x => x.SetName(WebsiteApiImageTag));
        });

    [Parameter("Render SRV - Service ID")]

    public readonly string RenderSrv = string.Empty;
    [Parameter("Github SRV Key - Key for Service ID")]
    public readonly string RenderSrvKey = string.Empty;

    Target DeployToRender => _ => _
        .Requires(() => !string.IsNullOrWhiteSpace(RenderSrv) && !string.IsNullOrWhiteSpace(RenderSrvKey))
        .Executes(async () =>
        {
            var client = new HttpClient();

            var builder = new UriBuilder($"https://api.render.com/deploy/{RenderSrv}");
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["key"] = RenderSrvKey;
            query["imgURL"] = WebsiteApiImageTag;
            builder.Query = query.ToString();
            var url = builder.ToString();

            await client.GetAsync(url);
        });

    Target Default => _ => _
        .DependsOn(Clean, Restore, Compile, Test);
}
