using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using DigitalSignatureSigningXML;

namespace DigitalSignerConsole
{
    public enum QuizTypes
    {
        Default,
        TakeInSignSourcePath,
        TakeInValidationSourcePath,
        TakeInDestinationPath,
        ChooseCertificate,
        AddCertificatePassword,
        StartSigning,
        StartValidating
    }

    public class Quiz
    {
        private static Quiz startQuiz = null;
        private static bool useConsoleArgs = false;
        private static string[] passedConsoleArgs;

        private string question = "";
        private QuizTypes type = QuizTypes.Default;
        private Quiz previousQuiz = null;
        private IDictionary<string, Quiz> answers = null;

        public Quiz PreviousQuiz { get => previousQuiz; set => previousQuiz = value; }
        public IDictionary<string, Quiz> Answers { get => answers; set => answers = value; }
        public string Question { get => question; set => question = value; }
        public static Quiz StartQuiz { get => startQuiz; set => startQuiz = value; }
        public QuizTypes Type { get => type; set => type = value; }
        public static string[] PassedConsoleArgs { get => passedConsoleArgs; set => passedConsoleArgs = value; }
        public static bool UseConsoleArgs { get => useConsoleArgs; set => useConsoleArgs = value; }

        public Quiz(string setQuestion, Dictionary<string, Quiz> setAnswers = null, Quiz setPreviousQuiz = null, QuizTypes setType = QuizTypes.Default)
        {
            this.Question = setQuestion;
            this.Type = setType;
            this.Answers = setAnswers;
            this.PreviousQuiz = setPreviousQuiz;
        }

        public void AnsweredQuestion(string answer)
        {
            if (answer.Equals("0"))
            {
                if (PreviousQuiz == null)
                    TryStartQuiz();
                else
                    PreviousQuiz.AskQuestionEnableBacking();

                return;
            }

            this.DoActionBasedOnAnswer(answer);
        }

        public void AskQuestionEnableBacking()
        {
            string answer = AskQuestionGetString(this.Question);

            if (answer.Equals("0"))
            {
                if (PreviousQuiz == null)
                    TryStartQuiz();
                else
                    PreviousQuiz.AskQuestionEnableBacking();

                return;
            }

            this.DoActionBasedOnAnswer(answer);
        }

        public void DoActionBasedOnAnswer(string answer)
        {
            switch (type)
            {
                case QuizTypes.TakeInSignSourcePath:
                    {
                        this.GeneralSourceCheck(answer);
                        break;
                    }
                case QuizTypes.TakeInValidationSourcePath:
                    {
                        this.ValidationSourceCheck(answer);
                        break;
                    }
                case QuizTypes.TakeInDestinationPath:
                    {
                        this.StartValidatingDestination(answer);
                        break;
                    }
                case QuizTypes.ChooseCertificate:
                    {
                        this.ChooseCertificate();
                        break;
                    }
                case QuizTypes.AddCertificatePassword:
                    {
                        this.SetPassowrd(answer);
                        break;
                    }
                case QuizTypes.StartSigning:
                    {
                        this.StartSigning(answer);
                        break;
                    }
                case QuizTypes.StartValidating:
                    {
                        this.StartValidating(answer);
                        break;
                    }
                default:
                    {
                        this.MatchAnswer(answer);
                        break;
                    }
            }
        }

        public void StartValidating(string answer)
        {
            if (Answers != null)
            {
                Quiz nextQuiz;

                if (answer == this.Answers.First<KeyValuePair<string, Quiz>>().Key)
                {
                    nextQuiz = this.Answers.First<KeyValuePair<string, Quiz>>().Value;

                    ReportsManager.SourcePathReport = PathValidator.ValidateSourceLocation();

                    if (ReportsManager.SourcePathReport.IsSuccessful)
                    {
                        DigitalSignatureSigner.SourcePath = PathValidator.SourceLocation;
                        SuccessReport signingReport = DigitalSignatureSigner.VerifyDigitalSignatureInXMLXades();

                        if (signingReport.IsSuccessful)
                            Console.WriteLine("Dokumentas pasirašytas tinkamai!");
                        else
                        {
                            Console.WriteLine("Dokumentas nepasirašytas tinkamai!");
                            Console.WriteLine(signingReport.Message);
                        }

                        //resets quiz
                        if (nextQuiz == null)
                            TryStartQuiz();

                        this.AskNextQuestion(nextQuiz);
                    }
                    else
                    {
                        Console.WriteLine(ReportsManager.SourcePathReport.Message);
                        TryStartQuiz();
                    }
                }
            }

            this.ReaskQuestion();
        }

