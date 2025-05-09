using System.Diagnostics;
using BuildingBlocks.Abstractions.Correlation;
using BuildingBlocks.Core.Utils;
using BuildingBlocks.Web.Models;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BuildingBlocks.Web.Middlewares
{
    public class EndpointLoggingMiddleware(
        RequestDelegate next,
        ILogger<EndpointLoggingMiddleware> logger,
        ICorrelationService correlationService)
       // ITopicProducer<string, EndpointLogModel> endpointLogProducer)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var timer = Stopwatch.StartNew();
            if (IsSwaggerEndpoint(context))
            {
                await next(context);
                return;
            }

            if (IsBodylessRequest(context))
            {
                if (context.Request.QueryString.HasValue)
                {
                    LogStartWithQueryString(context);
                }
                else
                {
                    LogStartPlainRequest(context);
                }
            }
            else
            {
                await LogStartWithRequestBodyAsync(context);
            }

            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await next.Invoke(context);

            timer.Stop();

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            if (responseText.Length > 0)
            {
                LogEndWithResponseBody(context, timer.ElapsedMilliseconds, responseText);
            }
            else
            {
                LogEndPlainResponse(context, timer.ElapsedMilliseconds);
            }

            #region Async Kafka logging

            // var endpointLogModel = new EndpointLogModel(
            //     Provider: AppDomain.CurrentDomain.FriendlyName,
            //     Uri: context.Request.Path,
            //     HttpMethod: context.Request.Method,
            //     RequestPayload: responseText,
            //     RequestHeaders: JsonConvert.SerializeObject(
            //         context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString())),
            //     Status: context.Response.StatusCode,
            //     ResponsePayload: responseText,
            //     ResponseHeaders: JsonConvert.SerializeObject(
            //         context.Response.Headers.ToDictionary(h => h.Key, h => h.Value.ToString())),
            //     ResponseTime: timer.ElapsedMilliseconds,
            //     CreatedBy: context.User?.Identity?.Name ?? "Anonymous",
            //     CreatedDate: DateTime.UtcNow,
            //     LastModifiedBy: context.User?.Identity?.Name ?? "Anonymous",
            //     LastModifiedDate: DateTime.UtcNow,
            //     UserId: context.User?.FindFirst("sub")?.Value,
            //     ErrMsg: context.Items["ErrorMessage"]?.ToString(),
            //     ErrCode: context.Items["ErrorCode"]?.ToString(),
            //     Success: (context.Response.StatusCode is >= 100 and < 400).ToString(),
            //     CorrelationId: correlationService.CorrelationId,
            //     TransactionId: Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier,
            //     ClientCreateTime: DateTime.UtcNow,
            //     DeviceId: context.Request.Headers["X-Device-ID"].FirstOrDefault(),
            //     ClientIp: context.Connection.RemoteIpAddress?.ToString(),
            //     UserAgent: context.Request.Headers.UserAgent.FirstOrDefault(),
            //     AppRegion: "",
            //     AppZone: ""
            // );
            //
            // _ = Task.Run(async () =>
            // {
            //     const int maxRetryCount = 3, retryDelay = 1000;
            //     int attempt = 0;
            //
            //     while (attempt < maxRetryCount)
            //     {
            //         try
            //         {
            //             attempt++;
            //             await endpointLogProducer.Produce(
            //                 key: correlationService.CorrelationId,
            //                 value: endpointLogModel);
            //             break;
            //         }
            //         catch (Exception ex)
            //         {
            //             if (attempt == maxRetryCount)
            //             {
            //                 logger.LogCritical(
            //                     exception: ex,
            //                     message:
            //                     "Error occured attempting to send a kafka endpoint with correlationId: {CorrelationId} and errorMessage: {ErrorMessage}",
            //                     correlationService.CorrelationId,
            //                     ex.Message);
            //                 break;
            //             }
            //
            //             await Task.Delay(retryDelay);
            //         }
            //     }
            // });
            //
             #endregion Async Kafka logging
            //
             await responseBody.CopyToAsync(originalBodyStream);
        }


        #region Conditions

        private bool IsSwaggerEndpoint(HttpContext context)
        {
            var path = context.Request.Path;
            return path.Value != null && path.Value.Contains("swagger");
        }

        private bool IsBodylessRequest(HttpContext context)
        {
            return context.Request.Method == "GET";
        }

        #endregion


        #region Logs

        private async Task LogStartWithRequestBodyAsync(HttpContext context)
        {
            var reader = new StreamReader(context.Request.Body);
            var requestBody = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;

            logger.LogInformation("Request Recevied: {Path}, Request: {Request}", context.Request.Path,
                requestBody);
        }

        private void LogStartWithQueryString(HttpContext context)
        {
            logger.LogInformation("Request Recevied: {Path}, QueryString: {Request}", context.Request.Path,
                context.Request.QueryString);
        }

        private void LogStartPlainRequest(HttpContext context)
        {
            logger.LogInformation("Request Recevied: {Path}", context.Request.Path);
        }

        private void LogEndWithResponseBody(HttpContext context, long elapsed, string responseText)
        {
            logger.LogInformation(
                "Request Finished: {Path} Status Code: {StatusCode}, Time Spent: {TimeSpent} ms, Response: {Response}",
                context.Request.Path,
                context.Response.StatusCode,
                elapsed,
                JsonUtils.Beautify(responseText)
            );
        }

        private void LogEndPlainResponse(HttpContext context, long elapsed)
        {
            logger.LogInformation(
                "Request Finished: {Path} Status Code: {StatusCode}, Time Spent: {TimeSpent} ms",
                context.Request.Path,
                context.Response.StatusCode,
                elapsed
            );
        }

        #endregion
    }
}
