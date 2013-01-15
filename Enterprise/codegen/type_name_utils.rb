# Contains static utility methods for working with .NET type names
class TypeNameUtils

  # returns the short name of the .NET type (removes namespace and assembly qualifier)
  def TypeNameUtils.getShortName(typeName)
    TypeNameUtils.removeAssemblyQualifier(typeName).split('.')[-1]
  end
  
  # returns the namespace of the type, or nil if typeName does not contain a namespace
  def TypeNameUtils.getNamespace(typeName)
    rightMostDotPos = TypeNameUtils.removeAssemblyQualifier(typeName).rindex('.')
    rightMostDotPos ? typeName[0, rightMostDotPos] : nil
  end
  
  # removes the assembly qualifier from the typeName, returning the namespace-qualified type
  def TypeNameUtils.removeAssemblyQualifier(typeName)
    commaPos = typeName.index(",")
    commaPos ? typeName[0, commaPos] : typeName
  end
  
  # returns the specified typeName as a qualified type, qualifying it with the specified defaultNamespace if it is not already qualified
  def TypeNameUtils.getQualifiedName(typeName, defaultNamespace)
    # if typeName is a short name, qualify it with the specified namespace
    # otherwise, assume it is a long name, and just return it (but without assembly qualifier)
    (typeName == TypeNameUtils.getShortName(typeName)) ? defaultNamespace + "." + typeName : TypeNameUtils.removeAssemblyQualifier(typeName)
  end
  
  # extracts the name of the underlying type from an "Hbm" type
  # this is a hack, and uses the convention that Y.Hibernate.XHbm --> Y.X
  # returns a qualified name
  def TypeNameUtils.getTypeNameFromHbm(qualifiedHbmName)
    # remove the assembly name if present
    # remove "EnumHbm" or "Hbm" - this is a hack to extract the underlying datatype from "mapper" types
    shortName = TypeNameUtils.getShortName(qualifiedHbmName).sub(/EnumHbm$/, "").sub(/Hbm$/, "")

    # remove the final ".Hibernate" element from the namespace
    namespace = TypeNameUtils.getNamespace(qualifiedHbmName).sub(/.Hibernate$/, "")
	
	# another hack - map ClearCanvas.Enterprise to ClearCanvas.Enterprise.Core
	namespace = (namespace + ".Core") if namespace =~ /.Enterprise$/

    # return the qualified underlying type
    TypeNameUtils.getQualifiedName(shortName, namespace)
  end
  
  # tests if the specified type name, which may be qualified, is an "EnumHbm" type
  def TypeNameUtils.isEnumHbm(typeName)
	TypeNameUtils.removeAssemblyQualifier(typeName) =~ /EnumHbm$/
  end

  # tests if the specified type name, which may be qualified, is an "Hbm" type
  def TypeNameUtils.isHbm(typeName)
	TypeNameUtils.removeAssemblyQualifier(typeName) =~ /Hbm$/
  end
end
