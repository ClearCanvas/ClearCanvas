using System.Configuration;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Desktop;
namespace ClearCanvas.ImageViewer.Tools.MedicalPhysics.Properties {


    // This class allows you to handle specific events on the settings class:
    //  The SettingChanging event is raised before a setting's value is changed.
    //  The PropertyChanged event is raised after a setting's value is changed.
    //  The SettingsLoaded event is raised after the setting values are loaded.
    //  The SettingsSaving event is raised before the setting values are saved.
    [SettingsGroupDescription("Stores settings for the medical physics module.")]
    [SettingsProvider(typeof(StandardSettingsProvider))]
    internal sealed partial class Settings {
        
        public Settings() {
           
        }
        
        private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e) {
            // Add code to handle the SettingChangingEvent event here.
        }
        
        private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e) {
            // Add code to handle the SettingsSaving event here.
        }
    }
}
