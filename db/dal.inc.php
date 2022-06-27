<?php 
require_once("$_SERVER[DOCUMENT_ROOT]/../db/common.dal.inc.php");

function DBCreateLogoped($FIO, $Log1n, $Pass) {
	_DBQuery(
		"INSERT INTO logoped(FIO, Log1n, Pass) VALUES('$FIO','$Log1n','$Pass')"
	);
}

function DBListLogopeds() {
	return _DBListQuery("SELECT * FROM logoped");
}

function DBListFioLogopeds() {
	return _DBListQuery("SELECT ID, FIO FROM logoped WHERE ID NOT IN (1)");
}

function DBReadLogoped($id) {
	return _DBGetQuery("
		SELECT 
			logoped.ID As ID,
			logoped.FIO As FIO,
			logoped.Log1n As Log1n,
			logoped.Pass As Pass
		FROM 
			logoped
		WHERE 
			ID=$id
	");
}

function DBUpdateLogoped($id,$FIO,$Log1n,$Pass) {	
	_DBQuery("
		UPDATE logoped 
		SET 
			FIO='$FIO',
			Log1n='$Log1n',
			Pass='$Pass'
		WHERE ID=$id
	");
}

function DBDeleteLogoped($id) {	
	_DBQuery("DELETE FROM logoped WHERE id=$id"); 	
}
// ==================Group=========================
function DBCreateGroup($NumberGr,$IDlog) {
	_DBQuery(
		"INSERT INTO gruppa(NumberGr,IDlog) VALUES('$NumberGr','$IDlog')"
	);
}
function DBListGroups() {
	return _DBListQuery("
		SELECT 
			gruppa.IDgr As IDgr,
			gruppa.NumberGr As NumberGr,
			gruppa.IDlog As IDlog,
			logoped.FIO As Logoped
		FROM 
			gruppa,logoped
		WHERE 
			gruppa.IDlog=logoped.ID
		ORDER BY NumberGr
	");
}
function DBListGroupsUser($IDlog) {
	return _DBListQuery("
		SELECT 
			gruppa.IDgr As IDgr,
			gruppa.NumberGr As NumberGr,
			gruppa.IDlog As IDlog,
			logoped.FIO As Logoped
		FROM 
			gruppa,logoped
		WHERE 
			gruppa.IDlog=logoped.ID and logoped.ID=$IDlog
		ORDER BY NumberGr
	");
}

function DBReadGroup($id) {
	return _DBGetQuery("
		SELECT 
			gruppa.IDgr As IDgr,
			gruppa.NumberGr As NumberGr,
			gruppa.IDlog As IDlog
		FROM 
			gruppa
		WHERE 
			IDgr=$id
	");
}
function DBUpdateGroup($id,$NumberGr,$IDlog) {	
	_DBQuery("
		UPDATE gruppa 
		SET 
			NumberGr='$NumberGr',
			IDlog='$IDlog'
		WHERE IDgr=$id
	");
}

function DBDeleteGroup($id) {	
	_DBQuery("DELETE FROM gruppa WHERE IDgr=$id"); 	
}
// ==================Children=========================
function DBCreateChild($FIO,$DateB,$FIOMam,$TelMam,$FioPap,$TelPap,$Email,$IDgr) {
	_DBQuery(
		"INSERT INTO children(FIO,DateB,FIOMam,TelMam,FioPap,TelPap,Email,IDgr)
		VALUES('$FIO','$DateB','$FIOMam','$TelMam','$FioPap','$TelPap','$Email','$IDgr')"
	);
}
// Список всех детей
function DBListChildren() {
	return _DBListQuery("
		SELECT 
			children.ID As ID,
			children.FIO As FIO,
			children.DateB As DateB,
			children.FIOMam As FIOMam,
			children.TelMam As TelMam,
			children.FioPap As FioPap,
			children.TelPap As TelPap,
			children.Email As Email,
			children.IDgr As IDgr,
			gruppa.NumberGr As NumberGr
		FROM 
			children,gruppa
		WHERE 
			children.IDgr=gruppa.IDgr
		ORDER BY FIO
	");
}
// Список детей из определённой группы
function DBListChildrenInGroup($IDgr) {
	return _DBListQuery("
		SELECT 
			children.ID As ID,
			children.FIO As FIO,
			children.IDgr As IDgr
		FROM 
			children
		WHERE 
			IDgr=$IDgr
		ORDER BY FIO
	");
}
// Список детей из определённой группы и проходил диагностику и зачислен в логопункт
function DBListChildrenInGroupLogopunct($IDgr) {
	return _DBListQuery("
		SELECT 
			children.ID As ID,
			children.FIO As FIO,
			children.IDgr As IDgr
		FROM 
			children,diagnostics
		WHERE 
			IDgr=$IDgr and children.ID = diagnostics.IDchild and EnrollmentInLogocentre = true
		ORDER BY FIO
	");
}
function DBReadChild($id) {
	return _DBGetQuery("
		SELECT 
			children.ID As ID,
			children.FIO As FIO,
			children.DateB As DateB,
			children.FIOMam As FIOMam,
			children.TelMam As TelMam,
			children.FioPap As FioPap,
			children.TelPap As TelPap,
			children.Email As Email,
			children.IDgr As IDgr
		FROM 
			children
		WHERE 
			ID=$id
	");
}
function DBUpdateChild($id,$FIO,$DateB,$FIOMam,$TelMam,$FioPap,$TelPap,$Email,$IDgr) {	
	_DBQuery("
		UPDATE children 
		SET 
			FIO='$FIO',
			DateB='$DateB',
			FIOMam='$FIOMam',
			TelMam='$TelMam',
			FioPap='$FioPap',
			TelPap='$TelPap',
			Email='$Email',
			IDgr='$IDgr'
		WHERE ID=$id
	");
}

function DBDeleteChild($id) {	
	_DBQuery("DELETE FROM children WHERE ID=$id"); 	
}

// ==================Diagnostics =========================
function DBCreateDiagnostic($IDchild,$ItogScore1,$IDvioal1,$ItogScore2,$IDvioal2,$NeedsHelp,$SpecialInstitution,$EnrollmentInLogocentre,
$DateEnrollment,$Releas,$ReleasInSchool,$ReleasOther,$DateReleas,$SchoolLogocentre) {
	// require_once("$_SERVER[DOCUMENT_ROOT]/../db/dbinit.inc.php");
	// global $cms_db_link;
	_DBQuery(
		"INSERT INTO diagnostics(IDchild,ItogScore1,IDvioal1,ItogScore2,IDvioal2,NeedsHelp,SpecialInstitution,EnrollmentInLogocentre,
		DateEnrollment,Releas,ReleasInSchool,ReleasOther,DateReleas,SchoolLogocentre)
		VALUES('$IDchild','$ItogScore1','$IDvioal1','$ItogScore2','$IDvioal2','$NeedsHelp','$SpecialInstitution','$EnrollmentInLogocentre',
		'$DateEnrollment','$Releas','$ReleasInSchool','$ReleasOther','$DateReleas','$SchoolLogocentre')"
	);
	// $link=@mysqli_connect("localhost","root","");
	//return mysqli_insert_id($cms_db_link);
}

//mysql_insert_id()
//select last_insert_id();

// function DBGetMaxIdDiagnostics()
// {
// 	return _DBGetQuery("
// 		SELECT 
// 			MAX(id) AS ID
// 		FROM 
// 			diagnostics
// 	");
// }


function DBListDiagnostics() {
	return _DBListQuery("
		SELECT 
			diagnostics.ID As ID,
			diagnostics.IDchild As IDchild,
			children.FIO As FIOchild,
			diagnostics.ItogScore1 As ItogScore1,
			diagnostics.IDvioal1 As IDvioal1,
			violations.Name As Violation1,
			diagnostics.ItogScore2 As ItogScore2,
			diagnostics.IDvioal2 As IDvioal2,
			violations.Name As Violation2,
			diagnostics.NeedsHelp As NeedsHelp,
			diagnostics.SpecialInstitution As SpecialInstitution,
			diagnostics.EnrollmentInLogocentre As EnrollmentInLogocentre,
			diagnostics.DateEnrollment As DateEnrollment,
			diagnostics.Releas As Releas,
			diagnostics.ReleasInSchool As ReleasInSchool,
			diagnostics.ReleasOther As ReleasOther,
			diagnostics.DateReleas As DateReleas,
			diagnostics.SchoolLogocentre As SchoolLogocentre
		FROM 
			children
			JOIN violations
			LEFT JOIN diagnostics ON diagnostics.IDchild = children.ID
			AND diagnostics.IDvioal1 = violations.ID
		WHERE 
			diagnostics.IDchild=children.ID and diagnostics.IDvioal1=violations.ID
		ORDER BY FIOchild
	");
}

function DBListDiagnosticsUser($IDlog) {
	return _DBListQuery("
		SELECT 
			diagnostics.ID As ID,
			diagnostics.IDchild As IDchild,
			children.FIO As FIOchild,
			diagnostics.ItogScore1 As ItogScore1,
			diagnostics.IDvioal1 As IDvioal1,
			violations.Name As Violation1,
			diagnostics.ItogScore2 As ItogScore2,
			diagnostics.IDvioal2 As IDvioal2,
			violations.Name As Violation2,
			diagnostics.NeedsHelp As NeedsHelp,
			diagnostics.SpecialInstitution As SpecialInstitution,
			diagnostics.EnrollmentInLogocentre As EnrollmentInLogocentre,
			diagnostics.DateEnrollment As DateEnrollment,
			diagnostics.Releas As Releas,
			diagnostics.ReleasInSchool As ReleasInSchool,
			diagnostics.ReleasOther As ReleasOther,
			diagnostics.DateReleas As DateReleas,
			diagnostics.SchoolLogocentre As SchoolLogocentre
		FROM 
			children
			JOIN violations
			LEFT JOIN diagnostics ON diagnostics.IDchild = children.ID
			AND diagnostics.IDvioal1 = violations.ID
		WHERE 
			diagnostics.IDchild=children.ID and diagnostics.IDvioal1=violations.ID and children.IDgr in
				(SELECT 
					gruppa.IDgr As IDgr
				FROM 
						gruppa
				WHERE 
						gruppa.IDlog=$IDlog)
		ORDER BY FIOchild
	");
}

function DBReadDiagnostic($id) {
	return _DBGetQuery("
		SELECT 
			diagnostics.ID As ID,
			diagnostics.IDchild As IDchild,
			diagnostics.ItogScore1 As ItogScore1,
			diagnostics.IDvioal1 As IDvioal1,
			diagnostics.ItogScore2 As ItogScore2,
			diagnostics.IDvioal2 As IDvioal2,
			diagnostics.NeedsHelp As NeedsHelp,
			diagnostics.SpecialInstitution As SpecialInstitution,
			diagnostics.EnrollmentInLogocentre As EnrollmentInLogocentre,
			diagnostics.DateEnrollment As DateEnrollment,
			diagnostics.Releas As Releas,
			diagnostics.ReleasInSchool As ReleasInSchool,
			diagnostics.ReleasOther As ReleasOther,
			diagnostics.DateReleas As DateReleas,
			diagnostics.SchoolLogocentre As SchoolLogocentre
		FROM 
			diagnostics
		WHERE 
			ID=$id
	");
}

function DBReadDiagnosticIDchild($idchild) {
	return _DBGetQuery("
		SELECT 
			diagnostics.IDchild As IDchild,
			diagnostics.IDvioal1 As IDvioal1,
			diagnostics.IDvioal2 As IDvioal2
		FROM 
			diagnostics
		WHERE 
			IDchild=$idchild
	");
}

function DBUpdateDiagnostic($id,$IDchild,$ItogScore1,$IDvioal1,$ItogScore2,$IDvioal2,$NeedsHelp,$SpecialInstitution,$EnrollmentInLogocentre,
$DateEnrollment,$Releas,$ReleasInSchool,$ReleasOther,$DateReleas,$SchoolLogocentre) {	
	_DBQuery("
		UPDATE diagnostics 
		SET 
			IDchild='$IDchild',
			ItogScore1='$ItogScore1',
			IDvioal1='$IDvioal1',
			ItogScore2='$ItogScore2',
			IDvioal2='$IDvioal2',
			NeedsHelp='$NeedsHelp',
			SpecialInstitution='$SpecialInstitution',
			EnrollmentInLogocentre='$EnrollmentInLogocentre',
			DateEnrollment='$DateEnrollment',
			Releas='$Releas',
			ReleasInSchool='$ReleasInSchool',
			ReleasOther='$ReleasOther',
			DateReleas='$DateReleas',
			SchoolLogocentre='$SchoolLogocentre'
		WHERE ID=$id
	");
}

function DBDeleteDiagnostic($id) {	
	_DBQuery("DELETE FROM diagnostics WHERE ID=$id"); 	
}


//=======diagnosticpoints=================================================
function DBCreateDiagnosticPoints($IDdiagn,$StartEnd,$SoundPronunciation,$SyllabicStructure,$PhonemicRepresentations,$Grammar,$LexicalStock,$SpeechUnderstanding,$ConnectedSpeech) {
	_DBQuery(
		"INSERT INTO diagnosticpoints(IDdiagn,StartEnd,SoundPronunciation,SyllabicStructure,PhonemicRepresentations,Grammar,LexicalStock,SpeechUnderstanding,ConnectedSpeech)
		VALUES('$IDdiagn ','$StartEnd','$SoundPronunciation','$SyllabicStructure','$PhonemicRepresentations','$Grammar','$LexicalStock','$SpeechUnderstanding','$ConnectedSpeech')"
	);
}
// Баллы в начале года
function DBGetDiagnosticPoints($IDdiagn,$StartEnd) {
	return _DBGetQuery(
	 	"SELECT * FROM diagnosticpoints WHERE IDdiagn='$IDdiagn' AND StartEnd='$StartEnd'"
	);
}
// // Баллы в конце года // МОжно сразу передавать второй параметр и эта функция будет не нужна
// function DBGetDiagnosticPointsEnd2($ID) {
// 	return _DBGetQuery(
// 	 	"SELECT * FROM diagnosticpoints WHERE ID='$ID'"
// 	);
// }
// Диаграммы///////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Баллы в начале года
function DBGetDiagnosticPointsDiagram($IDchild,$StartEnd) {
	return _DBGetQuery(
	 	"SELECT * FROM diagnosticpoints WHERE StartEnd='$StartEnd' AND IDdiagn=(SELECT ID FROM diagnostics WHERE IDchild='$IDchild')"
	);
}

// Средние знач. в начале/конце года
function DBGetDiagnosticPointsAvgDiagram($StartEnd) {
	return _DBGetQuery(
		"SELECT avg(SoundPronunciation) as 'AvgSoundPronunciation', avg(SyllabicStructure) as 'AvgSyllabicStructure',
		avg(PhonemicRepresentations) as 'AvgPhonemicRepresentations', avg(Grammar) as 'AvgGrammar',
		avg(LexicalStock) as 'AvgLexicalStock', avg(SpeechUnderstanding) as 'AvgSpeechUnderstanding',
		avg(ConnectedSpeech) as 'AvgConnectedSpeech' FROM diagnosticpoints WHERE StartEnd='$StartEnd'
		AND IDdiagn IN (SELECT ID FROM diagnostics WHERE EnrollmentInLogocentre=true)"
	);
}

function DBUpdateDiagnosticPoints($id,$IDdiagn,$StartEnd,$SoundPronunciation,$SyllabicStructure,$PhonemicRepresentations,$Grammar,$LexicalStock,$SpeechUnderstanding,$ConnectedSpeech) {	
	_DBQuery("
		UPDATE diagnosticpoints
		SET 
			IDdiagn='$IDdiagn',
			StartEnd='$StartEnd',
			SoundPronunciation='$SoundPronunciation',
			SyllabicStructure='$SyllabicStructure',
			PhonemicRepresentations='$PhonemicRepresentations',
			Grammar='$Grammar',
			LexicalStock='$LexicalStock',
			SpeechUnderstanding='$SpeechUnderstanding',
			ConnectedSpeech='$ConnectedSpeech'
		WHERE ID=$id
	");
}
//==============Speech cards=============================================
function DBCreateSpeechCard($IDchild,$DateOfExamination,$Lips,$Teeth,$Bite,$Tongue,$HyoidFrenulum,$Sky,$Salivation,$ComboBoxes,$SoundPronunciation,
$SoundDifferentiation,$SyllableDifferentiation,$WordDifference,$SoundHighlight) {
	_DBQuery(
		"INSERT INTO speechcards(IDchild,DateOfExamination,Lips,Teeth,Bite,Tongue,HyoidFrenulum,Sky,Salivation,ComboBoxes,SoundPronunciation,
		SoundDifferentiation,SyllableDifferentiation,WordDifference,SoundHighlight) VALUES('$IDchild','$DateOfExamination','$Lips','$Teeth',
		'$Bite','$Tongue','$HyoidFrenulum','$Sky','$Salivation','$ComboBoxes','$SoundPronunciation','$SoundDifferentiation','$SyllableDifferentiation',
		'$WordDifference','$SoundHighlight')"
	);
}
function DBListSpeechCards() {
	return _DBListQuery("
		SELECT 
			speechcards.ID As ID,
			speechcards.IDchild As IDchild,
			children.FIO As FIO
		FROM 
			speechcards,children
		WHERE 
			speechcards.IDchild=children.ID
		ORDER BY FIO
	");
}
function DBListSpeechCardsUser($IDlog) {
	return _DBListQuery("
		SELECT 
			speechcards.ID As ID,
			speechcards.IDchild As IDchild,
			children.FIO As FIO
		FROM 
			speechcards,children,gruppa
		WHERE 
			speechcards.IDchild=children.ID and children.IDgr=gruppa.IDgr and gruppa.IDlog=$IDlog
	");
}
function DBReadSpeechCard($id) {
	return _DBGetQuery("
		SELECT 
			speechcards.ID As ID,
			speechcards.IDchild As IDchild,
			speechcards.DateOfExamination As DateOfExamination,
			speechcards.Lips As Lips,
			speechcards.Teeth As Teeth,
			speechcards.Bite As Bite,
			speechcards.Tongue As Tongue,
			speechcards.HyoidFrenulum As HyoidFrenulum,
			speechcards.Sky As Sky,
			speechcards.Salivation As Salivation,
			speechcards.ComboBoxes As ComboBoxes,
			speechcards.SoundPronunciation As SoundPronunciation,
			speechcards.SoundDifferentiation As SoundDifferentiation,
			speechcards.SyllableDifferentiation As SyllableDifferentiation,
			speechcards.WordDifference As WordDifference,
			speechcards.SoundHighlight As SoundHighlight
		FROM 
			speechcards
		WHERE 
			ID=$id
	");
}

function DBUpdateSpeechCard($id,$IDchild,$DateOfExamination,$Lips,$Teeth,$Bite,$Tongue,$HyoidFrenulum,$Sky,$Salivation,$ComboBoxes,$SoundPronunciation,
$SoundDifferentiation,$SyllableDifferentiation,$WordDifference,$SoundHighlight) {	
	_DBQuery("
		UPDATE speechcards
		SET 
			IDchild='$IDchild',
			DateOfExamination='$DateOfExamination',
			Lips='$Lips',
			Teeth='$Teeth',
			Bite='$Bite',
			Tongue='$Tongue',
			HyoidFrenulum='$HyoidFrenulum',
			Sky='$Sky',
			Salivation='$Salivation',
			ComboBoxes='$ComboBoxes',
			SoundPronunciation='$SoundPronunciation',
			SoundDifferentiation='$SoundDifferentiation',
			SyllableDifferentiation='$SyllableDifferentiation',
			WordDifference='$WordDifference',
			SoundHighlight='$SoundHighlight'
		WHERE ID=$id
	");
}

function DBDeleteSpeechCard($id) {	
	_DBQuery("DELETE FROM speechcards WHERE ID=$id"); 	
}
// ==================IndividualPlans=========================
function DBCreateIndividPlan($SettingSounds, $SoundDifferentiation, $VocabularyEnrichment, $DevelopmentGrammatical, $FormationCoherentSpeech, $IDchild) {
	_DBQuery(
		"INSERT INTO individualplans(SettingSounds, SoundDifferentiation, VocabularyEnrichment,DevelopmentGrammatical,FormationCoherentSpeech,IDchild)
		VALUES('$SettingSounds','$SoundDifferentiation','$VocabularyEnrichment','$DevelopmentGrammatical','$FormationCoherentSpeech','$IDchild')"
	);
}
function DBListIndividPlans() {
	return _DBListQuery("
		SELECT 
			individualplans.ID As ID,
			individualplans.IDchild As IDchild,
			children.FIO As FIO
		FROM 
			individualplans,children
		WHERE 
			individualplans.IDchild=children.ID
		ORDER BY FIO
	");
}
function DBListIndividPlansUser($IDlog) {
	return _DBListQuery("
		SELECT 
			individualplans.ID As ID,
			individualplans.IDchild As IDchild,
			children.FIO As FIO
		FROM 
			individualplans,children,gruppa
		WHERE 
			individualplans.IDchild=children.ID and children.IDgr=gruppa.IDgr and gruppa.IDlog=$IDlog
		ORDER BY FIO
	");
}
function DBReadIndividPlan($id) {
	return _DBGetQuery("
		SELECT 
			individualplans.ID As ID,
			individualplans.SettingSounds,
			individualplans.SoundDifferentiation,
			individualplans.VocabularyEnrichment,
			individualplans.DevelopmentGrammatical,
			individualplans.FormationCoherentSpeech,
			individualplans.IDchild As IDchild
		FROM 
			individualplans
		WHERE 
			ID=$id
	");
}
function DBUpdateIndividPlan($id,$SettingSounds, $SoundDifferentiation, $VocabularyEnrichment, $DevelopmentGrammatical, $FormationCoherentSpeech, $IDchild) {	
	_DBQuery("
		UPDATE individualplans
		SET 
			SettingSounds='$SettingSounds',
			SoundDifferentiation='$SoundDifferentiation',
			VocabularyEnrichment='$VocabularyEnrichment',
			DevelopmentGrammatical='$DevelopmentGrammatical',
			FormationCoherentSpeech='$FormationCoherentSpeech',
			IDchild='$IDchild'
		WHERE ID=$id
	");
}

function DBDeleteIndividPlan($id) {	
	_DBQuery("DELETE FROM individualplans WHERE ID=$id"); 	
}
// =====Запросы для отчётов======================================================================
// Количество детей с диагнозом НПОЗ/ФФНР 2/3
function DBGetCountNPOZ_FFNR($IDvioal1) {
	return _DBGetQuery("
		SELECT 
			Count(*) As Count
		FROM 
			diagnostics 
		WHERE
			IDvioal1 = $IDvioal1
	");
}
// Количество детей с диагнозами ОНР
function DBGetCountONRs() {
	return _DBGetQuery("
		SELECT 
			Count(*) As Count
		FROM 
			diagnostics 
		WHERE
			IDvioal1=4 or IDvioal1=5 or IDvioal1=6 or IDvioal1=7
	");
}

// Количество детей зачисленных к логопеду с НПОЗ/ФФНР 2/3
function DBGetCountInLogocentreNPOZ_FFNR($IDvioal1) {
	return _DBGetQuery("
		SELECT 
			Count(*) As Count
		FROM 
			diagnostics 
		WHERE
			EnrollmentInLogocentre=true and IDvioal1 = $IDvioal1
	");
}

// Количество детей зачисленных к логопеду с ОНР =============================================================================
function DBGetCountInLogocentreONRs() {
	return _DBGetQuery("
		SELECT 
			Count(*) As Count
		FROM 
			diagnostics 
		WHERE
			EnrollmentInLogocentre=true and IDvioal1=4 or IDvioal1=5 or IDvioal1=6 or IDvioal1=7
	");
}
// Количество выведенных детей (ВСЕХ)
function DBGetCountReleas() {
	return _DBGetQuery("
		SELECT 
			Count(*) As Count
		FROM 
			diagnostics 
		WHERE
			Releas=true
	");
}
// Количество выведенных в школу детей
function DBGetCountReleasInSchool() {
	return _DBGetQuery("
		SELECT 
			Count(*) As Count
		FROM 
			diagnostics 
		WHERE
			ReleasInSchool=true
	");
}
// Количество детей, нуждающихся в продолжении занятий в школе
function DBGetCountSchoolLogocentre() {
	return _DBGetQuery("
		SELECT 
			Count(*) As Count
		FROM 
			diagnostics 
		WHERE
			SchoolLogocentre=true
	");
}
// Количество детей, направленных в спец. учреждение
function DBGetCountSpecialInstitution() {
	return _DBGetQuery("
		SELECT 
			Count(*) As Count
		FROM 
			diagnostics 
		WHERE
			SpecialInstitution=true
	");
}
// Количество детей, выбывших по др. причинам ОБЩЕЕ
function DBGetCountReleasOther() {
	return _DBGetQuery("
		SELECT 
			Count(*) As Count
		FROM 
			diagnostics 
		WHERE
			ReleasOther=true
	");
}



