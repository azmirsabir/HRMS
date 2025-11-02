using Core.Domain.Employee;

namespace Application.Interfaces;

public interface ISalaryCalculator
{
    decimal CalculateTotalSalary(decimal baseSalary, DegreeLevel degree, int serviceInYears);
}