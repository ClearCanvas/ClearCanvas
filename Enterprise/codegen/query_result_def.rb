require 'query_class_def'

# Represents the definition of a query criteria class
class QueryResultDef < QueryClassDef
  def initialize(model, className, defaultNamespace, superClassName, queryDef)
    super(model, className, defaultNamespace, superClassName, queryDef)
  end
  
  # returns the set of QueryFieldDef objects that represent the fields of this class
  def fields
    return @fields if @fields != nil
    @fields = self.queryDef.resultFieldMappings(false).map {|mapping| QueryFieldDef.new(model, mapping)}
  end
end
