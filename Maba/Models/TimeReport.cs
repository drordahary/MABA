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
		[DataType(DataType.DateTime)]
		public DateTime? StartTime { get; set; }
		[DataType(DataType.DateTime)]
		public DateTime? EndTime { get; set; }
		public double TotalTime { get; set; }
		//public string Remarks { get; set; }
		public long ID { get; set; }
		public string Remarks { get; set; }
		[DataType(DataType.DateTime)]
		public DateTime? NowDate { get; set; }
	}
}
