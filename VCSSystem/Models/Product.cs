using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VCSSystem.Models
{
	public class Product
	{
		int Id { get; set; }
		[Required]
		public string ProductyName { get; set; }
		[Required]
		public DateTime ExpireDate { get; set; }
		[Required]
		public string Stock {  get; set; }
		[Required]
		public string Ingredient { get; set; }


	}
}