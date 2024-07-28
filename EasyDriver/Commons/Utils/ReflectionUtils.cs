using System.Reflection;

namespace Comfast.Commons.Utils;

/// <summary>
/// Read/Set any nested private/public Property or Field based on dot path.
/// e.g. object.ReadField("Some.Nested._privateField.Id")
/// e.g. object.SetField("newValue", "Some._nestedObj.MyProperty")
/// </summary>
public static class ReflectionUtils {
    private static BindingFlags _universalFlags = BindingFlags.Public | BindingFlags.NonPublic |
                                                  BindingFlags.Instance | BindingFlags.IgnoreCase;

    /// <summary>
    /// Rewrite all fields from source to target object.
    /// </summary>
    public static T RewriteFrom<T>(this T target, T source) {
        foreach (var prop in typeof(T).GetProperties(_universalFlags)) {
            prop.SetValue(target, prop.GetValue(source));
        }

        return target;
    }

    /// <summary>
    /// Read any nested any nested private/public Property or Field.
    /// </summary>
    /// <param name="target">object to get field from</param>
    /// <param name="fieldPath">field name or dotSeparated._nested.Path</param>
    /// <typeparam name="T">field type</typeparam>
    /// <returns>Field value</returns>
    public static T? ReadField<T>(this object target, string fieldPath) => target.ReadField<T>(fieldPath.Split('.'));

    /// <summary>
    /// Write any nested any nested private/public Property or Field.
    /// </summary>
    /// <param name="target">object to get field from</param>
    /// <param name="fieldPath">field name or dotSeparated._nested.Path</param>
    /// <param name="valueToSet">Value to be set in field</param>
    public static void WriteField(this object target, string fieldPath, object? valueToSet) =>
        target.WriteField(fieldPath.Split('.'), valueToSet);

    private static T? ReadField<T>(this object target, string[] fieldPath) {
        var curr = target;
        for (int i = 0; i < fieldPath.Length; i++) {
            var name = fieldPath[i];
            if (curr == null) {
                var parentsPath = fieldPath.SkipLast(fieldPath.Length - i);
                throw new($"Can't get field '{name}', because parent '{string.Join(".", parentsPath)}' is null");
            }

            curr = getOneMember(curr.GetType(), name).GetValue(curr);
        }

        return (T?)curr;
    }

    private static void WriteField(this object target, string[] fieldPath, object? valueToSet) {
        var parentsPath = fieldPath.SkipLast(1).ToArray();
        var name = fieldPath.Last();
        var parent = target.ReadField<object?>(parentsPath);
        if (parent == null)
            throw new($"Can't get field '{name}', because parent '{string.Join(".", parentsPath)}' is null");

        getOneMember(parent.GetType(), fieldPath.Last()).SetValue(parent, valueToSet);
    }

    public static void SetValue(this MemberInfo memberInfo, object target, object? value) {
        switch (memberInfo) {
            case FieldInfo f:
                f.SetValue(target, value);
                return;
            case PropertyInfo p:
                if (!p.CanWrite) throw new($"Can't set Property '{memberInfo.Name}' without setter.");
                p.SetValue(target, value);
                return;
            default: throw new("Fatal error: invalid MemberInfo type returned.");
        }
    }

    public static object? GetValue(this MemberInfo memberInfo, object target) {
        return memberInfo switch {
            FieldInfo f => f.GetValue(target),
            PropertyInfo p => p.GetValue(target),
            _ => throw new($"Fatal error: invalid MemberInfo type returned for field '{memberInfo.Name}'")
        };
    }

    private static MemberInfo getOneMember(Type type, string name) {
        var prop = type.GetProperty(name, _universalFlags);
        if (prop != null) return prop;

        var field = type.GetField(name, _universalFlags);
        if (field != null) return field;

        throw new ArgumentException($"Not found field '{name}' in {type.Name}");
    }
}