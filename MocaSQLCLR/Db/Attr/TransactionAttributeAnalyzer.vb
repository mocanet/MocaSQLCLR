
Imports Moca.Aop
Imports Moca.Attr
Imports Moca.Util

Namespace Db.Attr

	''' <summary>
	''' トランザクション属性解析
	''' </summary>
	''' <remarks></remarks>
	Public Class TransactionAttributeAnalyzer
		Implements IAttributeAnalyzer

		Public Function Analyze(ByVal target As System.Type) As Di.MocaComponent Implements IAttributeAnalyzer.Analyze
			Return Nothing
		End Function

		Public Function Analyze(ByVal target As Object, ByVal field As System.Reflection.FieldInfo) As Di.MocaComponent Implements IAttributeAnalyzer.Analyze
			Return Nothing
		End Function

		Public Function Analyze(ByVal targetType As System.Type, ByVal method As System.Reflection.MethodInfo) As Aop.IAspect() Implements IAttributeAnalyzer.Analyze
			Dim attr As TransactionAttribute
			Dim aspects As ArrayList

			aspects = New ArrayList()

			attr = ClassUtil.GetCustomAttribute(Of TransactionAttribute)(method)
			If attr Is Nothing Then
				Return Nothing
			End If
			aspects.Add(attr.CreateAspect(method))

			Return DirectCast(aspects.ToArray(GetType(IAspect)), IAspect())
		End Function

		Public Function Analyze(ByVal targetType As System.Type, ByVal prop As System.Reflection.PropertyInfo) As Aop.IAspect() Implements IAttributeAnalyzer.Analyze
			Return Nothing
		End Function

		Public Function Analyze(ByVal targetType As System.Type, ByVal method As System.Reflection.EventInfo) As Aop.IAspect() Implements IAttributeAnalyzer.Analyze
			Return Nothing
		End Function

	End Class

End Namespace
