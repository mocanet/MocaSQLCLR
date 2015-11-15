
Imports System.Reflection

Namespace Util

	''' <summary>
	''' �^�C�v�𑀍삷��̂ɕ֗��ȃ��\�b�h�W
	''' </summary>
	''' <remarks></remarks>
	Public Class ClassUtil

#Region " CreateInstance "

		''' <summary>
		''' �w�肳�ꂽ�^�̃C���X�^���X��
		''' </summary>
		''' <param name="type">�C���X�^���X���������^</param>
		''' <returns>���������C���X�^���X</returns>
		''' <remarks>
		''' ���������R���X�g���N�^�̂Ƃ�
		''' </remarks>
		Public Shared Function NewInstance(ByVal type As Type) As Object
			Return Activator.CreateInstance(type)
		End Function

		''' <summary>
		''' �w�肳�ꂽ�^�̃C���X�^���X��
		''' </summary>
		''' <param name="type">�C���X�^���X���������^</param>
		''' <param name="args">�R���X�g���N�^�̈���</param>
		''' <returns>���������C���X�^���X</returns>
		''' <remarks>
		''' �����L��R���X�g���N�^�̂Ƃ�
		''' </remarks>
		Public Shared Function NewInstance(ByVal type As Type, ByVal args() As Object) As Object
			Return Activator.CreateInstance(type, args)
		End Function

#End Region
#Region " �v���p�e�B��`�̎擾 "

		''' <summary>
		''' �����^�C�v���̃v���p�e�B��`��Ԃ�
		''' </summary>
		''' <param name="typ">�^�C�v</param>
		''' <returns>�v���p�e�B��`�z��</returns>
		''' <remarks></remarks>
		Public Shared Function GetProperties(ByVal typ As Type) As PropertyInfo()
			Return typ.GetProperties(BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Instance)
		End Function

		''' <summary>
		''' �����C���X�^���X���̃v���p�e�B����Ԃ�
		''' </summary>
		''' <param name="target">�C���X�^���X</param>
		''' <returns>�v���p�e�B��`�z��</returns>
		''' <remarks></remarks>
		Public Shared Function GetProperties(ByVal target As Object) As PropertyInfo()
			Return GetProperties(target.GetType)
		End Function

		''' <summary>
		''' �����^�C�v���̃v���p�e�B��`��Ԃ�
		''' </summary>
		''' <param name="typ">�^�C�v</param>
		''' <returns>�v���p�e�B��`�z��</returns>
		''' <remarks></remarks>
		Public Shared Function GetProperties(ByVal typ As Type, ByVal name As String) As PropertyInfo
			Return typ.GetProperty(name, BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Instance)
		End Function

#End Region
#Region " �t�B�[���h��`�̎擾 "

		''' <summary>
		''' �����^�C�v���̃t�B�[���h��`��Ԃ�
		''' </summary>
		''' <param name="typ">�^�C�v</param>
		''' <returns>�t�B�[���h��`�z��</returns>
		''' <remarks></remarks>
		Public Shared Function GetFields(ByVal typ As Type) As FieldInfo()
			Return typ.GetFields(BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Instance)
		End Function

		''' <summary>
		''' �����C���X�^���X���̃t�B�[���h����Ԃ�
		''' </summary>
		''' <param name="target">�C���X�^���X</param>
		''' <returns>�t�B�[���h��`�z��</returns>
		''' <remarks></remarks>
		Public Shared Function GetFields(ByVal target As Object) As FieldInfo()
			Return GetFields(target.GetType)
		End Function

#End Region
#Region " ���\�b�h��`�̎擾 "

		''' <summary>
		''' �����^�C�v���̃��\�b�h��`��Ԃ�
		''' </summary>
		''' <param name="typ">�^�C�v</param>
		''' <returns>���\�b�h��`�z��</returns>
		''' <remarks></remarks>
		Public Shared Function GetMethods(ByVal typ As Type) As MethodInfo()
			Return typ.GetMethods(BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Instance)
		End Function

		''' <summary>
		''' �����C���X�^���X���̃��\�b�h����Ԃ�
		''' </summary>
		''' <param name="target">�C���X�^���X</param>
		''' <returns>���\�b�h��`�z��</returns>
		''' <remarks></remarks>
		Public Shared Function GetMethods(ByVal target As Object) As MethodInfo()
			Return GetMethods(target.GetType)
		End Function

#End Region
#Region " �C�x���g��`�̎擾 "

		''' <summary>
		''' �����^�C�v���̃C�x���g��`��Ԃ�
		''' </summary>
		''' <param name="typ">�^�C�v</param>
		''' <returns>�C�x���g��`�z��</returns>
		''' <remarks></remarks>
		Public Shared Function GetEvents(ByVal typ As Type) As EventInfo()
			Return typ.GetEvents(BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Instance)
		End Function

		''' <summary>
		''' �����C���X�^���X���̃C�x���g����Ԃ�
		''' </summary>
		''' <param name="target">�C���X�^���X</param>
		''' <returns>�C�x���g��`�z��</returns>
		''' <remarks></remarks>
		Public Shared Function GetEvents(ByVal target As Object) As EventInfo()
			Return GetEvents(target.GetType)
		End Function

#End Region
#Region " �J�X�^�������̎擾 "

#Region " Type "

		''' <summary>
		''' �w�肳�ꂽ�^�ɑ��݂���w�肳�ꂽ�J�X�^���������܂ޔz���Ԃ��܂��B
		''' </summary>
		''' <typeparam name="T">�ΏۂƂȂ�J�X�^������</typeparam>
		''' <param name="typ">�ΏۂƂȂ�^</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Shared Function GetCustomAttributes(Of T)(ByVal typ As Type) As T()
			Dim ary() As Object
			Dim aryLst As ArrayList
			ary = typ.GetCustomAttributes(GetType(T), False)
			aryLst = New ArrayList(ary)
			Return DirectCast(aryLst.ToArray(GetType(T)), T())
		End Function

		''' <summary>
		''' �w�肳�ꂽ�^�ɑ��݂���w�肳�ꂽ�J�X�^���������܂ޔz���Ԃ��܂��B
		''' </summary>
		''' <typeparam name="T">�ΏۂƂȂ�J�X�^������</typeparam>
		''' <param name="typ">�ΏۂƂȂ�^</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Shared Function GetCustomAttribute(Of T)(ByVal typ As Type) As T
			Dim ary() As T
			ary = GetCustomAttributes(Of T)(typ)
			If ary.Length = 0 Then
				Return Nothing
			End If
			Return ary(0)
		End Function

		''' <summary>
		''' �w�肳�ꂽ�t�B�[���h�Ɏw�肳�ꂽ�J�X�^���������܂ޔz���Ԃ��܂��B
		''' </summary>
		''' <param name="typ">�ΏۂƂȂ�t�B�[���h</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Shared Function GetCustomAttributes(ByVal typ As Type) As Attribute()
			Dim arylst As ArrayList
			arylst = New ArrayList(typ.GetCustomAttributes(False))
			Return DirectCast(arylst.ToArray(GetType(Attribute)), Attribute())
		End Function

#End Region
#Region " PropertyInfo "

		''' <summary>
		''' �w�肳�ꂽ�v���p�e�B�ɑ��݂���w�肳�ꂽ�J�X�^���������܂ޔz���Ԃ��܂��B
		''' </summary>
		''' <typeparam name="T">�ΏۂƂȂ�J�X�^������</typeparam>
		''' <param name="prop">�ΏۂƂȂ�v���p�e�B</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Shared Function GetCustomAttributes(Of T)(ByVal prop As PropertyInfo) As T()
			Dim ary() As Object
			Dim aryLst As ArrayList
			ary = prop.GetCustomAttributes(GetType(T), False)
			aryLst = New ArrayList(ary)
			Return DirectCast(aryLst.ToArray(GetType(T)), T())
		End Function

		''' <summary>
		''' �w�肳�ꂽ�v���p�e�B�ɑ��݂���w�肳�ꂽ�J�X�^��������Ԃ��܂��B
		''' </summary>
		''' <typeparam name="T">�ΏۂƂȂ�J�X�^������</typeparam>
		''' <param name="prop">�ΏۂƂȂ�v���p�e�B</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Shared Function GetCustomAttribute(Of T)(ByVal prop As PropertyInfo) As T
			Dim ary() As T
			ary = GetCustomAttributes(Of T)(prop)
			If ary.Length = 0 Then
				Return Nothing
			End If
			Return ary(0)
		End Function

#End Region
#Region " FieldInfo "

		''' <summary>
		''' �w�肳�ꂽ�t�B�[���h�Ɏw�肳�ꂽ�J�X�^��������Ԃ��܂��B
		''' </summary>
		''' <typeparam name="T">�ΏۂƂȂ�J�X�^������</typeparam>
		''' <param name="field">�ΏۂƂȂ�t�B�[���h</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Shared Function GetCustomAttribute(Of T)(ByVal field As FieldInfo) As T
			Dim ary() As T
			ary = GetCustomAttributes(Of T)(field)
			If ary.Length = 0 Then
				Return Nothing
			End If
			Return ary(0)
		End Function

		''' <summary>
		''' �w�肳�ꂽ�t�B�[���h�Ɏw�肳�ꂽ�J�X�^���������܂ޔz���Ԃ��܂��B
		''' </summary>
		''' <typeparam name="T">�ΏۂƂȂ�J�X�^������</typeparam>
		''' <param name="field">�ΏۂƂȂ�t�B�[���h</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Shared Function GetCustomAttributes(Of T)(ByVal field As FieldInfo) As T()
			Dim ary() As Object
			Dim aryLst As ArrayList
			ary = field.GetCustomAttributes(GetType(T), False)
			aryLst = New ArrayList(ary)
			Return DirectCast(aryLst.ToArray(GetType(T)), T())
		End Function

		''' <summary>
		''' �w�肳�ꂽ�t�B�[���h�Ɏw�肳�ꂽ�J�X�^���������܂ޔz���Ԃ��܂��B
		''' </summary>
		''' <param name="field">�ΏۂƂȂ�t�B�[���h</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Shared Function GetCustomAttributes(ByVal field As FieldInfo) As Attribute()
			Dim arylst As ArrayList
			arylst = New ArrayList(field.GetCustomAttributes(False))
			Return DirectCast(arylst.ToArray(GetType(Attribute)), Attribute())
		End Function

#End Region
#Region " MethodBase "

		''' <summary>
		''' �w�肳�ꂽ�t�B�[���h�ɑ��݂���J�X�^���������܂ޔz���Ԃ��܂��B
		''' </summary>
		''' <typeparam name="T">�ΏۂƂȂ�J�X�^������</typeparam>
		''' <param name="method">�ΏۂƂȂ�t�B�[���h</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Shared Function GetCustomAttributes(Of T)(ByVal method As MethodBase, Optional ByVal inherit As Boolean = False) As T()
			Dim ary() As Object
			Dim aryLst As ArrayList
			ary = method.GetCustomAttributes(GetType(T), inherit)
			aryLst = New ArrayList(ary)
			Return DirectCast(aryLst.ToArray(GetType(T)), T())
		End Function

		''' <summary>
		''' �w�肳�ꂽ�t�B�[���h�ɑ��݂���J�X�^��������Ԃ��܂��B
		''' </summary>
		''' <typeparam name="T">�ΏۂƂȂ�J�X�^������</typeparam>
		''' <param name="method">�ΏۂƂȂ�t�B�[���h</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Shared Function GetCustomAttribute(Of T)(ByVal method As MethodBase, Optional ByVal inherit As Boolean = False) As T
			Dim ary() As T
			ary = GetCustomAttributes(Of T)(method, inherit)
			If ary.Length = 0 Then
				Return Nothing
			End If
			Return ary(0)
		End Function

#End Region
#Region " Event "

		''' <summary>
		''' �w�肳�ꂽ�t�B�[���h�ɑ��݂���J�X�^���������܂ޔz���Ԃ��܂��B
		''' </summary>
		''' <typeparam name="T">�ΏۂƂȂ�J�X�^������</typeparam>
		''' <param name="method">�ΏۂƂȂ�t�B�[���h</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Shared Function GetCustomAttributes(Of T)(ByVal method As EventInfo, Optional ByVal inherit As Boolean = False) As T()
			Dim raiseMethod As MethodBase
			Dim ary() As Object
			Dim aryLst As ArrayList
			raiseMethod = method.GetRaiseMethod(inherit)
			If raiseMethod Is Nothing Then
				Return Nothing
			End If
			ary = raiseMethod.GetCustomAttributes(GetType(T), inherit)
			aryLst = New ArrayList(ary)
			Return DirectCast(aryLst.ToArray(GetType(T)), T())
		End Function

		''' <summary>
		''' �w�肳�ꂽ�t�B�[���h�ɑ��݂���J�X�^��������Ԃ��܂��B
		''' </summary>
		''' <typeparam name="T">�ΏۂƂȂ�J�X�^������</typeparam>
		''' <param name="method">�ΏۂƂȂ�t�B�[���h</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Shared Function GetCustomAttribute(Of T)(ByVal method As EventInfo, Optional ByVal inherit As Boolean = False) As T
			Dim ary() As T
			ary = GetCustomAttributes(Of T)(method, inherit)
			If ary.Length = 0 Then
				Return Nothing
			End If
			Return ary(0)
		End Function

#End Region

#End Region
#Region " �t�B�[���h�փC���X�^���X�𒍓� "

		''' <summary>
		''' �t�B�[���h�փC���X�^���X�𒍓�
		''' </summary>
		''' <param name="target">�ΏۂƂȂ�C���X�^���X</param>
		''' <param name="field">�ΏۂƂȂ�C���X�^���X�̃t�B�[���h</param>
		''' <param name="args">�t�B�[���h�֐ݒ肷��C���X�^���X�̔z��</param>
		''' <remarks></remarks>
		Public Shared Sub Inject(ByVal target As Object, ByVal field As FieldInfo, ByVal args() As Object)
			Dim bFlags As BindingFlags

			' �t�B�[���h�ɒl���Z�b�g����ׂ�BindingFlags
			bFlags = BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.SetField

			' �C���X�^���X���t�B�[���h�֒����I
			target.GetType().InvokeMember(field.Name, bFlags, Nothing, target, args)
		End Sub

#End Region
#Region " Check "

		''' <summary>
		''' �C���^�t�F�[�X����������Ă��邩�`�F�b�N����
		''' </summary>
		''' <param name="targetType">�ΏۂƂȂ�^</param>
		''' <param name="checkType">�`�F�b�N����^</param>
		''' <returns>True �͑��݂���AFalse �͑��݂��Ȃ�</returns>
		''' <remarks></remarks>
		Public Shared Function IsInterfaceImpl(ByVal targetType As Type, ByVal checkType As Type) As Boolean
			Dim ok As Boolean
			ok = False
			For Each item As Type In targetType.GetInterfaces
				If item.Equals(checkType) Then
					ok = True
					Exit For
				End If
			Next
			Return ok
		End Function

#End Region

	End Class

End Namespace
