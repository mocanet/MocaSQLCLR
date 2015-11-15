
Imports Moca.Attr
Imports Moca.Util

Namespace Db.Attr

	''' <summary>
	''' DAOëÆê´âêÕ
	''' </summary>
	''' <remarks></remarks>
	Public Class DaoAttributeAnalyzer
		Implements IAttributeAnalyzer

		Public Function Analyze(ByVal target As System.Type) As Di.MocaComponent Implements IAttributeAnalyzer.Analyze
			Return Nothing
		End Function

		Public Function Analyze(ByVal target As Object, ByVal field As System.Reflection.FieldInfo) As Di.MocaComponent Implements IAttributeAnalyzer.Analyze
			Dim attr As DaoAttribute

			attr = ClassUtil.GetCustomAttribute(Of DaoAttribute)(field.FieldType)
			If attr Is Nothing Then
				Return Nothing
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
