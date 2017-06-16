exec("./profiles.cs");
exec("./ServerBrowser.gui");
exec("./GuiPlus_ServerSwitchGui.gui");
$ServerBrowser::Mode = "grid";
// Here we go
function AddBind(%division, %name, %command)
{
	for(%i=0;%i<$remapCount;%i++)
	{
		if($remapDivision[%i] $= %division)
		{
			%foundDiv = 1;
			continue;
		}
		if(%foundDiv && $remapDivision[%i] !$= "")
		{
			%position = %i;
			break;
		}
	}
	if(!%foundDiv)
	{
		error("Division not found: " @ %division);
		return;
	}
	if(!%position)
	{
		$remapName[$remapCount] = %name;
		$remapCmd[$remapCount] = %command;
		$remapCount++;
		return;
	}
	for(%i=$remapCount;%i>%position;%i--)
	{
		$remapDivision[%i] = $remapDivision[%i - 1];
		$remapName[%i] = $remapName[%i - 1];
		$remapCmd[%i] = $remapCmd[%i - 1];
	}
	$remapDivision[%position] = "";
	$remapName[%position] = %name;
	$remapCmd[%position] = %command;
	$remapCount++;
}

AddBind("Gui","Toggle In-Game ServerBrowser","GuiPlus_OpenServerBrowserSwitcher");

//Remove this when developement is over
if(isPackage(GUIPlus_ServerInfo))
	deactivatePackage(GUIPlus_ServerInfo);

package GUIPlus_ServerInfo
{
	function JoinServerPassGui::enterPass()
	{
		$Blota::NLGUI::SelectedServer["pass"] = JSP_txtPass.getValue();
		Parent::enterPass();
		Connecting_Text.setText("Connecting to " @ $Blota::NLGUI::SelectedServer["IP"] @ ":" @ $Blota::NLGUI::SelectedServer["port"] @ " with password");
			connectToServer($Blota::NLGUI::SelectedServer["IP"] @ ":" @ $Blota::NLGUI::SelectedServer["port"], 
				$Blota::NLGUI::SelectedServer["pass"], 1, 1);
	}

	function onSimplePingTimeout(%addressPort, %ping, %groupNum)
	{
		Parent::onSimplePingReceived(%addressPort, %ping, %groupNum);
		//We know, it's dead.
		%address = stripAddress(%addressPort);
		if(isObject(%pingText = findServerPingObjByIP(%address)))
			%pingText.setText("<font:arial:16><color:e9bc21>???");
		findServerObjByIP(%address).ping = %ping;
	}

	function onSimplePingReceived(%addressPort, %ping, %groupNum)
	{
		Parent::onSimplePingReceived(%addressPort, %ping, %groupNum);
		%address = stripAddress(%addressPort);
		%uhh = "Add-Ons/System_GuiPlus/modules/serverBrowser/ui/ping_";
		if(%ping > 0 && %ping < 150) //Ping is good
			%lol = "low";
		else if(%ping >= 150 && %ping < 500) //Ping is good
			%lol = "med";
		else if(%ping >= 5000) //Ping is good
			%lol = "high";
		else %lol = "none";

		%pingy = 1 - 1 / 500 * %ping;
		if(%pingy > 1) %pingy = 1;
		if(%pingy < 0) %pingy = 0;
		%color = RGBToHex(greenToRed(%pingy));

		if(isObject(%img = findServerPingImgObjByIP(%address)))
			%img.setBitmap(%uhh @ %lol);
		if(isObject(%pingText = findServerPingObjByIP(%address)))
			%pingText.setText("<font:arial:16><color:" @ %color @ ">" @ %ping @ "");
		("ServSwatch_" @ %groupNum).ping = %ping;
	}
};
activatePackage(GUIPlus_ServerInfo);

// Master Server Parser
// Preview Imaging
// Listing generation
// Support functions

//Revised by Visolator
//Let's create a function to find the object for that server by their IP address
function findServerObjByIP(%ip)
{
	%ip = stripAddress(%ip);
	if(ServerBrowser_Canvas.getCount() <= 0) return -1; //Do not do an endless loop
	for(%i=0;%i<ServerBrowser_Canvas.getCount();%i++)
	{
		%obj = ServerBrowser_Canvas.getObject(%i);
		if(%obj.ip $= %ip)
			return %obj;
	}
}

