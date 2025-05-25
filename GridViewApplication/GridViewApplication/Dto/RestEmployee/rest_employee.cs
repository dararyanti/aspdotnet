using Newtonsoft.Json;
using System;

public class rest_employee
{
    [JsonProperty("employeeId")]
    public string EmployeeId { get; set; }
    [JsonProperty("firstName")]
    public string FirstName { get; set; }

    [JsonProperty("lastName")]
    public string LastName { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("hireDate")]
    public DateTime HireDate { get; set; }

    [JsonProperty("departmentId")]
    public string DepartmentId { get; set; }
    [JsonProperty("departmentName")]
    public string DepartmentName { get; set; }
    [JsonProperty("createName")]
    public string CreateName { get; set; }
    [JsonProperty("createDate")]
    public DateTime CreateDate { get; set; }
}
