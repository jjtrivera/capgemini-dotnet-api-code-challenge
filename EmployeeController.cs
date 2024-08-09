using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace dotnet_api_code_challenge
{
    public class EmployeeController
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly EmployeeRepository _repository;
        private readonly EmployeeCustomService _sqlService;

        public EmployeeController(ILogger<EmployeeController> logger, EmployeeRepository repository, EmployeeCustomService sqlService)
        {
            _logger = logger;
            _repository = repository;
            _sqlService = sqlService;
        }

        [Function("GetAllEmployees")]
        public async Task<HttpResponseData> GetAllEmployees(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "employees")] HttpRequestData req)
        {
            var employees = await _repository.GetAllEmployeesAsync();

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(employees);
            return response;
        }

        [Function("GetEmployeeById")]
        public async Task<HttpResponseData> GetEmployeeById(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "employees/{id}")] HttpRequestData req, int id)
        {
            var employee = await _repository.GetEmployeeByIdAsync(id);

            var response = employee != null
                ? req.CreateResponse(HttpStatusCode.OK)
                : req.CreateResponse(HttpStatusCode.NotFound);

            if (employee != null)
            {
                await response.WriteAsJsonAsync(employee);
            }

            return response;
        }

        [Function("AddEmployee")]
        public async Task<HttpResponseData> AddEmployee(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "employees")] HttpRequestData req)
        {
            var employee = await JsonSerializer.DeserializeAsync<Employee>(req.Body);

            if (employee == null)
            {
                var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequestResponse.WriteStringAsync("Invalid employee data.");
                return badRequestResponse;
            }

            var createdEmployee = await _repository.AddEmployeeAsync(employee);

            var response = req.CreateResponse(HttpStatusCode.Created);
            response.Headers.Add("Location", $"/api/employees/{createdEmployee.Id}");
            await response.WriteAsJsonAsync(createdEmployee);

            return response;
        }

        [Function("UpdateEmployee")]
        public async Task<HttpResponseData> UpdateEmployee(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "employees/{id}")] HttpRequestData req, int id)
        {
            var employee = await JsonSerializer.DeserializeAsync<Employee>(req.Body);

            if (employee == null || employee.Id != id)
            {
                var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequestResponse.WriteStringAsync("Invalid employee data.");
                return badRequestResponse;
            }

            var updatedEmployee = await _repository.UpdateEmployeeAsync(employee);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(updatedEmployee);

            return response;
        }

        [Function("DeleteEmployee")]
        public async Task<HttpResponseData> DeleteEmployee(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "employees/{id}")] HttpRequestData req, int id)
        {
            await _repository.DeleteEmployeeAsync(id);

            var response = req.CreateResponse(HttpStatusCode.NoContent);
            return response;
        }

        [Function("GetEmployeesHiredInLastYear")]
        public async Task<HttpResponseData> GetEmployeesHiredInLastYear(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "employees-hired-last-year")] HttpRequestData req)
        {
            var employees = await _sqlService.GetEmployeesHiredInLastYearAsync();

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(employees);
            return response;
        }
    }
}
