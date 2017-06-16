$currSlipVersion = 2;
if($SlipVersion > $currSlipVersion)
	return;
$SlipVersion = $currSlipVersion;

//Profiles.cs
//Contains all the profiles that are needed to use SLip.
//By Wrapperup
//Added more stuff by Visolator

if(!isObject(slipWindowProfile))
	new GuiControlProfile(slipWindowProfile : BlockWindowProfile) 
	{
		fillColor = "255 255 255 255";
		fillColorHL = "255 255 255 255";
		fillColorNA = "255 255 255 255";
		fontType = "Trebuchet MS";
		fontSize = "18";
		fontColor = "100 100 100 255";
		textOffset = "5 5";
		bitmap = "./slipWindow.png";
	};

if(!isObject(slipButtonProfile))
	new GuiControlProfile(slipButtonProfile : blockButtonProfile) 
	{
		fontType = "Trebuchet MS";
		fontSize = "18";
		fontColor = "100 100 100 255";
		textOffset = "6 6";
	};

if(!isObject(slipScrollProfile))
	new GuiControlProfile(slipScrollProfile : BlockScrollProfile) 
	{
		border = "0";
		borderThickness = "0";
		fontType = "Trebuchet MS";
		fontSize = "18";
		fontColor = "0 0 0 255";
		bitmap = "./slipScroll.png";
	};

if(!isObject(slipTextProfile))
	new GuiControlProfile(slipTextProfile : GuiTextProfile) 
	{
		fontType = "Trebuchet MS";
		fontSize = "18";
		fontColor = "100 100 100 255";
	};

if(!isObject(slipBorderProfile))
	new GuiControlProfile(slipBorderProfile : GuiBitmapBorderProfile)
	{
		bitmap = "./slipBorder.png";
	};

if(!isObject(slipBorderWhiteProfile))
	new GuiControlProfile(slipBorderWhiteProfile : GuiBitmapBorderProfile)
	{
		bitmap = "./slipBorderWhite.png";
	};

if(!isObject(slipTextListProfile))
	new GuiControlProfile(slipTextListProfile : GuiTextListProfile)
	{
		mouseOverSelected = "1";
		fillColor = "200 200 200 255";
		fillColorHL = "171 0 171 255";
		fillColorNA = "200 200 200 255";
		fontType = "Trebuchet MS";
		fontSize = "18";
		fontColors[0] = "0 0 0 255";
		fontColors[1] = "128 128 128 255";
		fontColors[2] = "128 128 128 255";
		fontColors[3] = "200 200 200 255";
		fontColors[4] = "255 96 96 255";
		fontColors[5] = "0 0 255 255";
		fontColors[6] = "100 100 100 255";
		fontColors[7] = "100 100 100 255";
		fontColors[8] = "100 100 100 255";
		fontColors[9] = "100 100 100 255";
		fontColor = "100 100 100 255";
		fontColorHL = "128 128 128 255";
		fontColorNA = "128 128 128 255";
		fontColorSEL = "200 200 200 255";
		fontColorLink = "255 96 96 255";
		fontColorLinkHL = "0 0 255 255";
		cursorColor = "0 255 0 255";
	};