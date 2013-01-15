:: Re-generates the dbml file from the sdf file
:: The output is sent to a file name temp.dbml.  From there, you should manually merge your changes into the DicomStoreDataContext.dbml file.  Do not overwrite DicomStoreDataContext.dbml entirely.

sqlmetal /pluralize /namespace:ClearCanvas.ImageViewer.StudyManagement.Core.Storage /dbml:temp.dbml /context:DicomStoreDataContext dicom_store.sdf