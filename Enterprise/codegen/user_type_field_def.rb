require 'field_def'
require 'type_name_utils'

# Represents the definition of a field that is based on an NHibernate User-defined type mapper
class UserTypeFieldDef < FieldDef
  def initialize(model, fieldNode)
    super(model, fieldNode)
    @dataType = TypeNameUtils.getTypeNameFromHbm(fieldNode.attributes['type'])
  end

  def kind
    :userType
  end
  
  def dataType
    @dataType
  end
  
  def initialValue
    "new #{dataType}()"
  end
  
  # a usertype field is mandatory if it is not nullable
  # TODO: does this actually work?
  def isMandatory
    !nullable
  end
  
  # usertypes are not searchable unless they have a corresponding C# SearchCriteria object defined
  # TODO: how do we accomodate this?
  def isSearchable
    false
  end

end
