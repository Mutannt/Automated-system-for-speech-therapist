<?php
require_once("$_SERVER[DOCUMENT_ROOT]/../db/dal.inc.php");
require_once("XML/RPC/Server.php");
require_once("XML/RPC.php");

//--------Logoped-------------------------------
function CreateLogoped($params)
{
	$struct = $params->getParam(0)->getval();

	DBCreateLogoped(
		$struct["FIO"],
		$struct["Log1n"],
		$struct["Pass"]
	);

	return new XML_RPC_Response(
		new XML_RPC_Value(_DBInsertID(), "int")
	);
}
function ListLogopeds()
{
	$logopeds = DBListLogopeds();
	//Приведение значений, полученных из БД
	//к их правильным типам данных.
	//По умолчанию все значения, полученные из
	//БД имеют тип string.
	foreach ($logopeds as $k => $logoped) {
		$logopeds[$k]["ID"] = (int)$logoped["ID"];
		$logopeds[$k]["FIO"] = $logoped["FIO"];
		$logopeds[$k]["Log1n"] = $logoped["Log1n"];
		$logopeds[$k]["Pass"] = $logoped["Pass"];
	}

	return new XML_RPC_Response(
		XML_RPC_encode($logopeds)
	);
}

function ListFioLogopeds()
{
	$logopeds = DBListFioLogopeds();
	//Приведение значений, полученных из БД
	//к их правильным типам данных.
	//По умолчанию все значения, полученные из
	//БД имеют тип string.
	foreach ($logopeds as $k => $logoped) {
		$logopeds[$k]["ID"] = (int)$logoped["ID"];
		$logopeds[$k]["FIO"] = $logoped["FIO"];
	}

	return new XML_RPC_Response(
		XML_RPC_encode($logopeds)
	);
}

function ReadLogoped($params)
{
	$id = $params->getParam(0)->getval();

	$logoped = DBReadLogoped($id);

	$logoped["ID"] = (int)$logoped["ID"];
	$logoped["FIO"] = $logoped["FIO"];
	$logoped["Log1n"] = $logoped["Log1n"];
	$logoped["Pass"] = $logoped["Pass"];

	return new XML_RPC_Response(
		XML_RPC_encode(
			$logoped
		)
	);
}
function UpdateLogoped($params)
{
	$id = $params->getParam(0)->getval();
	$struct = $params->getParam(1)->getval();
	DBUpdateLogoped(
		$id,
		$struct["FIO"],
		$struct["Log1n"],
		$struct["Pass"]
	);

	return new XML_RPC_Response(
		new XML_RPC_Value(true, "boolean")
	);
}
function DeleteLogoped($params)
{
	$id = $params->getParam(0)->getval();
	DBDeleteLogoped($id);
	return new XML_RPC_Response(
		new XML_RPC_Value(true, "boolean")
	);
}
//========Gruppa==============================
function CreateGroup($params)
{
	$struct = $params->getParam(0)->getval();

	DBCreateGroup(
		$struct["NumberGr"],
		$struct["IDlog"]
	);

	return new XML_RPC_Response(
		new XML_RPC_Value(_DBInsertID(), "int")
	);
}
function ListGroups()
{
	$groups = DBListGroups();
	//Приведение значений, полученных из БД
	//к их правильным типам данных.
	//По умолчанию все значения, полученные из
	//БД имеют тип string.
	foreach ($groups as $k => $group) {
		$groups[$k]["IDgr"] = (int)$group["IDgr"];
		$groups[$k]["NumberGr"] = (int)$group["NumberGr"];
		$groups[$k]["IDlog"] = (int)$group["IDlog"];
	}

	return new XML_RPC_Response(
		XML_RPC_encode($groups)
	);
}
// Каждый пользователь получает только те группы к которым относится
function ListGroupsUser($params)
{
	$IDlog = $params->getParam(0)->getval();
	$groups = DBListGroupsUser($IDlog);
	//Приведение значений, полученных из БД
	//к их правильным типам данных.
	//По умолчанию все значения, полученные из
	//БД имеют тип string.
	foreach ($groups as $k => $group) {
		$groups[$k]["IDgr"] = (int)$group["IDgr"];
		$groups[$k]["NumberGr"] = (int)$group["NumberGr"];
		$groups[$k]["IDlog"] = (int)$group["IDlog"];
	}

	return new XML_RPC_Response(
		XML_RPC_encode($groups)
	);
}
function ReadGroup($params)
{
	$id = $params->getParam(0)->getval();

	$group = DBReadGroup($id);

	$group["IDgr"] = (int)$group["IDgr"];
	$group["NumberGr"] = (int)$group["NumberGr"];
	$group["IDlog"] = (int)$group["IDlog"];

	return new XML_RPC_Response(
		XML_RPC_encode(
			$group
		)
	);
}
function UpdateGroup($params)
{
	$id = $params->getParam(0)->getval();
	$struct = $params->getParam(1)->getval();
	DBUpdateGroup(
		$id,
		$struct["NumberGr"],
		$struct["IDlog"]
	);

	return new XML_RPC_Response(
		new XML_RPC_Value(true, "boolean")
	);
}

