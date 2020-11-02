using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace EmployeeService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class EmployeeService : IEmployeeService
    {
        public Employee GetEmployee(int Id)
        {
            Employee employee = new Employee();

            string cs = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

            try
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    //command object
                    SqlCommand cmd = new SqlCommand("spGetEmployee", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    //parameter object
                    SqlParameter parameterId = new SqlParameter();
                    parameterId.ParameterName = "@Id";
                    parameterId.Value = Id;

                    cmd.Parameters.Add(parameterId);

                    con.Open();
                    //executing the command frm cmd obj
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        //retrieving values from Database and associating with the employee object
                        employee.Id = Convert.ToInt32(reader["Id"]);
                        employee.Name = reader["Name"].ToString();
                        employee.Gender = reader["Gender"].ToString();
                        employee.DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"]);
                    }
                }
                return employee;
            }
            catch (Exception ex)
            {

                throw new FaultException(ex.Message);
            }
        }

        public void SaveEmployee(Employee employee)
        {
            string cs = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

            using (SqlConnection con = new SqlConnection(cs))
            {
                //cmd object
                SqlCommand cmd = new SqlCommand("spSaveEmployee", con);
                cmd.CommandType = CommandType.StoredProcedure;

                //parameter obj for Id
                SqlParameter parameterId = new SqlParameter()
                {
                    ParameterName = "@Id",
                    Value = employee.Id,
                };
                cmd.Parameters.Add(parameterId);

                //parameter obj for Name
                SqlParameter parameterName = new SqlParameter()
                {
                    ParameterName = "@Name",
                    Value = employee.Name
                };
                cmd.Parameters.Add(parameterName);

                //parameter obj for Gender
                SqlParameter parameterGender = new SqlParameter()
                {
                    ParameterName = "@Gender",
                    Value = employee.Gender
                };
                cmd.Parameters.Add(parameterGender);

                //parameter obj for Date of Birth
                SqlParameter parameterDateOfBirth = new SqlParameter()
                {
                    ParameterName = "@DateOfBirth",
                    Value = employee.DateOfBirth
                };
                cmd.Parameters.Add(parameterDateOfBirth);

                con.Open();

                cmd.ExecuteNonQuery();
            }
        }
    }
}
