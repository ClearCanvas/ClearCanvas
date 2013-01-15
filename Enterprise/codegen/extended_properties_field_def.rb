require 'collection_field_def'
require 'type_name_utils'

# Represents the definition of a field that is a dictionary of extended properties
class ExtendedPropertiesFieldDef < CollectionFieldDef
  
  def initialize(model, fieldNode, defaultNamespace)
    super(model, fieldNode, defaultNamespace)
  end

  # true if this field should appear in SearchCriteria classes 
  def isSearchable
    true
  end
  
  # the C# datatype of the field to be used in SearchCriteria classes
  def searchCriteriaDataType
	"ExtendedPropertiesSearchCriteria"
  end
  
  # the C# return datatype of the field to be used in SearchCriteria classes
  # (this is necessary because the return type is potentially an interface)
  def searchCriteriaReturnType
	"ExtendedPropertiesSearchCriteria"
  end
  
  def attributes
	attrs = super
	attrs << "ExtendedPropertiesCollection"
	attrs
  end

end
