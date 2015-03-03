using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExportCsvFiletTool.Entity
{
    public class ApiFieldsEntity
    {
        public string WebSite { get; set; }
        public string User { get; set; }
        public string Pwd { get; set; }
        public bool AnyStatus { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Limit { get; set; }
    }
}
