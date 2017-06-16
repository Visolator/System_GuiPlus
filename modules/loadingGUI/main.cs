// New Loading GUI - Support Code

loadingGui.delete();
exec("./profiles.cs");
exec("./newLoadingGUI.gui");
newLoadingGUI.setName("LoadingGui");

// Preview Downloader

function NLGUI_fetchPreview(%ip, %port)
{
	discoverFile(%imageFile);
	if(!isFile(%imageFile = "base/GuiPlus/Cache/" @ $Blota::NLGUI::SelectedServer["preview"] @ ".jpg"))
		return;
	NLGUI_PreviewImage.setBitmap($Blota::NLGUI::SelectedServer["preview"]);
}

// Server Details

function NLGUI_SetServerDetails()
{
	NLGUI_HostName.setText("<font:Arial:36><color:7A7A7A>" @ $Blota::NLGUI::SelectedServer["host"]);
	NLGUI_ServerName.setText("<font:Arial:40><color:000000>" @ $Blota::NLGUI::SelectedServer["name"]);
	NLGUI_Players.setText("<font:Arial:20><color:C4C4C4>Players: " @ $Blota::NLGUI::SelectedServer["playerCount"] @ "/" @ $Blota::NLGUI::SelectedServer["maxPlayers"]);
	NLGUI_Bricks.setText("<font:Arial:20><color:C4C4C4>Bricks: " @ $Blota::NLGUI::SelectedServer["brickCount"]);
	NLGUI_Ping.setText("<font:Arial:20><color:C4C4C4>Ping: " @ $Blota::NLGUI::SelectedServer["ping"] @ "ms");

	%x0 = getWord(getRes(),0) - getWord(NLGUI_EstimLoader.extent,0)*1.35;
	%y0 = getWord(getRes(),1) - getWord(NLGUI_EstimLoader.extent,1)*2.5;
	NLGUI_EstimLoader.gonePosition = %x0 SPC %y0;

	%x1 = getWord(getRes(),0) - getWord(NLGUI_EstimLoader.extent,0)*1.35;
	%y1 = getWord(getRes(),1) - getWord(NLGUI_EstimLoader.extent,1)*1.1;
	NLGUI_EstimLoader.placedPosition = %x1 SPC %y1;

	NLGUI_EstimLoader.resize(%x0,%y0,getWord(NLGUI_EstimLoader.extent,0),getWord(NLGUI_EstimLoader.extent,1));
	if($Blota::NLGUI::Mode $= "internet")
		NLGUI_fetchPreview($Blota::NLGUI::SelectedServer["IP"], $Blota::NLGUI::SelectedServer["port"]);
		
	if($Blota::NLGUI::Mode $= "local")
		NLGUI_PreviewImage.setBitmap("Add-ons/system_guiplus/modules/loadingGui/ui/local");
	
	NLGUI_PurgePlayerList();
			
	schedule(1000, 0, NLGUI_PingUpdate);
	schedule(2000, 0, LoadingGUI_initiateChat);
}

function getConnectionTime()
{
	return getTimeString(mFloor((getSimTime() - NLGUI_EstimLoader.connectTime)/1000));
}

function getConnectionSpawnTime()
{
	return getTimeString(mFloor(getConnectionTime() * (2 - LoadingProgress.getValue())));
}

function getConnectionTimeString()
{
	%upTimeString = getConnectionSpawnTime();
	%upTimeStringFields = strReplace(%upTimeString,":","" TAB "");
	if(getFieldCount(%upTimeStringFields) >= 3)
	{
		%RTime = getField(%upTimeStringFields,1) + (24 * getField(%upTimeStringFields,0));
		%rRTime = mFloor(getField(%upTimeStringFields,2));
	}
	else
	{
		%RTime = getField(%upTimeStringFields,0);
		%rRTime = mFloor(getField(%upTimeStringFields,1));
	}
	if(%Rtime >= 24) %time["hour"] = mFloor(%Rtime / 24);
	if(%Rtime > 0) %time["minute"] = %Rtime;
	if(%time["hour"] > 0) %string = %string @ %time["hour"] @ " hour" @ (%time["hour"] != 1 ? "s" @ (%time["minute"] > 0 ? "," : "") : (%time["minute"] > 0 ? "," : ""));
	if(%time["minute"] > 0)
	{
		if(%time["hour"] > 0)
			%time["minute"] = %time["minute"] - %time["hour"] * 24;
		%string = %string @ (%string !$= "" ? " " : "") @ %time["minute"] @ " minute" @ (%time["minute"] != 1 ? "s" @ (%rRTime > 0 ? "," : "") : (%rRTime > 0 ? "," : ""));
	}
	if(%rRTime > 0) %string = %string @ (%time["minute"] !$= "" ? " and " : " ") @ %rRTime @ " secound" @ (%rRTime != 1 ? "s" : "");
	return %string;
}

