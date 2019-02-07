using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Maba.Models;
using System.Net.Mail;
using System.Dynamic;
using System.Globalization;

namespace Maba.Controllers
{
	public class UsersController : Controller
	{
		private readonly MabaContext _context;

		public UsersController(MabaContext context)
		{
			_context = context;
		}

		// GET: Students
		// Requires using Microsoft.AspNetCore.Mvc.Rendering;
		public async Task<IActionResult> TeacherIndex(string studentGrade, string searchStudent)
		{
			string passedTeacherName = TempData["TeacherName"].ToString();
			TempData.Keep("TeacherName");
			ViewData["TeacherName"] = passedTeacherName;

			string passedTeacherGrade = TempData["TeacherGrade"].ToString();
			TempData.Keep("TeacherGrade");

			var studentsVar = from m in _context.User
						 select m;
		

			foreach (var role in _context.User)
			{
				if (role.Role == null || role.Role == "Student")
				{
					role.Role = "Student";
					studentsVar = studentsVar.Where(s => s.Grade == passedTeacherGrade && s.Role == "Student");
				}
			}

			if (!String.IsNullOrEmpty(searchStudent))
			{
				studentsVar = studentsVar.Where(g => g.FullName.Contains(searchStudent));
			}


			var studentGradeVM = new Users
			{
				UserList = await studentsVar.ToListAsync()
			};


			return View(studentGradeVM);
		}




		public async Task<IActionResult> StudentIndex(string start, string end)
		{
			string passedID = TempData["ID"].ToString();
			TempData.Keep("ID");

			string passingNextID = passedID;
			TempData["NextID"] = passingNextID;

			string passedName = TempData["Username"].ToString();
			TempData.Keep("Username");
			ViewData["DisplayName"] = passedName;


			

			var getUserSource = from m in _context.User
								where m.IDNumber == passedID
								select m;


			var getUserReports = from n in _context.TimeReport
								 where n.IDNum == passedID
								 select n;


			////////////////////////////////
			string formats = "dd/MM/yyyy HH:mm";

			string nowDate = DateTime.Now.ToString();
			DateTime now = DateTime.ParseExact(nowDate, formats, new CultureInfo("he-IL"), DateTimeStyles.None);

			ViewData["NowDate"] = nowDate;

			////////////////////////////


			TimeReport tr = new TimeReport
			{
				StartTime = DateTime.Now,
				EndTime = DateTime.Now
			};
			List<TimeReport> _Reports = new List<TimeReport>();

			
			_Reports.Add(tr);
			

			var VMConnection = new ViewModel
			{
				Details = await getUserSource.ToListAsync(),
				Reports = _Reports
			};
			VMConnection.Reports.AddRange(await getUserReports.ToListAsync());

			var getTimeSpan = from n in _context.TimeReport
							  where n.IDNum == passedID
							  select n;


			DateTime startSelected;
			DateTime endSelected;

			if (!String.IsNullOrEmpty(start) && !String.IsNullOrEmpty(end))
			{


				try
				{
					if (start.Length < 16)
					{
						if (start[11] != '0' && start[0] != 0)
						{
							start = start.Insert(11, "0");
						}
					}


					if (end.Length < 16)
					{
						if (end[11] != '0')
						{
							end = end.Insert(11, "0");
						}
					}

				}
				catch (Exception)
				{
					//TempData["ValueError"] = "<script>alert('חיפוש שעות לא חוקי');</script>";
					//TempData.Keep("ValueError");
				}





				startSelected = DateTime.ParseExact(start, formats, new CultureInfo("he-IL"), DateTimeStyles.None);
				endSelected = DateTime.ParseExact(end, formats, new CultureInfo("he-IL"), DateTimeStyles.None);

				getTimeSpan = from m in _context.TimeReport
							  where m.IDNum == passedID && m.StartTime <= startSelected && m.EndTime >= endSelected
							  select m;



			}

			VMConnection.Reports = await getTimeSpan.ToListAsync();

			ViewData["DisplayEmail"] = VMConnection.Details[0].Email;
			ViewData["DisplayPhone"] = VMConnection.Details[0].Phone;


			return View(VMConnection);
		}




		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> StudentUpdateDetails(string email, string phone)
		{
			string id = TempData["ID"].ToString();
			var student = await _context.User
				.FirstOrDefaultAsync(m => m.IDNumber == id);


			try
			{
				MailAddress ma = new MailAddress(email);
				student.Email = email;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.StackTrace);
			}


