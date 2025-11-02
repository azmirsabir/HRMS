using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Domain.BaseEntity;
using Core.Domain.Users;

namespace Core.Domain.Leave;

public class Leave : BaseEntity<int>
{
        public LeaveType Type { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int EmployeeId { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        public Employee.Employee Employee { get; set; } = null!;

        public LeaveStatus Status { get; set; }

        [MaxLength(100)]
        public string? Reason { get; set; }
        
        [MaxLength(100)]
        public string? RejectionReason { get; set; }

        /// <summary>
        /// Factory method to create a new Leave entity.
        /// </summary>
        /// <param name="type">Type of leave</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <param name="employeeId">Employee ID</param>
        /// <param name="status">Leave status</param>
        /// <param name="reason">Rejection reason (required if Rejected)</param>
        /// <returns>A new Leave instance</returns>
        /// <exception cref="ArgumentException">Thrown when parameters are invalid</exception>
        public static Leave Create(
            LeaveType type,
            DateTime startDate,
            DateTime endDate,
            int employeeId,
            LeaveStatus status = LeaveStatus.Pending,
            string? reason = null)
        {
            if (employeeId <= 0)
                throw new ArgumentException("Employee ID must be greater than zero.", nameof(employeeId));

            if (startDate > endDate)
                throw new ArgumentException("Start date cannot be later than end date.", nameof(startDate));

            if (status == LeaveStatus.Rejected && string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Reason is required when leave is rejected.", nameof(reason));

            return new Leave
            {
                Type = type,
                StartDate = startDate,
                EndDate = endDate,
                EmployeeId = employeeId,
                Status = status,
                Reason = reason,
                // CreatedAt = DateTime.UtcNow,
                // UpdatedAt = DateTime.UtcNow,
                // Deleted = false
            };
        }
    }

    public enum LeaveType
    {
        Annual = 0,
        Sick = 1,
        Unpaid = 2,
        Personal = 3
    }

    public enum LeaveStatus
    {
        Pending = 0,
        Accepted = 1,
        Rejected = 2
    }