using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAI_Project.Models
{
    public class StockInfo
    {
        public string StockCode { get; set; } = "";
        public string StockName { get; set; } = "";
        public string MarketType { get; set; } = "";

        public override string ToString()
        {
            return StockName;
        }
    }
}
