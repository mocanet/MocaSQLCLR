
Namespace Db

	''' <summary>
	''' �֐����̃��f��
	''' </summary>
	''' <remarks></remarks>
	Public Class DbInfoFunction
		Inherits DbInfo

		''' <summary>�\�[�X</summary>
		Private _src As String

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
		''' �\�[�X�v���p�e�B
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Property Src() As String
			Get
				Return _src
			End Get
			Set(ByVal value As String)
				_src = value
			End Set
		End Property

#End Region

	End Class

End Namespace
