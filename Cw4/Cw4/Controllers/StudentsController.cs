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
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetStudents()
        {
            var list = new List<Student>();

            using (SqlConnection con = new SqlConnection(getConnectionString("db-mssql", "s17570", true)))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "SELECT student.firstname, student.lastname, student.birthdate, studies.name, enrollment.semester " +
                    "FROM student " +
                    "INNER JOIN enrollment ON student.idenrollment=enrollment.idenrollment " +
                    "INNER JOIN studies ON enrollment.idstudy=studies.idstudy";

                con.Open();
                SqlDataReader dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var st = new Student();
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    st.BirthDate = dr["BirthDate"].ToString();
                    st.StudiesName = dr["Name"].ToString();
                    st.Semester = dr["Semester"].ToString();
                    list.Add(st);
                }
            }

            return Ok(list);
        }
        

        public string getConnectionString(string dataSource, string initialCatalog, bool isIntegratedSecurity)
        {
            var connectionBuilder = new SqlConnectionStringBuilder();
            connectionBuilder.DataSource = dataSource;
            connectionBuilder.InitialCatalog = initialCatalog;
            connectionBuilder.IntegratedSecurity = isIntegratedSecurity;

            return connectionBuilder.ConnectionString;
        }
    }
}
