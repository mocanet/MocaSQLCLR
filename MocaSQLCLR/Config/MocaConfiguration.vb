
Imports System.Configuration

Namespace Config

	''' <summary>
	''' moca の設定
	''' </summary>
	''' <remarks></remarks>
	Public Class MocaConfiguration

#Region " Declare "

		''' <summary>シングルトン用インスタンス</summary>
		Private Shared ReadOnly _instance As MocaConfiguration = New MocaConfiguration()

#End Region

#Region " コンストラクタ "

		''' <summary>
		''' コンストラクタ
		''' </summary>
		''' <remarks></remarks>
		Protected Sub New()
			Me.TransactionType = Config.TransactionType.Scope
		End Sub

#End Region
#Region " Property "

		''' <summary>
		''' トランザクションタイプ
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Property TransactionType As TransactionType

#End Region
#Region " Method "

		''' <summary>
		''' セクション
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Shared ReadOnly Property Section() As MocaConfiguration
			Get
				Return _instance
			End Get
		End Property

#End Region

	End Class

End Namespace
