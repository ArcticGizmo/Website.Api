using System.Collections.Generic;
using System.Collections.Immutable;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Docker;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.Npm;
using Nuke.Common.Tools.Octopus;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

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
        .Executes(() => {
        });

    Target DockerPRBuild => _ => _
        .Before(Test)
        .Executes(() => {
            DockerTasks.DockerBuild(x => x
                .SetPath(".")
                .SetFile(Solution.Website_Api.Directory / "Dockerfile")
                .SetTag("website-api")
                .SetProcessWorkingDirectory(SourceDirectory));
        });

    [Parameter("Github Token - PAT or Pipeline for writing to ghrc")]
    public readonly string GithubToken = string.Empty;

    const string Repository = "ghcr.io";
    public string WebsiteApiImageTag => $"{Repository}/arcticgizmo/website-api:{GitVersion.SemVer}";

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

    Target Default => _ => _
        .DependsOn(Clean, Restore, Compile, Test);
}
