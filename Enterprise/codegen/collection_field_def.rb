require 'field_def'
require 'type_name_utils'

# Represents the definition of a field that is a collection type,
# such as an IList or ISet
class CollectionFieldDef < FieldDef
  
  def initialize(model, fieldNode, defaultNamespace)
    super(model, fieldNode)
    @dataType = DATATYPE_MAPPINGS[fieldNode.name]
    @isLazy = (fieldNode.attributes['lazy'] == 'true')
    
    # get the element node (must exist)
    @elementNode = fieldNode.elements['element'] || fieldNode.elements['composite-element'] || fieldNode.elements['one-to-many'] || fieldNode.elements['many-to-many']
    @elementType = 
	case 
	    when @elementNode.attributes['class'] : TypeNameUtils.getQualifiedName(@elementNode.attributes['class'], defaultNamespace)
	    when @elementNode.attributes['type']
			t = @elementNode.attributes['type']
			TypeNameUtils.isHbm(t) ? TypeNameUtils.getTypeNameFromHbm(t) : TypeNameUtils.getShortName(DATATYPE_MAPPINGS[t] || t)
	end
    
    # check for an index node (may or may not exist depending on collection type)
    @indexNode = fieldNode.elements['index']
    @indexType = TypeNameUtils.getShortName(DATATYPE_MAPPINGS[@indexNode.attributes['type']] || @indexNode.attributes['type']) if (@indexNode && @indexNode.attributes['type'])
  end
  
  def kind
    :collection
  end

  def dataType
    @dataType == 'IDictionary' ? "#{@dataType}<#{@indexType}, #{@elementType}>" : "#{@dataType}<#{@elementType}>"
  end
  
  def elementType
    @elementType
  end
  
  def initialValue
    baseInitialValue = CSHARP_INITIALIZERS[@dataType]
    @dataType == 'IDictionary' ? "#{baseInitialValue}<#{@indexType}, #{@elementType}>()" : "#{baseInitialValue}<#{@elementType}>()"
  end
  
  def supportInitialValue
    "new #{supportDataType}()"
  end
  
  # a collection field is never mandatory
  def isMandatory
    false
  end
  
  # collection setters should be protected otherwise NHibernate will produce an error  
  def setterAccess
	  "protected"
  end
  
  # true if this field is a lazy collection
  def isLazy
    @isLazy
  end
  
  # searching on collection fields is not currently supported
  def isSearchable
    false
  end
 
  def attributes
    attrs = super
    if(@elementNode)
	    case
	      when @elementNode.name == 'composite-element' : attrs << "EmbeddedValueCollection(typeof(#{elementType}))"
	      #when @elementNode.name == 'one-to-many' : attrs << "OneToMany(typeof(#{elementType}))"
	      #when @elementNode.name == 'many-to-many' : attrs << "ManyToMany(typeof(#{elementType}))"
	    end
    end
    attrs
  end
  
protected
  def collectionElementClassDef
     model.findClass(@elementType)
  end
 
end
