
Namespace Db.Attr

	''' <summary>
	''' �񖼑���
	''' </summary>
	''' <remarks>
	''' �񖼂��v���p�e�B���Ƃ͈قȂ�Ƃ��Ɏw�肷��B
	''' </remarks>
	<AttributeUsage(AttributeTargets.Property)> _
	Public Class ColumnAttribute
		Inherits Attribute

		''' <summary>�J������</summary>
		Private _columnName As String

#Region " �R���X�g���N�^ "

		''' <summary>
		''' �R���X�g���N�^
		''' </summary>
		''' <param name="columnName">��</param>
		''' <remarks></remarks>
		Public Sub New(ByVal columnName As String)
			_columnName = columnName
		End Sub

#End Region

#Region " �v���p�e�B "

		''' <summary>
		''' �񖼃v���p�e�B
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public ReadOnly Property ColumnName() As String
			Get
				Return _columnName
			End Get
		End Property

#End Region

	End Class

End Namespace