function DeleteGroup($params)
{
	$id = $params->getParam(0)->getval();
	DBDeleteGroup($id);
	return new XML_RPC_Response(
		new XML_RPC_Value(true, "boolean")
	);
}
//=========Children=================================
function CreateChild($params)
{
	$struct = $params->getParam(0)->getval();

	DBCreateChild(
		$struct["FIO"],
		$struct["DateB"],
		$struct["FIOMam"],
		$struct["TelMam"],
		$struct["FioPap"],
		$struct["TelPap"],
		$struct["Email"],
		$struct["IDgr"]
	);

	return new XML_RPC_Response(
		new XML_RPC_Value(_DBInsertID(), "int")
	);
}
function ListChildren()
{
	$children = DBListChildren();
	//Приведение значений, полученных из БД
	//к их правильным типам данных.
	//По умолчанию все значения, полученные из
	//БД имеют тип string.
	foreach ($children as $k => $child) {
		$children[$k]["ID"] = (int)$child["ID"];
		$children[$k]["FIO"] = $child["FIO"];
		$children[$k]["DateB"] = $child["DateB"]; // В php нет типа Date 
		$children[$k]["FIOMam"] = $child["FIOMam"];
		$children[$k]["TelMam"] = $child["TelMam"];
		$children[$k]["FioPap"] = $child["FioPap"];
		$children[$k]["TelPap"] = $child["TelPap"];
		$children[$k]["Email"] = $child["Email"];
		$children[$k]["IDgr"] = (int)$child["IDgr"];
	}

	return new XML_RPC_Response(
		XML_RPC_encode($children)
	);
}
// Список детей из определённой группы
function ListChildrenInGroup($params)
{
	$IDgr = $params->getParam(0)->getval();

	$children = DBListChildrenInGroup($IDgr);
	//Приведение значений, полученных из БД
	//к их правильным типам данных.
	//По умолчанию все значения, полученные из
	//БД имеют тип string.
	foreach ($children as $k => $child) {
		$children[$k]["ID"] = (int)$child["ID"];
		$children[$k]["FIO"] = $child["FIO"];
		$children[$k]["IDgr"] = (int)$child["IDgr"];
	}

	return new XML_RPC_Response(
		XML_RPC_encode($children)
	);
}
// Список детей из определённой группы
function ListChildrenInGroupLogopunct($params)
{
	$IDgr = $params->getParam(0)->getval();

	$children = DBListChildrenInGroupLogopunct($IDgr);

	foreach ($children as $k => $child) {
		$children[$k]["ID"] = (int)$child["ID"];
		$children[$k]["FIO"] = $child["FIO"];
		$children[$k]["IDgr"] = (int)$child["IDgr"];
	}

	return new XML_RPC_Response(
		XML_RPC_encode($children)
	);
}
function ReadChild($params)
{
	$id = $params->getParam(0)->getval();

	$сhild = DBReadChild($id);

	$сhild["ID"] = (int)$сhild["ID"];
	$сhild["FIO"] = $сhild["FIO"];
	$сhild["DateB"] = $сhild["DateB"];
	$сhild["FIOMam"] = $сhild["FIOMam"];
	$сhild["TelMam"] = $сhild["TelMam"];
	$сhild["FioPap"] = $сhild["FioPap"];
	$сhild["TelPap"] = $сhild["TelPap"];
	$сhild["Email"] = $сhild["Email"];
	$сhild["IDgr"] = (int)$сhild["IDgr"];

	return new XML_RPC_Response(
		XML_RPC_encode(
			$сhild
		)
	);
}
function UpdateChild($params)
{
	$id = $params->getParam(0)->getval();
	$struct = $params->getParam(1)->getval();
	DBUpdateChild(
		$id,
		$struct["FIO"],
		$struct["DateB"],
		$struct["FIOMam"],
		$struct["TelMam"],
		$struct["FioPap"],
		$struct["TelPap"],
		$struct["Email"],
		$struct["IDgr"]
	);

	return new XML_RPC_Response(
		new XML_RPC_Value(true, "boolean")
	);
}

function DeleteChild($params)
{
	$id = $params->getParam(0)->getval();
	DBDeleteChild($id);
	return new XML_RPC_Response(
		new XML_RPC_Value(true, "boolean")
	);
}
//=========Diagnostics=================================
function CreateDiagnostic($params)
{
	$struct = $params->getParam(0)->getval();

	DBCreateDiagnostic(
		$struct["IDchild"],
		$struct["ItogScore1"],
		$struct["IDvioal1"],
		$struct["ItogScore2"],
		$struct["IDvioal2"],
		$struct["NeedsHelp"],
		$struct["SpecialInstitution"],
		$struct["EnrollmentInLogocentre"],
		$struct["DateEnrollment"],
		$struct["Releas"],
		$struct["ReleasInSchool"],
		$struct["ReleasOther"],
		$struct["DateReleas"],
		$struct["SchoolLogocentre"]
	);

	return new XML_RPC_Response(
		new XML_RPC_Value(_DBInsertID(), "int")
	);
}

