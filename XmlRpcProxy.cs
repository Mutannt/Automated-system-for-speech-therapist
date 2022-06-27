using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookComputing.XmlRpc;

namespace SOA_Client
{
    //Объявление прокси-класса для работы по протоколу XML-RPC
    [XmlRpcUrl("http://myservice.ru/xmlrpc.api.php")]
    public interface IMyProxy : IXmlRpcProxy
    {
        // Логопеды---------------------------------------------
        [XmlRpcMethod("myservice:CreateLogoped")]
        int CreateLogoped(XMLRPC_Logoped logoped);

        [XmlRpcMethod("myservice:ListLogopeds")]
        XMLRPC_Logoped[] ListLogopeds();

        [XmlRpcMethod("myservice:ListFioLogopeds")]
        XMLRPC_Logoped[] ListFioLogopeds();

        [XmlRpcMethod("myservice:ReadLogoped")]
        XMLRPC_Logoped ReadLogoped(int id);

        [XmlRpcMethod("myservice:UpdateLogoped")]
        bool UpdateLogoped(int id, XMLRPC_Logoped logoped);

        [XmlRpcMethod("myservice:DeleteLogoped")]
        bool DeleteLogoped(int id);

        // Группы       ======================================
        [XmlRpcMethod("myservice:CreateGroup")]
        int CreateGroup(XMLRPC_Group group);

        [XmlRpcMethod("myservice:ListGroups")]
        XMLRPC_Group[] ListGroups();
        
        [XmlRpcMethod("myservice:ListGroupsUser")]
        XMLRPC_Group[] ListGroupsUser(int IDlog);

        [XmlRpcMethod("myservice:ReadGroup")]
        XMLRPC_Group ReadGroup(int id);

        [XmlRpcMethod("myservice:UpdateGroup")]
        bool UpdateGroup(int id, XMLRPC_Group group);

        [XmlRpcMethod("myservice:DeleteGroup")]
        bool DeleteGroup(int id);
        // Дети  =========================================
        [XmlRpcMethod("myservice:CreateChild")]
        int CreateChild(XMLRPC_Child child);

        [XmlRpcMethod("myservice:ListChildren")]
        XMLRPC_Child[] ListChildren();

        [XmlRpcMethod("myservice:ListChildrenInGroup")]
        XMLRPC_Child[] ListChildrenInGroup(int IDgr);

        [XmlRpcMethod("myservice:ListChildrenInGroupLogopunct")]
        XMLRPC_Child[] ListChildrenInGroupLogopunct(int IDgr);

        [XmlRpcMethod("myservice:ReadChild")]
        XMLRPC_Child ReadChild(int id);

        [XmlRpcMethod("myservice:UpdateChild")]
        bool UpdateChild(int id, XMLRPC_Child child);

        [XmlRpcMethod("myservice:DeleteChild")]
        bool DeleteChild(int id);
        // Диагностика  =========================================
        [XmlRpcMethod("myservice:CreateDiagnostic")]
        int CreateDiagnostic(XMLRPC_Diagnostic diagnostic);

        [XmlRpcMethod("myservice:ListDiagnostics")]
        XMLRPC_Diagnostic[] ListDiagnostics();

        [XmlRpcMethod("myservice:ListDiagnosticsUser")]
        XMLRPC_Diagnostic[] ListDiagnosticsUser(int IDlog);

        [XmlRpcMethod("myservice:ReadDiagnostic")]
        XMLRPC_Diagnostic ReadDiagnostic(int id);

        [XmlRpcMethod("myservice:ReadDiagnosticIDchild")]
        XMLRPC_Diagnostic ReadDiagnosticIDchild(int id);

        [XmlRpcMethod("myservice:UpdateDiagnostic")]
        bool UpdateDiagnostic(int id, XMLRPC_Diagnostic diagnostic);

        [XmlRpcMethod("myservice:DeleteDiagnostic")]
        bool DeleteDiagnostic(int id);
        //=======diagnosticpoints=================================================
        [XmlRpcMethod("myservice:CreateDiagnosticPoints")]
        int CreateDiagnosticPoints(XMLRPC_DiagnosticPoints DiagnosticPoints);

        [XmlRpcMethod("myservice:GetDiagnosticPoints")]
        XMLRPC_DiagnosticPoints GetDiagnosticPoints(int IDdiagn, string StartEnd);

        [XmlRpcMethod("myservice:UpdateDiagnosticPoints")]
        bool UpdateDiagnosticPoints(int id, XMLRPC_DiagnosticPoints DiagnosticPoints);

        //============== Речевые карты =============================================
        [XmlRpcMethod("myservice:CreateSpeechCard")]
        int CreateSpeechCard(XMLRPC_SpeechCard speechcard);

        [XmlRpcMethod("myservice:ListSpeechCards")]
        XMLRPC_SpeechCard[] ListSpeechCards();

        [XmlRpcMethod("myservice:ListSpeechCardsUser")]
        XMLRPC_SpeechCard[] ListSpeechCardsUser(int IDlog);

        [XmlRpcMethod("myservice:ReadSpeechCard")]
        XMLRPC_SpeechCard ReadSpeechCard(int id);

        [XmlRpcMethod("myservice:UpdateSpeechCard")]
        bool UpdateSpeechCard(int id, XMLRPC_SpeechCard speechcard);

        [XmlRpcMethod("myservice:DeleteSpeechCard")]
        bool DeleteSpeechCard(int id);
        //============== Индивидуальные планы =============================================
        [XmlRpcMethod("myservice:CreateIndividPlan")]
        int CreateIndividPlan(XMLRPC_IndividPlan individplan);

        [XmlRpcMethod("myservice:ListIndividPlans")]
        XMLRPC_IndividPlan[] ListIndividPlans();