function findServerImgObjByID(%num)
{
	%num = mFloor(%num);
	if(ServerBrowser_Canvas.getCount() <= 0) return -1; //Do not do an endless loop
	for(%i=0;%i<ServerBrowser_Canvas.getCount();%i++)
	{
		%obj = ServerBrowser_Canvas.getObject(%i);
		if(%obj.num == %num)
			return nameToID("ServPreview_" @ %obj.num);
	}
}

function findServerPingImgObjByIP(%ip)
{
	%ip = stripAddress(%ip);
	if(ServerBrowser_Canvas.getCount() <= 0) return -1; //Do not do an endless loop
	for(%i=0;%i<ServerBrowser_Canvas.getCount();%i++)
	{
		%obj = ServerBrowser_Canvas.getObject(%i);
		if(%obj.ip $= %ip)
			return nameToID("ServPingImg_" @ %obj.num);
	}
}

function findServerPingObjByIP(%ip)
{
	%ip = stripAddress(%ip);
	if(ServerBrowser_Canvas.getCount() <= 0) return -1; //Do not do an endless loop
	for(%i=0;%i<ServerBrowser_Canvas.getCount();%i++)
	{
		%obj = ServerBrowser_Canvas.getObject(%i);
		if(%obj.ip $= %ip)
			return nameToID("ServPing_" @ %obj.num);
	}
}

function findServerObjByNum(%num)
{
	%obj = "ServSwatch_" @ %num;
	if(!isObject(%obj)) return -1;
	return nameToID(%obj);
}

function stripAddress(%ipPort)
{
	//example: 123.456.789:28100
	//this would be: 123.456.789
	%ipPort = trim(%ipPort);
	if(%ipPort $= "" || strPos(%ipPort,":") == -1) return %ipPort;
	return getSubStr(%ipPort,0,strPos(%ipPort,":"));
}

function GuiPlus_OpenServerBrowser()
{
	canvas.PushDialog("serverbrowser");
	cancel($Blota::OpenServBroSch);
	if($pref::Gui::AutoQueryMasterServer) //Only query the server list if the client enabled it, otherwise they have to figure it out
		$Blota::OpenServBroSch = schedule(1000,0,ServerBrowser_FetchServerList);
}

function GuiPlus_OpenServerBrowserSwitcher(%x)
{
	if(%x)
		return;
	if(GuiPlus_ServerSwitchGui.isActive())
		canvas.popDialog(GuiPlus_ServerSwitchGui);
	else
		canvas.pushDialog(GuiPlus_ServerSwitchGui);
}

function ServerBrowserB::onWake(%this)
{
	%this.center();
}

// Master server parser
function ServerBrowser_FetchServerList(%old, %search)
{
		serverBrowser_Canvas.clear();

		ServerBrowser_ListList.clear();
		ServerBrowser_InputClick.setVisible(1);
	
		if(!isObject(ServerBrowser_HTTPObject))
			new HTTPObject(ServerBrowser_HTTPObject);
		
		if(!%old)
	 	ServerBrowser_flushImgCache("base/GuiPlus/Cache/*");
	
		ServerBrowser_HTTPObject.renderServers = false;
		ServerBrowser_HTTPObject.useOld = %old;
		ServerBrowser_HTTPObject.lineCount = 0;
		ServerBrowser_HTTPObject.get("master2.blockland.us:80", "/index.php");
}

function ServerBrowser_StartPingSearch()
{
	if($Blota::NoCommunication) return;
	ServerInfoSO_StartPingAll();
}

