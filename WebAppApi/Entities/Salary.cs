using System.ComponentModel.DataAnnotations;

namespace WebAppApi.Entities
{
    public class Salary
    {
        [Key]
        public int SalaryBandId { get; set; }
        public decimal MinSalary { get; set; }
        public decimal MaxSalary
        {
            get; set;
        }
    }
}