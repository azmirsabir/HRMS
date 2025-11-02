using Application.Interfaces;
using Core.Domain.Employee;
using FluentAssertions;
using Infrastructure.Services;

namespace Test.Infrastructure.Services;

public class SalaryCalculatorTests
{
    private readonly ISalaryCalculator _salaryCalculator;

    public SalaryCalculatorTests()
    {
        _salaryCalculator = new SalaryCalculator();
    }

    [Fact]
    public void SalaryCalculator()
    {
        int baseSalary = 500;
        DegreeLevel degree = DegreeLevel.Master;
        int serviceInYears = 9;
        decimal result = _salaryCalculator.CalculateTotalSalary(baseSalary, degree, serviceInYears);
        result.Should().Be(850m);
    }
}