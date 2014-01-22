ClearCanvas
===========

Open source code base for enabling software innovation in imaging. 
The extensible and robust platform includes viewing, archiving, management, 
workflow and distribution of images as well as an open architecture for 
core competency tool development. 

How to build using Github For Windows
-------------------------------------
Read this section if you intend to build the ClearCanvas projects only, and
read the section on *Preparing clones for forked contributions* in addition to
this section if you intend to make code changes you want to share.

1. Download Github for Windows and install it.
2. Using Github for Windows clone the ClearCanvas/ClearCanvas repository to
   your local drive, for example, into ....Documents/Github/ClearCanvas.
3. Using Github for Windows clone the ClearCanvas/ReferencedAssemblies to your
   local drive, for example into ....Documents/Github/ReferencedAssemblies.
4. Using a command-line window, create a symbolic link within the ClearCanvas
   directory called "ReferencedAssemblies" that points to
   ..\ReferencedAssemblies.
   In Windows 7, the command-line to do this is
   `mklink ReferencedAssemblies ..\ReferencedAssemblies`
5. Load the ImageViewer/ImageViewer.sln into Visual Studio.
6. Build.

Preparing clones for forked contributions
-----------------------------------------
If you intend to make changes to the code and contribute it to the ClearCanvas
(upstream) repository, then you must first use Fork on Github.com to create a
clone of ClearCanvas/ClearCanvas on Github.com. The ClearCanvas/ClearCanvas
repository itself is moderated and you will not be able to automatically
publish your changes to it. 

Therefore, 

1. Use Fork on Github.com to create a clone of ClearCanvas/ClearCanvas. For
   example, to yourUserName/ClearCanvas.
2. If you intend to make changes to the ClearCanvas/ReferencedAssemblies, make
   a fork/clone of this repository as well. Otherwise, it is unnecessary to do
   so.
3. Download Github for Windows and install it.
4. Using Github for Windows clone the yourUserName/ClearCanvas repository to
   your local drive.
5. Using Github for Windows clone the yourUserName/ReferencedAssemblies
   repository to your local drive if you have forked it on Github, otherwise,
   simply clone the repository from ClearCanvas/ClearCanvas to your local drive.
6. Create a symbolic link so that ReferencedAssemblies can be referenced from
   within your ClearCanvas directory.

You are now ready to make changes to the code in your private fork/clone.


