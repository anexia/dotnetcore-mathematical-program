using System.Net;
using System.Net.Http.Json;
using Anexia.MathematicalProgram.Model;
using Anexia.MathematicalProgram.Model.Scalar;
using Anexia.MathematicalProgram.Model.Variable;
using Anexia.MathematicalProgram.MPaaS.Model;

namespace Anexia.MathematicalProgram.MPaaS.Client;

public record MPaaSClient(Uri MpaasServer, MPaaSClientConfig Config, HttpClient HttpClient)
{
    public async Task<PostJobsResponse> StartJobAsync(PostJobsRequest request,
        CancellationToken cancellationToken = default)
    {
        var postJobResponse = await HttpClient.PostAsJsonAsync(new Uri(MpaasServer, "ilps"), request,
            cancellationToken: cancellationToken);
        
        return (await postJobResponse.Content.ReadFromJsonAsync<PostJobsResponse>(cancellationToken))!;
    }

    public Task<PostJobsResponse> StartJobAsync(
        ICompletedOptimizationModel<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar> model) =>
        StartJobAsync(model.ToMPaasModel());

    public async Task<GetSolutionResponse?> GetSolutionAsync(string jobId,
        CancellationToken cancellationToken = default)
    {
        var getSolutionResponse =
            await HttpClient.GetAsync(new Uri(MpaasServer, $"ilps/{jobId}/solution"), cancellationToken);
        if (getSolutionResponse.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        return await getSolutionResponse.Content.ReadFromJsonAsync<GetSolutionResponse>(cancellationToken);
    }

    public async Task<GetSolutionResponse> PollSolutionAsync(string jobId,
        CancellationToken cancellationToken = default)
    {
        var solution = await GetSolutionAsync(jobId, cancellationToken);
        while (solution is null && !cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(Config.PollingInterval, cancellationToken);
            solution = await GetSolutionAsync(jobId, cancellationToken);
        }

        return solution!;
    }

    public Task<GetSolutionResponse> SolveAsync(
        ICompletedOptimizationModel<IIntegerVariable<IRealScalar>, RealScalar, IRealScalar> model) =>
        SolveAsync(model.ToMPaasModel()); 
    
    public async Task<GetSolutionResponse> SolveAsync(PostJobsRequest request,
        CancellationToken cancellationToken = default)
    {
        var idResponse = await StartJobAsync(request, cancellationToken);
        var id = idResponse.JobId;

        await Task.Delay(Config.PollingInterval, cancellationToken);

        return PollSolutionAsync(id, cancellationToken).Result;
    }
}