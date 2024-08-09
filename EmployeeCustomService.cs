using System;
using Microsoft.EntityFrameworkCore;

namespace dotnet_api_code_challenge;

public class EmployeeCustomService
{
    private readonly EmployeeContext _context;

    public EmployeeCustomService(EmployeeContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Employee>> GetEmployeesHiredInLastYearAsync()
    {
        var p0 = DateTime.UtcNow.AddYears(-1);

        //var query = $"SELECT * FROM Employees WHERE DateOfHire >= {p0} ORDER BY DateOfHire DESC";
        return await _context.Employees.Where(x => x.DateOfHire >= p0).ToListAsync<Employee>(); //  .FromSql(query).ToListAsync();
    }
}
