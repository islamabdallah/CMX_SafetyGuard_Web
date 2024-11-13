using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebDriverViolation.Services.Models.APIModels
{
    public class PPsViolationAPIModel
    {
        public string camera_name { get; set; }

        public List<string>? image { get; set; }

        public DateTime time { get; set; }

        public int violType { get; set; }

        public double AvgProp { get; set; }

        public string? Spare1 { get; set; }

        public string? Spare2 { get; set; }
    }
}
