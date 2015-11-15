Imports System.Data.SqlClient

Namespace Db.SQLCLR

	''' <summary>
	''' SQLCLRストアドプロシージャのインタフェース
	''' </summary>
	''' <remarks></remarks>
	Public Interface ISQLCLRProcedure

		''' <summary>
		''' メッセージ
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		ReadOnly Property Message As String

		''' <summary>
		''' SQLコマンド
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		ReadOnly Property Command As SqlCommand

		''' <summary>
		''' 実行
		''' </summary>
		''' <remarks></remarks>
		Sub Execute()

	End Interface

End Namespace
