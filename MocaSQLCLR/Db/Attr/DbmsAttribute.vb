
Namespace Db.Attr

	''' <summary>
	''' DBMS属性
	''' </summary>
	''' <remarks>
	''' <see cref="IDbAccess"/> クラスに DBMS を関連付けるときに使用する。
	''' </remarks>
	<AttributeUsage(AttributeTargets.Class Or AttributeTargets.Interface)> _
	Public Class DbmsAttribute
		Inherits Attribute

		''' <summary>キー値</summary>
		Private _connectionString As String

#Region " コンストラクタ "

		''' <summary>
		''' コンストラクタ
		''' </summary>
		''' <param name="connectionString">ConnectionString</param>
		''' <remarks></remarks>
		Public Sub New(ByVal connectionString As String)
			_connectionString = connectionString
		End Sub

#End Region

#Region " プロパティ "

		''' <summary>ConnectionString</summary>
		Public Property ConnectionString() As String
			Get
				Return _connectionString
			End Get
			Set(ByVal value As String)
				_connectionString = value
			End Set
		End Property

#End Region

		''' <summary>
		''' DBMSを返す
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Function GetDbms() As Dbms
			Return DbmsManager.GetDbms(ConnectionString)
		End Function

	End Class

End Namespace
