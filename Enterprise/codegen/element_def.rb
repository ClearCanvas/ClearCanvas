require 'rexml/document'
require 'constants'

# Base class for all definitions in the model
# the Model itself is a subclass of ElementDef
# see other subclasses ClassDef, FieldDef
class ElementDef
  attr_reader :namespace,               # the .NET namespace of this element
              :elementName,             # the short name (unqualified name) of this element
              :qualifiedName            # the long name (namespace-qualified name) of this element
     
  # Used by the Template class to determine the binding context for all code
  # contained within the template.  The symbols in the template code are
  # resolved with respect to the binding.
  def get_binding
    binding
  end
  
  # by default, do not suppress code generation for this element
  def suppressCodeGen
    false
  end
end
