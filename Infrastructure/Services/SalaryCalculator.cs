using Application.Interfaces;
using Core.Domain.Employee;

namespace Infrastructure.Services;

public class SalaryCalculator : ISalaryCalculator
{
    public decimal CalculateTotalSalary(decimal baseSalary, DegreeLevel degree, int serviceInYears)
    {
        var allowancePercentage = degree switch
        {
            DegreeLevel.NoDegree => (decimal)AllowanceRate.NoDegree,
            DegreeLevel.HighSchool => (decimal)AllowanceRate.HighSchool,
            DegreeLevel.Bachelor => (decimal)AllowanceRate.Bachelor,
            DegreeLevel.Master => (decimal)AllowanceRate.Master,
            DegreeLevel.PhD => (decimal)AllowanceRate.PhD,
            _ => 0m
        };

        decimal allowanceAmount = baseSalary * allowancePercentage / 100m;
        decimal serviceBonus = (serviceInYears / 4) * 100m;

        return baseSalary + allowanceAmount + serviceBonus;
    }
    
    private enum AllowanceRate
    {
        NoDegree = 5,
        HighSchool = 10,
        Bachelor = 20,
        Master = 30,
        PhD = 40
    }
}