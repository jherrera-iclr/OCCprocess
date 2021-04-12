using System;
using System.Collections.Generic;
using System.Text;

namespace OCCprocess
{
    class SecurityList
    {
        public string RptID { get; set; }
        public string Symbol { get; set; }
        public DateTime MatDt { get; set; }
        public string CreateDt { get; set; }
        public string InactiveDt { get; set; }
    }
}