function ServerBrowser_HTTPObject::onLine(%this, %line)
{
	if($Blota::NoCommunication) return;
	if(%line $= "END")
	{
		if($ServerBrowserCount > 0)	
		{
			for(%i=0;%i<$ServerBrowserCount;%i++)
			{
				%rawData = $ServerBrowserNum[%i];
				%server_ip			  = getField(%rawData, 0);
				%server_port			= getField(%rawData, 1);
				%server_passworded	= getField(%rawData, 2);
				%server_dedicated	 = getField(%rawData, 3);
				%server_servername	= getField(%rawData, 4);
				%server_players		= getField(%rawData, 5);
				%server_maxplayers	= getField(%rawData, 6);
				%server_mapname		= getField(%rawData, 7);
				%server_brickcount	= getField(%rawData, 8);

				if(%server_players >= %server_maxplayers)
					%playerString = "FULL";
				else
					%playerString = %server_players @ "/" @ %server_maxplayers;

				if(strLen(%server_ip) > 0 && strLen(%server_port) > 0)
				{
					serverListSwatch.addRow(serverListSwatch.rowCount(),
						%server_servername TAB  
						%server_mapname	TAB
						%server_brickcount TAB 
						%playerString TAB
						strReplace(strReplace(%server_passworded, "1", "Yes"), "0", "No"));

					ServerBrowser_ListList.addRow(ServerBrowser_ListList.rowCount(), %server_servername TAB 
						strReplace(strReplace(%server_dedicated, "1", "Yes"), "0", "No") TAB 
						strReplace(strReplace(%server_passworded, "1", "Yes"), "0", "No") TAB 
						%playerString TAB 
						%server_brickcount TAB 
						%server_mapname);
				}
			}
		}

		%this.renderServers = false;
		if(strLen($ServerBrowserNum[0]) > 0) //Make sure it exists
			ServerBrowser_RenderServer($ServerBrowserNum[0]);
		else //Otherwise tell them something is wrong
			SoftMessageBoxOk("Uh oh!","Something is currently wrong getting the data, please refresh the server list.");
	}
	  
	if(%this.renderServers)
	{
		$ServerBrowserNum[$ServerBrowserCount] = ""; //We don't know if there will be data
		$ServerBrowserNum[$ServerBrowserCount] = %line;
		$ServerBrowserCount++;

		%this.lineCount++;
	}
	
	if(%line $= "START")
	{
		%this.renderServers = true;
		$ServerBrowser::NumServers = 0;
		$ServerBrowserCount = 0;
		ServerBrowser_Canvas.deleteAll();
		ServerInfoSO_ClearAll();
	}
}

function ServerBrowser_RenderNext(%num)
{
	if($Blota::NoCommunication) return;
	if(%num > $ServerBrowserCount) //If we are out of the table count, don't keep going, otherwise, start pinging the servers.
		ServerBrowser_StartPingSearch(); //This means we have successfully added all the servers, so they need to ping.
	else
		schedule(250,0,ServerBrowser_RenderServer,$ServerBrowserNum[%num]);
}