function NLGUI_EstimLoader::setProgress(%this)
{
	%percent = LoadingProgress.getValue() * 100;
	%time = getConnectionTimeString();

	NLGUI_EstimLoaderText.setValue("Estimated spawn time: " @ %time);
}

package blota_joinServerPackage
{
	function LoadingProgress::setValue(%this,%value)
	{
		NLGUI_EstimLoader.setProgress();
		Parent::setValue(%this,%value);
	}

	function JoinServerGui::join(%this)
	{
		parent::join(%this);
		%SDO = ServerInfoGroup.getObject(JS_serverList.getSelectedID());
		$Blota::NLGUI::SelectedServer["name"] = getField(strReplace(%SDO.name, "\'" , "\t"), 1);
		$Blota::NLGUI::SelectedServer["host"] = getField(strReplace(%SDO.name, "\'" , "\t"), 0) @ "\'s";
		$Blota::NLGUI::SelectedServer["playerCount"] = %SDO.currPlayers;
		$Blota::NLGUI::SelectedServer["maxPlayers"] = %SDO.maxPlayers;
		$Blota::NLGUI::SelectedServer["brickCount"] = %SDO.brickCount;
		$Blota::NLGUI::SelectedServer["ping"] = %SDO.ping;
		$Blota::NLGUI::SelectedServer["IP"] = getField(strReplace(%SDO.ip, ":", "\t"), 0);
		$Blota::NLGUI::SelectedServer["port"] = getField(strReplace(%SDO.ip, ":", "\t"), 1);
		$Blota::NLGUI::Mode = "internet";
		
		if(getWord($Blota::NLGUI::SelectedServer["name"], 0) $= "s")
			$Blota::NLGUI::SelectedServer["name"] = restWords($Blota::NLGUI::SelectedServer["name"]);

		NLGUI_SetServerDetails();
	}
	
	function NewPlayerListGui::update(%this, %cl, %name, %BL_ID, %trust, %admin, %score)
	 {
		parent::update(%this, %cl, %name, %BL_ID, %trust, %admin, %score);
		NLGUI_AddPlayerToList(%name, %BL_ID, %score);
	}
	
	function createServer(%type)
	{
		if(%type $= "internet")
		{
			$Blota::NLGUI::SelectedServer["name"] = $Pref::Server::Name;
			$Blota::NLGUI::SelectedServer["host"] = $pref::Player::NetName;
			$Blota::NLGUI::SelectedServer["playerCount"] = 1;
			$Blota::NLGUI::SelectedServer["maxPlayers"] = $Pref::Server::MaxPlayers;
			$Blota::NLGUI::SelectedServer["brickCount"] = 0;
			$Blota::NLGUI::Mode = "local";
		}
		if(%type $= "SinglePlayer")
		{
			$Blota::NLGUI::SelectedServer["name"] = "Single Player";
			$Blota::NLGUI::SelectedServer["host"] = $pref::Player::LanName;
			$Blota::NLGUI::SelectedServer["playerCount"] = 1;
			$Blota::NLGUI::SelectedServer["maxPlayers"] = 1;
			$Blota::NLGUI::SelectedServer["brickCount"] = 0;
			$Blota::NLGUI::Mode = "local";
		}
		NLGUI_SetServerDetails();
		parent::createServer(%type);
	}

	function disconnectedCleanup()
	{
		NLGUI_Shadow.remove(NewChatHud);
		NLGUI_PurgePlayerList();
		NLGUI_EstimLoader.tangoMoveTo(NLGUI_EstimLoader.gonePosition,100,"expo");
		return Parent::disconnectedCleanup();
	}
};
activatePackage(blota_joinServerPackage);

