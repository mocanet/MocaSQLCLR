
Namespace Db

	''' <summary>
	''' �e�[�u�����̃��f��
	''' </summary>
	''' <remarks></remarks>
	Public Class DbInfoTable
		Inherits DbInfo

		''' <summary>����</summary>
		Private _columns As DbInfoColumnCollection

#Region " �R���X�g���N�^ "

		''' <summary>
		''' �R���X�g���N�^
		''' </summary>
		''' <param name="catalog">�J�^���O��</param>
		''' <param name="schema">�X�L�[�}��</param>
		''' <param name="name">����</param>
		''' <param name="typ">�^</param>
		''' <remarks></remarks>
		Public Sub New(ByVal catalog As String, ByVal schema As String, ByVal name As String, ByVal typ As String)
			MyBase.New(catalog, schema, name, typ)
		End Sub

#End Region

#Region " �v���p�e�B "

		''' <summary>
		''' ����
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Property Columns() As DbInfoColumnCollection
			Get
				Return _columns
			End Get
			Set(ByVal value As DbInfoColumnCollection)
				_columns = value
			End Set
		End Property

		''' <summary>
		''' �e�[�u�����̂ݕԂ�
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public ReadOnly Property ToTableName() As String
			Get
				Return Me.Name
			End Get
		End Property

#End Region

	End Class

End Namespace
