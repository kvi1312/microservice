﻿using Carter;
using Hangfire.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOS.ScheduledJob;

namespace Hangfire.API.Endpoints
{
    public class HangfireEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/scheduled-jobs/send-email-reminder-checkout-order", SendReminderEmail);
            app.MapDelete("api/scheduled-jobs/delete/jobId/{jobId}", DeleteJobId);
        }

        private async Task<IResult> DeleteJobId(IBackgroundJobServices service, string jobId)
        {
           var result = service.ScheduledServices.Delete(jobId);
            return Results.Ok(result);
        }

        private async Task<IResult> SendReminderEmail(IBackgroundJobServices service, [FromBody] ReminderCheckoutOrderDto model)
        {
            var jobId = service.SendEmailContent(model.email, model.subject, model.emailContent, model.enqueueAt);
            return Results.Ok(jobId);
        }
    }
}
