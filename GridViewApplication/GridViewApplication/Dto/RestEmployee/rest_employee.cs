using GridViewApplication.Dto.RestLevel;
using GridViewApplication.Dto.RestPosition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GridViewApplication.Dto.RestEmployee
{
    public class rest_employee
    {
        public string id { get; set; }
        public string name { get; set; }
        public string department { get; set; }
        public rest_level level { get; set; }
        public rest_position position { get; set; }
        
    }
}