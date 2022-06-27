<?
require_once("$_SERVER[DOCUMENT_ROOT]/../includes/flight/Flight.php");
require_once("$_SERVER[DOCUMENT_ROOT]/../db/dal.inc.php");

// ===========Logoped,Logopeds================
function CreateLogoped()
{
	//file_put_contents("log.txt",var_export(Flight::request()->data["FIO"],TRUE));
	DBCreateLogoped(
		Flight::request()->data["FIO"],
		Flight::request()->data["Log1n"],
		Flight::request()->data["Pass"]
		);
}
Flight::route('PUT /rest/logoped', "CreateLogoped");

function ListLogopeds()
{
	//echo "Hello from REST";
	Flight::json(DBListLogopeds());
}
Flight::route("GET /rest/logopeds", "ListLogopeds");

function ListFioLogopeds()
{
	Flight::json(DBListFioLogopeds());
}
Flight::route("GET /rest/FioLogopeds", "ListFioLogopeds");

function ReadLogoped($id)
{
	Flight::json(DBReadLogoped($id));
}

Flight::route('GET /rest/logoped\?id=@id', "ReadLogoped");

function UpdateLogoped($id)
{
	DBUpdateLogoped(
		$id,
		Flight::request()->data["FIO"],
		Flight::request()->data["Log1n"],
		Flight::request()->data["Pass"]
	);
}
Flight::route('PATCH /rest/logoped\?id=@id', "UpdateLogoped");

function DeleteLogoped($id)
{
	DBDeleteLogoped($id);
}
Flight::route('DELETE /rest/logoped\?id=@id', "DeleteLogoped");

// ===========Group,Groups================
function CreateGroup()
{
	//file_put_contents("log.txt",var_export(Flight::request()->data["NumberGr"],TRUE));
	DBCreateGroup(
		Flight::request()->data["NumberGr"],
		Flight::request()->data["IDlog"]);
}
Flight::route('PUT /rest/group', "CreateGroup");

function ListGroups()
{
	//echo "Hello from REST";
	Flight::json(DBListGroups());
}
Flight::route("GET /rest/groups", "ListGroups");

function ReadGroup($id)
{
	Flight::json(DBReadGroup($id));
}

Flight::route('GET /rest/group\?id=@id', "ReadGroup");

function UpdateGroup($id)
{
	DBUpdateGroup(
		$id,
		Flight::request()->data["NumberGr"],
		Flight::request()->data["IDlog"]
	);
}
Flight::route('PATCH /rest/group\?id=@id', "UpdateGroup");

function DeleteGroup($id)
{
	DBDeleteGroup($id);
}
Flight::route('DELETE /rest/group\?id=@id', "DeleteGroup");

// ===========Child,Children================
function CreateChild()
{
	//file_put_contents("log.txt",var_export(Flight::request()->data["NumberGr"],TRUE));
	DBCreateChild(
		Flight::request()->data["FIO"],
		Flight::request()->data["DateB"],
		Flight::request()->data["FIOMam"],
		Flight::request()->data["TelMam"],
		Flight::request()->data["FioPap"],
		Flight::request()->data["TelPap"],
		Flight::request()->data["Email"],
		Flight::request()->data["IDgr"]
	);
}
Flight::route('PUT /rest/child', "CreateChild");

function ListChildren()
{
	//echo "Hello from REST";
	Flight::json(DBListChildren());
}
Flight::route("GET /rest/children", "ListChildren");

function ListChildrenInGroup($IDgr)
{
	Flight::json(DBListChildrenInGroup($IDgr));
}
Flight::route("GET /rest/childrenInGroup\?IDgr=@IDgr", "ListChildrenInGroup");

function ListChildrenInGroupDiagn($IDgr)
{
	Flight::json(DBListChildrenInGroupDiagn($IDgr));
}
Flight::route("GET /rest/childrenInGroupDiagn\?IDgr=@IDgr", "ListChildrenInGroupDiagn");

function ReadChild($id)
{
	Flight::json(DBReadChild($id));
}

Flight::route('GET /rest/child\?id=@id', "ReadChild");