function ListDiagnostics()
{
	$diagnostics = DBListDiagnostics();
	//Приведение значений, полученных из БД к их правильным типам данных.
	//По умолчанию все значения, полученные из БД имеют тип string.
	foreach ($diagnostics as $k => $diagnostic) {
		$diagnostics[$k]["ID"] = (int)$diagnostic["ID"];
		$diagnostics[$k]["IDchild"] = (int)$diagnostic["IDchild"];
		$diagnostics[$k]["ItogScore1"] = (int)$diagnostic["ItogScore1"];
		$diagnostics[$k]["IDvioal1"] = (int)$diagnostic["IDvioal1"];
		$diagnostics[$k]["ItogScore2"] = (int)$diagnostic["ItogScore2"];
		$diagnostics[$k]["IDvioal2"] = (int)$diagnostic["IDvioal2"];
		$diagnostics[$k]["NeedsHelp"] = (bool)$diagnostic["NeedsHelp"];
		$diagnostics[$k]["SpecialInstitution"] = (bool)$diagnostic["SpecialInstitution"];
		$diagnostics[$k]["EnrollmentInLogocentre"] = (bool)$diagnostic["EnrollmentInLogocentre"];
		$diagnostics[$k]["DateEnrollment"] = $diagnostic["DateEnrollment"];
		$diagnostics[$k]["Releas"] = (bool)$diagnostic["Releas"];
		$diagnostics[$k]["ReleasInSchool"] = (bool)$diagnostic["ReleasInSchool"];
		$diagnostics[$k]["ReleasOther"] = (bool)$diagnostic["ReleasOther"];
		$diagnostics[$k]["DateReleas"] = $diagnostic["DateReleas"];
		$diagnostics[$k]["SchoolLogocentre"] = (bool)$diagnostic["SchoolLogocentre"];
	}

	return new XML_RPC_Response(
		XML_RPC_encode($diagnostics)
	);
}

function ListDiagnosticsUser($params)
{
	$IDlog = $params->getParam(0)->getval();
	$diagnostics = DBListDiagnosticsUser($IDlog);
	//Приведение значений, полученных из БД к их правильным типам данных.
	//По умолчанию все значения, полученные из БД имеют тип string.
	foreach ($diagnostics as $k => $diagnostic) {
		$diagnostics[$k]["ID"] = (int)$diagnostic["ID"];
		$diagnostics[$k]["IDchild"] = (int)$diagnostic["IDchild"];
		$diagnostics[$k]["ItogScore1"] = (int)$diagnostic["ItogScore1"];
		$diagnostics[$k]["IDvioal1"] = (int)$diagnostic["IDvioal1"];
		$diagnostics[$k]["ItogScore2"] = (int)$diagnostic["ItogScore2"];
		$diagnostics[$k]["IDvioal2"] = (int)$diagnostic["IDvioal2"];
		$diagnostics[$k]["NeedsHelp"] = (bool)$diagnostic["NeedsHelp"];
		$diagnostics[$k]["SpecialInstitution"] = (bool)$diagnostic["SpecialInstitution"];
		$diagnostics[$k]["EnrollmentInLogocentre"] = (bool)$diagnostic["EnrollmentInLogocentre"];
		$diagnostics[$k]["DateEnrollment"] = $diagnostic["DateEnrollment"];
		$diagnostics[$k]["Releas"] = (bool)$diagnostic["Releas"];
		$diagnostics[$k]["ReleasInSchool"] = (bool)$diagnostic["ReleasInSchool"];
		$diagnostics[$k]["ReleasOther"] = (bool)$diagnostic["ReleasOther"];
		$diagnostics[$k]["DateReleas"] = $diagnostic["DateReleas"];
		$diagnostics[$k]["SchoolLogocentre"] = (bool)$diagnostic["SchoolLogocentre"];
	}

	return new XML_RPC_Response(
		XML_RPC_encode($diagnostics)
	);
}

function ReadDiagnostic($params)
{
	$id = $params->getParam(0)->getval();

	$diagnostic = DBReadDiagnostic($id);

	$diagnostic["ID"] = (int)$diagnostic["ID"];
	$diagnostic["IDchild"] = (int)$diagnostic["IDchild"];
	$diagnostic["ItogScore1"] = (int)$diagnostic["ItogScore1"];
	$diagnostic["IDvioal1"] = (int)$diagnostic["IDvioal1"];
	$diagnostic["ItogScore2"] = (int)$diagnostic["ItogScore2"];
	$diagnostic["IDvioal2"] = (int)$diagnostic["IDvioal2"];
	$diagnostic["NeedsHelp"] = (bool)$diagnostic["NeedsHelp"];
	$diagnostic["SpecialInstitution"] = (bool)$diagnostic["SpecialInstitution"];
	$diagnostic["EnrollmentInLogocentre"] = (bool)$diagnostic["EnrollmentInLogocentre"];
	$diagnostic["DateEnrollment"] = $diagnostic["DateEnrollment"];
	$diagnostic["Releas"] = (bool)$diagnostic["Releas"];
	$diagnostic["ReleasInSchool"] = (bool)$diagnostic["ReleasInSchool"];
	$diagnostic["ReleasOther"] = (bool)$diagnostic["ReleasOther"];
	$diagnostic["DateReleas"] = $diagnostic["DateReleas"];
	$diagnostic["SchoolLogocentre"] = (bool)$diagnostic["SchoolLogocentre"];

	return new XML_RPC_Response(
		XML_RPC_encode(
			$diagnostic
		)
	);
}
function ReadDiagnosticIDchild($params)
{
	$id = $params->getParam(0)->getval();

	$diagnostic = DBReadDiagnosticIDchild($id);

	$diagnostic["IDchild"] = (int)$diagnostic["IDchild"];
	$diagnostic["IDvioal1"] = (int)$diagnostic["IDvioal1"];
	$diagnostic["IDvioal2"] = (int)$diagnostic["IDvioal2"];

	return new XML_RPC_Response(
		XML_RPC_encode(
			$diagnostic
		)
	);
}
function UpdateDiagnostic($params)
{
	$id = $params->getParam(0)->getval();
	$struct = $params->getParam(1)->getval();
	DBUpdateDiagnostic(
		$id,
		$struct["IDchild"],
		$struct["ItogScore1"],
		$struct["IDvioal1"],
		$struct["ItogScore2"],
		$struct["IDvioal2"],
		$struct["NeedsHelp"],
		$struct["SpecialInstitution"],
		$struct["EnrollmentInLogocentre"],
		$struct["DateEnrollment"],
		$struct["Releas"],
		$struct["ReleasInSchool"],
		$struct["ReleasOther"],
		$struct["DateReleas"],
		$struct["SchoolLogocentre"]
	);

	return new XML_RPC_Response(
		new XML_RPC_Value(true, "boolean")
	);
}

