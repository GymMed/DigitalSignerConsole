using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalSignatureSigningXML
{
    public struct SuccessReport
    {
        private string message;
        private bool isSuccessful;

        public string Message { get => message; set => message = value; }
        public bool IsSuccessful { get => isSuccessful; set => isSuccessful = value; }

        public SuccessReport(string reportMessage, bool isReportSuccessful = true)
        {
            this.message = reportMessage;
            this.isSuccessful = isReportSuccessful;
        }

        public static SuccessReport GetReport()
        {
            return new SuccessReport("", false);
        }
    }
}
