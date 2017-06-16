// System_GUIPlus
//    A project to make blockland look better
//    Author: Fluffy, revised by Visolator
//    Designs by: SC, TheBlackParrot and Fluffy

// Load Modules
$GUIPlus::Version = 0.43;
exec("./support/main.cs"); //Always load support files
exec("./modules/slip/slipProfiles.cs");
GuiPlus_LoadFiles("add-ons/System_GuiPlus/support/*");

exec("./modules/main/main.cs");
exec("./modules/loadingGUI/main.cs");
exec("./modules/serverBrowser/main.cs");
exec("./modules/api/main.cs");