function ServerBrowser_RenderServer(%rawData)
{
	if($Blota::NoCommunication) return;
	%server_ip			  = getField(%rawData, 0);
	if(!strLen(%server_ip)) return;
	%server_port			= getField(%rawData, 1);
	%server_passworded	= getField(%rawData, 2);
	%server_dedicated	 = getField(%rawData, 3);
	%server_servername	= getField(%rawData, 4);
	%server_players		= getField(%rawData, 5);
	%server_maxplayers	= getField(%rawData, 6);
	%server_mapname		= getField(%rawData, 7);
	%server_brickcount	= getField(%rawData, 8);

	%compiledIP = strReplace(%server_ip,".","-");
	%compiledIP = %compiledIP @ "_" @ %server_port;

	ServerInfoSO_Add(%server_ip @ ":" @ %server_port,
		%server_passworded,
		%server_dedicated,
		%server_servername,
		%server_players,
		%server_maxplayers,
		%uhh,
		%server_mapname,
		%server_brickcount,
		0);
	
	%serverName = getField(strReplace(%server_servername, "\'" , "\t"), 1);
	
	if(getWord(%serverName, 0) $= "s")
		%serverName = restWords(%serverName);
		
	%host = getField(strReplace(%server_servername, "\'" , "\t"), 0);
	
	//Grid View
	
	//pos
	%row = mFloor($ServerBrowser::NumServers / 4);
	%col = $ServerBrowser::NumServers - (%row * 4);
	
	%posX = %col * (154 + 10);
	%posY = %row * (214 + 10);

	%colour = "222222";
	if(%server_passworded)
		%colour = "f7941d";		
		
		%obj = new GuiSwatchCtrl("ServSwatch_" @ $serverBrowser::NumServers)
		{
			profile = "GuiDefaultProfile";
			horizSizing = "right";
			vertSizing = "bottom";
			position = %posX SPC %posY;
			extent = "154 214";
			minExtent = "8 2";
			enabled = "1";
			visible = "1";
			clipToParent = "1";
			color = "0 0 0 0";
			ip = %server_ip;
			port = %server_port;
			compiledIP = %compiledIP;
			ping = "???";
			num = $ServerBrowser::NumServers;
			serverName = %server_servername;

			new GuiBitmapCtrl("ServPreview_" @ $serverBrowser::NumServers) 
			{
				profile = "GuiDefaultProfile";
				horizSizing = "right";
				vertSizing = "bottom";
				position = "0 0";
				extent = "154 154";
				minExtent = "8 2";
				enabled = "1";
				visible = "1";
				clipToParent = "1";
				bitmap = "add-ons/system_guiplus/modules/serverbrowser/ui/placeholder";
				wrap = "0";
				lockAspectRatio = "0";
				alignLeft = "0";
				alignTop = "0";
				overflowImage = "0";
				keepCached = "0";
				mColor = "255 255 255 255";
				mMultiply = "0";
			};
			new GuiMLTextCtrl() 
			{
				profile = "GuiMLTextProfile";
				horizSizing = "right";
				vertSizing = "bottom";
				position = "1 158";
				extent = "400 16";
				minExtent = "8 2";
				enabled = "1";
				visible = "1";
				clipToParent = "1";
				lineSpacing = "2";
				allowColorChars = "0";
				maxChars = "-1";
				text = "<font:arial:17><color:" @ %colour @ ">" @ %serverName;
				maxBitmapHeight = "-1";
				selectable = "1";
				autoResize = "1";
			};
			new GuiMLTextCtrl() 
			{
				profile = "GuiMLTextProfile";
				horizSizing = "right";
				vertSizing = "bottom";
				position = "1 174";
				extent = "400 16";
				minExtent = "8 2";
				enabled = "1";
				visible = "1";
				clipToParent = "1";
				lineSpacing = "2";
				allowColorChars = "0";
				maxChars = "-1";
				text = "<font:arial:15><color:333333>by<font:arial bold:16> " @ %host;
				maxBitmapHeight = "-1";
				selectable = "1";
				autoResize = "1";
			};
			new GuiMLTextCtrl() 
			{
				profile = "GuiMLTextProfile";
				horizSizing = "right";
				vertSizing = "bottom";
				position = "2 197";
				extent = "82 16";
				minExtent = "8 2";
				enabled = "1";
				visible = "1";
				clipToParent = "1";
				lineSpacing = "2";
				allowColorChars = "0";
				maxChars = "-1";
				text = "<color:333333><font:arial bold:16>" @ %server_players @ "/" @ %server_maxplayers @ "<font:arial:16> players";
				maxBitmapHeight = "-1";
				selectable = "1";
				autoResize = "1";
			};
			new GuiBitmapCtrl("ServPingImg_" @ $serverBrowser::NumServers) 
			{
				profile = "GuiDefaultProfile";
				horizSizing = "right";
				vertSizing = "bottom";
				position = "105 198";
				extent = "17 14";
				minExtent = "8 2";
				enabled = "1";
				visible = "1";
				clipToParent = "1";
				bitmap = "./ui/ping_none";
				wrap = "0";
				lockAspectRatio = "0";
				alignLeft = "0";
				alignTop = "0";
				overflowImage = "0";
				keepCached = "0";
				mColor = "255 255 255 255";
				mMultiply = "0";
			};
			new GuiMLTextCtrl("ServPing_" @ $serverBrowser::NumServers) {
				profile = "GuiMLTextProfile";
				horizSizing = "right";
				vertSizing = "bottom";
				position = "122 197";
				extent = "33 16";
				minExtent = "8 2";
				enabled = "1";
				visible = "1";
				clipToParent = "1";
				lineSpacing = "2";
				allowColorChars = "0";
				maxChars = "-1";
				text = "<font:arial:16><color:e9bc21>";
				maxBitmapHeight = "-1";
				selectable = "1";
				autoResize = "1";
			};
			new GuiBitmapButtonCtrl("GridButton_" @ $ServerBrowser::NumServers) 
			{
			  	profile = "GuiDefaultProfile";
			  	horizSizing = "right";
			  	vertSizing = "bottom";
			  	position = "0 0";
			  	extent = "154 214";
			  	minExtent = "8 2";
			  	enabled = "1";
			  	visible = "1";
			  	clipToParent = "1";
			  	command = "ServerBrowser_Join(\"" @ %rawData TAB %host TAB %serverName TAB %compiledIP @ "\");";
			  	altCommand = "";
			  	text = " ";
			  	groupNum = "-1";
			  	buttonType = "PushButton";
			  	bitmap = "add-ons/system_GUIplus/modules/main/images/buttons/misc/blank";
			  	lockAspectRatio = "0";
			  	alignLeft = "0";
			  	alignTop = "0";
			  	overflowImage = "0";
			  	mKeepCached = "0";
			  	mColor = "255 255 255 255";
		  };
		};
		if(%row < 5)
		{
			%obj.position = %posX SPC %posY + 600;
			%obj.tangoMoveTo(%posX SPC %posY, 600 + (600 * %row) + ($ServerBrowser::NumServers * 60), expo);
		}
		if(%server_brickcount > 0)
			ServerBrowser_StartPrevDownload($ServerBrowser::NumServers);
		$ServerBrowser::NumServers++;
		ServerBrowser_Canvas.add(%obj);
		%yResize = (%row + 1) * 214 + (10 * %row + 1);
		ServerBrowser_Canvas.resize(1, getWord(ServerBrowser_Canvas.position,1), 660, %yResize);
		ServerBrowser_RenderNext($ServerBrowser::NumServers);
}