        [XmlRpcMethod("myservice:ListIndividPlansUser")]
        XMLRPC_IndividPlan[] ListIndividPlansUser(int IDlog);

        [XmlRpcMethod("myservice:ReadIndividPlan")]
        XMLRPC_IndividPlan ReadIndividPlan(int id);

        [XmlRpcMethod("myservice:UpdateIndividPlan")]
        bool UpdateIndividPlan(int id, XMLRPC_IndividPlan individplan);

        [XmlRpcMethod("myservice:DeleteIndividPlan")]
        bool DeleteIndividPlan(int id);
        //========== Диаграммы =============================================================================================
        [XmlRpcMethod("myservice:GetDiagnosticPointsDiagram")]
        XMLRPC_DiagnosticPoints GetDiagnosticPointsDiagram(int IDchild, string StartEnd);

        [XmlRpcMethod("myservice:GetDiagnosticPointsAvgDiagram")]
        XMLRPC_Diagrams GetDiagnosticPointsAvgDiagram(string StartEnd);


        //==========ДЛЯ ОТЧЁТОВ=============================================================================================
        // Количество детей с диагнозом НПОЗ/ФФНР 2/3
        [XmlRpcMethod("myservice:GetCountNPOZ_FFNR")]
        XMLRPC_Diagnostic GetCountNPOZ_FFNR(int IDvioal1);

        // Количество детей с ОНР
        [XmlRpcMethod("myservice:GetCountONRs")]
        XMLRPC_Diagnostic GetCountONRs();

        // Количество детей зачисленных к логопеду с НПОЗ/ФФНР 2/3
        [XmlRpcMethod("myservice:GetCountInLogocentreNPOZ_FFNR")]
        XMLRPC_Diagnostic GetCountInLogocentreNPOZ_FFNR(int IDvioal1);

        // Количество детей зачисленных к логопеду с ОНР
        [XmlRpcMethod("myservice:GetCountInLogocentreONRs")]
        XMLRPC_Diagnostic GetCountInLogocentreONRs();

        // Количество выведенных детей
        [XmlRpcMethod("myservice:GetCountReleas")]
        XMLRPC_Diagnostic GetCountReleas();

        // Количество выведенных в школу детей
        [XmlRpcMethod("myservice:GetCountReleasInSchool")]
        XMLRPC_Diagnostic GetCountReleasInSchool();

        // Количество детей, нуждающихся в продолжении занятий в школе
        [XmlRpcMethod("myservice:GetCountSchoolLogocentre")]
        XMLRPC_Diagnostic GetCountSchoolLogocentre();

        // Количество детей, направленных в спец. учреждение
        [XmlRpcMethod("myservice:GetCountSpecialInstitution")]
        XMLRPC_Diagnostic GetCountSpecialInstitution();

        // Количество детей, выбывших по др. причинам
        [XmlRpcMethod("myservice:GetCountReleasOther")]
        XMLRPC_Diagnostic GetCountReleasOther();



    }

    //Объявление необходимых структур данных
    
    // Логопеды =======================================
    public struct XMLRPC_Logoped
    {
        public int ID;
        public string FIO;
        [XmlRpcMissingMapping(MappingAction.Ignore)] // Поле необязательное
        public string Log1n;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string Pass;
    }
    // Группы =======================================
    public struct XMLRPC_Group
    {
        public int IDgr;
        public int NumberGr;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public int IDlog;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string Logoped;
    }
    // Дети =======================================
    public struct XMLRPC_Child
    {
        public int ID;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string FIO;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string DateB;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string FIOMam;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string TelMam;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string FioPap;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string TelPap;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string Email;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public int IDgr;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string NumberGr;
    }
    // Диагностика =======================================
    public struct XMLRPC_Diagnostic
    {
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public int ID;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public int IDchild;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string FIOchild;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public int ItogScore1;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public int IDvioal1;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public int ItogScore2;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public int IDvioal2;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public bool NeedsHelp;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public bool SpecialInstitution;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public bool EnrollmentInLogocentre;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string DateEnrollment;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public bool Releas;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public bool ReleasInSchool;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public bool ReleasOther;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string DateReleas;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public bool SchoolLogocentre;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public int Count;
    }
    //=======DiagnosticPoints=================================================
    public struct XMLRPC_DiagnosticPoints
    {
        public int ID;
        public int IDdiagn;
        public string StartEnd;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public int SoundPronunciation;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public int SyllabicStructure;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public int PhonemicRepresentations;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public int Grammar;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public int LexicalStock;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public int SpeechUnderstanding;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public int ConnectedSpeech;
    }
    // Речевые карты =======================================
    public struct XMLRPC_SpeechCard
    {
        public int ID;
        public int IDchild;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string FIO;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string DateOfExamination;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string Lips;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string Teeth;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string Bite;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string Tongue;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string HyoidFrenulum;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string Sky;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string Salivation;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string ComboBoxes;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string SoundPronunciation;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string SoundDifferentiation;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string SyllableDifferentiation;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string WordDifference;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string SoundHighlight;
    }
    // Речевые карты =======================================
    public struct XMLRPC_IndividPlan
    {
        public int ID;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string SettingSounds;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string SoundDifferentiation;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string VocabularyEnrichment;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string DevelopmentGrammatical;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string FormationCoherentSpeech;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public int IDchild;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string FIO;
    }
    // Diagrams ==============================
    public struct XMLRPC_Diagrams
    {
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public double AvgSoundPronunciation;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public double AvgSyllabicStructure;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public double AvgPhonemicRepresentations;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public double AvgGrammar;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public double AvgLexicalStock;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public double AvgSpeechUnderstanding;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public double AvgConnectedSpeech;
    }

}// End
