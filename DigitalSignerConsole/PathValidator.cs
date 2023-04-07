using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitalSignerConsole;

namespace DigitalSignatureSigningXML
{
    public sealed class PathValidator
    {
        private static PathValidator instance = null;
        private static bool isSingleXMLDocument = true;

        private static string sourceLocation;
        private static string destinationLocation;
        private static string certificateLocation;

        private PathValidator()
        {

        }

        public static PathValidator Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PathValidator();
                }
                return instance;
            }
        }

        public static bool IsSingleXMLDocument { get => isSingleXMLDocument; set => isSingleXMLDocument = value; }
        public static string SourceLocation { get => sourceLocation; set => sourceLocation = value; }
        public static string DestinationLocation { get => destinationLocation; set => destinationLocation = value; }
        public static string CertificateLocation { get => certificateLocation; set => certificateLocation = value; }

        public static void ValidatePaths()
        {
            ReportsManager.SourcePathReport = ValidateSourceLocation();
            ReportsManager.DestinationPathReport = ValidateDestinationLocation();
        }

        public static void ValidatePathsForSignatureVerification(ApplicationFileFormats availableFormat)
        {
            ReportsManager.SourcePathReport = ValidateSourceLocation();
            ReportsManager.CertificateSelectionReport = ValidateCertificateLocation(availableFormat);
        }

        public static void ValidateAllPaths(ApplicationFileFormats availableFormat)
        {
            ValidatePaths();
            ReportsManager.CertificateSelectionReport = ValidateCertificateLocation(availableFormat);
        }

        public static SuccessReport ValidateSourceLocation()
        {
            //default report success status = false
            SuccessReport report = SuccessReport.GetReport();

            if (sourceLocation != "")
            {
                if (isSingleXMLDocument)
                {
                    if (File.Exists(sourceLocation))
                    {
                        report.Message = "Dokumento lokacija tinkama!";
                        report.IsSuccessful = true;
                    }
                    else if (Directory.Exists(sourceLocation))
                    {
                        report.Message = "Jūs negalite nurodyti aplankalą, kaip vieną pasirašymo dokumentą!";
                    }
                    else
                    {
                        report.Message = "Pasirašymo dokumento lokacija kompiuteryje nerasta!";
                    }
                }
                else
                {
                    if (Directory.Exists(sourceLocation))
                    {
                        bool containsXml = Directory.GetFiles(sourceLocation, "*.xml").Length > 0;

                        if (containsXml)
                        {
                            report.Message = "Dokumentų aplankalo lokacija tinkama!";
                            report.IsSuccessful = true;
                        }
                        else
                        {
                            report.Message = "Pasirašymo aplankalo lokacijoje nerasti XML dokumentai!";
                        }
                    }
                    else
                    {
                        report.Message = "Pasirašymo aplankalo lokacija kompiuteryje nerasta!";
                    }
                }
            }
            else
            {
                report.Message = "Pasirašytų dokumentų talpinimo lokacijos laukelis negali būti tuščias!";
            }

            return report;
        }

        public static SuccessReport ValidateCertificateLocation(ApplicationFileFormats availableFormat = ApplicationFileFormats.PFX)
        {
            //default report success status = false
            SuccessReport report = SuccessReport.GetReport();

            if (certificateLocation != "")
            {
                if (File.Exists(certificateLocation))
                {

                    if (Path.GetExtension(certificateLocation).Equals(Program.fileFormatsExtensions[(int)availableFormat], StringComparison.OrdinalIgnoreCase))
                    {
                        report.Message = "Sertifikato lokacija tinkama!";
                        report.IsSuccessful = true;
                    }
                    else
                        report.Message = "Sertifikatas pateiktas netinkamu formatu! tinka tik: " + Program.fileFormats[(int) availableFormat];
                }
                else if (Directory.Exists(certificateLocation))
                {
                    report.Message = "Jūs nurodėte aplankalą, o ne sertifikatą!";
                }
                else
                {
                    report.Message = "Sertifikato lokacija kompiuteryje nerasta!";
                }
            }
            else
            {
                report.Message = "Sertifikato lokacijos laukelis negali būti tuščias!";
            }

            return report;
        }

        public static SuccessReport ValidateDestinationLocation()
        {
            //default report success status = false
            SuccessReport report = SuccessReport.GetReport();

            if (destinationLocation != "")
            {
                report.Message = "Pasirašytų dokumentų talpinimo lokacijos laukelis negali būti tuščias!";
            }
            if (!Directory.Exists(destinationLocation))
            {
                /*DialogResult result = MessageBox.Show("Įvesta pasirašytų dokumentų lokacija nerasta. Ar Jūs norite sukurti lokacija?", "Sukurti lokacija", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    Directory.CreateDirectory(destinationLocation);
                    MessageBox.Show("Sukurta lokacija: " + destinationLocation, "Sukurti lokacija", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    report.Message = "Įvesta tinkama talpinimo lokacija.";
                    report.IsSuccessful = true;
                }
                else
                {
                    MessageBox.Show("Lokacija nesukurta. Pasirinkite kita lokaciją.", "Sukurti lokacija", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    report.Message = "Įvesta neegzistuojanti lokacija!";
                }*/
            }
            else
            {
                report.Message = "Įvesta tinkama talpinimo lokacija.";
                report.IsSuccessful = true;
            }

            return report;
        }
    }
}
