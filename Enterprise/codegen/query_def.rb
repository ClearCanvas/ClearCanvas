require 'element_def'
require 'query_result_def'
require 'query_criteria_def'
require 'model'
require 'type_name_utils'

# Represents the definition of a query (an HQL report query)
class QueryDef < ElementDef
  attr_reader   :queryName,
                :isAbstract
  
  def initialize(model, queryNode, defaultNamespace)
    @model = model
    @joins = []
    @resultFieldMappings = []
    @criteriaFieldMappings = []
    
    @queryName = TypeNameUtils.getShortName(queryNode.attributes['name'])
    @namespace = TypeNameUtils.getNamespace(queryNode.attributes['name']) || defaultNamespace
    @isAbstract = queryNode.attributes['abstract'] ? (queryNode.attributes['abstract'].downcase() == 'true') : false;
    @superClassName = queryNode.attributes['extends'] ? TypeNameUtils.getQualifiedName(queryNode.attributes['extends'], defaultNamespace) : nil
    queryNode.each_element do |subNode|
      case subNode.name
        when 'root'
          @root = QueryRoot.new(self, subNode, defaultNamespace)
        when 'join'
          @joins << QueryJoin.new(self, subNode, defaultNamespace)
        when 'result'
          @resultFieldMappings = createMappings(subNode, defaultNamespace);
          @resultClassName = TypeNameUtils.getQualifiedName(subNode.attributes['name'], defaultNamespace)
          resultSuperClassName = subNode.attributes['extends'] ? TypeNameUtils.getQualifiedName(subNode.attributes['extends'], defaultNamespace) : nil
          @resultClass = QueryResultDef.new(model, @resultClassName, defaultNamespace, resultSuperClassName, self)
        when 'criteria'
          @criteriaFieldMappings = createMappings(subNode, defaultNamespace);
          @criteriaClassName = TypeNameUtils.getQualifiedName(subNode.attributes['name'], defaultNamespace)
          criteriaSuperClassName = subNode.attributes['extends'] ? TypeNameUtils.getQualifiedName(subNode.attributes['extends'], defaultNamespace) : 'ClearCanvas.Enterprise.Core.SearchCriteria'
          @criteriaClass = QueryCriteriaDef.new(model, @criteriaClassName, defaultNamespace, criteriaSuperClassName, self)
      end
    end
  end
  
  def suppressCodeGen
    @isAbstract # suppress code generation if this queryDef is 'abstract'
  end
  
  def elementName
    @queryName
  end
  
  def qualifiedName
    TypeNameUtils.getQualifiedName(@queryName, @namespace)
  end
  
  def namespace
    @namespace
  end
  
  def superClass
    return nil if(@superClassName == nil)
    sc = @model.findDef(@superClassName)
    raise "QueryDef " + qualifiedName + " extends undefined class " + @superClassName if(sc == nil)
    return sc
  end
  
  # returns a QueryResultDef that defines a result class for this QueryDef
  def resultClass(includeInherited = true)
    return @resultClass if (@resultClass || !includeInherited)
    if(superClass)
      return superClass.resultClass(includeInherited)
    else
      raise "QueryDef " + qualifiedName + " does not define or inherit a result mapping"
    end
  end
  
  # returns a QueryCriteriaDef that defines a criteria class for this QueryDef
  def criteriaClass(includeInherited = true)
    return @criteriaClass if (@criteriaClass || !includeInherited)
    if(superClass)
      return superClass.criteriaClass(includeInherited)
    else
       raise "QueryDef " + qualifiedName + " does not define or inherit a criteria mapping"
    end
  end
  
  # returns the QueryRoot object for this QueryDef
  def root(includeInherited = true)
    @root || (includeInherited && superClass ? superClass.root(includeInherited) : nil)
  end
  
  # returns a list of QueryJoin objects, representing the joins, in the ordered they were declared,
  # proceeding up the inheritance chain if includeInherited = true
  # use the orderedJoins property to obtain a list of joins ordered by dependency
  def joins(includeInherited = true)
    @joins + (includeInherited && superClass ? superClass.joins(includeInherited) : [])
  end
  
  # returns a list of QueryFieldMapping objects representing the criteria field mappings for the QueryCriteriaDef (see criteriaClass property)
  def criteriaFieldMappings(includeInherited = true)
    @criteriaFieldMappings + (includeInherited && superClass ? superClass.criteriaFieldMappings(includeInherited) : [])
  end
  
  # returns a list of QueryFieldMapping objects representing the result field mappings for the QueryResultDef (see resultClass property)
  def resultFieldMappings(includeInherited = true)
    @resultFieldMappings + (includeInherited && superClass ? superClass.resultFieldMappings(includeInherited) : [])
  end
  
  # returns the data type for the specified HQL source
  # source is a string HQL expression in the form of x.Property1.Property2.etc, where x is an HQL alias
  def resolveHqlSource(source)
    sourcePath = source.split('.')
    sourcePart = (joins + [root]).find {|part| part.hqlAlias == sourcePath[0]}
    resolveDataType(sourcePart.dataType, sourcePath[1..-1])
  end
  
  # returns a list of QueryJoin objects, including inherited joins, ordered by dependency
  def orderedJoins
    ordered = []
    joins.each do |join|
      sourcePath = join.source.split('.')
      dependsOnJoin = ordered.find {|x| x.hqlAlias == sourcePath[0] }
      if(dependsOnJoin)
        ordered.insert(ordered.index(dependsOnJoin), join)
      else
        ordered << join
      end
    end
    ordered.reverse
  end
  
