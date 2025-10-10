// ------------------------------------------------------------------------------------------
//  <copyright file = "JobQueue.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------

using System.Text.Json;
using Anexia.MathematicalProgram.MPaaS.Client;
using Microsoft.Extensions.Options;
using Anexia.MathematicalProgram.MPaaS.Model;
using Anexia.MathematicalProgram.Solve;
using Anexia.MathematicalProgram.SolverConfiguration;

namespace Anexia.MathematicalProgram.MPaaS;

public sealed class JobRunner(
    IOptionsMonitor<MPaaSOptions> mpaasOptions,
    JobsRepository jobsRepository,
    ILogger<JobRunner> logger,
    ILogger<IlpSolver> loggerForSolver) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await jobsRepository.JobsToRun.Reader.WaitToReadAsync(stoppingToken))
        {
            var jobId = await jobsRepository.JobsToRun.Reader.ReadAsync(stoppingToken);

            var fileName = jobsRepository.GetJobPath(jobId);
            await using var file = File.OpenRead(fileName);

            if (File.Exists(fileName))
            {
                var job = await JsonSerializer.DeserializeAsync<PostJobsRequest>(file,
                    cancellationToken: stoppingToken);

                var optimizationModel = job!.ToOptimizationModel();

                logger.LogInformation("Starting job {JobId} with solver {SolverType}", jobId,
                    mpaasOptions.CurrentValue.SolverType);

                var result =
                    new IlpSolver(mpaasOptions.CurrentValue.SolverType, mpaasOptions.CurrentValue.SolverType,
                        loggerForSolver).Solve(
                        optimizationModel,
                        new SolverParameter());

                await jobsRepository.StoreSolutionAsync(jobId, result.ToMPaaSSolution(optimizationModel.Variables));
            }
        }
    }
}