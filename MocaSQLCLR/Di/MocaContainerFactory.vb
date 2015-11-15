
Imports Moca.Exceptions

Namespace Di

	''' <summary>
	''' �R���e�i�̃t�@�N�g���N���X
	''' </summary>
	''' <remarks></remarks>
	Public Class MocaContainerFactory

		''' <summary>�V���O���g���p�R���e�i�C���X�^���X</summary>
		Private Shared ReadOnly _instance As IContainer = New MocaContainer

		''' <summary>
		''' ����������
		''' </summary>
		''' <remarks></remarks>
		Public Shared Sub Init()
			_instance.Init()
		End Sub

		''' <summary>
		''' �R���|�[�l���g�̏���
		''' </summary>
		''' <remarks></remarks>
		Public Shared Sub Destroy()
			If _instance Is Nothing Then
				Exit Sub
			End If
			_instance.Destroy()
		End Sub

		''' <summary>
		''' �f�t�H���g�̃R���e�i�C���X�^���X�쐬
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Shared ReadOnly Property Container() As IContainer
			Get
				If _instance Is Nothing Then
					Throw New MocaRuntimeException("�R���e�i�̏����������s����Ă��܂���B")
				End If
				Return _instance
			End Get
		End Property

	End Class

End Namespace
