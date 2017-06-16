if(!isObject(SoftMessageBoxYesNoDlg))
	exec("add-ons/System_GuiPlus/modules/api/guis/SoftMessageBoxYesNoDlg.gui");

if(!isObject(SoftMessageOKCancelDlg))
	exec("add-ons/System_GuiPlus/modules/api/guis/SoftMessageBoxOKCancelDlg.gui");

if(!isObject(SoftMessageOKDlg))
	exec("add-ons/System_GuiPlus/modules/api/guis/SoftMessageBoxOKDlg.gui");

function SoftMessageBoxYesNo(%title,%message,%callback,%noCallback)
{
	canvas.pushDialog(SoftMessageBoxYesNoDlg);
	SoftMessageBoxYesNoDlg.yesCallback = %callback;
	SoftMessageBoxYesNoDlg.noCallback = %noCallback;
	SoftMBYesNoText.setText(%message);
	SoftMBYesNoFrame.setText(%title);
	%extentY = getWord(SoftMBYesNoText.extent,1);
	SoftMBYesNoFrame.resize(getWord(SoftMBYesNoFrame.position,0),
		getWord(getRes(),1) + getWord(SoftMBYesNoFrame.extent,1)*2,
		getWord(SoftMBYesNoFrame.defaultExtent,0),
		getWord(SoftMBYesNoFrame.defaultExtent,1) + (%extentY - 14)
		);
	SoftMBYesNoFrame.tangoMoveToCenter(500,"expo");
}

function SoftMessageBoxOKCancel(%title,%message,%callback,%noCallback)
{
	canvas.pushDialog(SoftMessageBoxOKCancelDlg);
	SoftMessageBoxOKCancelDlg.callback = %callback;
	SoftMessageBoxOKCancelDlg.cancelCallback = %noCallback;
	SoftMBOKCancelText.setText(%message);
	SoftMBOKCancelFrame.setText(%title);
	%extentY = getWord(SoftMBOKCancelText.extent,1);
	SoftMBOKCancelFrame.resize(getWord(SoftMBOKCancelFrame.position,0),
		getWord(getRes(),1) + getWord(SoftMBOKCancelFrame.extent,1)*2,
		getWord(SoftMBOKCancelFrame.defaultExtent,0),
		getWord(SoftMBOKCancelFrame.defaultExtent,1) + (%extentY - 14)
		);
	SoftMBOKCancelFrame.tangoMoveToCenter(500,"expo");
}

function SoftMessageBoxOK(%title,%message,%callback)
{
	canvas.pushDialog(SoftMessageBoxOKDlg);
	SoftMessageBoxOKDlg.callback = %callback;
	SoftMBOKText.setText(%message);
	SoftMBOKFrame.setText(%title);
	%extentY = getWord(SoftMBOKText.extent,1);
	SoftMBOKText.resize(getWord(SoftMBOKText.position,0),
		getWord(getRes(),1) + getWord(SoftMBOKFrame.extent,1)*2,
		getWord(SoftMBOKText.defaultExtent,0),
		getWord(SoftMBOKText.defaultExtent,1) + (%extentY - 14)
		);
	SoftMBOKFrame.tangoMoveToCenter(500,"expo");
}