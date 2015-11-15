
Namespace Db.Helper

	''' <summary>
	''' ヘルパークラスのファクトリー
	''' </summary>
	''' <remarks></remarks>
	Public Class DbAccessHelperFactory

#Region " Declare "

		''' <summary>構成ファイルの接続文字列セクション又はDB接続文字列を管理</summary>
		Private _dbSetting As DbSetting

#End Region

#Region " コンストラクタ "

		''' <summary>
		''' コンストラクタ
		''' </summary>
		''' <param name="dbSetting"></param>
		''' <remarks></remarks>
		Public Sub New(ByVal dbSetting As DbSetting)
			_dbSetting = dbSetting
		End Sub

#End Region

#Region " Methods "

		''' <summary>
		''' ヘルパークラス生成
		''' </summary>
		''' <param name="dba"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Function Create(ByVal dba As IDao) As IDbAccessHelper
			Select Case _dbSetting.ProviderName
				Case "System.Data.SqlClient"
					Return New SqlDbAccessHelper(dba)

				Case Else
					Return Nothing
			End Select
		End Function

#End Region

	End Class

End Namespace
