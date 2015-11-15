
Imports System.Reflection

Imports Moca.Aop

Namespace Db.Interceptor

	''' <summary>
	''' DBのカラム情報を返す Getter メソッドインターセプター
	''' </summary>
	''' <remarks></remarks>
	Public Class ColumnInfoInterceptor
		Implements IMethodInterceptor

		''' <summary>桁数</summary>
		Private _info As DbInfoColumn

#Region " コンストラクタ "

		''' <summary>
		''' コンストラクタ
		''' </summary>
		''' <remarks></remarks>
		Public Sub New(ByVal info As DbInfoColumn)
			_info = info
		End Sub

#End Region

		Public Function Invoke(ByVal invocation As Aop.IMethodInvocation) As Object Implements Aop.IMethodInterceptor.Invoke
			Return _info
		End Function

	End Class

End Namespace
