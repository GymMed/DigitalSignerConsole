using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalSignatureSigningXML
{
    public sealed class ReportsManager
    {
        private static ReportsManager instance = null;
        private static SuccessReport sourcePathReport;
        private static SuccessReport destinationPathReport;
        private static SuccessReport certificateSelectionReport;
        private static SuccessReport generalReport;

        private ReportsManager()
        {

        }

        public static ReportsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ReportsManager();
                }
                return instance;
            }
        }

        public static SuccessReport SourcePathReport { get => sourcePathReport; set => sourcePathReport = value; }
        public static SuccessReport DestinationPathReport { get => destinationPathReport; set => destinationPathReport = value; }
        public static SuccessReport CertificateSelectionReport { get => certificateSelectionReport; set => certificateSelectionReport = value; }
        public static SuccessReport GeneralReport { get => generalReport; set => generalReport = value; }
    }
}
