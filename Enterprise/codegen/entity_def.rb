require 'class_def'

# Represents the definition of an entity class
class EntityDef < ClassDef
  attr_reader :superClassName, :isSubClass, :suppressBrokerGen, :isAbstract
  
  def initialize(model, classNode, namespace, superClassName, directives)
    super(model, TypeNameUtils.getShortName(classNode.attributes['name']), namespace, directives)
    @superClassName = superClassName
    @isSubClass = (superClassName != "Entity")
	@isAbstract = classNode.attributes['abstract'] == 'true'
    @suppressBrokerGen = directives.include?("nobroker")
    classNode.each_element do |fieldNode|
      if(NHIBERNATE_FIELD_TYPES.include?(fieldNode.name))
        processField(fieldNode) 
        processUniqueKeys(fieldNode)
      end
    end
	
	# recognize field named "Deactivation" as a special field
	@deactivationField = fields.find { |f| f.accessorName === "Deactivated" }
  end
  
  def kind
    :entity
  end
  
  def supportClassName
    className + "Info"
  end
  
  def supportSuperClassName
    @superClassName + "Info"
  end
  
  def searchCriteriaClassName
    className + "SearchCriteria"
  end
  
  def searchCriteriaSuperClassName
    # if this entity has a superclass, inherit from it's searchCriteria class, otherwise use "EntitySearchCriteria"
    superClass ? superClass.searchCriteriaClassName : "EntitySearchCriteria<"+className+">"
  end
  
  def attributes
    attrs = []
    attrs << "DeactivationFlag(\"#{@deactivationField.accessorName}\")" if @deactivationField
    attrs
  end
  
protected

  def processUniqueKeys(fieldNode)
	  #not yet implemented
  end
end
