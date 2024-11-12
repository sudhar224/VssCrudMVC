using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VCSSystem.Models;
using System.Data;
using System.Xml.Linq;

namespace VCSSystem.Controllers
{
    public class EmployeeController : Controller
    {
		string constr = ConfigurationManager.ConnectionStrings["Dbconnection"].ConnectionString;
		// GET: Employee
		public ActionResult Index()
		{
			EmployeeViewModel viewModel = new EmployeeViewModel();
			viewModel.EmployeeList = GetEmployeeList();
			return View(viewModel);
		}

		[HttpPost]
		public ActionResult Create(EmployeeViewModel viewModel)
		{
			var objEmployee = viewModel.NewEmployee;
			try
			{
				if (objEmployee.ImageFile != null && objEmployee.ImageFile.ContentLength > 0)
				{
					string fileName = Path.GetFileNameWithoutExtension(objEmployee.ImageFile.FileName);
					string extension = Path.GetExtension(objEmployee.ImageFile.FileName);
					fileName = fileName + extension;
					objEmployee.Photo = "~/Image/" + fileName;
					string filePath = Path.Combine(Server.MapPath("~/Image/"), fileName);
					objEmployee.ImageFile.SaveAs(filePath);
				}
				else
				{
					objEmployee.Photo = "~/Image/default.jpg";
				}

				using (SqlConnection con = new SqlConnection(constr))
				{
					con.Open();
					SqlCommand cmd = new SqlCommand("sp_add_emp", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@Name", objEmployee.Name);
					cmd.Parameters.AddWithValue("@Email", objEmployee.Email);
					cmd.Parameters.AddWithValue("@DOB", objEmployee.DOB);
					cmd.Parameters.AddWithValue("@Photo", objEmployee.Photo);

					int result = cmd.ExecuteNonQuery();
					con.Close();
					if(result > 0)
					{
						TempData["Success"] = "Employee Added Successfully";
						return RedirectToAction("Index");
					}
					else
					{
						TempData["Error"] = "Employee Error Adding";
					}
				}
				return RedirectToAction("Index");

			}
			catch
			{
				viewModel.EmployeeList = GetEmployeeList();
				return View("Index", viewModel);
			}
		}

		private List<Employee> GetEmployeeList()
		{
			List<Employee> EmployeeObj = new List<Employee>();
			using (SqlConnection con = new SqlConnection(constr))
			{
				con.Open();
				SqlCommand cmd = new SqlCommand("sp_list_employee", con);
				SqlDataReader sdr = cmd.ExecuteReader();
				while (sdr.Read())
				{
					EmployeeObj.Add(new Employee
					{
						Id = Convert.ToInt32(sdr["Id"]),
						Name = sdr["Name"].ToString(),
						Email = sdr["Email"].ToString(),
						DOB = Convert.ToDateTime(sdr["DOB"]),
						Photo = sdr["Photo"].ToString()
					});
				}
				sdr.Close();
				con.Close();
			}
			return EmployeeObj;
		}
	

	// GET: Employee/Edit/5
	public ActionResult Edit(int id)
        {
            Employee objEmployee = new Employee();
            using(SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_getEmployee_byId", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                SqlDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    objEmployee = new Employee
                    {
						Id = Convert.ToInt32(sdr["id"]),
						Name = sdr["Name"].ToString(),
						Email = sdr["Email"].ToString(),
						DOB = Convert.ToDateTime(sdr["DOB"]),
						Photo = sdr["Photo"].ToString()
					};
                }
                sdr.Close();
                con.Close();
            }
            return View(objEmployee);
        }