function DeleteDiagnostic($params)
{
	$id = $params->getParam(0)->getval();
	DBDeleteDiagnostic($id);
	return new XML_RPC_Response(
		new XML_RPC_Value(true, "boolean")
	);
}
//=======diagnosticpoints=================================================
function CreateDiagnosticPoints($params)
{
	$struct = $params->getParam(0)->getval();

	DBCreateDiagnosticPoints(
		$struct["IDdiagn"],
		$struct["StartEnd"],
		$struct["SoundPronunciation"],
		$struct["SyllabicStructure"],
		$struct["PhonemicRepresentations"],
		$struct["Grammar"],
		$struct["LexicalStock"],
		$struct["SpeechUnderstanding"],
		$struct["ConnectedSpeech"]
	);

	return new XML_RPC_Response(
		new XML_RPC_Value(_DBInsertID(), "int")
	);
}
function GetDiagnosticPoints($params)
{
	$IDdiagn = $params->getParam(0)->getval();
	$StartEnd = $params->getParam(1)->getval();

	$DiagnosticPoints = DBGetDiagnosticPoints($IDdiagn,$StartEnd);

	$DiagnosticPoints["ID"] = (int)$DiagnosticPoints["ID"];
	$DiagnosticPoints["IDdiagn"] = (int)$DiagnosticPoints["IDdiagn"];
	$DiagnosticPoints["StartEnd"] = $DiagnosticPoints["StartEnd"];
	$DiagnosticPoints["SoundPronunciation"] = (int)$DiagnosticPoints["SoundPronunciation"];
	$DiagnosticPoints["SyllabicStructure"] = (int)$DiagnosticPoints["SyllabicStructure"];
	$DiagnosticPoints["PhonemicRepresentations"] = (int)$DiagnosticPoints["PhonemicRepresentations"];
	$DiagnosticPoints["Grammar"] = (int)$DiagnosticPoints["Grammar"];
	$DiagnosticPoints["LexicalStock"] = (int)$DiagnosticPoints["LexicalStock"];
	$DiagnosticPoints["SpeechUnderstanding"] = (int)$DiagnosticPoints["SpeechUnderstanding"];
	$DiagnosticPoints["ConnectedSpeech"] = (int)$DiagnosticPoints["ConnectedSpeech"];

	return new XML_RPC_Response(
		XML_RPC_encode(
			$DiagnosticPoints
		)
	);
}

function UpdateDiagnosticPoints($params)
{
	$id = $params->getParam(0)->getval();
	$struct = $params->getParam(1)->getval();
	DBUpdateDiagnosticPoints(
		$id,
		$struct["IDdiagn"],
		$struct["StartEnd"],
		$struct["SoundPronunciation"],
		$struct["SyllabicStructure"],
		$struct["PhonemicRepresentations"],
		$struct["Grammar"],
		$struct["LexicalStock"],
		$struct["SpeechUnderstanding"],
		$struct["ConnectedSpeech"]
	);

	return new XML_RPC_Response(
		new XML_RPC_Value(true, "boolean")
	);
}

