using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace SOA_Client
{
    //Объявление прокси-класса для работы по протоколу REST
    [ServiceContract]
    [DataContractFormat]
    public interface IRest2018
    {
        //====Logoped=============================================================================================

        [WebInvoke(Method = "PUT", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, UriTemplate = "/rest/logoped")]
        void CreateLogoped(REST_Logoped logoped);

        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rest/logopeds")]
        REST_Logoped[] ListLogopeds();

        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rest/FioLogopeds")]
        REST_Logoped[] ListFioLogopeds();

        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rest/logoped?id={id}")]
        REST_Logoped ReadLogoped(int id);

        [WebInvoke(Method = "PATCH", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, UriTemplate = "/rest/logoped?id={id}")]
        void UpdateLogoped(int id, REST_Logoped logoped);

        [WebInvoke(Method = "DELETE", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rest/logoped?id={id}")]
        void DeleteLogoped(int id);

        //====Goup=================================================================================================

        [WebInvoke(Method = "PUT", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, UriTemplate = "/rest/group")]
        void CreateGroup(REST_Group group);

        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rest/groups")]
        REST_Group[] ListGroups();

        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rest/group?id={id}")]
        REST_Group ReadGroup(int id);

        [WebInvoke(Method = "PATCH", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, UriTemplate = "/rest/group?id={id}")]
        void UpdateGroup(int id, REST_Group group);

        [WebInvoke(Method = "DELETE", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rest/group?id={id}")]
        void DeleteGroup(int id);

        //====Child=================================================================================================

        [WebInvoke(Method = "PUT", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, UriTemplate = "/rest/child")]
        void CreateChild(REST_Child child);

        // Список всех детей
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rest/children")]
        REST_Child[] ListChildren();


        // Список детей из определённой группы
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rest/childrenInGroup?IDgr={IDgr}")]
        REST_Child[] ListChildrenInGroup(int IDgr);

        // Список детей, прошедших диагностику, из определённой группы
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rest/childrenInGroupDiagn?IDgr={IDgr}")]
        REST_Child[] ListChildrenInGroupDiagn(int IDgr);


        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rest/child?id={id}")]
        REST_Child ReadChild(int id);

        [WebInvoke(Method = "PATCH", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, UriTemplate = "/rest/child?id={id}")]
        void UpdateChild(int id, REST_Child child);

        [WebInvoke(Method = "DELETE", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rest/child?id={id}")]
        void DeleteChild(int id);

        //====Diagnostic=================================================================================================

        [WebInvoke(Method = "PUT", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, UriTemplate = "/rest/diagnostic")]
        void CreateDiagnostic(REST_Diagnostic diagnostic);

        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rest/diagnostics")]
        REST_Diagnostic[] ListDiagnostics();

        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rest/diagnostic?id={id}")]
        REST_Diagnostic ReadDiagnostic(int id);

        // Запись с диагностикой определённого ребёнка
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rest/diagnosticIDchild?idchild={idchild}")]
        REST_Diagnostic ReadDiagnosticIDchild(int idchild);


        [WebInvoke(Method = "PATCH", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, UriTemplate = "/rest/diagnostic?id={id}")]
        void UpdateDiagnostic(int id, REST_Diagnostic diagnostic);

        [WebInvoke(Method = "DELETE", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rest/diagnostic?id={id}")]
        void DeleteDiagnostic(int id);

        //====DiagnosticPoints===========================================================================

        [WebInvoke(Method = "PUT", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, UriTemplate = "/rest/diagnosticpoint")]
        void CreateDiagnosticPoints(REST_DiagnosticPoints diagnosticPoints);

        //&StartEnd={Начало}
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rest/diagnosticpoint?IDdiagn={IDdiagn}")]
        REST_DiagnosticPoints GetDiagnosticPointsStart(int IDdiagn);

        //[WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rest/diagnosticpoint?IDdiagn={IDdiagn}")]
        //REST_DiagnosticPoints GetDiagnosticPointsEnd(int IDdiagn);

        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rest/diagnosticpoint?ID={ID}")]
        REST_DiagnosticPoints GetDiagnosticPointsEnd2(int ID);
        // Диаграмма (Баллы в начале года)
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rest/diagnosticpointStartDiagram?IDchild={IDchild}")]
        REST_DiagnosticPoints GetDiagnosticPointsStartDiagram(int IDchild);

        [WebInvoke(Method = "PATCH", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, UriTemplate = "/rest/diagnosticpoint?id={id}")]
        void UpdateDiagnosticPoints(int id, REST_DiagnosticPoints diagnosticPoints);

        //====Speech cards===========================================================================
        [WebInvoke(Method = "PUT", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, UriTemplate = "/rest/speechcard")]
        void CreateSpeechCard(REST_SpeechCard speechcard);

        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rest/speechcards")]
        REST_SpeechCard[] ListSpeechCards();

        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rest/speechcard?id={id}")]
        REST_SpeechCard ReadSpeechCard(int id);

        [WebInvoke(Method = "PATCH", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, UriTemplate = "/rest/speechcard?id={id}")]
        void UpdateSpeechCard(int id, REST_SpeechCard speechcard);

        [WebInvoke(Method = "DELETE", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rest/speechcard?id={id}")]
        void DeleteSpeechCard(int id);

        //====IndividPlans===========================================================================
        [WebInvoke(Method = "PUT", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, UriTemplate = "/rest/individplan")]
        void CreateIndividPlan(REST_IndividPlan individplan);

        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rest/individplans")]
        REST_IndividPlan[] ListIndividPlans();

        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rest/individplan?id={id}")]
        REST_IndividPlan ReadIndividPlan(int id);

        [WebInvoke(Method = "PATCH", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, UriTemplate = "/rest/individplan?id={id}")]
        void UpdateIndividPlan(int id, REST_IndividPlan individplan);

        [WebInvoke(Method = "DELETE", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rest/individplan?id={id}")]
        void DeleteIndividPlan(int id);

        //====Violation=============================================================================================

        [WebInvoke(Method = "PUT", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, UriTemplate = "/rest/violation")]
        void CreateViolation(REST_Violation violation);

        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rest/violations")]
        REST_Violation[] ListViolations();

        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rest/violation?id={id}")]
        REST_Violation ReadViolation(int id);

        [WebInvoke(Method = "PATCH", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, UriTemplate = "/rest/violation?id={id}")]
        void UpdateViolation(int id, REST_Violation violation);

        [WebInvoke(Method = "DELETE", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rest/violation?id={id}")]
        void DeleteViolation(int id);

        //==========ДЛЯ ОТЧЁТОВ=============================================================================================

        // Количество детей с НПОЗ
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rest/diagnosticsCountNpoz")]
        REST_Diagnostic GetCountNPOZ();

        // Количество детей с ФФНР
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rest/diagnosticsCountFfnr")]
        REST_Diagnostic GetCountFFNR();

        // Количество детей с ОНР
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rest/diagnosticsCountOnrs")]
        REST_Diagnostic GetCountONRs();

        // Количество детей зачисленных к логопеду с НПОЗ
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rest/diagnosticsCountInLogocentreNpoz")]
        REST_Diagnostic GetCountInLogocentreNPOZ();

        // Количество детей зачисленных к логопеду с ФФНР
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rest/diagnosticsCountInLogocentreFfnr")]
        REST_Diagnostic GetCountInLogocentreFFNR();

        // Количество детей зачисленных к логопеду с ОНР
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rest/diagnosticsCountInLogocentreOnrs")]
        REST_Diagnostic GetCountInLogocentreONRs();

        // Количество выведенных детей
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rest/diagnosticsCountReleas")]
        REST_Diagnostic GetCountReleas();

        // Количество выведенных в школу детей
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rest/diagnosticsCountReleasInSchool")]
        REST_Diagnostic GetCountReleasInSchool();

        // Количество детей, нуждающихся в продолжении занятий в школе
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rest/diagnosticsCountSchoolLogocentre")]
        REST_Diagnostic GetCountSchoolLogocentre();

        // Количество детей, направленных в спец. учреждение
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rest/diagnosticsCountSpecialInstitution")]
        REST_Diagnostic GetCountSpecialInstitution();

        // Количество детей, выбывших по др. причинам
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rest/diagnosticsCountReleasOther")]
        REST_Diagnostic GetCountReleasOther();

        //========== Диаграммы =============================================================================================
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rest/diagnosticpointAvgStartDiagram")]
        REST_Diagrams GetDiagnosticPointsAvgStartDiagram();

        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rest/diagnosticpointAvgEndDiagram")]
        REST_Diagrams GetDiagnosticPointsAvgEndDiagram();
    }

    //Объявление необходимые структур данных
    // Logoped ==============================
    [DataContract]
    public class REST_Logoped
    {
        [DataMember]
        public int ID;

        [DataMember]
        public string FIO;

        [DataMember]
        public string Log1n;

        [DataMember]
        public string Pass;

        public REST_Logoped(string FIO, string Log1n, string Pass)
        {
            this.FIO = FIO;
            this.Log1n = Log1n;
            this.Pass = Pass;
        }

        public REST_Logoped(int ID, string FIO)
        {
            this.ID = ID;
            this.FIO = FIO;
        }

        public REST_Logoped(int ID, string FIO, string Log1n, string Pass)
        {
            this.ID = ID;
            this.FIO = FIO;
            this.Log1n = Log1n;
            this.Pass = Pass;
        }

        public REST_Logoped() { }
    }
    // Group ======================================
    [DataContract]
    public class REST_Group
    {
        [DataMember]
        public int IDgr;

        [DataMember]
        public int NumberGr;

        [DataMember]
        public int IDlog;

        [DataMember]
        public string Logoped;

        public REST_Group(int NumberGr, int IDlog)
        {
            this.NumberGr = NumberGr;
            this.IDlog = IDlog;
        }

        public REST_Group(int IDgr, int NumberGr, int IDlog)
        {
            this.IDgr = IDgr;
            this.NumberGr = NumberGr;
            this.IDlog = IDlog;
        }

        public REST_Group() { }
    }

    // Child ======================================
    public class REST_Child
    {
        [DataMember]
        public int ID;

        [DataMember]
        public string FIO;

        [DataMember]
        public string DateB;

        [DataMember]
        public string FIOMam;

        [DataMember]
        public string TelMam;

        [DataMember]
        public string FioPap;

        [DataMember]
        public string TelPap;

        [DataMember]
        public string Email;

        [DataMember]
        public int IDgr;

        [DataMember]
        public string NumberGr;

        public REST_Child(string FIO, string DateB, string FIOMam, string TelMam, string FioPap, string TelPap, string  Email, int IDgr)
        {
            this.FIO = FIO;
            this.DateB = DateB;
            this.FIOMam = FIOMam;
            this.TelMam = TelMam;
            this.FioPap = FioPap;
            this.TelPap = TelPap;
            this.Email = Email;
            this.IDgr = IDgr;
        }

        public REST_Child(int ID, string FIO, string DateB, string FIOMam, string TelMam, string FioPap, string TelPap, string Email, int IDgr)
        {
            this.ID = ID;
            this.FIO = FIO;
            this.DateB = DateB;
            this.FIOMam = FIOMam;
            this.TelMam = TelMam;
            this.FioPap = FioPap;
            this.TelPap = TelPap;
            this.Email = Email;
            this.IDgr = IDgr;
        }

        public REST_Child() { }
    }

    // Diagnostic ======================================
    public class REST_Diagnostic
    {
        [DataMember]
        public int ID;

        [DataMember]
        public int IDchild;

        [DataMember]
        public string FIOchild;

        [DataMember]
        public int ItogScore1;

        [DataMember]
        public int IDvioal1;

        [DataMember]
        public int ItogScore2;

        [DataMember]
        public int IDvioal2;

        [DataMember]
        public bool NeedsHelp;

        [DataMember]
        public bool SpecialInstitution;

        [DataMember]
        public bool EnrollmentInLogocentre;

        [DataMember]
        public string DateEnrollment;

        [DataMember]
        public bool Releas;

        [DataMember]
        public bool ReleasInSchool;
        [DataMember]
        public bool ReleasOther;

        [DataMember]
        public string DateReleas;

        [DataMember]
        public int Count;

        [DataMember]
        public bool SchoolLogocentre;


        public REST_Diagnostic(int IDchild, int ItogScore1, int IDvioal1, int ItogScore2, int IDvioal2, bool NeedsHelp, bool SpecialInstitution, bool EnrollmentInLogocentre, 
            string DateEnrollment, bool Releas, bool ReleasInSchool, bool ReleasOther, string DateReleas, bool SchoolLogocentre)
        {
            this.IDchild = IDchild;
            this.ItogScore1 = ItogScore1;
            this.IDvioal1 = IDvioal1;
            this.ItogScore2 = ItogScore2;
            this.IDvioal2 = IDvioal2;
            this.NeedsHelp = NeedsHelp;
            this.SpecialInstitution = SpecialInstitution;
            this.EnrollmentInLogocentre = EnrollmentInLogocentre;
            this.DateEnrollment = DateEnrollment;
            this.Releas = Releas;
            this.ReleasInSchool = ReleasInSchool;
            this.ReleasOther = ReleasOther;
            this.DateReleas = DateReleas;
            this.SchoolLogocentre = SchoolLogocentre;
        }

        // без итогового балла за конец
        public REST_Diagnostic(int IDchild, int ItogScore1, int IDvioal1, int IDvioal2, bool NeedsHelp, bool SpecialInstitution, bool EnrollmentInLogocentre, string DateEnrollment,
            bool Releas, bool ReleasInSchool, bool ReleasOther, string DateReleas, bool SchoolLogocentre)
        {
            this.IDchild = IDchild;
            this.ItogScore1 = ItogScore1;
            this.IDvioal1 = IDvioal1;
            this.IDvioal2 = IDvioal2;
            this.NeedsHelp = NeedsHelp;
            this.SpecialInstitution = SpecialInstitution;
            this.EnrollmentInLogocentre = EnrollmentInLogocentre;
            this.DateEnrollment = DateEnrollment;
            this.Releas = Releas;
            this.ReleasInSchool = ReleasInSchool;
            this.ReleasOther = ReleasOther;
            this.DateReleas = DateReleas;
            this.SchoolLogocentre = SchoolLogocentre;
        }

        public REST_Diagnostic(int ID, int IDchild, int ItogScore1, int IDvioal1, int ItogScore2, int IDvioal2, bool NeedsHelp, bool SpecialInstitution, bool EnrollmentInLogocentre,
            string DateEnrollment, bool Releas, bool ReleasInSchool, bool ReleasOther, string DateReleas, bool SchoolLogocentre)
        {
            this.ID = ID;
            this.IDchild = IDchild;
            this.ItogScore1 = ItogScore1;
            this.IDvioal1 = IDvioal1;
            this.ItogScore2 = ItogScore2;
            this.IDvioal2 = IDvioal2;
            this.NeedsHelp = NeedsHelp;
            this.SpecialInstitution = SpecialInstitution;
            this.EnrollmentInLogocentre = EnrollmentInLogocentre;
            this.DateEnrollment = DateEnrollment;
            this.Releas = Releas;
            this.ReleasInSchool = ReleasInSchool;
            this.ReleasOther = ReleasOther;
            this.DateReleas = DateReleas;
            this.SchoolLogocentre = SchoolLogocentre;
        }

        public REST_Diagnostic() { }
    }

    // DiagnosticPoints ======================================
    public class REST_DiagnosticPoints
    {
        [DataMember]
        public int ID;

        [DataMember]
        public int IDdiagn;

        [DataMember]
        public string StartEnd;

        [DataMember]
        public int SoundPronunciation;

        [DataMember]
        public int SyllabicStructure;

        [DataMember]
        public int PhonemicRepresentations;

        [DataMember]
        public int Grammar;

        [DataMember]
        public int LexicalStock;

        [DataMember]
        public int SpeechUnderstanding;

        [DataMember]
        public int ConnectedSpeech;




        public REST_DiagnosticPoints(int IDdiagn, string StartEnd, int SoundPronunciation, int SyllabicStructure, int PhonemicRepresentations, int Grammar, int LexicalStock, int SpeechUnderstanding, int ConnectedSpeech)
        {
            this.IDdiagn = IDdiagn;
            this.StartEnd = StartEnd;
            this.SoundPronunciation = SoundPronunciation;
            this.SyllabicStructure = SyllabicStructure;
            this.PhonemicRepresentations = PhonemicRepresentations;
            this.Grammar = Grammar;
            this.LexicalStock = LexicalStock;
            this.SpeechUnderstanding = SpeechUnderstanding;
            this.ConnectedSpeech = ConnectedSpeech;
        }

        public REST_DiagnosticPoints(int ID, int IDdiagn, string StartEnd, int SoundPronunciation, int SyllabicStructure, int PhonemicRepresentations, int Grammar, int LexicalStock, int SpeechUnderstanding, int ConnectedSpeech)
        {
            this.ID = ID;
            this.IDdiagn = IDdiagn;
            this.StartEnd = StartEnd;
            this.SoundPronunciation = SoundPronunciation;
            this.SyllabicStructure = SyllabicStructure;
            this.PhonemicRepresentations = PhonemicRepresentations;
            this.Grammar = Grammar;
            this.LexicalStock = LexicalStock;
            this.SpeechUnderstanding = SpeechUnderstanding;
            this.ConnectedSpeech = ConnectedSpeech;
        }

        public REST_DiagnosticPoints() { }

        public REST_DiagnosticPoints(int IDdiagn)
        {
            this.IDdiagn = IDdiagn;
        }
    }

    // SpeechCards ======================================
    public class REST_SpeechCard
    {
        [DataMember]
        public int ID;

        [DataMember]
        public int IDchild;

        [DataMember]
        public string FIO;

        [DataMember]
        public string DateOfExamination;

        [DataMember]
        public string Lips;

        [DataMember]
        public string Teeth;

        [DataMember]
        public string Bite;

        [DataMember]
        public string Tongue;

        [DataMember]
        public string HyoidFrenulum;

        [DataMember]
        public string Sky;

        [DataMember]
        public string Salivation;

        [DataMember]
        public string ComboBoxes;

        [DataMember]
        public string SoundPronunciation;

        [DataMember]
        public string SoundDifferentiation;

        [DataMember]
        public string SyllableDifferentiation;

        [DataMember]
        public string WordDifference;

        [DataMember]
        public string SoundHighlight;



        public REST_SpeechCard(int IDchild, string DateOfExamination, string Lips, string Teeth, string Bite, string Tongue, string HyoidFrenulum, string Sky, 
            string Salivation, string ComboBoxes, string SoundPronunciation, string SoundDifferentiation, string SyllableDifferentiation, string WordDifference, string SoundHighlight)
        {
            this.IDchild = IDchild;
            this.DateOfExamination = DateOfExamination;
            this.Lips = Lips;
            this.Teeth = Teeth;
            this.Bite = Bite;
            this.Tongue = Tongue;
            this.HyoidFrenulum = HyoidFrenulum;
            this.Sky = Sky;
            this.Salivation = Salivation;
            this.ComboBoxes = ComboBoxes;
            this.SoundPronunciation = SoundPronunciation;
            this.SoundDifferentiation = SoundDifferentiation;
            this.SyllableDifferentiation = SyllableDifferentiation;
            this.WordDifference = WordDifference;
            this.SoundHighlight = SoundHighlight;
        }

        public REST_SpeechCard(int ID, int IDchild, string DateOfExamination, string Lips, string Teeth, string Bite, string Tongue, string HyoidFrenulum, string Sky,
            string Salivation, string ComboBoxes, string SoundPronunciation, string SoundDifferentiation, string SyllableDifferentiation, string WordDifference, string SoundHighlight)
        {
            this.ID = ID;
            this.IDchild = IDchild;
            this.DateOfExamination = DateOfExamination;
            this.Lips = Lips;
            this.Teeth = Teeth;
            this.Bite = Bite;
            this.Tongue = Tongue;
            this.HyoidFrenulum = HyoidFrenulum;
            this.Sky = Sky;
            this.Salivation = Salivation;
            this.ComboBoxes = ComboBoxes;
            this.SoundPronunciation = SoundPronunciation;
            this.SoundDifferentiation = SoundDifferentiation;
            this.SyllableDifferentiation = SyllableDifferentiation;
            this.WordDifference = WordDifference;
            this.SoundHighlight = SoundHighlight;
        }

        public REST_SpeechCard() { }
    }

    // IndividPlans ======================================
    public class REST_IndividPlan
    {
        [DataMember]
        public int ID;

        [DataMember]
        public string SettingSounds;

        [DataMember]
        public string SoundDifferentiation;

        [DataMember]
        public string VocabularyEnrichment;

        [DataMember]
        public string DevelopmentGrammatical;

        [DataMember]
        public string FormationCoherentSpeech;

        [DataMember]
        public int IDchild;

        [DataMember]
        public string FIO;

        public REST_IndividPlan(string SettingSounds, string SoundDifferentiation, string VocabularyEnrichment, string DevelopmentGrammatical, string FormationCoherentSpeech, int IDchild)
        {
            this.SettingSounds = SettingSounds;
            this.SoundDifferentiation = SoundDifferentiation;
            this.VocabularyEnrichment = VocabularyEnrichment;
            this.DevelopmentGrammatical = DevelopmentGrammatical;
            this.FormationCoherentSpeech = FormationCoherentSpeech;
            this.IDchild = IDchild;
        }

        public REST_IndividPlan(int ID, string SettingSounds, string SoundDifferentiation, string VocabularyEnrichment, string DevelopmentGrammatical, string FormationCoherentSpeech, int IDchild)
        {
            this.ID = ID;
            this.SettingSounds = SettingSounds;
            this.SoundDifferentiation = SoundDifferentiation;
            this.VocabularyEnrichment = VocabularyEnrichment;
            this.DevelopmentGrammatical = DevelopmentGrammatical;
            this.FormationCoherentSpeech = FormationCoherentSpeech;
            this.IDchild = IDchild;
        }

        public REST_IndividPlan() { }
    }

    // Diagrams ==============================
    [DataContract]
    public class REST_Diagrams
    {
        [DataMember]
        public float AvgSoundPronunciation;

        [DataMember]
        public float AvgSyllabicStructure;

        [DataMember]
        public float AvgPhonemicRepresentations;

        [DataMember]
        public float AvgGrammar;

        [DataMember]
        public float AvgLexicalStock;

        [DataMember]
        public float AvgSpeechUnderstanding;

        [DataMember]
        public float AvgConnectedSpeech;

        public REST_Diagrams(float AvgSoundPronunciation, float AvgSyllabicStructure, float AvgPhonemicRepresentations, float AvgGrammar,
            float AvgLexicalStock, float AvgSpeechUnderstanding, float AvgConnectedSpeech)
        {
            this.AvgSoundPronunciation = AvgSoundPronunciation;
            this.AvgSyllabicStructure = AvgSyllabicStructure;
            this.AvgPhonemicRepresentations = AvgPhonemicRepresentations;
            this.AvgGrammar = AvgGrammar;
            this.AvgLexicalStock = AvgLexicalStock;
            this.AvgSpeechUnderstanding = AvgSpeechUnderstanding;
            this.AvgConnectedSpeech = AvgConnectedSpeech;
        }

        public REST_Diagrams() { }
    }


    // Violation ==============================
    [DataContract]
    public class REST_Violation
    {
        [DataMember]
        public int ID;

        [DataMember]
        public string Name;

        [DataMember]
        public string Description;

        [DataMember]
        public int PreparatoryStageTreatmentMethods;

        public REST_Violation(string Name, string Description, int PreparatoryStageTreatmentMethods)
        {
            this.Name = Name;
            this.Description = Description;
            this.PreparatoryStageTreatmentMethods = PreparatoryStageTreatmentMethods;
        }

        public REST_Violation(int ID, string Name, string Description, int PreparatoryStageTreatmentMethods)
        {
            this.ID = ID;
            this.Name = Name;
            this.Description = Description;
            this.PreparatoryStageTreatmentMethods = PreparatoryStageTreatmentMethods;
        }

        public REST_Violation() { }
    }
   

}//=====End
