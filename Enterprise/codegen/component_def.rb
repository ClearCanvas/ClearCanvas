require 'class_def'
require 'type_name_utils'

# Represents the definition of a component class
class ComponentDef < ClassDef
  
  def initialize(model, componentNode, namespace, directives)
    super(model, TypeNameUtils.getShortName(componentNode.attributes['class']), namespace, directives)
    componentNode.each_element do |fieldNode|
      processField(fieldNode) if(NHIBERNATE_FIELD_TYPES.include?(fieldNode.name))
    end
  end
  
  def kind
    :component
  end
  
  def superClassName
    "ValueObject"
  end
  
  def supportClassName
    className + "Info"
  end

  def searchCriteriaClassName
    className + "SearchCriteria"
  end
  
   def searchCriteriaSuperClassName
    # a component never has a superclass, therefore always return "SearchCriteria"
   "SearchCriteria"
  end
end
