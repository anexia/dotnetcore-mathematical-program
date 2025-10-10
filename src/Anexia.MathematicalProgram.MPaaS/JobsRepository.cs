// ------------------------------------------------------------------------------------------
//  <copyright file = "JobsService.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------


using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Channels;
using Anexia.MathematicalProgram.MPaaS.Model;
using Microsoft.Extensions.Options;

namespace Anexia.MathematicalProgram.MPaaS;

public sealed class JobsRepository(IOptionsMonitor<MPaaSOptions> mpaasOptions)
{
    private const string JobFolderName = "jobs";
    private const string SolutionFolderName = "solutions";
    private const string? JobAndSolutionFileExtension = ".json";

    internal Channel<string> JobsToRun { get; } = Channel.CreateUnbounded<string>();
    private readonly ConcurrentDictionary<string, bool> _finishedJobs = new();
    public ValueTask StartJob(string jobId) => JobsToRun.Writer.WriteAsync(jobId);

    internal string GetJobPath(string jobId) =>
        Path.ChangeExtension(Path.Join(mpaasOptions.CurrentValue.Workspace, JobFolderName, jobId),
            JobAndSolutionFileExtension);

    internal string GetSolutionPath(string jobId) =>
        Path.ChangeExtension(Path.Join(mpaasOptions.CurrentValue.Workspace, SolutionFolderName, jobId),
            JobAndSolutionFileExtension);

    private void EnsureFolderExists()
    {
        var jobsFolder = Path.Join(mpaasOptions.CurrentValue.Workspace, JobFolderName);
        var solutionsFolder = Path.Join(mpaasOptions.CurrentValue.Workspace, SolutionFolderName);
        Directory.CreateDirectory(jobsFolder);
        Directory.CreateDirectory(solutionsFolder);
    }
    
    public async Task<PostJobsResponse> StoreJobAsync(PostJobsRequest request)
    {
        EnsureFolderExists();
        var id = Guid.NewGuid().ToString();
        await using var file = File.Create(GetJobPath(id));
        await JsonSerializer.SerializeAsync(file, request);

        return new PostJobsResponse(id);
    }

    public async Task StoreSolutionAsync(string jobId, GetSolutionResponse response)
    {
        EnsureFolderExists();
        await using var file = File.Create(GetSolutionPath(jobId));
        await JsonSerializer.SerializeAsync(file, response);
        _finishedJobs[jobId] = true;
    }
    
    public async Task<GetSolutionResponse?> GetSolutionAsync(string jobId)
    {
        EnsureFolderExists();
        if (_finishedJobs.ContainsKey(jobId))
        {
            await using var file = File.OpenRead(GetSolutionPath(jobId));
            return await JsonSerializer.DeserializeAsync<GetSolutionResponse>(file);
        }

        return null;
    }

    public Task DeleteJobAsync(string jobId)
    {
        EnsureFolderExists();
        try
        {
            File.Delete(GetJobPath(jobId));
            File.Delete(GetSolutionPath(jobId));
        }
        catch (FileNotFoundException){}
        finally
        {
            _finishedJobs.Remove(jobId, out _);
        }

        return Task.CompletedTask;
    }
}