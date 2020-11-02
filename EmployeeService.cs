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
            //set employee obj to null due to herited class
            Employee employee = null;

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
                        if ((EmployeeType)reader["EmployeeType"] == EmployeeType.FullTimeEmployee)
                        {
                            employee = new FullTimeEmployee
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Name = reader["Name"].ToString(),
                                Gender = reader["Gender"].ToString(),
                                DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"]),
                                Type = EmployeeType.FullTimeEmployee,
                                AnnualSalary = Convert.ToInt32(reader["AnnualSalary"])
                            };
                        }
                        else
                        {
                            employee = new PartTimeEmployee
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Name = reader["Name"].ToString(),
                                Gender = reader["Gender"].ToString(),
                                DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"]),
                                Type = EmployeeType.PartTimeEmployee,
                                HourlyPay = Convert.ToInt32(reader["HourlyPay"]),
                                HoursWorked = Convert.ToInt32(reader["HoursWorked"])
                            };
                        }
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

                //getting employee by EmployeeType
                SqlParameter parameterEmployeeType = new SqlParameter
                {
                    ParameterName = "@EmployeeType",
                    Value = employee.Type
                };
                cmd.Parameters.Add(parameterEmployeeType);

                if (employee.GetType() == typeof(FullTimeEmployee))
                {
                    SqlParameter parameterAnnualSalary = new SqlParameter
                    {
                        ParameterName = "AnnualSalary",
                        Value = ((FullTimeEmployee)employee).AnnualSalary,
                    };
                    cmd.Parameters.Add(parameterAnnualSalary);
                }
                else
                {
                    SqlParameter parameterHourlyPay = new SqlParameter
                    {
                        ParameterName = "HourlyPay",
                        Value = ((PartTimeEmployee)employee).HourlyPay
                    };
                    cmd.Parameters.Add(parameterHourlyPay);


                    SqlParameter parameterHoursWorked = new SqlParameter
                    {
                        ParameterName = "HoursWorked",
                        Value = ((PartTimeEmployee)employee).HoursWorked
                    };
                    cmd.Parameters.Add(parameterHoursWorked);
                }

                con.Open();

                cmd.ExecuteNonQuery();
            }
        }
    }
}