function LoadingGUI_initiateChat()
{
	if(isObject(serverConnection))
	{
		newLoadingGuiWindow.center(); //Do it once, the ping update will keep centering it in-case funny people decide to resize it when loading..
		NLGUI_Shadow.extent = getWords(getRes(),0,1);
		NLGUI_Shadow.position = "0 0";
		NLGUI_Shadow.add(NewChatHud);
		moveMap.push();
		//NLGUI_Shadow.bringToFront(NewChatHud);
		NewChatHud.position = "0 0";
		NLGUI_EstimLoader.connectTime = getSimTime();
		NLGUI_EstimLoader.setVisible(0); //Currently this thing is broken so we will just make it invisible
		NLGUI_EstimLoader.tangoMoveTo(NLGUI_EstimLoader.placedPosition,500,"expo");
	}
}

function NLGUI_PingUpdate()
{
	cancel($Blota::NLGUI::PingUpdate);
	if(isObject(ServerConnection))
	{ 
		NLGUI_Ping.setText("<font:Arial:20><color:C4C4C4>Ping: " @ serverConnection.getPing() @ "ms");	
		$Blota::NLGUI::PingUpdate = schedule(2000, 0, NLGUI_PingUpdate);
	}
	newLoadingGuiWindow.center(); //In case they did something, let's just re-center it.
}

function NLGUI_AddPlayerToList(%name, %BL_ID, %score)
{
	$Blota::NLGUI::CurrID++;
	
	%yPos = 20 * $Blota::NLGUI::CurrID;
	%yheight = 20 * ($Blota::NLGUI::CurrID + 2);
	
	%a = new GuiMLTextCtrl() {
		profile = "GuiMLTextProfile";
		horizSizing = "right";
		vertSizing = "bottom";
		position = "3" SPC %yPos;
		extent = "239 18";
		minExtent = "8 2";
		enabled = "1";
		visible = "1";
		clipToParent = "1";
		lineSpacing = "2";
		allowColorChars = "0";
		maxChars = "-1";
		text = "<font:Arial:18><color:666666>" @ %name;
		maxBitmapHeight = "-1";
		selectable = "1";
		autoResize = "1";
	};
	%b = new GuiMLTextCtrl() {
		profile = "GuiMLTextProfile";
		horizSizing = "right";
		vertSizing = "bottom";
		position = "253" SPC %yPos;
		extent = "94 18";
		minExtent = "8 2";
		enabled = "1";
		visible = "1";
		clipToParent = "1";
		lineSpacing = "2";
		allowColorChars = "0";
		maxChars = "-1";
		text = "<font:Arial:18><color:666666>" @ %BL_ID;
		maxBitmapHeight = "-1";
		selectable = "1";
		autoResize = "1";
	};
	%c = new GuiMLTextCtrl() {
		profile = "GuiMLTextProfile";
		horizSizing = "right";
		vertSizing = "bottom";
		position = "354" SPC %yPos;
		extent = "94 18";
		minExtent = "8 2";
		enabled = "1";
		visible = "1";
		clipToParent = "1";
		lineSpacing = "2";
		allowColorChars = "0";
		maxChars = "-1";
		text = "<font:Arial:18><color:666666>" @ %score;
		maxBitmapHeight = "-1";
		selectable = "1";
		autoResize = "1";
	};
	
	NLGUI_Scroll_Canvas.add(%a);
	NLGUI_Scroll_Canvas.add(%b);
	NLGUI_Scroll_Canvas.add(%c);
	
	if(getWord(NLGUI_Scroll_Expander.extent, 1) < %yheight)
		NLGUI_Scroll_Expander.resize(1, 1, 418, %yheight);

}

function NLGUI_PurgePlayerList()
{
	NLGUI_Scroll_Canvas.clear();
	NLGUI_Scroll_Expander.resize(1, 1, 418, 274);
	$Blota::NLGUI::CurrID = 0;
}