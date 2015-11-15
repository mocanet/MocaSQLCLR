
Imports System.Reflection

Imports Moca.Aop

Namespace Db.Interceptor

	''' <summary>
	''' DBのカラム情報を返す Getter メソッドインターセプター
	''' </summary>
	''' <remarks></remarks>
	Public Class TableInfoInterceptor
		Implements IMethodInterceptor

		''' <summary>Table</summary>
		Private _info As DbInfoTable

#Region " コンストラクタ "

		''' <summary>
		''' コンストラクタ
		''' </summary>
		''' <remarks></remarks>
		Public Sub New(ByVal info As DbInfoTable)
			_info = info
		End Sub

#End Region

		Public Function Invoke(ByVal invocation As Aop.IMethodInvocation) As Object Implements Aop.IMethodInterceptor.Invoke
			Return _info
		End Function

	End Class

End Namespace
