
using System.Linq;
using JetBrains.Metadata.Reader.API;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

public static class ExpressionReferenceUtils
{
    public static readonly IClrTypeName LayerMask = new ClrTypeName("UnityEngine.LayerMask");
    
    public static bool IsLayerMaskNameToLayerMethod(this IInvocationExpression expr)
    {
        return IsSpecificMethod(expr, LayerMask, "Q","ArrQ");
    }
    private static bool IsSpecificMethod(IInvocationExpression invocationExpression, IClrTypeName typeName, params string[] methodNames)
    {
        var declaredElement = invocationExpression.Reference.Resolve().DeclaredElement as IMethod;
        if (declaredElement == null)
            return false;

        if (methodNames.Any(t => t.Equals(declaredElement.ShortName)))
            return true;//declaredElement.GetContainingType()?.GetClrName().Equals(typeName) == true;
        return false;
    }
}