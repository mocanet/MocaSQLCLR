
Imports System.Threading

Namespace Di

	''' <summary>
	''' �R���|�[�l���g�����̃R���e�i
	''' </summary>
	''' <remarks>
	''' <see cref="ReaderWriterLock"/> ���g���ăX���b�h�Z�[�t�ɂ��Ă܂��B<br/>
	''' </remarks>
	Public Class MocaContainer
		Implements IContainer, IDisposable

		''' <summary>�R���|�[�l���g�i�[</summary>
		Private _components As Dictionary(Of Object, MocaComponent)

		''' <summary>���b�N�p</summary>
		Private _rwLock As New ReaderWriterLock()

#Region " �R���X�g���N�^ "

		''' <summary>
		''' �f�t�H���g�R���X�g���N�^
		''' </summary>
		''' <remarks></remarks>
		Public Sub New()
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

				' �R���|�[�l���g���������
				Destroy()
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

		''' <summary>
		''' ����������
		''' </summary>
		''' <remarks></remarks>
		Public Sub Init() Implements IContainer.Init
			If _components IsNot Nothing Then
				Return
			End If
			_components = New Dictionary(Of Object, MocaComponent)
		End Sub

		''' <summary>
		''' �i�[���Ă���R���|�[�l���g��Ԃ��B
		''' </summary>
		''' <param name="componentType">�擾����^</param>
		''' <returns>�Y������R���|�[�l���g�B�Y�����Ȃ��Ƃ��� Nothing ��Ԃ��B</returns>
		''' <remarks></remarks>
		Public Function GetComponent(ByVal componentType As System.Type) As MocaComponent Implements IContainer.GetComponent
			Try
				' ���[�_�[���b�N���擾
				_rwLock.AcquireReaderLock(Timeout.Infinite)

				If Not _components.ContainsKey(componentType) Then
					Return Nothing
				End If
				Return _components(componentType)
			Finally
				' ���[�_�[���b�N�����
				_rwLock.ReleaseReaderLock()
			End Try
		End Function

		''' <summary>
		''' �i�[���Ă���R���|�[�l���g��Ԃ��B
		''' </summary>
		''' <param name="componentKey">�擾����L�[</param>
		''' <returns>�Y������R���|�[�l���g�B�Y�����Ȃ��Ƃ��� Nothing ��Ԃ��B</returns>
		''' <remarks></remarks>
		Public Function GetComponent(ByVal componentKey As String) As MocaComponent Implements IContainer.GetComponent
			Try
				' ���[�_�[���b�N���擾
				_rwLock.AcquireReaderLock(Timeout.Infinite)

				If Not _components.ContainsKey(componentKey) Then
					Return Nothing
				End If
				Return _components(componentKey)
			Finally
				' ���[�_�[���b�N�����
				_rwLock.ReleaseReaderLock()
			End Try
		End Function

		''' <summary>
		''' �R���|�[�l���g���i�[����B
		''' </summary>
		''' <param name="component">�Ώۂ̃R���|�[�l���g</param>
		''' <remarks></remarks>
		Public Sub SetComponent(ByVal component As MocaComponent) Implements IContainer.SetComponent
			Try
				' ���C�^�[���b�N���擾
				_rwLock.AcquireWriterLock(Timeout.Infinite)

				If component.ImplType Is Nothing Then
					' �L�[�Ŋi�[
					If GetComponent(component.Key) IsNot Nothing Then
						Exit Sub
					End If
					_components.Add(component.Key, component)
					Exit Sub
				End If

				' �^�Ŋi�[
				If GetComponent(component.ImplType) IsNot Nothing Then
					Exit Sub
				End If
				_components.Add(component.ImplType, component)
			Finally
				' ���C�^�[���b�N�����
				_rwLock.ReleaseWriterLock()
			End Try
		End Sub

		''' <summary>
		''' �R���|�[�l���g�̏���
		''' </summary>
		''' <remarks></remarks>
		Public Sub Destroy() Implements IContainer.Destroy
			If _components Is Nothing Then
				Exit Sub
			End If
			For Each component As MocaComponent In _components.Values
				If component Is Nothing Then
					Continue For
				End If
				component.Dispose()
			Next
		End Sub

		''' <summary>
		''' <see cref="MocaComponent"/> �𔽕���������񋓎q��Ԃ��܂��B
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Function GetEnumerator() As IEnumerator(Of MocaComponent) Implements IContainer.GetEnumerator
			Return _components.Values.GetEnumerator()
		End Function

	End Class

End Namespace
