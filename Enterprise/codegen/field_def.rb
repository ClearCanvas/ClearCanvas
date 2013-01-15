require 'element_def'

# represents the definition of a field within a ClassDef
# see subclasses PrimitiveFieldDef, EnumFieldDef, ComponentFieldDef, EntityFieldDef, CollectionFieldDef, UserTypeFieldDef
class FieldDef < ElementDef
  attr_reader  :model,
               :fieldName,            # the name of the field
               :accessorName,         # the name of the property
               :dataType,             # the type of the field, and return type of the property
               :hasGetter,            # true if a getter should be generated
               :hasSetter,            # true if a setter should be generated
	           :setterAccess,    	  # access level of the setter, returns nil by default (eg no access modifier), or protected, internal, private
               :nullable              # true if the field is nullable
   
  def initialize(model, fieldNode)
    @model = model
    
    # establish some basic things about the field
    
    # determine the access strategy - if the strategy contains multiple parts separated by . then only take the first part
    # use "property" as the default strategy, as hibernate does
    access = (fieldNode.attributes['access'] || "property").split('.')[0];
    
    @hasGetter = ['property', 'nosetter'].include?(access)
    @hasSetter = (access == 'property')
    
    @accessorName = fieldNode.attributes['name']
    @fieldName = "_" + @accessorName[0..0].downcase + @accessorName[1..-1]
    
    # check for a "column" sub-element
    columnNode = fieldNode.elements['column']

    #if 'not-null' attribute is omitted, the default value is false (eg. the column is nullable)
    @nullable = columnNode ? columnNode.attributes['not-null'] != 'true' : fieldNode.attributes['not-null'] != 'true'
    
    # length of the field if specified, or nil otherwise
    @length = columnNode ? columnNode.attributes['length'] : fieldNode.attributes['length']
    
    # true if the field has a unique constraint, false otherwise
    @unique = (fieldNode.attributes['unique'] == 'true')
  end
  
  def namespace
    @model.namespace
  end
  
  # the C# datatype of the field
  def dataType
    nil # defer to subclass
  end
  
  # the initial value for the field, defaults to nil, meaning the field is not initialized
  def initialValue
    nil
  end
  
  # the C# datatype of the field to be used in support classes
  # defaults to the dataType of the field
  def supportDataType
    dataType
  end
  
  # the initial value for the field to be used in support classes
  # defaults to the initialValue of the field
  def supportInitialValue
    initialValue
  end
  
  # true if this field is a mandatory field
  # by default, a field is mandatory if it is not nullable
  def isMandatory
    return !@nullable   
  end 

  # true if this field should appear in SearchCriteria classes 
  def isSearchable
    true
  end
  
  # the C# datatype of the field to be used in SearchCriteria classes
  def searchCriteriaDataType
    "SearchCondition<#{dataType}>"
  end
  
  # the C# return datatype of the field to be used in SearchCriteria classes
  # (this is necessary because the return type is potentially an interface)
  def searchCriteriaReturnType
    "ISearchCondition<#{dataType}>"
  end
  
  def attributes
    attrs = []
    attrs << "PersistentProperty"
    attrs << "Required" if isMandatory
    attrs << "Length(#{@length})" if @length
    attrs << "Unique" if @unique
    attrs
  end
  

end