//==============Speech cards=============================================
function CreateSpeechCard($params)
{
	$struct = $params->getParam(0)->getval();

	DBCreateSpeechCard(
		$struct["IDchild"],
		$struct["DateOfExamination"],
		$struct["Lips"],
		$struct["Teeth"],
		$struct["Bite"],
		$struct["Tongue"],
		$struct["HyoidFrenulum"],
		$struct["Sky"],
		$struct["Salivation"],
		$struct["ComboBoxes"],
		$struct["SoundPronunciation"],
		$struct["SoundDifferentiation"],
		$struct["SyllableDifferentiation"],
		$struct["WordDifference"],
		$struct["SoundHighlight"]
	);

	return new XML_RPC_Response(
		new XML_RPC_Value(_DBInsertID(), "int")
	);
}
function ListSpeechCards()
{
	$speechcards = DBListSpeechCards();
	//Приведение значений, полученных из БД к их правильным типам данных.
	//По умолчанию все значения, полученные из БД имеют тип string.
	foreach ($speechcards as $k => $speechcard) {
		$speechcards[$k]["ID"] = (int)$speechcard["ID"];
		$speechcards[$k]["IDchild"] = (int)$speechcard["IDchild"];
		$speechcards[$k]["FIO"] = $speechcard["FIO"];
	}

	return new XML_RPC_Response(
		XML_RPC_encode($speechcards)
	);
}
function ListSpeechCardsUser($params)
{
	$IDlog = $params->getParam(0)->getval();
	$speechcards = DBListSpeechCardsUser($IDlog);
	//Приведение значений, полученных из БД к их правильным типам данных.
	//По умолчанию все значения, полученные из БД имеют тип string.
	foreach ($speechcards as $k => $speechcard) {
		$speechcards[$k]["ID"] = (int)$speechcard["ID"];
		$speechcards[$k]["IDchild"] = (int)$speechcard["IDchild"];
		$speechcards[$k]["FIO"] = $speechcard["FIO"];
	}

	return new XML_RPC_Response(
		XML_RPC_encode($speechcards)
	);
}
function ReadSpeechCard($params)
{
	$id = $params->getParam(0)->getval();

	$speechcard = DBReadSpeechCard($id);

	$speechcard["ID"] = (int)$speechcard["ID"];
	$speechcard["IDchild"] = (int)$speechcard["IDchild"];
	$speechcard["DateOfExamination"] = $speechcard["DateOfExamination"];
	$speechcard["Lips"] = $speechcard["Lips"];
	$speechcard["Teeth"] = $speechcard["Teeth"];
	$speechcard["Bite"] = $speechcard["Bite"];
	$speechcard["Tongue"] = $speechcard["Tongue"];
	$speechcard["HyoidFrenulum"] = $speechcard["HyoidFrenulum"];
	$speechcard["Sky"] = $speechcard["Sky"];
	$speechcard["Salivation"] = $speechcard["Salivation"];
	$speechcard["ComboBoxes"] = $speechcard["ComboBoxes"];
	$speechcard["SoundPronunciation"] = $speechcard["SoundPronunciation"];
	$speechcard["SoundDifferentiation"] = $speechcard["SoundDifferentiation"];
	$speechcard["SyllableDifferentiation"] = $speechcard["SyllableDifferentiation"];
	$speechcard["WordDifference"] = $speechcard["WordDifference"];
	$speechcard["SoundHighlight"] = $speechcard["SoundHighlight"];

	return new XML_RPC_Response(
		XML_RPC_encode(
			$speechcard
		)
	);
}
function UpdateSpeechCard($params)
{
	$id = $params->getParam(0)->getval();
	$struct = $params->getParam(1)->getval();
	DBUpdateSpeechCard(
		$id,
		$struct["IDchild"],
		$struct["DateOfExamination"],
		$struct["Lips"],
		$struct["Teeth"],
		$struct["Bite"],
		$struct["Tongue"],
		$struct["HyoidFrenulum"],
		$struct["Sky"],
		$struct["Salivation"],
		$struct["ComboBoxes"],
		$struct["SoundPronunciation"],
		$struct["SoundDifferentiation"],
		$struct["SyllableDifferentiation"],
		$struct["WordDifference"],
		$struct["SoundHighlight"]
	);

	return new XML_RPC_Response(
		new XML_RPC_Value(true, "boolean")
	);
}
function DeleteSpeechCard($params)
{
	$id = $params->getParam(0)->getval();
	DBDeleteSpeechCard($id);
	return new XML_RPC_Response(
		new XML_RPC_Value(true, "boolean")
	);
}

