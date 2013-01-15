The AjaxControlToolkit.dll in this folder is not the original AjaxControlToolkit.dll from MS.

It has been patched with the fix for the Hungarian bug (http://ajaxcontroltoolkit.codeplex.com/workitem/23221)

Steps to recompile this patch:

1. Download Ajax Control Toolkit rev 10920 (http://ajaxcontroltoolkit.codeplex.com/releases/view/4941)
2. Replace the original source file with the files located in AjaxControlToolkit_10920_patch.zip
3. Build the solution