        public void ValidationSourceCheck(string answer)
        {
            if (File.Exists(answer))
            {
                Console.WriteLine("Įvesta dokumento lokacija.\n");
                if (Path.GetExtension(answer) == ".xml")
                {
                    Console.WriteLine("Dokumentas yra xml formato.\n");
                    PathValidator.IsSingleXMLDocument = true;
                    StartValidatingSource(answer);
                }
                else
                {
                    Console.WriteLine("Dokumentas netinkamo formato.\n");
                    this.ReaskQuestion();
                }
            }
            else if (Directory.Exists(answer))
            {
                Console.WriteLine("Įvesta aplankalo lokacija. Tinka tik .xml formato dokumento lokacija.\n");
                this.ReaskQuestion();
            }
            else
            {
                Console.WriteLine("Įvesta lokacija nerasta.\n");
                this.AskQuestionEnableBacking();
            }
        }

        public void GeneralSourceCheck(string answer)
        {
            if (File.Exists(answer))
            {
                Console.WriteLine("Įvesta dokumento lokacija.\n");
                if (Path.GetExtension(answer) == ".xml")
                {
                    Console.WriteLine("Dokumentas yra xml formato.\n");
                    PathValidator.IsSingleXMLDocument = true;
                    StartValidatingSource(answer);
                }
                else
                {
                    Console.WriteLine("Dokumentas netinkamo formato.\n");
                    this.ReaskQuestion();
                }
            }
            else if (Directory.Exists(answer))
            {
                Console.WriteLine("Įvesta aplankalo lokacija.\n");
                PathValidator.IsSingleXMLDocument = false;
                StartValidatingSource(answer);
            }
            else
            {
                Console.WriteLine("Įvesta lokacija nerasta.\n");
                this.AskQuestionEnableBacking();
            }
        }

        public void ChooseCertificate()
        {
            SuccessReport report;

            if (UseConsoleArgs && PassedConsoleArgs.Length == 4)
            {
                Console.WriteLine("\nRenkamas el. parašo sertifikatas pagal piršto antspaudą!");
                report = DigitalSignatureSigner.TrySelectingCertificateByThumbprint(PassedConsoleArgs[2]);
            }
            else
            {
                Console.WriteLine("\nPasirinkite el. parašo sertifikatą!");
                report = DigitalSignatureSigner.SelectCertificateFromStore();

            }

            ReportsManager.CertificateSelectionReport = report;

            if (report.IsSuccessful)
            {
                Quiz nextQuiz = this.Answers.First<KeyValuePair<string, Quiz>>().Value;

                if (nextQuiz != null)
                {
                    Console.WriteLine("Pasirinktas sertifikatas!");
                    this.AskNextQuestion(nextQuiz);
                }
                else
                    Console.WriteLine("Tusčias klausimas!");
            }
            else
            {
                Console.WriteLine("Nepasirinktas sertifikatas!\n\n");

                if (useConsoleArgs)
                    useConsoleArgs = false;

                TryStartQuiz();
            }
        }

        public void SetPassowrd(string password)
        {
            Console.WriteLine(password);
            SecureString securePwd = new SecureString();
            //prieš priskiriant slaptažodį galima patikrinti sertifikato informaciją
            //X509Certificate2.Subject, o jei turi pora raktų X509Certificate2.Publickey

            foreach (char c in password)
            {
                securePwd.AppendChar(c);
            }

            DigitalSignatureSigner.SecurePassowrd = securePwd;

            Quiz nextQuiz = this.Answers.First<KeyValuePair<string, Quiz>>().Value;

            if (nextQuiz != null)
            {
                //Console.WriteLine("Pasirinktas sertifikatas!");
                this.AskNextQuestion(nextQuiz);
            }
            else
                Console.WriteLine("Tusčias klausimas!");
        }

        public void StartSigning(string answer)
        {
            if (answer == this.answers.First<KeyValuePair<string, Quiz>>().Key)
            {
                PathValidator.ValidatePaths();

                if (ReportsManager.SourcePathReport.IsSuccessful &&
                    ReportsManager.DestinationPathReport.IsSuccessful &&
                    ReportsManager.CertificateSelectionReport.IsSuccessful)
                {
                    SuccessReport signingReport = SuccessReport.GetReport();

                    if (PathValidator.IsSingleXMLDocument)
                    {
                        signingReport = SetDigitalSignerPathsAndSign(PathValidator.SourceLocation, Path.Combine(PathValidator.DestinationLocation, Path.GetFileName(PathValidator.SourceLocation)));

                        if (signingReport.IsSuccessful)
                        {
                            signingReport.Message = "Sėkmingai pasirašytas dokumentas!";
                        }
                    }
                    else
                    {
                        string[] xmlFiles = Directory.GetFiles(PathValidator.SourceLocation, "*.xml");

                        foreach (string XMLDocumentPath in xmlFiles)
                        {
                            signingReport = SetDigitalSignerPathsAndSign(XMLDocumentPath, Path.Combine(PathValidator.DestinationLocation, Path.GetFileName(XMLDocumentPath)));

                            if (!signingReport.IsSuccessful)
                                break;
                        }

                        if (signingReport.IsSuccessful)
                        {
                            signingReport.Message = "Sėkmingai pasirašyti dokumentai!";
                        }
                    }

                    Console.WriteLine(signingReport.Message);
                }
                else
                {
                    Console.WriteLine("Aptikta klaida! Bandykite iš naujo!\n");
                    TryStartQuiz();
                }
            }
            else
                this.ReaskQuestion();
        }

