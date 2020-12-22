using Cw4.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Cw4.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    public class EnrollmentsController : ControllerBase
    {
        //zad. 4.4 -> SQLInjection
        private string sqlInjectionString = "'aa'; DROP TABLE student;";


        [HttpGet("{indexNumber}")]
        public IActionResult GetEnrollments(string indexNumber)
        {
            var list = new List<Enrollment>();

            using (SqlConnection con = new SqlConnection(Program.GetConnectionString()))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                //zad 4.4 -> SQLInjection, oczywiście nie zadziała dzięki zastosowaniu mechanizmu poniżej
                //update zad 4.5 -> chodziło o zastosowanie mechanizmu, który prewencyjnie zastosowałem już w 4.4
                com.Parameters.AddWithValue("index", sqlInjectionString);

                //com.Parameters.AddWithValue("index", indexNumber);
                com.CommandText = "SELECT * FROM enrollment " +
                    "INNER JOIN student ON student.idenrollment=enrollment.idenrollment " +
                    "WHERE student.indexnumber=@index";

                con.Open();
                SqlDataReader dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var enrollment = new Enrollment();
                    enrollment.IdEnrollment = dr["IdEnrollment"].ToString();
                    enrollment.Semester = dr["Semester"].ToString();
                    enrollment.IdStudy = dr["IdStudy"].ToString();
                    enrollment.StartDate = dr["StartDate"].ToString();
                    list.Add(enrollment);
                }
            }

            if (list.Count == 0)
                return NotFound("Student o podanym indeksie nie istnieje w bazie danych.");
            else
                return Ok(list);
        }
    }
}
