using System.Diagnostics;

namespace ShortUrl.AppHost.Commands
{
    internal static class DevSupportSeedCommand
    {
        public static IResourceBuilder<ProjectResource> WithSeedCommand(
            this IResourceBuilder<ProjectResource> builder)
        {
            var commandOptions = new CommandOptions
            {
                IconName = "DatabaseLightning",
                IconVariant = IconVariant.Filled,
                UpdateState = OnUpdateResourceState
            };
            builder.WithCommand(
                name: "seed-database",
                displayName: "Seed Database",
                executeCommand: context => OnRunSeedDatabaseCommandAsync(builder, context),
                commandOptions: commandOptions);
            return builder;
        }
        private static async Task<ExecuteCommandResult> OnRunSeedDatabaseCommandAsync(
            IResourceBuilder<ProjectResource> builder,
            ExecuteCommandContext context)
        {
            // TODO non funziona, aspare nuova versione!!!
            try
            {
                // Corrected: await and CancellationToken added back
                var projectMetadata = builder.Resource.GetProjectMetadata();
                if (projectMetadata is null)
                {
                    return CommandResults.Failure("Failed to get project metadata.");
                }

                var projectPath = projectMetadata.ProjectPath;
                if (string.IsNullOrEmpty(projectPath))
                {
                    return CommandResults.Failure("Project path is empty.");
                }

                var workingDirectory = Path.GetDirectoryName(projectPath);
                if (workingDirectory is null)
                {
                    return CommandResults.Failure($"Could not determine working directory from project path '{projectPath}'.");
                }

                // Corrected: Reverted to GetEffectiveEnvironmentAsync
                var environment = await builder.Resource.GetEnvironmentVariableValuesAsync(DistributedApplicationOperation.Publish);

                var startInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"run --project {projectPath} -- seed",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = workingDirectory
                };

                foreach (var env in environment)
                {
                    if (env.Key is not null) // Ensure key is not null
                    {
                        startInfo.Environment[env.Key] = env.Value;
                    }
                }

                using var process = Process.Start(startInfo);
                if (process is null)
                {
                    return CommandResults.Failure("Failed to start process.");
                }

                await process.WaitForExitAsync(context.CancellationToken);

                if (process.ExitCode != 0)
                {
                    var output = await process.StandardOutput.ReadToEndAsync(context.CancellationToken);
                    var error = await process.StandardError.ReadToEndAsync(context.CancellationToken);
                    return CommandResults.Failure($"Command failed with exit code {process.ExitCode}. Output: {output}. Error: {error}");
                }
                else
                {
                    var output = await process.StandardOutput.ReadToEndAsync(context.CancellationToken);
                    var error = await process.StandardError.ReadToEndAsync(context.CancellationToken);
                }
            }
            catch (Exception ex)
            {
                return CommandResults.Failure(ex.Message);
            }

            return CommandResults.Success();
        }

        private static ResourceCommandState OnUpdateResourceState(UpdateCommandStateContext context)
        {
            // Always enable the command in development environment
            return ResourceCommandState.Enabled;
        }
    }
}