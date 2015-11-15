
Namespace Db.Attr

	''' <summary>
	''' DBMS����
	''' </summary>
	''' <remarks>
	''' <see cref="IDbAccess"/> �N���X�� DBMS ���֘A�t����Ƃ��Ɏg�p����B
	''' </remarks>
	<AttributeUsage(AttributeTargets.Class Or AttributeTargets.Interface)> _
	Public Class DbmsAttribute
		Inherits Attribute

		''' <summary>�L�[�l</summary>
		Private _connectionString As String

#Region " �R���X�g���N�^ "

		''' <summary>
		''' �R���X�g���N�^
		''' </summary>
		''' <param name="connectionString">ConnectionString</param>
		''' <remarks></remarks>
		Public Sub New(ByVal connectionString As String)
			_connectionString = connectionString
		End Sub

#End Region

#Region " �v���p�e�B "

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
		''' DBMS��Ԃ�
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Function GetDbms() As Dbms
			Return DbmsManager.GetDbms(ConnectionString)
		End Function

	End Class

End Namespace
