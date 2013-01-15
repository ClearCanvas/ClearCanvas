require 'element_def'
require 'field_def'
require 'field_def_factory'
require 'type_name_utils'

# represents the definition of a logical class (a class as defined by an NHibernate mapping).
# A number of C# classes that serve different purposes will typically be generated for each logical class
# There are several different categories of logical classes:
# see subclasses EnumDef, EntityDef, and ComponentDef
class ClassDef < ElementDef
  attr_reader :className, :model, :fields
  
  # model - the model to which this class belongs
  # className - the short name of this class
  # namespace - the namespace in which this class is contained
  # directives - array of directives that affect code generation for this class
  def initialize(model, className, namespace, directives)
    @model = model
    @className = className
    @namespace = namespace
    @directives = directives
    @fields = []
  end
  
  def elementName
    @className
  end
  
  def namespace
    @namespace
  end
  
  def qualifiedName
    TypeNameUtils.getQualifiedName(@className, @namespace)
  end
  
  # the kind of class (:entity, :enum, :component)
  def kind
    nil # defer to subclass
  end
  
  def suppressCodeGen
    @directives.include?("ignore")
  end
  
  # returns the set of non-collection fields defined in this class, not including superclass fields
  def singleValuedFields
    @fields.select {|f| f.kind != :collection}
  end
  
  # returns the set of mandatory (non-nullable) fields defined in this class, not including superclass fields
  def mandatoryFields
    @fields.select {|f| f.isMandatory }
  end
  
  # true if this class is a subclass
  def isSubClass
    false # default to false
  end
  
  # returns the superclass as a ClassDef, or null if the superclass is "Entity"
  def superClass
    return nil if !isSubClass
    @model.findDef(@superClassName)
  end
  
  # returns the set of inherited fields as an array of FieldDef
  def inheritedFields
    superClass ? (superClass.inheritedFields + superClass.fields) : []
  end
  
  # returns the set of inherited fields that are mandatory (non-nullable) as an array of FieldDef
  def inheritedMandatoryFields
    superClass ? (superClass.mandatoryFields + superClass.inheritedMandatoryFields) : []
  end
  
  # returns the set of fields that require initialization
  def initializedFields
    @fields.select {|f| f.initialValue }
  end
  
  # returns the set of fields that are searchable (a search criteria or condition can be created for the field)
  def searchableFields
    @fields.select {|f| f.isSearchable }
  end
  
  # returns the FieldDef matching the specified fieldName, or nil if not found
  # searches the inheritance chain if the field is not found in this class
  # the fieldName may be either the name of the field or its property accessor (e.g. _field or Field)
  def findField(fieldName)
    return (@fields.find {|f| f.fieldName == fieldName || f.accessorName == fieldName}) || (isSubClass ? superClass.findField(fieldName) : nil)
  end
  
  # gets the name of the search criteria class to generate for this classDef, or nil if not applicable
  def searchCriteriaClassName
    nil # defer to subclass
  end
  
  # same as searchCriteriaClassName, but returns a qualified version
  def searchCriteriaQualifiedClassName
    searchCriteriaClassName ? TypeNameUtils.getQualifiedName(searchCriteriaClassName, @namespace) : nil
  end
  
protected  
  # processes fields to create instances of FieldDef
  # a "field" is a node of type property, map, set, many-to-one, and others
  def processField(fieldNode)
    field = FieldDefFactory.CreateFieldDef(@model, fieldNode, namespace)
    @fields << field
    
    # check for components/composite elements, and process them
    if(fieldNode.name == 'component' || fieldNode.name == 'nested-composite-element')
      # pass the namespace of this class as the default namespace for the component
      processComponent(fieldNode, namespace, @directives) 
    elsif(field.kind == :collection && (compositeElementNode = fieldNode.elements['composite-element']))
      processComponent(compositeElementNode, namespace, @directives)
    end
  end
  
  # processes componentNode to create instances of ComponentDef
  def processComponent(componentNode, defaultNamespace, directives)
   componentDef = ComponentDef.new(@model, componentNode, defaultNamespace, directives)
   
   # only add the component def to the model if it wasn't already defined by another entity class
   @model.addDef(componentDef.qualifiedName, componentDef) if !@model.findDef(componentDef.qualifiedName)
  end
end