			if (!String.IsNullOrEmpty(phone))
			{
				if (phone.StartsWith("0"))
				{
					student.Phone = phone;
				}
			}

			_context.SaveChanges();
			

			return RedirectToAction(nameof(StudentIndex));
		}





		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ReportNewTime(string start, string end, int year, int month, string remarks)
		{
			string passedID = TempData["ID"].ToString();

			var report = new TimeReport
			{
				IDNum = passedID
			};


			string formats = "dd/MM/yyyy HH:mm";
			string[] formatForStartMonth = { "01/MM/yyyy" };
			string[] formatForEndMonth = { "dd/MM/yyyy" };


			var obj = await _context.TimeReport
				.FirstOrDefaultAsync(i => i.IDNum == passedID);

			month = DateTime.Now.Month;
			year = DateTime.Now.Year;

			int days = DateTime.DaysInMonth(year, month);


			


			try
			{
				if (start.Length < 16)
				{
					if (start[11] != '0' && start[0] != 0)
					{
						start = start.Insert(11, "0");
					}
				}


				if (end.Length < 16)
				{
					if (end[11] != '0')
					{
						end = end.Insert(11, "0");
					}
				}
				
			}
			catch (Exception)
			{
				TempData["ValueError"] = "<script>alert('הכנסת דיווח לא חוקית');</script>";
				TempData.Keep("ValueError");

				return RedirectToAction(nameof(StudentIndex));
			}


			DateTime getStart;
			DateTime getLast;


			try
			{
				getStart = DateTime.ParseExact(start, formats, new CultureInfo("he-IL"), DateTimeStyles.None);
				getLast = DateTime.ParseExact(end, formats, new CultureInfo("he-IL"), DateTimeStyles.None);
			}
			catch (Exception)
			{
				TempData["DateError"] = "<script>alert('מבנה תאריך לא חוקי');</script>";
				TempData.Keep("DateError");
				

				return RedirectToAction(nameof(StudentIndex));
			}


			TimeSpan duration = getLast - getStart;

			int total = duration.Hours;

			
			string startMonth = "01/" + month + "/" + year;
			string endMonth = days + "/" + month + "/" + year;

			DateTime getStartMonth = DateTime.ParseExact(startMonth, formatForStartMonth, new CultureInfo("he-IL"), DateTimeStyles.None);
			DateTime getEndMonth = DateTime.ParseExact(endMonth, formatForEndMonth, new CultureInfo("he-IL"), DateTimeStyles.None);


			if (duration.Hours < 10 && duration.Hours > 0)
			{
				report.StartTime = getStart;
				report.EndTime = getLast;


				var getTotalID = from t in _context.TimeReport
								 where t.IDNum == passedID
								 select t.IDNum;

				var getTotalTime = from m in _context.TimeReport
								   where m.IDNum == passedID
								   select m.TotalTime;


				if (getTotalID == null)
				{
					report.IDNum = passedID;
				}

				if (getTotalTime == null)
				{
					report.TotalTime = 0;
				}

				report.TotalTime = total;

				report.Remarks = remarks;

				await _context.AddAsync(report);
				_context.SaveChanges();


				double sumMonth = _context.TimeReport
					.Where(t => t.IDNum == passedID && t.StartTime >= getStartMonth && t.EndTime <= getEndMonth)
					.Select(t => t.TotalTime).Sum();


				double sumYear = _context.TimeReport
					.Where(m => m.IDNum == passedID)
					.Select(t => t.TotalTime).Sum();


				ViewData["Error"] = null;

				_context.SaveChanges();
			}

			else
			{
				TempData["Error"] = "<script>alert('לא ניתן לדווח יותר מ 10 שעות');</script>";
				TempData.Keep("Error");
				
			}
			

			return RedirectToAction(nameof(StudentIndex));
		}





