using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAI_Project.Models
{
    public class StockPriceInfo
    {
        public string CurrentPrice { get; set; } = "";
        public string ChangeRate { get; set; } = "";
        public string Volume { get; set; } = "";
        public string High52Week { get; set; } = "";
    }
}