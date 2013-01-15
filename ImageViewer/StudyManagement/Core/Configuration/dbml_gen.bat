:: Re-generates the dbml file from the sdf file
:: The output is sent to a file name temp.dbml.  From there, you should manually merge your changes into the ConfigurationDataContext.dbml file.  Do not overwrite ConfigurationDataContext.dbml entirely.

sqlmetal /pluralize /namespace:ClearCanvas.ImageViewer.StudyManagement.Core.Configuration /dbml:temp.dbml /context:ConfigurationDataContext configuration.sdf