		// GET: Students/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var student = await _context.User
				.FirstOrDefaultAsync(m => m.ID == id);

			if (student == null)
			{
				return NotFound();
			}

			return View(student);
		}

		// GET: Students/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: Students/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to 
		
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("ID,IDNumber,FullName,Hours,Description,Grade,Role")] Users student)
		{
			if (ModelState.IsValid)
			{
				_context.Add(student);

				await _context.SaveChangesAsync();


				return RedirectToAction(nameof(TeacherIndex));
			}


			return View(student);
		}





		// GET: Students/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var student = await _context.User.FindAsync(id);

			if (student == null)
			{
				return NotFound();
			}


			return View(student);
		}





		// POST: Students/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("ID,IDNumber,FullName,Hours,Description,Grade,Role")] Users student)
		{
			Users result = (from p in _context.User
							where p.IDNumber == student.IDNumber
							select p).SingleOrDefault();
			

			if (id != student.ID)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(student);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!StudentExists(student.ID))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}


				return RedirectToAction(nameof(TeacherIndex));
			}


			return View(student);
		}

		// GET: Students/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var student = await _context.User
				.FirstOrDefaultAsync(m => m.ID == id);

			if (student == null)
			{
				return NotFound();
			}

			return View(student);
		}

		// POST: Students/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var student = await _context.User.FindAsync(id);

			_context.User.Remove(student);

			await _context.SaveChangesAsync();


			return RedirectToAction(nameof(TeacherIndex));
		}

		private bool StudentExists(int id)
		{
			return _context.User.Any(e => e.ID == id);
		}





		// Login System. That method will be invoked at first by the Startup.cs

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(string idNumber, string password)
		{
		
			if (!String.IsNullOrEmpty(idNumber) && !String.IsNullOrEmpty(password))
			{

				var obj = await _context.User
					.FirstOrDefaultAsync(i => i.IDNumber == idNumber && i.Password == password);

				var users = from m in _context.User
							select m;

				var retUser = new Users();


				if (obj != null && obj.IDNumber == idNumber && obj.Password == password)
				{
					retUser = obj;
					

					if (retUser.Role == "Teacher" || retUser.Role == "Admin")
					{
						retUser.LastLogin = DateTime.Now;

						string teacher = retUser.FullName;
						string teacherGrade = retUser.Grade;

						TempData["TeacherGrade"] = teacherGrade;
						TempData["TeacherName"] = teacher;


						return RedirectToAction(nameof(TeacherIndex));
					}

					else if (retUser.Role == null || retUser.Role == "Student")
					{
						retUser.Role = "Student";

						if (obj.IDNumber == retUser.IDNumber && obj.Password == retUser.Password)
						{
							retUser.LastLogin = DateTime.Now;

							string id = retUser.IDNumber;
							string name = retUser.FullName;
							string email = retUser.Email;
							string phone = retUser.Phone;


							TempData["ID"] = id;
							TempData["Username"] = name;
							TempData["UserEmail"] = email;
							TempData["UserPhone"] = phone;


							return RedirectToAction(nameof(StudentIndex));
						}
					}
				}
			}
			return View();
		}

		public async Task<IActionResult> EditStudent(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var student = await _context.User.FindAsync(id);

			if (student == null)
			{
				return NotFound();
			}


			return View(student);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditStudent(int id, [Bind("ID,IDNumber,FullName,Hours,Description,Grade")] Users studentUser)
		{
			Users result = (from p in _context.User
							   where p.IDNumber == studentUser.IDNumber
							   select p).SingleOrDefault();

			//update email and phone number only
			result.Email = studentUser.Email;
			result.Phone = studentUser.Phone;
			
			
			_context.SaveChanges();
			
			
			if (id != studentUser.ID)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(result);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!StudentExists(studentUser.ID))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}


				return RedirectToAction(nameof(StudentIndex));
			}


			return View(result);
		}

		
		public async Task<IActionResult> DetailsStudent(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var student = await _context.User
				.FirstOrDefaultAsync(m => m.ID == id);

			if (student == null)
			{
				return NotFound();
			}


			return View(student);
		}
	}
}