function UpdateChild($id)
{
	DBUpdateChild(
		$id,
		Flight::request()->data["FIO"],
		Flight::request()->data["DateB"],
		Flight::request()->data["FIOMam"],
		Flight::request()->data["TelMam"],
		Flight::request()->data["FioPap"],
		Flight::request()->data["TelPap"],
		Flight::request()->data["Email"],
		Flight::request()->data["IDgr"]
	);
}
Flight::route('PATCH /rest/child\?id=@id', "UpdateChild");

function DeleteChild($id)
{
	DBDeleteChild($id);
}
Flight::route('DELETE /rest/child\?id=@id', "DeleteChild");

// ===========Diagnostic,Diagnostics================
function CreateDiagnostic()
{
	//file_put_contents("log.txt",var_export(Flight::request()->data["NumberGr"],TRUE));
	DBCreateDiagnostic(
		Flight::request()->data["IDchild"],
		Flight::request()->data["ItogScore1"],
		Flight::request()->data["IDvioal1"],
		Flight::request()->data["ItogScore2"],
		Flight::request()->data["IDvioal2"],
		Flight::request()->data["NeedsHelp"],
		Flight::request()->data["SpecialInstitution"],
		Flight::request()->data["EnrollmentInLogocentre"],
		Flight::request()->data["DateEnrollment"],
		Flight::request()->data["Releas"],
		Flight::request()->data["ReleasInSchool"],
		Flight::request()->data["ReleasOther"],
		Flight::request()->data["DateReleas"],
		Flight::request()->data["SchoolLogocentre"]
	);

	// Возврат id созданной записи
    //Flight::json(Array("id"=>$id));
}
Flight::route('PUT /rest/diagnostic', "CreateDiagnostic");

function ListDiagnostics()
{
	//echo "Hello from REST";
	Flight::json(DBListDiagnostics());
}
Flight::route("GET /rest/diagnostics", "ListDiagnostics");

function ReadDiagnostic($id)
{
	Flight::json(DBReadDiagnostic($id));
}
Flight::route('GET /rest/diagnostic\?id=@id', "ReadDiagnostic");

function ReadDiagnosticIDchild($idchild)
{
	Flight::json(DBReadDiagnosticIDchild($idchild));
}
Flight::route('GET /rest/diagnosticIDchild\?idchild=@idchild', "ReadDiagnosticIDchild");

// function GetMaxIdDiagnostics()
// {
// 	Flight::json(DBGetMaxIdDiagnostics());
// }
// Flight::route('GET /rest/diagnostics2', "GetMaxIdDiagnostics");

function UpdateDiagnostic($id)
{
	DBUpdateDiagnostic(
		$id,
		Flight::request()->data["IDchild"],
		Flight::request()->data["ItogScore1"],
		Flight::request()->data["IDvioal1"],
		Flight::request()->data["ItogScore2"],
		Flight::request()->data["IDvioal2"],
		Flight::request()->data["NeedsHelp"],
		Flight::request()->data["SpecialInstitution"],
		Flight::request()->data["EnrollmentInLogocentre"],
		Flight::request()->data["DateEnrollment"],
		Flight::request()->data["Releas"],
		Flight::request()->data["ReleasInSchool"],
		Flight::request()->data["ReleasOther"],
		Flight::request()->data["DateReleas"],
		Flight::request()->data["SchoolLogocentre"]
	);
}
Flight::route('PATCH /rest/diagnostic\?id=@id', "UpdateDiagnostic");

function DeleteDiagnostic($id)
{
	DBDeleteDiagnostic($id);
}
Flight::route('DELETE /rest/diagnostic\?id=@id', "DeleteDiagnostic");

//=====DiagnosticPoints==================================

function CreateDiagnosticPoints()
{
	//file_put_contents("log.txt",var_export(Flight::request()->data["NumberGr"],TRUE));
	DBCreateDiagnosticPoints(
		Flight::request()->data["IDdiagn"],
		Flight::request()->data["StartEnd"],
		Flight::request()->data["SoundPronunciation"],
		Flight::request()->data["SyllabicStructure"],
		Flight::request()->data["PhonemicRepresentations"],
		Flight::request()->data["Grammar"],
		Flight::request()->data["LexicalStock"],
		Flight::request()->data["SpeechUnderstanding"],
		Flight::request()->data["ConnectedSpeech"]
	);
}
Flight::route('PUT /rest/diagnosticpoint', "CreateDiagnosticPoints");

