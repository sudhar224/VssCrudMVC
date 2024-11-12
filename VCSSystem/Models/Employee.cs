using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VCSSystem.Models
{
	public class Employee
	{		
		public int Id { get; set; }
		[Required]
		public string Name { get; set; }
		[Required]
		public string Email { get; set; }
		[Required]
		public DateTime DOB { get; set; }
		[Required]
		public string Photo { get; set; }
		public HttpPostedFileBase ImageFile { get; set; }
	}
}