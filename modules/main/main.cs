// Exec sub modules
exec("./profiles.cs");
exec("./iMain.gui");

function iMain_init()
{
	if($iMainInit) return;
	$iMainInit = 1;
	mainMenuGui.setName("oldMainMenu");
	iMain.setName("mainMenuGui");
	//MainMenuButtonsGui.delete();
	canvas.pushDialog("mainMenuGui");
	canvas.popDialog("oldMainMenu");
}

function iMain_timeLoop()
{
	cancel($iMain::TimeLoop);
	%time = getSubStr(getDateTime(),9,5);
	%len = strLen(%time);
	%m = " AM";
	%firstColon = strPos(%time,":");
	%hours = getSubStr(%time,0,%firstcolon);
	if(%hours >= 12)
	{
		%hours = %hours - 12;
		if(%hours == 0)
		   %hours = "12";
		%m = " PM";
		%time = %hours @ getSubStr(%time,%firstColon,%len);
	}
	if(%time !$= iMain_DATETIME.time)
	{
		%date = iMain_getDate();
		iMain_DATETIME.setText("<font:Tahoma bold:16><just:left>" @ %time @ %m @ "<font:Tahoma:16>  " @ %date);
		iMain_DATETIME.time = %time;
	}
	iMain_AuthText.setText(MM_AuthText.getValue());
	iMain_AuthText.tangoMoveTo(getRes()/2 - getWord(iMain_AuthText.extent,0)/2 SPC "5",250,"expo");
	iMain_AuthOpt.outOfPosition = getRes() - getWord(iMain_AuthOpt.extent,0) SPC getWord(iMain_AuthOpt.extent,1)*-3;
	iMain_AuthOpt.placedPosition = getRes() - getWord(iMain_AuthOpt.extent,0) SPC "20";
	if(isObject(ServerConnection)) return;
	$Blota::TimeLoopCount++;
	$Blota::TimeLoop = schedule(1000, 0, iMain_timeLoop);
}

function iMain_getDate()
{
	%date = getDateTime();
	%monthNum = getSubStr(%date,0,2);
	switch(%monthNum)
	{
		case 00:
			%month = "lol u broke it";
		case 01:
			%month = "January";
		case 02:
			%month = "Feburary";
		case 03:
			%month = "March";
		case 04:
			%month = "April";
		case 05:
			%month = "May";
		case 06:
			%month = "June";
		case 07:
			%month = "July";
		case 08:
			%month = "August";
		case 09:
			%month = "September";
		case 10:
			%month = "October";
		case 11:
			%month = "November";
		case 12:
			%month = "December";
	}
	%preday = getSubStr(%date, 3, 2);
	if(%preday < 10)
	{
		%day = getSubStr(%date, 4, 1);
	}
	if(%preday >= 10)
	{
		%day = %preday;
	}
	%year = getSubStr(%date, 6, 2);
	%text = %month SPC %day @ "," SPC "20" @ %year;
	return %text;
}
iMain_TimeLoop();

package iMainOverride
{
	function GameModeGui::clickBack(%this)
	{
		canvas.PopDialog(%this.getName());
	}

	function MainMenuGUI::showButtons()
	{
		//Nothing - Yay let's do nothing
	}

	function MainMenuGUI::onWake(%this)
	{
		Parent::onWake(%this);
		canvas.schedule(100,popDialog,MainMenuButtonsGui); //We can't delete it because we will have funny errors
	}
	
	function MainMenuGui::onRender()
	{
		Parent::onRender();
		canvas.popDialog(MainMenuButtonsGui); //We can't delete it because we will have funny errors
	   	if(!$iMain::started)
	   	{
			iMain_init();
		 	//ServerBrowser_FetchServerList(0); - They don't need to do this until they open the server list
		 	GUIPlus_APIInit();
	   	}	 
	  	$iMain::started = true;
	}

	function disconnectedCleanUp()
	{
		canvas.schedule(100,pushDialog,MainMenuGUI);
		return Parent::disconnectedCleanUp();
	}
};
activatePackage(iMainOverride);

function iMain::showButtons()
{
	//No go away pls
	canvas.schedule(100,popDialog,MainMenuButtonsGui);
}

function iMain::onWake(%this)
{
	canvas.schedule(100,popDialog,MainMenuButtonsGui); //We can't delete it because we will have funny errors
	iMain_AuthOpt.position = iMain_AuthOpt.outOfPosition;
	auth_init_client();
}

function iMain_ToggleAuthOpt()
{
	if(iMain_AuthOpt.position $= iMain_AuthOpt.placedPosition)
	{
		iMain_AuthOpt.position = iMain_AuthOpt.placedPosition;
		iMain_AuthOpt.tangoMoveTo(iMain_AuthOpt.outOfPosition,250,"expo");
		return;
	}
	iMain_AuthOpt.position = iMain_AuthOpt.outOfPosition;
	iMain_AuthOpt.tangoMoveTo(iMain_AuthOpt.placedPosition,250,"expo");
	iMain_AuthOpt.setVisible(1);
}
//schedule(4000, 0, iMain_init);