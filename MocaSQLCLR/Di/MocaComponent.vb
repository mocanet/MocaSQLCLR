
Imports System.Reflection
Imports Moca.Aop
Imports Moca.Attr
Imports Moca.Util

Namespace Di

	''' <summary>
	''' �R���e�i�Ɋi�[����W���I�ȃR���|�[�l���g
	''' </summary>
	''' <remarks></remarks>
	Public Class MocaComponent
		Implements IDisposable

		''' <summary>�R���|�[�l���g�̃L�[</summary>
		Private _key As String
		''' <summary>���Ԃ̌^</summary>
		Private _implType As Type
		''' <summary>�t�B�[���h�̌^</summary>
		Private _fieldType As Type
		''' <summary>�A�X�y�N�g�z��</summary>
		Private _aspects() As IAspect

#Region " �R���X�g���N�^ "

		''' <summary>
		''' �R���X�g���N�^
		''' </summary>
		''' <param name="implType">���Ԃ̌^</param>
		''' <param name="fieldType">�t�B�[���h�̌^</param>
		''' <remarks></remarks>
		Public Sub New(ByVal implType As Type, ByVal fieldType As Type)
			_implType = implType
			_key = _implType.FullName
			_fieldType = fieldType
		End Sub

		''' <summary>
		''' �R���X�g���N�^
		''' </summary>
		''' <param name="key">�R���|�[�l���g�̃L�[</param>
		''' <param name="fieldType">�t�B�[���h�̌^</param>
		''' <remarks></remarks>
		Public Sub New(ByVal key As String, ByVal fieldType As Type)
			_implType = Nothing
			_key = key
			_fieldType = fieldType
		End Sub

#End Region
#Region " IDisposable Support "

		Private disposedValue As Boolean = False		' �d������Ăяo�������o����ɂ�

		' IDisposable
		Protected Overridable Sub Dispose(ByVal disposing As Boolean)
			If Not Me.disposedValue Then
				If disposing Then
					' TODO: �����I�ɌĂяo���ꂽ�Ƃ��Ƀ}�l�[�W ���\�[�X��������܂�
				End If

				' TODO: ���L�̃A���}�l�[�W ���\�[�X��������܂�

				' �ێ����Ă���I�u�W�F�N�g�̊J��
			End If
			Me.disposedValue = True
		End Sub

		' ���̃R�[�h�́A�j���\�ȃp�^�[���𐳂��������ł���悤�� Visual Basic �ɂ���Ēǉ�����܂����B
		Public Sub Dispose() Implements IDisposable.Dispose
			' ���̃R�[�h��ύX���Ȃ��ł��������B�N���[���A�b�v �R�[�h����� Dispose(ByVal disposing As Boolean) �ɋL�q���܂��B
			Dispose(True)
			GC.SuppressFinalize(Me)
		End Sub
#End Region
#Region " �v���p�e�B "

		''' <summary>
		''' �A�X�y�N�g�z��v���p�e�B
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Property Aspects() As IAspect()
			Get
				Return _aspects
			End Get
			Set(ByVal value As IAspect())
				_aspects = value
			End Set
		End Property

		''' <summary>
		''' ���Ԃ̌^�v���p�e�B
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public ReadOnly Property ImplType() As Type
			Get
				Return _implType
			End Get
		End Property

		''' <summary>
		''' �L�[�v���p�e�B
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public ReadOnly Property Key() As String
			Get
				Return _key
			End Get
		End Property

		''' <summary>
		''' �t�B�[���h�̌^�v���p�e�B
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public ReadOnly Property FieldType() As Type
			Get
				Return _fieldType
			End Get
		End Property

#End Region

		''' <summary>
		''' �I�u�W�F�N�g���C���X�^���X�����ĕԂ��܂��B
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Overridable Function Create() As Object
			If _aspects Is Nothing Then
				Return createObject(Nothing)
			End If
			If _aspects.Length = 0 Then
				Return createObject(Nothing)
			End If
			Return createProxyObject(Nothing)
		End Function

		''' <summary>
		''' �I�u�W�F�N�g���C���X�^���X�����ĕԂ��܂��B
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Overridable Function Create(ByVal target As Object) As Object
			If _aspects Is Nothing Then
				Return createObject(target)
			End If
			If _aspects.Length = 0 Then
				Return createObject(target)
			End If
			Return createProxyObject(target)
		End Function

		''' <summary>
		''' �I�u�W�F�N�g���C���X�^���X�����ĕԂ��܂��B
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		Protected Overridable Function createObject(ByVal target As Object) As Object
			Dim val As Object
			val = ClassUtil.NewInstance(_implType)
			Return val
		End Function

		''' <summary>
		''' �I�u�W�F�N�g���v���L�V�Ƃ��ăC���X�^���X�����ĕԂ��܂��B
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		Protected Overridable Function createProxyObject(ByVal target As Object) As Object
			Dim val As Object
			Dim proxy As AopProxy

			proxy = New AopProxy(_implType, _aspects)
			val = proxy.Create()

			Return val
		End Function

	End Class

End Namespace
