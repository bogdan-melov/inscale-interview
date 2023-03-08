namespace InScale.Functions.Functions
{
    using FluentResults;
    using InScale.Commands.InScaleFile.Commands;
    using InScale.Domain.InScaleFile.Entities;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Http;

    public partial class InScaleFunction
    {
        [FunctionName("UploadInScaleFile")]
        public async Task<IActionResult> RunUploadInScaleFileAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "file")] HttpRequest req,
            ILogger log)
        {
            try
            {
                var formdata = await req.ReadFormAsync();

                IFormFile file = req.Form.Files.GetFile("file");
                string version = req.Form["version"];
                List<string> regions = JsonConvert.DeserializeObject<List<string>>(formdata["regions"]);
                List<string> channels = JsonConvert.DeserializeObject<List<string>>(formdata["channels"]);
                DateTime availableFrom = DateTime.Parse(req.Form["availableFrom"]);

                var command = new AddInScaleFileCommand(file: file,
                                                        version: version,
                                                        availableInRegions: regions,
                                                        availableFrom: availableFrom,
                                                        channels: channels);

                Result<InScaleFile> inScaleFileResult = await _mediator.Send(command);

                if (inScaleFileResult.IsFailed)
                {
                    return new InternalServerErrorResult();
                }

                return new OkObjectResult(inScaleFileResult.Value);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }
        }
    }
}
