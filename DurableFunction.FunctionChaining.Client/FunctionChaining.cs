using DurableFunction.FunctionChaining.Client.Constants;
using DurableFunction.FunctionChaining.Client.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;


namespace DurableFunction.FunctionChaining.Client
{
    /// <summary>
    /// FunctionChaining
    /// </summary>
    public static class FunctionChaining
    {
        /// <summary>
        /// Runs the orchestrator.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>EmployeeDetails</returns>
        [FunctionName(AppConstants.FunctionChainingOrchestraor)]
        public static async Task<EmployeeDetails> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();
            var input = context.GetInput<Employee>();

            var onBoardResult = await context.CallActivityAsync<EmployeeDetails>(AppConstants.FunctionChainingEmployeeOnBoard, input);
            var domainAllocatedResult = await context.CallActivityAsync<EmployeeDetails>(AppConstants.FunctionChainingEmployeeDomainAllocation, onBoardResult);
            var managerAllocatedResult = await context.CallActivityAsync<EmployeeDetails>(AppConstants.FunctionChainingEmployeeManagerAllocation, domainAllocatedResult);
            var projectAllocatedResult = await context.CallActivityAsync<EmployeeDetails>(AppConstants.FunctionChainingEmployeeProjectAllocation, managerAllocatedResult);

            return projectAllocatedResult;
        }

        /// <summary>
        /// Functions the chaining employee on board.
        /// </summary>
        /// <param name="emp">The emp.</param>
        /// <param name="log">The log.</param>
        /// <returns>EmployeeDetails</returns>
        [FunctionName(AppConstants.FunctionChainingEmployeeOnBoard)]
        public static EmployeeDetails FunctionChainingEmployeeOnBoard([ActivityTrigger] Employee emp, ILogger log)
        {
            log.LogInformation($"Processing {emp.Name} for Onboarding.");
            var empDetails = new EmployeeDetails
            {
                Id = emp.Id,
                Name = emp.Name,
                OnBoarded = true
            };
            return empDetails;
        }

        /// <summary>
        /// Functions the chaining employee domain allocation.
        /// </summary>
        /// <param name="emp">The emp.</param>
        /// <param name="log">The log.</param>
        /// <returns>EmployeeDetails</returns>
        [FunctionName(AppConstants.FunctionChainingEmployeeDomainAllocation)]
        public static EmployeeDetails FunctionChainingEmployeeDomainAllocation([ActivityTrigger] EmployeeDetails emp, ILogger log)
        {
            log.LogInformation($"Processing {emp.Name} for Domain allocation.");
            var empDetails = emp;
            empDetails.DomainAllocated = ((DomainList)RandomEnumValue<DomainList>()).ToString();
            return empDetails;
        }

        /// <summary>
        /// Functions the chaining employee manager allocation.
        /// </summary>
        /// <param name="emp">The emp.</param>
        /// <param name="log">The log.</param>
        /// <returns>EmployeeDetails</returns>
        [FunctionName(AppConstants.FunctionChainingEmployeeManagerAllocation)]
        public static EmployeeDetails FunctionChainingEmployeeManagerAllocation([ActivityTrigger] EmployeeDetails emp, ILogger log)
        {
            log.LogInformation($"Processing {emp.Name} for Manager allocation.");
            var empDetails = emp;
            empDetails.ManagerAllocated = RandomListValue("M");
            return empDetails;
        }

        /// <summary>
        /// Functions the chaining employee project allocation.
        /// </summary>
        /// <param name="emp">The emp.</param>
        /// <param name="log">The log.</param>
        /// <returns>EmployeeDetails</returns>
        [FunctionName(AppConstants.FunctionChainingEmployeeProjectAllocation)]
        public static EmployeeDetails FunctionChainingEmployeeProjectAllocation([ActivityTrigger] EmployeeDetails emp, ILogger log)
        {
            log.LogInformation($"Processing {emp.Name} for Project allocation.");
            var empDetails = emp;
            empDetails.ProjectAllocated = RandomListValue("P");
            return empDetails;
        }

        /// <summary>
        /// Randoms the enum value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>T</returns>
        private static T RandomEnumValue<T>()
        {
            var values = Enum.GetValues(typeof(T));
            Random rnd = new Random();
            int random = rnd.Next(0, values.Length);
            return (T)values.GetValue(random);
        }

        /// <summary>
        /// Randoms the list value.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>string</returns>
        private static string RandomListValue(string type)
        {
            var random = new Random();
            var list = new List<string>();
            int index = 0;
            if (type == "M")
            {
                list = new List<string> { "M1", "M2", "M3", "M4" };
                index = random.Next(list.Count);
            }
            else
            {
                list = new List<string> { "P1", "P2", "P3", "P4" };
                index = random.Next(list.Count);
            }

            return list[index];
        }

        /// <summary>
        /// HTTPs the start.
        /// </summary>
        /// <param name="req">The req.</param>
        /// <param name="starter">The starter.</param>
        /// <param name="log">The log.</param>
        /// <returns>HttpResponseMessage</returns>
        [FunctionName(AppConstants.FunctionChainingClient)]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            var reqInput = await req.Content.ReadAsAsync<Employee>();
            string instanceId = await starter.StartNewAsync(AppConstants.FunctionChainingOrchestraor, reqInput);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}