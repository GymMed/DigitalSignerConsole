using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalSignerConsole
{
    public enum ApplicationFileFormats
    {
        XML,
        PFX,
        CER
    }

    public partial class Program
    {
        public static readonly string[] fileFormats = new string[]
        {
            "XML dokumentai (*.xml)|*.xml",
            "Sertifikato dokumentai (*.pfx)|*.pfx",
            "Sertifikato dokumentai (*.cer)|*.cer"
        };

        public static readonly string[] fileFormatsExtensions = new string[]
        {
            ".xml",
            ".pfx",
            ".cer"
        };

        static void Main(string[] args)
        {
            Dictionary<string, Quiz> startingAnswers = new Dictionary<string, Quiz>();
            Dictionary<string, Quiz> signSourceAnswers = new Dictionary<string, Quiz>();
            Dictionary<string, Quiz> signDestinationAnswers = new Dictionary<string, Quiz>();
            Dictionary<string, Quiz> signCertificateAnswers = new Dictionary<string, Quiz>();
            //Dictionary<string, Quiz> signTypeInCertificatePasswordAnswers = new Dictionary<string, Quiz>();
            Dictionary<string, Quiz> signXMLAnswers = new Dictionary<string, Quiz>();

            Dictionary<string, Quiz> validateSourceAnswers = new Dictionary<string, Quiz>();
            Dictionary<string, Quiz> validateCertificateAnswers = new Dictionary<string, Quiz>();
            Dictionary<string, Quiz> validateStartAnswers = new Dictionary<string, Quiz>();

            Quiz getSignSource = new Quiz("Įveskite pasirašymo dokumento/aplankalo vietą.", signSourceAnswers, null, QuizTypes.TakeInSignSourcePath);
            Quiz getSignDestination = new Quiz("Įveskite pasirašytų(-o) dokumento/aplankalo talpinimo vietą.", signDestinationAnswers, getSignSource, QuizTypes.TakeInDestinationPath);
            Quiz getSignCertificate = new Quiz("Pasirinkite sertifikatą.", signCertificateAnswers, getSignDestination, QuizTypes.ChooseCertificate);
            //Quiz getSignCertificatePassword = new Quiz("Įveskite Pin kodą.", signTypeInCertificatePasswordAnswers, getSignCertificate, QuizTypes.AddCertificatePassword);
            Quiz signXML = new Quiz("Parašykite 1, jei norite pradėti pasirašymą.", signXMLAnswers, getSignCertificate, QuizTypes.StartSigning);

            Quiz getValidationSource = new Quiz("Įveskite validavimo dokumento/aplankalo vietą.", validateSourceAnswers, null, QuizTypes.TakeInValidationSourcePath);
            Quiz getValidationCertificate = new Quiz("Pasirinkite sertifikatą.", validateCertificateAnswers, getValidationSource, QuizTypes.ChooseCertificate);
            Quiz getValidationStart = new Quiz("Parašykite 1, jei norite pradėti validavimą.", validateStartAnswers, getValidationCertificate, QuizTypes.StartValidating);

            startingAnswers.Add("1", getSignSource);
            startingAnswers.Add("2", getValidationSource);

            /////////////////////////////sgining/////////////////////////////
            signSourceAnswers.Add("1", getSignDestination);

            signDestinationAnswers.Add("1", getSignCertificate);

            signCertificateAnswers.Add("1", signXML);

            //signTypeInCertificatePasswordAnswers.Add("1", signXML);

            signXMLAnswers.Add("1", null);

            /////////////////////////////validating/////////////////////////////
            validateSourceAnswers.Add("1", getValidationCertificate);

            validateCertificateAnswers.Add("1", getValidationStart);

            validateStartAnswers.Add("1", null);

            Quiz.StartQuiz = new Quiz("Parašykite 1, jei norite pasirašyti xml dokumentą el. parašu. \nParašykite 2, jei norite validuoti xml dokumento el. parašą. \nVisada galite parašyti 0, jei norite grįžti atgal eigoje.",
                startingAnswers);

            if (args.Length > 0)
            {
                //Console.WriteLine(args[2]);
                Quiz.PassedConsoleArgs = args;
                Quiz.UseConsoleArgs = true;
                Quiz.TryStartQuiz();
            }
            else
            {
                Quiz.TryStartQuiz();
            }

            Console.Read();
        }
    }
}
