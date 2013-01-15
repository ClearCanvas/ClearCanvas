require 'field_def'
require 'constants'
require 'type_name_utils'

# Represents the definition of a field that is a primitive C# type,
# such as a string or integer
class PrimitiveFieldDef < FieldDef
  
  def initialize(model, fieldNode)
    super(model, fieldNode)
    
    @dataType = TypeNameUtils.getShortName(fieldNode.attributes['type'])
    @dataType = DATATYPE_MAPPINGS[@dataType] || @dataType

    # special handling of DateTime, because we need to support nullable
    @dataType << "?" if(@dataType == 'DateTime' && nullable)
  end
  
  def kind
    :primitive
  end
  
  def dataType
    @dataType
  end

  def initialValue
    CSHARP_INITIALIZERS[@dataType]
  end
end
