# This file is useful when doing development on Linux using X-Develop
# X-Develop will not execute the post-build steps that copy the files
# to the appropriate directories, so this manual script is helpful
# Copy it into the output folder (e.g. "bin/Debug Mono") for use.


# Delete contents of plugin directory
rm -r -f ./plugins
rm -r -f ./common

# Make required directories
mkdir ./common
mkdir ./plugins

# Copy shared assemblies
cp "../../../../Common/bin/Debug Mono/ClearCanvas.Common.dll" ./common
cp "../../../../Dicom/bin/Debug Mono/ClearCanvas.Dicom.dll" ./common
cp "../../../../Dicom/OffisWrapper/csharp/Debug/ClearCanvas.Dicom.OffisWrapper.dll" ./common
cp "../../../../Dicom/OffisWrapper/cppwrapper/Debug/libOffisDcm.so" ./common
cp "../../../SplashScreen/bin/Debug Mono/ClearCanvas.Workstation.SplashScreen.dll" ./common

# Copy plugin assemblies
cp "../../../View/GTK/bin/Debug Mono/ClearCanvas.Workstation.View.GTK.dll" ./plugins
cp "../../../Model/bin/Debug Mono/ClearCanvas.Workstation.Model.dll" ./plugins
cp "../../../Tools/Measurement/bin/Debug Mono/ClearCanvas.Workstation.Tools.Measurement.dll" ./plugins
cp "../../../Tools/Standard/bin/Debug Mono/ClearCanvas.Workstation.Tools.Standard.dll" ./plugins
cp "../../../Edit/bin/Debug Mono/ClearCanvas.Workstation.Edit.dll" ./plugins
cp "../../../Layout/Basic/bin/Debug Mono/ClearCanvas.Workstation.Layout.Basic.dll" ./plugins
cp "../../../Renderer/GDI/bin/Debug Mono/ClearCanvas.Workstation.Renderer.GDI.dll" ./plugins
cp "../../../StudyFinders/Local/bin/Debug Mono/ClearCanvas.Workstation.StudyFinders.Local.dll" ./plugins
cp "../../../StudyLoaders/Local/bin/Debug Mono/ClearCanvas.Workstation.StudyLoaders.Local.dll" ./plugins

# Temp HACK - Copy shared assemblies into root folder, because "common" isn't known by mono
cp "../../../../Common/bin/Debug Mono/ClearCanvas.Common.dll" .
cp "../../../../Dicom/bin/Debug Mono/ClearCanvas.Dicom.dll" .
cp "../../../../Dicom/OffisWrapper/csharp/Debug/ClearCanvas.Dicom.OffisWrapper.dll" .
cp "../../../../Dicom/OffisWrapper/cppwrapper/Debug/libOffisDcm.so" .