function ServerBrowser_StopAllCommunications()
{
	$Blota::NoCommunication = 1;
	schedule(1000,0,eval,"$Blota::NoCommunication = 0;");
}

function ServerBrowser_ResizeServerList(%x, %y) //This is future for resizing the window, this will be tough to fix.
{
	return; //wat
}

function ServerBrowser_StartPrevDownload(%id,%mode)
{	
	if($Blota::NoCommunication) return;
	if(%id > ServerBrowser_Canvas.getCount()) //Do not ever get out of the table
	{
		warn("ServerBrowser::StartPrevDownload - OUT OF INDEX (" @ %id @ ":" @ ServerBrowser_Canvas.getCount() @ ")");
		return -1;
	}
	%swatch = nameToID("ServSwatch_" @ %id);
	%prevSwatch = nameToID("ServPreview_" @ %id);
	if(!isObject(%swatch))
		return 0;
	%swatch.imageObj = %prevSwatch;
	%ip = ("ServSwatch_" @ %id).ip;
	%port = ("ServSwatch_" @ %id).port;
	%ipData = strReplace(%ip, ".", "-");
	%data = %ipData @ "_" @ %port;
	%swatch.imgData = %data;
	if(isObject(%swatch.tcpObj))
		return 0;
	switch$(%mode)
	{
		case "single":
		%obj.downloadMode = "SINGLE";

		default:
			%obj.downloadMode = "ALL";
	}
	%obj = connectToURL("http://image.blockland.us/detail/" @ %data @ ".jpg", "GET", "base/GuiPlus/Cache/" @ %data @ ".jpg", "ServerBrowser_PrevTCP");
	%obj.servID = %id;
	%obj.ip = %ip;
	%obj.port = %port;
	%obj.swatch = %swatch;
	return 1;
}

function ServerBrowser_PrevTCP::onDone(%this, %error)
{
	if($Blota::NoCommunication) return;
	%swatch = %this.swatch;
	if(!isObject(%swatch))
		return;
	%imageFile = "base/GuiPlus/Cache/" @ %swatch.imgData @ ".jpg";
	discoverFile(%imageFile);
	if(%error == 0 && isObject(%img = findServerImgObjByID(%this.servID)))
	{
		if(isFile(%imageFile))
			%img.schedule(200,setBitmap,%imageFile);
	}

	if(%error == 0)
		pingSingleServer(%this.ip @ ":" @ %this.port,0);
	%this.downloadMode = "ALL";
}

