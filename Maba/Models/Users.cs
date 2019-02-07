using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace Maba.Models
{
	[Table("Users")]
	public class Users
	{
	
	
		public int ID { get; set; }
		[Display(Name = "ID Number")]
		public string IDNumber { get; set; }
		[Display(Name = "Full Name")]
		[StringLength(30, MinimumLength = 2)]
		[Required]
		public string FullName { get; set; }
		public int Hours { get; set; }
		[StringLength(150, MinimumLength = 2)]
		public string Description { get; set; }
		[Required]
		public string Grade { get; set; }
		[Required]
		public string Password { get; set; }
		[Required]
		public string Email { get; set; }
		[Required]
		public string Phone { get; set; }
		[DataType(DataType.Date)]
		public DateTime LastLogin { get; set; }
		public string Role { get; set; }
		public string School { get; set; }

		public List<Users> UserList;
	}
		
	










	
		//public List<Users> oneStudentList;


		//public int Student_id { get; set; }


		//public int Approved { get; set; }
		//public string Remarks { get; set; }






		//public class AddClasses
		//{
		//	public UserDetail UserDetail { get; set; }
		//	public TimeReport TimeReport { get; set; }

		//}
}