//==============Speech cards=============================================
function CreateIndividPlan($params)
{
	$struct = $params->getParam(0)->getval();

	DBCreateIndividPlan(
		$struct["SettingSounds"],
		$struct["SoundDifferentiation"],
		$struct["VocabularyEnrichment"],
		$struct["DevelopmentGrammatical"],
		$struct["FormationCoherentSpeech"],
		$struct["IDchild"]
	);

	return new XML_RPC_Response(
		new XML_RPC_Value(_DBInsertID(), "int")
	);
}
function ListIndividPlans()
{
	$individplans = DBListIndividPlans();
	//Приведение значений, полученных из БД к их правильным типам данных.
	//По умолчанию все значения, полученные из БД имеют тип string.
	foreach ($individplans as $k => $individplan) {
		$individplans[$k]["ID"] = (int)$individplan["ID"];
		$individplans[$k]["IDchild"] = (int)$individplan["IDchild"];
		$individplans[$k]["FIO"] = $individplan["FIO"];
	}

	return new XML_RPC_Response(
		XML_RPC_encode($individplans)
	);
}
function ListIndividPlansUser($params)
{
	$IDlog = $params->getParam(0)->getval();
	$individplans = DBListIndividPlansUser($IDlog);
	//Приведение значений, полученных из БД к их правильным типам данных.
	//По умолчанию все значения, полученные из БД имеют тип string.
	foreach ($individplans as $k => $individplan) {
		$individplans[$k]["ID"] = (int)$individplan["ID"];
		$individplans[$k]["IDchild"] = (int)$individplan["IDchild"];
		$individplans[$k]["FIO"] = $individplan["FIO"];
	}

	return new XML_RPC_Response(
		XML_RPC_encode($individplans)
	);
}
function ReadIndividPlan($params)
{
	$id = $params->getParam(0)->getval();

	$individplan = DBReadIndividPlan($id);

	$individplan["ID"] = (int)$individplan["ID"];
	$individplan["SettingSounds"] = $individplan["SettingSounds"];
	$individplan["SoundDifferentiation"] = $individplan["SoundDifferentiation"];
	$individplan["VocabularyEnrichment"] = $individplan["VocabularyEnrichment"];
	$individplan["DevelopmentGrammatical"] = $individplan["DevelopmentGrammatical"];
	$individplan["FormationCoherentSpeech"] = $individplan["FormationCoherentSpeech"];
	$individplan["IDchild"] = (int)$individplan["IDchild"];

	return new XML_RPC_Response(
		XML_RPC_encode(
			$individplan
		)
	);
}
function UpdateIndividPlan($params)
{
	$id = $params->getParam(0)->getval();
	$struct = $params->getParam(1)->getval();
	DBUpdateIndividPlan(
		$id,
		$struct["SettingSounds"],
		$struct["SoundDifferentiation"],
		$struct["VocabularyEnrichment"],
		$struct["DevelopmentGrammatical"],
		$struct["FormationCoherentSpeech"],
		$struct["IDchild"]
	);

	return new XML_RPC_Response(
		new XML_RPC_Value(true, "boolean")
	);
}
function DeleteIndividPlan($params)
{
	$id = $params->getParam(0)->getval();
	DBDeleteIndividPlan($id);
	return new XML_RPC_Response(
		new XML_RPC_Value(true, "boolean")
	);
}
// ===== Diagrams ======================================================================
function GetDiagnosticPointsDiagram($params)
{
	$IDchild = $params->getParam(0)->getval();
	$StartEnd = $params->getParam(1)->getval();

	$DiagnosticPoints = DBGetDiagnosticPointsDiagram($IDchild,$StartEnd);

	$DiagnosticPoints["ID"] = (int)$DiagnosticPoints["ID"];
	$DiagnosticPoints["IDdiagn"] = (int)$DiagnosticPoints["IDdiagn"];
	$DiagnosticPoints["StartEnd"] = $DiagnosticPoints["StartEnd"];
	$DiagnosticPoints["SoundPronunciation"] = (int)$DiagnosticPoints["SoundPronunciation"];
	$DiagnosticPoints["SyllabicStructure"] = (int)$DiagnosticPoints["SyllabicStructure"];
	$DiagnosticPoints["PhonemicRepresentations"] = (int)$DiagnosticPoints["PhonemicRepresentations"];
	$DiagnosticPoints["Grammar"] = (int)$DiagnosticPoints["Grammar"];
	$DiagnosticPoints["LexicalStock"] = (int)$DiagnosticPoints["LexicalStock"];
	$DiagnosticPoints["SpeechUnderstanding"] = (int)$DiagnosticPoints["SpeechUnderstanding"];
	$DiagnosticPoints["ConnectedSpeech"] = (int)$DiagnosticPoints["ConnectedSpeech"];

	return new XML_RPC_Response(
		XML_RPC_encode(
			$DiagnosticPoints
		)
	);
}
// Средние знач. в начале/конце года
function GetDiagnosticPointsAvgDiagram($params)
{
	$StartEnd = $params->getParam(0)->getval();

	$DiagnosticPoints = DBGetDiagnosticPointsAvgDiagram($StartEnd);

	$DiagnosticPoints["AvgSoundPronunciation"] = (double)$DiagnosticPoints["AvgSoundPronunciation"];
	$DiagnosticPoints["AvgSyllabicStructure"] = (double)$DiagnosticPoints["AvgSyllabicStructure"];
	$DiagnosticPoints["AvgPhonemicRepresentations"] = (double)$DiagnosticPoints["AvgPhonemicRepresentations"];
	$DiagnosticPoints["AvgGrammar"] = (double)$DiagnosticPoints["AvgGrammar"];
	$DiagnosticPoints["AvgLexicalStock"] = (double)$DiagnosticPoints["AvgLexicalStock"];
	$DiagnosticPoints["AvgSpeechUnderstanding"] = (double)$DiagnosticPoints["AvgSpeechUnderstanding"];
	$DiagnosticPoints["AvgConnectedSpeech"] = (double)$DiagnosticPoints["AvgConnectedSpeech"];

	return new XML_RPC_Response(
		XML_RPC_encode(
			$DiagnosticPoints
		)
	);
}







// =====Запросы для отчётов======================================================================
// Количество детей с диагнозом НПОЗ/ФФНР 2/3
function GetCountNPOZ_FFNR($params)
{
	$IDvioal1 = $params->getParam(0)->getval();

	$Diagnostic = DBGetCountNPOZ_FFNR($IDvioal1);

	$Diagnostic["Count"] = (int)$Diagnostic["Count"];

	return new XML_RPC_Response(
		XML_RPC_encode(
			$Diagnostic
		)
	);
}
// Количество детей с диагнозами ОНРы
function GetCountONRs()
{
	$Diagnostic = DBGetCountONRs();

	$Diagnostic["Count"] = (int)$Diagnostic["Count"];

	return new XML_RPC_Response(
		XML_RPC_encode(
			$Diagnostic
		)
	);
}
// Количество детей зачисленных к логопеду с НПОЗ/ФФНР 2/3
function GetCountInLogocentreNPOZ_FFNR($params)
{
	$IDvioal1 = $params->getParam(0)->getval();

	$Diagnostic = DBGetCountInLogocentreNPOZ_FFNR($IDvioal1);

	$Diagnostic["Count"] = (int)$Diagnostic["Count"];

	return new XML_RPC_Response(
		XML_RPC_encode(
			$Diagnostic
		)
	);
}
// Количество детей зачисленных к логопеду с ОНР
function GetCountInLogocentreONRs()
{
	$Diagnostic = DBGetCountInLogocentreONRs();

	$Diagnostic["Count"] = (int)$Diagnostic["Count"];

	return new XML_RPC_Response(
		XML_RPC_encode(
			$Diagnostic
		)
	);
}
// Количество выведенных детей (ВСЕХ)
function GetCountReleas()
{
	$Diagnostic = DBGetCountReleas();

	$Diagnostic["Count"] = (int)$Diagnostic["Count"];

	return new XML_RPC_Response(
		XML_RPC_encode(
			$Diagnostic
		)
	);
}
// // Количество выведенных в школу детей
function GetCountReleasInSchool()
{
	$Diagnostic = DBGetCountReleasInSchool();

	$Diagnostic["Count"] = (int)$Diagnostic["Count"];

	return new XML_RPC_Response(
		XML_RPC_encode(
			$Diagnostic
		)
	);
}
// Количество детей, нуждающихся в продолжении занятий в школе
function GetCountSchoolLogocentre()
{
	$Diagnostic = DBGetCountSchoolLogocentre();

	$Diagnostic["Count"] = (int)$Diagnostic["Count"];

	return new XML_RPC_Response(
		XML_RPC_encode(
			$Diagnostic
		)
	);
}
// Количество детей, направленных в спец. учреждение
function GetCountSpecialInstitution()
{
	$Diagnostic = DBGetCountSpecialInstitution();

	$Diagnostic["Count"] = (int)$Diagnostic["Count"];

	return new XML_RPC_Response(
		XML_RPC_encode(
			$Diagnostic
		)
	);
}
// Количество детей, выбывших по др. причинам ОБЩЕЕ
function GetCountReleasOther()
{
	$Diagnostic = DBGetCountReleasOther();

	$Diagnostic["Count"] = (int)$Diagnostic["Count"];

	return new XML_RPC_Response(
		XML_RPC_encode(
			$Diagnostic
		)
	);
}



