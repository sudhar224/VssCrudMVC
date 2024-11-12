using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VCSSystem.Models
{
	public class EmployeeViewModel
	{
		public Employee NewEmployee { get; set; } = new Employee();
		public List<Employee> EmployeeList { get; set; }
	}
}