using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GridViewApplication.Dto.RestEmployee
{
    public class rest_create_employee
    {
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("hireDate")]
        public DateTime HireDate { get; set; }

        [JsonProperty("departmentId")]
        public Guid DepartmentId { get; set; }

        [JsonProperty("createName")]
        public string CreateName { get; set; }

        [JsonIgnore]
        public DateTime CreateDate { get; set; }
    }
}