//

function ServerBrowser_Join(%data)
{
	// Set up loading GUI module
	$Blota::NLGUI::SelectedServer["IP"] =			  getField(%data, 0);
	$Blota::NLGUI::SelectedServer["port"] =			getField(%data, 1);
	$Blota::NLGUI::SelectedServer["isPassworded"] =			getField(%data, 2);
	$Blota::NLGUI::SelectedServer["isDedicated"] =			getField(%data, 3);

	$Blota::NLGUI::SelectedServer["playerCount"] =  getField(%data, 5);
	$Blota::NLGUI::SelectedServer["maxPlayers"] =	getField(%data, 6);

	$Blota::NLGUI::SelectedServer["brickCount"] =	getField(%data, 8);

	$Blota::NLGUI::SelectedServer["host"] =			getField(%data, 10);
	$Blota::NLGUI::SelectedServer["name"] =			getField(%data, 11);

	$Blota::NLGUI::SelectedServer["ping"] =			findServerObjByIP(getField(%data,0)).ping;
	$Blota::NLGUI::SelectedServer["preview"] = getField(%data, 12);

	$Blota::NLGUI::Mode = "internet";

	NLGUI_SetServerDetails();
	if($Blota::NLGUI::SelectedServer["ping"] <= 0)
		%reason = "There is dead ping communication.";
	else if($Blota::NLGUI::SelectedServer["ping"] > 500)
		%reason = "Your ping is too high.";
	if(strLen(%reason) > 0)
		SoftMessageBoxYesNo("Warning","Are you sure you want to join this server?<br>" @ %reason,"ServerBrowser_JoinContin(\"" @ %data @ "\");");
	else
		ServerBrowser_JoinContin(%data);
}

function ServerBrowser_JoinContin(%data)
{
	$Blota::NLGUI::SelectedServer["IP"] =			  getField(%data, 0);
	$Blota::NLGUI::SelectedServer["port"] =			getField(%data, 1);
	$Blota::NLGUI::SelectedServer["isPassworded"] =			getField(%data, 2);
	$Blota::NLGUI::SelectedServer["isDedicated"] =			getField(%data, 3);

	$Blota::NLGUI::SelectedServer["playerCount"] =  getField(%data, 5);
	$Blota::NLGUI::SelectedServer["maxPlayers"] =	getField(%data, 6);

	$Blota::NLGUI::SelectedServer["brickCount"] =	getField(%data, 8);

	$Blota::NLGUI::SelectedServer["host"] =			getField(%data, 10);
	$Blota::NLGUI::SelectedServer["name"] =			getField(%data, 11);

	$Blota::NLGUI::SelectedServer["ping"] =			findServerObjByIP(getField(%data,0)).ping;
	$Blota::NLGUI::SelectedServer["preview"] = getField(%data, 12);


	$Blota::NLGUI::Mode = "internet";
	NLGUI_SetServerDetails();

	if(!$Blota::NLGUI::SelectedServer["isPassworded"])
	{
		canvas.PushDialog(connectingGui);
		Connecting_Text.setText("Connecting to " @ getField(%data, 0) @ ":" @ getField(%data, 1));
		connectToServer(getField(%data, 0) @ ":" @ getField(%data, 1), "", 1, 1);
	}
	else
		canvas.PushDialog(JoinServerPassGui);
}

// Support functions

function ServerBrowser_toggleViewPane()
{
	%toggleState = ServerBrowser_ViewPanel.visible;
	
	if($ServerBrowser::Mode $= "grid")
	{
		if(%toggleState)
		{
			ServerBrowser_ViewPanel.setVisible(0);
			ServerBrowser_viewButton.setBitmap("add-ons/system_guiplus/modules/serverbrowser/ui/buttons/viewG");
		}
		else
		{
			ServerBrowser_ViewPanel.setVisible(1);
			ServerBrowser_viewButton.setBitmap("add-ons/system_guiplus/modules/serverbrowser/ui/buttons/viewGi");
		}
	}
	
	if($ServerBrowser::Mode $= "list")
	{
		if(%toggleState)
		{
			ServerBrowser_ViewPanel.setVisible(0);
			ServerBrowser_viewButton.setBitmap("add-ons/system_guiplus/modules/serverbrowser/ui/buttons/viewL");
		}
		else
		{
			ServerBrowser_ViewPanel.setVisible(1);
			ServerBrowser_viewButton.setBitmap("add-ons/system_guiplus/modules/serverbrowser/ui/buttons/viewLi");
		}
	}
}