function GetDiagnosticPointsStart($IDdiagn)
{
	Flight::json(DBGetDiagnosticPointsStart($IDdiagn));
}
Flight::route('GET /rest/diagnosticpoint\?IDdiagn=@IDdiagn', "GetDiagnosticPointsStart");

function GetDiagnosticPointsEnd2($ID)
{
	Flight::json(DBGetDiagnosticPointsEnd2($ID));
}
Flight::route('GET /rest/diagnosticpoint\?ID=@ID', "GetDiagnosticPointsEnd2");

// Диаграммы//////////////////////////////////////////////////////////////////////////////
// Баллы в начале года
function GetDiagnosticPointsStartDiagram($IDchild)
{
	Flight::json(DBGetDiagnosticPointsStartDiagram($IDchild));
}
Flight::route("GET /rest/diagnosticpointStartDiagram\?IDchild=@IDchild", "GetDiagnosticPointsStartDiagram");

// Средние знач. в начале года
function GetDiagnosticPointsAvgStartDiagram()
{
	Flight::json(DBGetDiagnosticPointsAvgStartDiagram());
}
Flight::route("GET /rest/diagnosticpointAvgStartDiagram", "GetDiagnosticPointsAvgStartDiagram");

// Средние знач. в конце года
function GetDiagnosticPointsAvgEndDiagram()
{
	Flight::json(DBGetDiagnosticPointsAvgEndDiagram());
}
Flight::route("GET /rest/diagnosticpointAvgEndDiagram", "GetDiagnosticPointsAvgEndDiagram");

function UpdateDiagnosticPoints($id)
{
	DBUpdateDiagnosticPoints(
		$id,
		Flight::request()->data["IDdiagn"],
		Flight::request()->data["StartEnd"],
		Flight::request()->data["SoundPronunciation"],
		Flight::request()->data["SyllabicStructure"],
		Flight::request()->data["PhonemicRepresentations"],
		Flight::request()->data["Grammar"],
		Flight::request()->data["LexicalStock"],
		Flight::request()->data["SpeechUnderstanding"],
		Flight::request()->data["ConnectedSpeech"]
	);
}
Flight::route('PATCH /rest/diagnosticpoint\?id=@id', "UpdateDiagnosticPoints");

//==Speech cards======================================================
function CreateSpeechCard()
{
	DBCreateSpeechCard(
		Flight::request()->data["IDchild"],
		Flight::request()->data["DateOfExamination"],
		Flight::request()->data["Lips"],
		Flight::request()->data["Teeth"],
		Flight::request()->data["Bite"],
		Flight::request()->data["Tongue"],
		Flight::request()->data["HyoidFrenulum"],
		Flight::request()->data["Sky"],
		Flight::request()->data["Salivation"],
		Flight::request()->data["ComboBoxes"],
		Flight::request()->data["SoundPronunciation"],
		Flight::request()->data["SoundDifferentiation"],
		Flight::request()->data["SyllableDifferentiation"],
		Flight::request()->data["WordDifference"],
		Flight::request()->data["SoundHighlight"]
	);
}
Flight::route('PUT /rest/speechcard', "CreateSpeechCard");

function ListSpeechCards()
{
	//echo "Hello from REST";
	Flight::json(DBListSpeechCards());
}
Flight::route("GET /rest/speechcards", "ListSpeechCards");

function ReadSpeechCard($id)
{
	Flight::json(DBReadSpeechCard($id));
}
Flight::route('GET /rest/speechcard\?id=@id', "ReadSpeechCard");

function UpdateSpeechCard($id)
{
	DBUpdateSpeechCard(
		$id,
		Flight::request()->data["IDchild"],
		Flight::request()->data["DateOfExamination"],
		Flight::request()->data["Lips"],
		Flight::request()->data["Teeth"],
		Flight::request()->data["Bite"],
		Flight::request()->data["Tongue"],
		Flight::request()->data["HyoidFrenulum"],
		Flight::request()->data["Sky"],
		Flight::request()->data["Salivation"],
		Flight::request()->data["ComboBoxes"],
		Flight::request()->data["SoundPronunciation"],
		Flight::request()->data["SoundDifferentiation"],
		Flight::request()->data["SyllableDifferentiation"],
		Flight::request()->data["WordDifference"],
		Flight::request()->data["SoundHighlight"]
	);
}
Flight::route('PATCH /rest/speechcard\?id=@id', "UpdateSpeechCard");

