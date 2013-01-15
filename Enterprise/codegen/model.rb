require 'element_def'
require 'entity_def'
require 'enum_def'
require 'component_def'
require 'query_def'
require 'type_name_utils'

# Represents a domain model defined by a set of NHibernate XML mappings
class Model < ElementDef
  attr_reader :namespace
  
  def initialize()
    @entityDefs = []
    @enumDefs = []
    @componentDefs = []
    @queryDefs = []
    
    @symbolMap = {}
  end
  
  def elementName
    @namespace + " Model"
  end
  
  # add the specified file to the model
  # directives is an array of strings that affect code generation. valid directives are:
  #	"ignore" - suppress code generation for all classes defined in this file
  #	"hardenum" - for enum classes only, forces generation of hard enum classes, even if not referenced
  def add(fileName, directives)
    case
      when fileName.include?('.hbm.xml') : addHbmFile(fileName, directives)
      when fileName.include?('.hrq.xml') : addHrqFile(fileName)
    end
  end
  
  # searches the model for the specified definition, and returns the ElementDef or nil if not found
  def findDef(qualifiedDefName)
    @symbolMap[qualifiedDefName]
  end
  
  # adds the specified element definition to the model
  def addDef(qualifiedDefName, elementDef)
    if(@symbolMap[qualifiedDefName])
      raise "Symbol " + qualifiedDefName + " already defined"
    end
    @symbolMap[qualifiedDefName] = elementDef
  end

  def entityDefs
    @symbolMap.values.select {|v| EntityDef === v }
  end
  
  def enumDefs
    @symbolMap.values.select {|v| EnumDef === v }
  end

  def componentDefs
    @symbolMap.values.select {|v| ComponentDef === v }
  end

  def queryDefs
    @symbolMap.values.select {|v| QueryDef === v }
  end
  
protected
  def addHbmFile(hbmFile, directives)
    # read the hbm xml file
    mappings = REXML::Document.new(File.new(hbmFile))
    
    # process each class in the hbm file
    mappings.root.each_element do |c|
      if(NHIBERNATE_CLASS_TYPES.include?(c.name))
        className = c.attributes['name']
        
        # try to extract namespace from the class name, or the default namespace if not included in the class name
        namespace = TypeNameUtils.getNamespace(className) || mappings.root.attributes['namespace']
        
        if(/Enum$/.match(className))	#does the class name end with "Enum"?
          processEnum(c, namespace, directives)
        else
          superClassName = c.attributes['extends'] ? TypeNameUtils.getQualifiedName(c.attributes['extends'], namespace) : "ClearCanvas.Enterprise.Core.Entity"
          processEntity(c, namespace, superClassName, directives)
        end
      end
    end
  end
  
  def addHrqFile(hrqFile)
    # read the hrq xml file
    mappings = REXML::Document.new(File.new(hrqFile))
    
    # process each query in the hrq file
    mappings.root.each_element do |queryNode|
      if(queryNode.name == 'query')
        queryName = queryNode.attributes['name']
        
        # try to extract namespace from the class name, or the default namespace if not included in the class name
        namespace = TypeNameUtils.getNamespace(queryName) || mappings.root.attributes['namespace']
        
        processQuery(queryNode, namespace)
      end
    end
  end
  
  # processes classNode to create instances of EntityDef
  def processEntity(classNode, namespace, superClassName, directives)
    #create EntityDef for classNode
    entityDef = EntityDef.new(self, classNode, namespace, superClassName, directives)
    addDef(entityDef.qualifiedName, entityDef)
    
    #process any contained subclassses recursively
    classNode.each_element do |subclassNode|
      processEntity(subclassNode, namespace, entityDef.qualifiedName, directives) if(NHIBERNATE_CLASS_TYPES.include?(subclassNode.name))
    end
  end
  
  # processes classNode to create instances of EnumDef
  def processEnum(classNode, namespace, directives)
    enumDef = EnumDef.new(self, classNode, namespace, directives)
    addDef(enumDef.qualifiedName, enumDef)
  end
  
  def processQuery(queryNode, namespace)
    queryDef = QueryDef.new(self, queryNode, namespace)
    addDef(queryDef.qualifiedName,queryDef)
  end
   
end
