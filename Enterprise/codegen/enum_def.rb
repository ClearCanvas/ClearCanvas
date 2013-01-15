require 'class_def'
require 'type_name_utils'

# Represents the definition of an enum class
class EnumDef < ClassDef
  attr_reader :enumName
  
  def initialize(model, classNode, namespace, directives)
    super(model, TypeNameUtils.getShortName(classNode.attributes['name']), namespace, directives)
    @enumName = @className.sub("Enum", "")
    @forceHardEnum = directives.include?("hardenum")
  end
  
  def kind
    :enum
  end
  
  # is this a "hard" enum (e.g. does it have a corresponding C# enum)
  def isHardEnum
    # return true if we can find an entity/component that has a field with a datatype that is the C# enum that corresponds to this class
    @forceHardEnum ||
	    (model.entityDefs + model.componentDefs).find { |classDef| 
		    classDef.fields.find { |fieldDef| fieldDef.kind == :enum && fieldDef.dataType + "Enum" == self.qualifiedName } }
  end
end