		// POST: Employee/Edit/5
		[HttpPost]
		public ActionResult Edit(int id, Employee objEmployee)
		{
			try
			{
				string existingPhotoPath = null;

				
				using (SqlConnection con = new SqlConnection(constr))
				{
					con.Open();
					SqlCommand cmd = new SqlCommand("sp_getEmployee_byId", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@Id", id);

					SqlDataReader sdr = cmd.ExecuteReader();
					if (sdr.Read())
					{
						existingPhotoPath = sdr["Photo"].ToString();
						objEmployee.Photo = existingPhotoPath; 
					}
					sdr.Close();
					con.Close();
				}

				
				if (objEmployee.ImageFile != null && objEmployee.ImageFile.ContentLength > 0)
				{
					
					if (!string.IsNullOrEmpty(existingPhotoPath))
					{
						string oldFilePath = Server.MapPath(existingPhotoPath);
						if (System.IO.File.Exists(oldFilePath))
						{
							System.IO.File.Delete(oldFilePath);
						}
					}

					
					string fileName = Path.GetFileNameWithoutExtension(objEmployee.ImageFile.FileName);
					string extension = Path.GetExtension(objEmployee.ImageFile.FileName);
					fileName = fileName + extension;
					objEmployee.Photo = "~/Image/" + fileName;
					string filePath = Path.Combine(Server.MapPath("~/Image/"), fileName);
					objEmployee.ImageFile.SaveAs(filePath);
				}

				
				using (SqlConnection con = new SqlConnection(constr))
				{
					con.Open();
					SqlCommand cmd = new SqlCommand("sp_edit_employee", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@Id", id);
					cmd.Parameters.AddWithValue("@Name", objEmployee.Name);
					cmd.Parameters.AddWithValue("@Email", objEmployee.Email);
					cmd.Parameters.AddWithValue("@DOB", objEmployee.DOB);
					cmd.Parameters.AddWithValue("@Photo", objEmployee.Photo); 

					int result = cmd.ExecuteNonQuery();
					if (result > 0)
					{
						TempData["Success"] = "Employee Details Updated";
						return RedirectToAction("Index");
					}
					else
					{
						TempData["Error"] = "Update failed.";
					}

					con.Close();
				}

				return View(objEmployee);
			}
			catch(Exception ex)
			{
				TempData["Error"] = ex.Message;
				return View(objEmployee);
			}
		}

		// GET: Employee/Delete/5
		public ActionResult Delete(int id)
        {
			Employee objEmployee = new Employee();
			using (SqlConnection con = new SqlConnection(constr))
			{
				con.Open();
				SqlCommand cmd = new SqlCommand("sp_getEmployee_byId", con);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@Id", id);
				SqlDataReader sdr = cmd.ExecuteReader();
				while (sdr.Read())
				{
					objEmployee = new Employee
					{
						Id = Convert.ToInt32(sdr["Id"]),
						Name = sdr["Name"].ToString(),
						Email = sdr["Email"].ToString(),
						DOB = Convert.ToDateTime(sdr["DOB"]),
						Photo = sdr["Photo"].ToString()
					};
				}
				sdr.Close();
				con.Close();
			}
			return View(objEmployee);
		}

        // POST: Employee/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, Employee EmployeeObj)
        {
			try
			{
				string existingPhotoPath = null;				
				using (SqlConnection con = new SqlConnection(constr))
				{
					con.Open();
					SqlCommand cmdGetPhoto = new SqlCommand("sp_getEmployee_byId", con);
					cmdGetPhoto.CommandType = CommandType.StoredProcedure;
					cmdGetPhoto.Parameters.AddWithValue("@Id", id);

					SqlDataReader sdr = cmdGetPhoto.ExecuteReader();
					if (sdr.Read())
					{
						existingPhotoPath = sdr["Photo"] as string; 
					}
					sdr.Close();
					con.Close();
				}
				
				if (!string.IsNullOrEmpty(existingPhotoPath))
				{
					string filePath = Server.MapPath(existingPhotoPath);
					if (System.IO.File.Exists(filePath))
					{
						System.IO.File.Delete(filePath); 
					}
				}
			
				using (SqlConnection con = new SqlConnection(constr))
				{
					con.Open();
					SqlCommand cmd = new SqlCommand("sp_delete_employee", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@Id", id);
					int result = cmd.ExecuteNonQuery();
					con.Close();
					if(result > 0)
					{
						TempData["Success"] = "Employee Removed";
						return RedirectToAction("Index");
					}
					else
					{
						TempData["Error"] = "Error Removing Employee";
						return View();
					}
				}				
			}
			catch (Exception ex)
			{				
				TempData["Error"] = "Error deleting employee: " + ex.Message;
				return View();
			}
		}
    }
}
