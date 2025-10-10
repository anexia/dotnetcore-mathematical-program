// ------------------------------------------------------------------------------------------
//  <copyright file = "JobController.cs" company = "ANEXIA® Internetdienstleistungs GmbH">
//  Copyright (c) ANEXIA® Internetdienstleistungs GmbH. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------------------


using Anexia.MathematicalProgram.MPaaS.Model;
using Microsoft.AspNetCore.Mvc;

namespace Anexia.MathematicalProgram.MPaaS;

[ApiController]
[Route("[controller]")]
public sealed class IlpsController(JobsRepository jobsRepository) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<PostJobsResponse>> PostAsync([FromBody] PostJobsRequest request)
    {
         var response = await jobsRepository.StoreJobAsync(request);
         await jobsRepository.StartJob(response.JobId);
         
         return new ActionResult<PostJobsResponse>(response);
    }
}