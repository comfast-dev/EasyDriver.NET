using System.Reflection;

namespace Comfast.Commons.Utils;

/// <summary>
/// Read/Set Any nested private/public Property or Field based on dot path.
/// e.g. object.ReadField("Some.Nested._privateField.Id")
/// e.g. object.SetField("newValue", "Some._nestedObj.MyProperty")
/// </summary>
public static class ReflectionUtils {
    
    public static T ReadField<T>(this object target, string fieldPath) 
        => target.ReadField<T>(fieldPath.Split('.'));
    
    public static T ReadField<T>(this object target, params string[] fieldPath) {
        var curr = target;
        foreach (var name in fieldPath) {
            if (curr == null) throw new Exception($"Can't read field {name} from null object");
            curr = GetFieldValue(curr, name);
        }
        
        if (curr == null) throw new Exception($"Field {fieldPath} is null");
        return (T)curr;
    }
    
    public static void SetField(this object target, object? valueToSet, string fieldPath)
        => target.SetField(valueToSet, fieldPath.Split('.'));
    
    public static void SetField(this object target, object? valueToSet, params string[] fieldPath) {
        var parentsPath = fieldPath.SkipLast(1).ToArray();
        var parent = target.ReadField<object?>(parentsPath);
        
        if (parent == null) throw new Exception("Not found field: " + parentsPath);
        
        switch (getOneFieldOrProp(parent, fieldPath.Last())) {
            case FieldInfo f: f.SetValue(parent, valueToSet); return;
            case PropertyInfo p: p.SetValue(parent, valueToSet); return;
            default: throw new Exception("Invalid type returned;");
        }
    }

    private static object? GetFieldValue(object target, string name) {
        return getOneFieldOrProp(target, name) switch {
            FieldInfo f => f.GetValue(target),
            PropertyInfo p => p.GetValue(target),
            _ => throw new Exception("Invalid type returned;")
        };
    }
    
    private static MemberInfo? getOneFieldOrProp(object target, string name) {
        var prop = target.GetType().GetProperty(name);
        if (prop != null) return prop;
        
        var privateField = target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
        if (privateField != null) return privateField;
        
        throw new Exception("Should never happen");
    }
}