protected
  # creates the set of QueryFieldMapping objects for the specified baseNode, which is either a "criteria" node or a "result" node
  def createMappings(baseNode, defaultNamespace)
    fields = []
    baseNode.each_element do |fieldNode|
      fields << QueryFieldMapping.new(self, fieldNode, defaultNamespace, fields.length)
    end
    return fields
  end
  
  # determines the dataType of the specified path wrt to the starting dataType
  # pathParts is an array of property names
  # e.g. if pathParts = ['Mrn', 'Id'] and dataType = 'ClearCanvas.Healthcare.PatientProfile'
  # this method will determine that PatientProfile.Mrn.Id is of type string and return 'string'
  def resolveDataType(dataType, pathParts)
    return dataType if(pathParts.length == 0)
    
    classDef = @model.findDef(dataType)
    fieldDef = classDef.findField(pathParts[0])
    return resolveDataType((fieldDef.kind == :collection) ? fieldDef.elementType : fieldDef.dataType, pathParts[1..-1])
  end
end

# Represents the definition of the root of an HQL query (e.g. "from Order o")
class QueryRoot
  attr_reader :dataType,  # the data type of the root (e.g. Order)
              :hqlAlias   # the alias (e.g. o)
  def initialize(queryDef, rootNode, defaultNamespace)
    @queryDef = queryDef
    @dataType = TypeNameUtils.getQualifiedName(rootNode.attributes['class'], defaultNamespace)
    @hqlAlias = rootNode.attributes['alias']
  end
end

# Represents an HQL join definition (e.g. "join o.Patient as p")
class QueryJoin
  attr_reader :hqlAlias,  # the alias (e.g. p)
              :source     # the source (e.g. o.Patient)
  def initialize(queryDef, joinNode, defaultNamespace)
    @queryDef = queryDef
    @hqlAlias = joinNode.attributes['alias']
    @source = joinNode.attributes['source']

    #get the dataType, if specified - otherwise it will be computed dynamically later
    @dataType = joinNode.attributes['class'] ? TypeNameUtils.getQualifiedName(joinNode.attributes['class'], defaultNamespace) : nil
  end
  
  # returns the dataType of this join, as defined by its source
  def dataType
    return @dataType if @dataType != nil
    @dataType = @queryDef.resolveHqlSource(@source)
  end
end

# Represents a mapping between a property of a criteria/result class and an HQL expression that defines that property
class QueryFieldMapping
  attr_reader :name,    # the name of the mapped property
              :source,  # the HQL expression that provides data for the property
              :index    # a zero-based index for this mapping
  def initialize(queryDef, baseNode, defaultNamespace, index)
    @queryDef = queryDef
    @name = baseNode.attributes['name']
    @source = baseNode.attributes['source']
    @index = index

    #get the dataType, if specified - otherwise it will be computed dynamically later
    @dataType = baseNode.attributes['type'] ? TypeNameUtils.getQualifiedName(baseNode.attributes['type'], defaultNamespace) : nil
  end
  
  # returns the dataType of the mapped property as defined by the HQL expression
  def dataType
    return @dataType if @dataType != nil
    @dataType = @queryDef.resolveHqlSource(@source)
  end
end