function DeleteSpeechCard($id)
{
	DBDeleteSpeechCard($id);
}
Flight::route('DELETE /rest/speechcard\?id=@id', "DeleteSpeechCard");

// ==================IndividualPlans=========================
function CreateIndividPlan()
{
	DBCreateIndividPlan(
		Flight::request()->data["SettingSounds"],
		Flight::request()->data["SoundDifferentiation"],
		Flight::request()->data["VocabularyEnrichment"],
		Flight::request()->data["DevelopmentGrammatical"],
		Flight::request()->data["FormationCoherentSpeech"],
		Flight::request()->data["IDchild"]
	);
}
Flight::route('PUT /rest/individplan', "CreateIndividPlan");

function ListIndividPlans()
{
	Flight::json(DBListIndividPlans());
}
Flight::route("GET /rest/individplans", "ListIndividPlans");

function ReadIndividPlan($id)
{
	Flight::json(DBReadIndividPlan($id));
}
Flight::route('GET /rest/individplan\?id=@id', "ReadIndividPlan");

function UpdateIndividPlan($id)
{
	DBUpdateIndividPlan(
		$id,
		Flight::request()->data["SettingSounds"],
		Flight::request()->data["SoundDifferentiation"],
		Flight::request()->data["VocabularyEnrichment"],
		Flight::request()->data["DevelopmentGrammatical"],
		Flight::request()->data["FormationCoherentSpeech"],
		Flight::request()->data["IDchild"]
	);
}
Flight::route('PATCH /rest/individplan\?id=@id', "UpdateIndividPlan");

function DeleteIndividPlan($id)
{
	DBDeleteIndividPlan($id);
}
Flight::route('DELETE /rest/individplan\?id=@id', "DeleteIndividPlan");
// ===========Violation,Violations================
function CreateViolation()
{
	//file_put_contents("log.txt",var_export(Flight::request()->data["NumberGr"],TRUE));
	DBCreateViolation(
		Flight::request()->data["Name"],
		Flight::request()->data["Description"],
		Flight::request()->data["PreparatoryStageTreatmentMethods"]
	);
}
Flight::route('PUT /rest/violation', "CreateViolation");

function ListViolations()
{
	//echo "Hello from REST";
	Flight::json(DBListViolations());
}
Flight::route("GET /rest/violations", "ListViolations");

function ReadViolation($id)
{
	Flight::json(DBReadViolation($id));
}
Flight::route('GET /rest/violation\?id=@id', "ReadViolation");

function UpdateViolation($id)
{
	DBUpdateViolation(
		$id,
		Flight::request()->data["Name"],
		Flight::request()->data["Description"],
		Flight::request()->data["PreparatoryStageTreatmentMethods"]
	);
}
Flight::route('PATCH /rest/violation\?id=@id', "UpdateViolation");

function DeleteViolation($id)
{
	DBDeleteViolation($id);
}
Flight::route('DELETE /rest/violation\?id=@id', "DeleteViolation");

// =====Запросы для отчётов======================================================================

// Количество детей с диагнозом НПОЗ
function GetCountNPOZ()
{
	Flight::json(DBGetCountNPOZ());
}
Flight::route("GET /rest/diagnosticsCountNpoz", "GetCountNPOZ");

// Количество детей с диагнозом ФФНР
function GetCountFFNR()
{
	Flight::json(DBGetCountFFNR());
}
Flight::route("GET /rest/diagnosticsCountFfnr", "GetCountFFNR");

// Количество детей с диагнозами ОНР
function GetCountONRs()
{
	Flight::json(DBGetCountONRs());
}
Flight::route("GET /rest/diagnosticsCountOnrs", "GetCountONRs");

// Количество детей зачисленных к логопеду с НПОЗ
function GetCountInLogocentreNPOZ()
{
	Flight::json(DBGetCountInLogocentreNPOZ());
}
Flight::route("GET /rest/diagnosticsCountInLogocentreNpoz", "GetCountInLogocentreNPOZ");

