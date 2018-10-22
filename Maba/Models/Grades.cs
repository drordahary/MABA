using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Maba.Models
{
	public class Grades
	{
		public List<Users> students;
		public SelectList gradesList;
		public string studentGrade { get; set; }
	}
}
