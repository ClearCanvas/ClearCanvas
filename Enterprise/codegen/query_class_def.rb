require 'element_def'

# Provides a common base class for QueryCriteriaDef and QueryResultDef
# This class does not have much significance in and of itself
class QueryClassDef < ElementDef
  attr_reader :className, :superClassName

  def initialize(model, className, defaultNamespace, superClassName, queryDef)
    @model = model
    @className = TypeNameUtils.getShortName(className)
    @namespace = TypeNameUtils.getNamespace(className) || defaultNamespace
    @queryDef = queryDef
    @superClassName = superClassName
  end
  
  def elementName
    @className
  end
  
  def qualifiedName
    TypeNameUtils.getQualifiedName(@className, @namespace)
  end
  
  def namespace
    @namespace
  end
  
protected
  # the QueryDef that is the parent of this object
  def queryDef
    @queryDef
  end
  
  def model
    @model
  end
end

# Represents a field of a QueryClassDef
class QueryFieldDef < ElementDef

  attr_reader :accessorName, :fieldName

  def initialize(model, mapping)
    @model = model
    @mapping = mapping
    @accessorName = mapping.name
    @fieldName = "_" + @accessorName[0..0].downcase + @accessorName[1..-1]
    @dataTypeCallback = Proc.new { mapping.dataType }
  end
  
  def elementName
    @accessorName
  end
  
  # the C# dataType of this field
  def dataType
    @mapping.dataType
  end
  
  # the C# datatype of the field to be used in SearchCriteria classes
  def searchCriteriaDataType
    classDef = @model.findDef(dataType)
    (classDef && classDef.searchCriteriaQualifiedClassName) ? classDef.searchCriteriaQualifiedClassName : "SearchCondition<#{dataType}>"
  end
  
  # the C# return datatype of the field to be used in SearchCriteria classes
  # (this is necessary because the return type is potentially an interface)
  def searchCriteriaReturnType
    classDef = @model.findDef(dataType)
    (classDef && classDef.searchCriteriaQualifiedClassName) ? classDef.searchCriteriaQualifiedClassName : "ISearchCondition<#{dataType}>"
  end
end

