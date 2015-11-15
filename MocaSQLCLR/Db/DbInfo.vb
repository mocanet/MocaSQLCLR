
Namespace Db

	''' <summary>
	''' �f�[�^�x�[�X���̋��ʃ��f��
	''' </summary>
	''' <remarks></remarks>
	Public Class DbInfo

		''' <summary>�J�^���O����</summary>
		Private _catalog As String
		''' <summary>�X�L�[�}��</summary>
		Private _schema As String
		''' <summary>����</summary>
		Private _name As String
		''' <summary>�^</summary>
		Private _typ As String

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
			Me._catalog = catalog
			Me._schema = schema
			Me._name = name
			Me._typ = typ
		End Sub

#End Region

#Region " �v���p�e�B "

		''' <summary>
		''' �J�^���O���̃v���p�e�B
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Property Catalog() As String
			Get
				Return _catalog
			End Get
			Set(ByVal value As String)
				_catalog = value
			End Set
		End Property

		''' <summary>
		''' �X�L�[�}���v���p�e�B
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Property Schema() As String
			Get
				Return _schema
			End Get
			Set(ByVal value As String)
				_schema = value
			End Set
		End Property

		''' <summary>
		''' ���̃v���p�e�B
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Property Name() As String
			Get
				Return _name
			End Get
			Set(ByVal value As String)
				_name = value
			End Set
		End Property

		''' <summary>
		''' �^�v���p�e�B
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Property Typ() As String
			Get
				Return _typ
			End Get
			Set(ByVal value As String)
				_typ = value
			End Set
		End Property

#End Region

#Region " Overrides "

		''' <summary>
		''' ToString �̏㏑������
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Overrides Function ToString() As String
			Return Catalog & "." & Schema & "." & Typ & "." & Name
		End Function

#End Region

	End Class

End Namespace
