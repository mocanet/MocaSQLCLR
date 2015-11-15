
Imports Moca.Util

Namespace Attr

	''' <summary>
	''' 実態を指定する属性の解析
	''' </summary>
	''' <remarks></remarks>
	Public Class ImplementationAttributeAnalyzer
		Implements IAttributeAnalyzer

		Public Function Analyze(ByVal target As System.Type) As Di.MocaComponent Implements IAttributeAnalyzer.Analyze
			Return Nothing
		End Function

		Public Function Analyze(ByVal target As Object, ByVal field As System.Reflection.FieldInfo) As Di.MocaComponent Implements IAttributeAnalyzer.Analyze
			Dim attr As ImplementationAttribute

			attr = ClassUtil.GetCustomAttribute(Of ImplementationAttribute)(field)
			If attr Is Nothing Then
				attr = ClassUtil.GetCustomAttribute(Of ImplementationAttribute)(field.FieldType)
				If attr Is Nothing Then
					Return Nothing
				End If
			End If

			Return attr.CreateComponent(field)
		End Function

		Public Function Analyze(ByVal targetType As System.Type, ByVal method As System.Reflection.MethodInfo) As Aop.IAspect() Implements IAttributeAnalyzer.Analyze
			Return Nothing
		End Function

		Public Function Analyze(ByVal targetType As System.Type, ByVal prop As System.Reflection.PropertyInfo) As Aop.IAspect() Implements IAttributeAnalyzer.Analyze
			Return Nothing
		End Function

		Public Function Analyze(ByVal targetType As System.Type, ByVal method As System.Reflection.EventInfo) As Aop.IAspect() Implements IAttributeAnalyzer.Analyze
			Return Nothing
		End Function

	End Class

End Namespace
