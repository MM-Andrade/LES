using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frontend.Models
{
    public class Chart
    {
        public string[] labels { get; set; }
        public List<DataSets> dataSets { get; set; }
    }
}