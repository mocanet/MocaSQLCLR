
Namespace Db

	''' <summary>
	''' ����̃��f��
	''' </summary>
	''' <remarks></remarks>
	Public Class DbInfoColumn
		Inherits DbInfo

		''' <summary>�ő包��</summary>
		Private _maxLength As Integer

		Private _precision As Integer

		Private _scale As Integer

		Private _uniCode As Boolean

		Private _columnType As Object

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

		''' <summary>�ő包��</summary>
		Public Property MaxLength() As Integer
			Get
				Return _maxLength
			End Get
			Set(ByVal value As Integer)
				_maxLength = value
			End Set
		End Property

		''' <summary>���j�R�[�h�����񂩂ǂ���</summary>
		Public Property UniCode() As Boolean
			Get
				Return _uniCode
			End Get
			Set(ByVal value As Boolean)
				_uniCode = value
			End Set
		End Property

		''' <summary>�ő包���i���p 1 �o�C�g�A�S�p 2 �o�C�g�Ƃ��āj</summary>
		Public ReadOnly Property MaxLengthB() As Integer
			Get
				Return CInt(IIf(_uniCode, _maxLength, _maxLength * 2))
			End Get
		End Property

		''' <summary>�����_�̉E������э����ɂ���ۑ��ł���ő啶��</summary>
		Public Property Precision() As System.Int32
			Get
				Return Me._precision
			End Get
			Set(ByVal value As System.Int32)
				Me._precision = value
			End Set
		End Property

		''' <summary>�����_�̉E���ɂ���ۑ��ł���ő啶��</summary>
		Public Property Scale() As System.Int32
			Get
				Return Me._scale
			End Get
			Set(ByVal value As System.Int32)
				Me._scale = value
			End Set
		End Property

		''' <summary>��̌^�I�u�W�F�N�g</summary>
		Public Property ColumnType() As System.Object
			Get
				Return Me._columnType
			End Get
			Set(ByVal value As System.Object)
				Me._columnType = value
			End Set
		End Property

#End Region

	End Class

End Namespace