// Сопоставление имён методов в коде тем именам, которые будут видны клиенту,
// другими словами проектирование пользовательского интерфейса пользователя
$map = array(
	//================= Логопеды ==============================
	"myservice:CreateLogoped" => array(
		"function" => "CreateLogoped",
		"signature" => array(
			array("int", "struct")
		)
	),
	"myservice:ListLogopeds" => array(
		"function" => "ListLogopeds",
		"signature" => array(
			array("array")
		)
	),
	"myservice:ListFioLogopeds" => array(
		"function" => "ListFioLogopeds",
		"signature" => array(
			array("array")
		)
	),
	"myservice:ReadLogoped" => array(
		"function" => "ReadLogoped",
		"signature" => array(
			array("struct", "int")
		)
	),
	"myservice:UpdateLogoped" => array(
		"function" => "UpdateLogoped",
		"signature" => array(
			array("boolean", "int", "struct")
		)
	),
	"myservice:DeleteLogoped" => array(
		"function" => "DeleteLogoped",
		"signature" => array(
			array("boolean", "int")
		)
	),
	//================= Группы ==============================
	"myservice:CreateGroup" => array(
		"function" => "CreateGroup",
		"signature" => array(
			array("int", "struct")
		)
	),
	"myservice:ListGroups" => array(
		"function" => "ListGroups",
		"signature" => array(
			array("array")
		)
	),
	"myservice:ListGroupsUser" => array(
		"function" => "ListGroupsUser",
		"signature" => array(
			array("array", "int")/////////////////XML-RPC с параметром/////////////////////////////////////////////////////////////////////////
		)
	),
	"myservice:ReadGroup" => array(
		"function" => "ReadGroup",
		"signature" => array(
			array("struct", "int")
		)
	),
	"myservice:UpdateGroup" => array(
		"function" => "UpdateGroup",
		"signature" => array(
			array("boolean", "int", "struct")
		)
	),
	"myservice:DeleteGroup" => array(
		"function" => "DeleteGroup",
		"signature" => array(
			array("boolean", "int")
		)
	),
	// ============== Children =============================
	"myservice:CreateChild" => array(
		"function" => "CreateChild",
		"signature" => array(
			array("int", "struct")
		)
	),
	"myservice:ListChildren" => array(
		"function" => "ListChildren",
		"signature" => array(
			array("array")
		)
	),
	"myservice:ListChildrenInGroup" => array(
		"function" => "ListChildrenInGroup",
		"signature" => array(
			array("array", "int")/////////////////XML-RPC с параметром/////////////////////////////////////////////////////////////////////////
		)
	),
	"myservice:ListChildrenInGroupLogopunct" => array(
		"function" => "ListChildrenInGroupLogopunct",
		"signature" => array(
			array("array", "int")/////////////////XML-RPC с параметром/////////////////////////////////////////////////////////////////////////
		)
	),
	"myservice:ReadChild" => array(
		"function" => "ReadChild",
		"signature" => array(
			array("struct", "int")
		)
	),
	"myservice:UpdateChild" => array(
		"function" => "UpdateChild",
		"signature" => array(
			array("boolean", "int", "struct")
		)
	),
	"myservice:DeleteChild" => array(
		"function" => "DeleteChild",
		"signature" => array(
			array("boolean", "int")
		)
	),
// ============== Diagnostics =============================
	"myservice:CreateDiagnostic" => array(
		"function" => "CreateDiagnostic",
		"signature" => array(
			array("int", "struct")
		)
	),
	"myservice:ListDiagnostics" => array(
		"function" => "ListDiagnostics",
		"signature" => array(
			array("array")
		)
	),
	"myservice:ListDiagnosticsUser" => array(
		"function" => "ListDiagnosticsUser",
		"signature" => array(
			array("array", "int")/////////////////XML-RPC с параметром/////////////////////////////////////////////////////////////////////////
		)
	),
	"myservice:ReadDiagnostic" => array(
		"function" => "ReadDiagnostic",
		"signature" => array(
			array("struct", "int")
		)
	),
	"myservice:ReadDiagnosticIDchild" => array(
		"function" => "ReadDiagnosticIDchild",
		"signature" => array(
			array("struct", "int")
		)
	),
	"myservice:UpdateDiagnostic" => array(
		"function" => "UpdateDiagnostic",
		"signature" => array(
			array("boolean", "int", "struct")
		)
	),
	"myservice:DeleteDiagnostic" => array(
		"function" => "DeleteDiagnostic",
		"signature" => array(
			array("boolean", "int")
		)
	),
	// ============== DiagnosticPoints =============================
	"myservice:CreateDiagnosticPoints" => array(
		"function" => "CreateDiagnosticPoints",
		"signature" => array(
			array("int", "struct")
		)
	),
	"myservice:GetDiagnosticPoints" => array(
		"function" => "GetDiagnosticPoints",
		"signature" => array(
			array("struct", "int", "string")
		)
	),
	"myservice:UpdateDiagnosticPoints" => array(
		"function" => "UpdateDiagnosticPoints",
		"signature" => array(
			array("boolean", "int", "struct")
		)
	),
	//==============Speech cards=============================================
	"myservice:CreateSpeechCard" => array(
		"function" => "CreateSpeechCard",
		"signature" => array(
			array("int", "struct")
		)
	),
	"myservice:ListSpeechCards" => array(
		"function" => "ListSpeechCards",
		"signature" => array(
			array("array")
		)
	),
	"myservice:ListSpeechCardsUser" => array(
		"function" => "ListSpeechCardsUser",
		"signature" => array(
			array("array", "int")/////////////////XML-RPC с параметром/////////////////////////////////////////////////////////////////////////
		)
	),
	"myservice:ReadSpeechCard" => array(
		"function" => "ReadSpeechCard",
		"signature" => array(
			array("struct", "int")
		)
	),
	"myservice:UpdateSpeechCard" => array(
		"function" => "UpdateSpeechCard",
		"signature" => array(
			array("boolean", "int", "struct")
		)
	),
	"myservice:DeleteSpeechCard" => array(
		"function" => "DeleteSpeechCard",
		"signature" => array(
			array("boolean", "int")
		)
	),
	//============== IndividPlan =============================================
	"myservice:CreateIndividPlan" => array(
		"function" => "CreateIndividPlan",
		"signature" => array(
			array("int", "struct")
		)
	),
	"myservice:ListIndividPlans" => array(
		"function" => "ListIndividPlans",
		"signature" => array(
			array("array")
		)
	),
	"myservice:ListIndividPlansUser" => array(
		"function" => "ListIndividPlansUser",
		"signature" => array(
			array("array", "int")/////////////////XML-RPC с параметром/////////////////////////////////////////////////////////////////////////
		)
	),
	"myservice:ReadIndividPlan" => array(
		"function" => "ReadIndividPlan",
		"signature" => array(
			array("struct", "int")
		)
	),
	"myservice:UpdateIndividPlan" => array(
		"function" => "UpdateIndividPlan",
		"signature" => array(
			array("boolean", "int", "struct")
		)
	),
	"myservice:DeleteIndividPlan" => array(
		"function" => "DeleteIndividPlan",
		"signature" => array(
			array("boolean", "int")
		)
	),
	// ============ Diagrams ===============================
	"myservice:GetDiagnosticPointsDiagram" => array(
		"function" => "GetDiagnosticPointsDiagram",
		"signature" => array(
			array("struct", "int", "string")
		)
	),
	"myservice:GetDiagnosticPointsAvgDiagram" => array(
		"function" => "GetDiagnosticPointsAvgDiagram",
		"signature" => array(
			array("struct", "string")
		)
	),
	// =====Запросы для отчётов======================================================================
	"myservice:GetCountNPOZ_FFNR" => array(
		"function" => "GetCountNPOZ_FFNR",
		"signature" => array(
			array("struct", "int")
		)
	),
	"myservice:GetCountONRs" => array(
		"function" => "GetCountONRs",
		"signature" => array(
			array("struct")
		)
	),
	"myservice:GetCountInLogocentreNPOZ_FFNR" => array(
		"function" => "GetCountInLogocentreNPOZ_FFNR",
		"signature" => array(
			array("struct", "int")
		)
	),
	"myservice:GetCountInLogocentreONRs" => array(
		"function" => "GetCountInLogocentreONRs",
		"signature" => array(
			array("struct")
		)
	),
	"myservice:GetCountReleas" => array(
		"function" => "GetCountReleas",
		"signature" => array(
			array("struct")
		)
	),
	"myservice:GetCountReleasInSchool" => array(
		"function" => "GetCountReleasInSchool",
		"signature" => array(
			array("struct")
		)
	),
	"myservice:GetCountSchoolLogocentre" => array(
		"function" => "GetCountSchoolLogocentre",
		"signature" => array(
			array("struct")
		)
	),
	"myservice:GetCountSpecialInstitution" => array(
		"function" => "GetCountSpecialInstitution",
		"signature" => array(
			array("struct")
		)
	),
	"myservice:GetCountReleasOther" => array(
		"function" => "GetCountReleasOther",
		"signature" => array(
			array("struct")
		)
	)


);

$srv = new XML_RPC_Server($map, 1, 1);
