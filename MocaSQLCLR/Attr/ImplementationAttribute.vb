
Imports System.Reflection
Imports Moca.Di
Imports Moca.Util

Namespace Attr

	''' <summary>
	''' ���Ԃ��w�肷�鑮��
	''' </summary>
	''' <remarks>
	''' Interface���́AField�݂̂Ɏw��ł��܂��B<br/>
	''' ���̑������w�肳�ꂽ�C���^�t�F�[�X�́A�����I�Ɉ����̃N���X�^�C�v���C���X�^���X�����ăt�B�[���h�֒������邱�Ƃ��o���܂��B<br/>
	''' </remarks>
	<AttributeUsage(AttributeTargets.Interface Or AttributeTargets.Field)> _
	Public Class ImplementationAttribute
		Inherits Attribute

		''' <summary>�w�肳�ꂽ���̉�����N���X�^�C�v</summary>
		Private _type As Type

#Region " �R���X�g���N�^ "

		''' <summary>
		''' �R���X�g���N�^
		''' </summary>
		''' <param name="typ">�N���X�^�C�v</param>
		''' <remarks></remarks>
		Public Sub New(ByVal typ As Type)
			_type = typ
		End Sub

#End Region
#Region " �v���p�e�B "

		''' <summary>
		''' �N���X�^�C�v�v���p�e�B
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public ReadOnly Property ImplType() As Type
			Get
				Return _type
			End Get
		End Property

#End Region

		''' <summary>
		''' �R���|�[�l���g�쐬
		''' </summary>
		''' <param name="field">�t�B�[���h</param>
		''' <returns>�R���|�[�l���g</returns>
		''' <remarks></remarks>
		Public Function CreateComponent(ByVal field As FieldInfo) As MocaComponent
			Dim component As MocaComponent
			component = New MocaComponent(_type, field.FieldType)
			Return component
		End Function

	End Class

End Namespace