// Количество детей зачисленных к логопеду с ФФНР
function GetCountInLogocentreFFNR()
{
	Flight::json(DBGetCountInLogocentreFFNR());
}
Flight::route("GET /rest/diagnosticsCountInLogocentreFfnr", "GetCountInLogocentreFFNR");

// Количество детей зачисленных к логопеду с ОНР
function GetCountInLogocentreONRs()
{
	Flight::json(DBGetCountInLogocentreONRs());
}
Flight::route("GET /rest/diagnosticsCountInLogocentreOnrs", "GetCountInLogocentreONRs");

// Количество выведенных детей
function GetCountReleas()
{
	Flight::json(DBGetCountReleas());
}
Flight::route("GET /rest/diagnosticsCountReleas", "GetCountReleas");



// Количество выведенных в школу детей
function GetCountReleasInSchool()
{
	Flight::json(DBGetCountReleasInSchool());
}
Flight::route("GET /rest/diagnosticsCountReleasInSchool", "GetCountReleasInSchool");

// Количество детей, нуждающихся в продолжении занятий в школе
function GetCountSchoolLogocentre()
{
	Flight::json(DBGetCountSchoolLogocentre());
}
Flight::route("GET /rest/diagnosticsCountSchoolLogocentre", "GetCountSchoolLogocentre");

// Количество детей, направленных в спец. учреждение
function GetCountSpecialInstitution()
{
	Flight::json(DBGetCountSpecialInstitution());
}
Flight::route("GET /rest/diagnosticsCountSpecialInstitution", "GetCountSpecialInstitution");

// Количество детей, выбывших по др. причинам ОБЩЕЕ
function GetCountReleasOther()
{
	Flight::json(DBGetCountReleasOther());
}
Flight::route("GET /rest/diagnosticsCountReleasOther", "GetCountReleasOther");




/*Flight::route('GET /wall\.post\.xml\?resp=@resp',function($resp) {
	if($resp=="ok")
	{
		echo "<response><post_id>1</post_id></response>";
	}
	else
	{
		echo "<error><error_code>2</error_code><error_msg>Hello</error_msg></error>";
	}
 });
 
 function Hello() {
	echo "Hello, RESTfull World !!!";
 }
 
 Flight::route('GET /hello',"Hello");
 
 //Аргументы в стартовой строке
 Flight::route('GET /myapi\?arg1=@val1&arg2=@val2',function($val1,$val2) {
	echo "Получены аргументы: arg1=[$val1] arg2=[$val2]";
 });
 
 //Аргументы в теле запроса
 Flight::route('POST /myapi_post',function() {	
	$arg1=Flight::request()->data->arg1;
	$arg2=Flight::request()->data->arg2;
	//header('Access-Control-Allow-Origin: 123');
	echo "POST1 Получены аргументы:".
	" arg1=".$arg1.
	" arg2=".$arg2;
	//var_dump(Flight::request());
 });
 
 //Аргументы в теле запроса
 Flight::route('PUT /myapi',function() {	
	$arg1=Flight::request()->data->arg1;
	$arg2=Flight::request()->data->arg2;
	//header('Access-Control-Allow-Origin: 123');
	echo "PUT Получены аргументы:".
	" arg1=".$arg1.
	" arg2=".$arg2;
	//var_dump(Flight::request());
 });
 
Flight::route('GET /\?method=flickr.interestingness.getList&api_key=@apiKey&extras=@extras', function($apiKey,$extras){    
	echo "<?xml version=\"1.0\"?>\n";
	?><photos page="1">
		<photo id="1" title="MyPhoto1"/>
		<photo id="2" title="MyPhoto2"/>
	  </photos>
	<?	
});

Flight::route('GET /\?myxml', function(){    
	echo "<?xml version=\"1.0\"?>\n";
	?><photos page="1">
		<photo id="1" title="MyPhoto1"/>
		<photo id="2" title="MyPhoto2"/>
	  </photos>
	<?	
	header('Access-Control-Allow-Origin: null');
});

Flight::route('GET /\?myjson', function(){
    echo "{\"resourceName\": \"demo\",\"a\":\"4\"}";	
});*/

Flight::start();
