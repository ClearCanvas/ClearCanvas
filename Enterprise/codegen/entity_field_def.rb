require 'field_def'
require 'type_name_utils'

# Represents the definition of a field that is a reference to another entity
class EntityFieldDef < FieldDef

  def initialize(model, fieldNode, defaultNamespace)
    super(model, fieldNode)
    @dataType = TypeNameUtils.getQualifiedName(fieldNode.attributes['class'], defaultNamespace)
  end

  def kind
    :entity
  end
  
  def dataType
    @dataType
  end
  
  def initialValue
    nil
  end
  
  def supportDataType
    "EntityRef"
  end
  
  def searchCriteriaDataType
    e = entityDef	
    #if the entityDef exists, get it's searchCriteria class name, otherwise use the default 
    #(the entityDef may not exist if our dataType refers to a class that this model doesn't know about)
    e && e.searchCriteriaQualifiedClassName ?  e.searchCriteriaQualifiedClassName : super
  end
  
  def searchCriteriaReturnType
    e = entityDef
    #if the entityDef exists, get it's searchCriteria class name, otherwise use the default 
    #(the entityDef may not exist if our dataType refers to a class that this model doesn't know about)
    e && e.searchCriteriaQualifiedClassName ?  e.searchCriteriaQualifiedClassName : super
  end
  
protected
  def entityDef
    model.findDef(@dataType)
  end
end
