function GuiPlus_LoadFiles(%path)
{
	%filePath = filePath(%path);
	for(%file=findFirstFile(%path);%file!$= "";%file=findNextFile(%path))
	{
		%fileExt = fileExt(%file);
		if(%fileExt $= ".cs" || %fileExt $= ".gui")
			exec(%file);
	}
}