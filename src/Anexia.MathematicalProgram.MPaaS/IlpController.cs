// ------------------------------------------------------------------------------------------
//  <copyright file = "JobController.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------


using Anexia.MathematicalProgram.MPaaS.Model;
using Microsoft.AspNetCore.Mvc;

namespace Anexia.MathematicalProgram.MPaaS;

[ApiController]
[Route("ilps/{jobId}")]
public sealed class IlpController(JobsRepository jobsRepository) : ControllerBase
{
    [HttpGet]
    [Route("solution")]
    public async Task<ActionResult<GetSolutionResponse>> SolutionAsync(string jobId)
    {
        var solution = await jobsRepository.GetSolutionAsync(jobId);

        if (solution is not null)
        {
            return new ActionResult<GetSolutionResponse>(solution);
        }
        
        return NotFound(new { ErrorMessage = $"No solution found for job {jobId}."});
    }
    
    [HttpDelete]
    public async Task DeleteAsync(string jobId)
    {
        await jobsRepository.DeleteJobAsync(jobId);
    }
}