        private void ReaskQuestion()
        {
            if(useConsoleArgs)
            {
                useConsoleArgs = false;
            }

            this.AskQuestionEnableBacking();
        }

        private void AskNextQuestion(Quiz nextQuiz)
        {
            if(UseConsoleArgs && PassedConsoleArgs.Length > 2)
            {
                switch(nextQuiz.Type)
                {
                    case QuizTypes.TakeInSignSourcePath:
                        {
                            nextQuiz.AnsweredQuestion(passedConsoleArgs[0]);
                            break;
                        }
                    case QuizTypes.TakeInDestinationPath:
                        {
                            nextQuiz.AnsweredQuestion(passedConsoleArgs[1]);
                            break;
                        }
                    case QuizTypes.AddCertificatePassword:
                        {
                            if (PassedConsoleArgs.Length == 3)
                                nextQuiz.AnsweredQuestion(passedConsoleArgs[2]);
                            else
                                nextQuiz.AnsweredQuestion(passedConsoleArgs[3]);
                            break;
                        }
                    default:
                        {
                            nextQuiz.AnsweredQuestion("1");
                            break;
                        }
                }
            }
            else
            {
                nextQuiz.AskQuestionEnableBacking();
            }
        }

        private SuccessReport SetDigitalSignerPathsAndSign(string inputPath, string outputPath)
        {
            DigitalSignatureSigner.SourcePath = inputPath;
            DigitalSignatureSigner.DestinationPath = outputPath;

            return DigitalSignatureSigner.DigitallySignXMLInXades();
        }

        private void MatchAnswer(string answer)
        {
            if (Answers != null)
            {
                Quiz nextQuiz;

                foreach (KeyValuePair<string, Quiz> keyPair in Answers)
                {
                    if (answer == keyPair.Key)
                    {
                        nextQuiz = keyPair.Value;

                        //resets quiz
                        if (nextQuiz == null)
                            break;

                        this.AskNextQuestion(nextQuiz);
                    }
                }
            }

            this.ReaskQuestion();
        }

        public void StartValidatingSource(string sourcePath)
        {
            PathValidator.SourceLocation = sourcePath;
            SuccessReport report = PathValidator.ValidateSourceLocation();
            Console.WriteLine(report.Message);

            if (report.IsSuccessful)
            {
                Quiz nextQuiz = this.Answers.First<KeyValuePair<string, Quiz>>().Value;

                if (nextQuiz != null)
                    if (this.Type == QuizTypes.TakeInSignSourcePath)
                    {
                        this.AskNextQuestion(nextQuiz);
                    }
                    else
                        nextQuiz.DoActionBasedOnAnswer("");
                else
                    Console.WriteLine("Tusčias klausimas!");
            }
            else
            {
                this.ReaskQuestion();
            }
        }

        public void StartValidatingDestination(string destinationPath)
        {
            PathValidator.DestinationLocation = destinationPath;
            SuccessReport report = PathValidator.ValidateDestinationLocation();
            Console.WriteLine(report.Message);

            if (report.IsSuccessful)
            {
                Quiz nextQuiz = this.answers.First<KeyValuePair<string, Quiz>>().Value;

                if (nextQuiz != null)
                    nextQuiz.DoActionBasedOnAnswer("");
                    //nextQuiz.AskQuestionEnableBacking();
                else
                    Console.WriteLine("Tusčias klausimas!");
            }
            else
            {
                this.ReaskQuestion();
            }
        }

        public static void TryStartQuiz()
        {
            if (startQuiz == null)
            {
                Console.WriteLine("Nėra pradinio klausimo!");
                Console.ReadLine();
                TryStartQuiz();
            }

            UseConsoleArgs = false;
            if (UseConsoleArgs)
                StartQuiz.AnsweredQuestion("1");
            else
                StartQuiz.AskQuestionEnableBacking();
        }

        public static string AskQuestionGetString(string Question)
        {
            Console.WriteLine(Question);
            return Console.ReadLine();
        }

        public static int AskQuestionGetInt(string Question)
        {
            Console.WriteLine(Question);
            return Convert.ToInt32(Console.ReadLine());
        }
    }
}
