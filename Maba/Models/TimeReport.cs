using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maba.Models
{
	[Table("TimeReport")]
	public class TimeReport
	{


		public string IDNum { get; set; }
		[DataType(DataType.Date)]
		public DateTime? StartTime { get; set; }
		[DataType(DataType.Date)]
		public DateTime? EndTime { get; set; }
		public double TotalTime { get; set; }
		//public string Remarks { get; set; }
		public long ID { get; set; }
		public double? TotalTimeMonth { get; set; }
		public double? TotalTimeYear { get; set; }


	}
}
