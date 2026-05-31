using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAI_Project.Models
{
    public class DartDisclosure
    {
        public string CorpName { get; set; } = "";
        public string ReportName { get; set; } = "";
        public string ReceiptNo { get; set; } = "";

        public DateTime ReportDate { get; set; }

        public string DartUrl =>
            $"https://dart.fss.or.kr/dsaf001/main.do?rcpNo={ReceiptNo}";
    }
}