function ServerBrowser_flushImgCache(%path)
{
	 for (%i = findFirstFile(%path); strLen(%i) > 0; %i = findNextFile(%path)) //Dangerous code
	 {
	 	if(fileExt(%i) $= ".jpeg")
		  	fileDelete(%i);
	 }
}

function ServerBrowser_SetGridView()
{
	$ServerBrowser::Mode = "grid";
	ServerBrowser_ListPane.setVisible(0);
	serverBrowser_scroll.setVisible(1);
	ServerBrowser_toggleViewPane();
	ServerBrowser_viewButton.setBitmap("add-ons/system_guiplus/modules/serverbrowser/ui/buttons/viewG");
}

function ServerBrowser_SetListView()
{
	$ServerBrowser::Mode = "list";
	ServerBrowser_ListPane.setVisible(1);
	serverBrowser_scroll.setVisible(0);
	ServerBrowser_toggleViewPane();
	ServerBrowser_viewButton.setBitmap("add-ons/system_guiplus/modules/serverbrowser/ui/buttons/viewL");
}

function ServerBrowser_SortList(%col)
{
	%bool = (ServerBrowser_SA @ %col).sortDir;
	%bool = !%bool;
	(ServerBrowser_SA @ %col).sortDir = %bool;
	ServerBrowser_ListList.sort(%col, %bool);
	
	if(%bool)
		(ServerBrowser_SA @ %col).setBitmap("add-ons/system_guiplus/modules/serverbrowser/ui/sortu");
	else
		(ServerBrowser_SA @ %col).setBitmap("add-ons/system_guiplus/modules/serverbrowser/ui/sortd");
}

function ServerBrowser_SortNUMList(%col)
{
	%bool = (ServerBrowser_SA @ %col).sortDir;
	%bool = !%bool;
	(ServerBrowser_SA @ %col).sortDir = %bool;
	ServerBrowser_ListList.sortNumerical(%col, %bool);
	
	if(%bool)
		(ServerBrowser_SA @ %col).setBitmap("add-ons/system_guiplus/modules/serverbrowser/ui/sortu");
	else
		(ServerBrowser_SA @ %col).setBitmap("add-ons/system_guiplus/modules/serverbrowser/ui/sortd");
}

function ServerBrowser_JoinButton()
{
	%id = ServerBrowser_ListList.getSelectedID();
	if(%id $= "-1")
		return;
		
	%cmd = ("GridButton_" @ %id).command;
	eval(%cmd);
}

function ServerBrowser_SwitchToInternet()
{
	ServerBrowser_Canvas.deleteAll();
	ServerBrowser_InternetTab.setColor("255 255 255 255");
	ServerBrowser_InternetButton.setProfile(ServerBrowserA13);
	ServerBrowser_LANTab.setColor("0 0 0 0");
	ServerBrowser_LANButton.setProfile(ServerBrowserAW13);
	ServerBrowser_StopAllCommunications();
	ServerBrowser_Canvas.resize(1, 1, 660, 5);
	ServerBrowser_FetchServerList();
}

function ServerBrowser_SwitchToLAN()
{
	ServerBrowser_Canvas.deleteAll();
	ServerBrowser_LANTab.setColor("255 255 255 255");
	ServerBrowser_LANButton.setProfile(ServerBrowserA13);
	ServerBrowser_InternetTab.setColor("0 0 0 0");
	ServerBrowser_InternetButton.setProfile(ServerBrowserAW13);
	ServerBrowser_StopAllCommunications();
	ServerBrowser_Canvas.resize(1, 1, 660, 5);
	//ServerBrowser_FetchServerLANList();
}