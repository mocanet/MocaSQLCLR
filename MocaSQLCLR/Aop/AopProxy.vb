
Imports System.Runtime.Remoting.Proxies
Imports System.Runtime.Remoting.Messaging
Imports System.Reflection
Imports Moca.Exceptions

Namespace Aop

	''' <summary>
	''' ���ߓI�v���N�V
	''' </summary>
	''' <remarks></remarks>
	Public Class AopProxy
		Inherits RealProxy

		''' <summary>���ߓI�v���N�V���쐬����^</summary>
		Private _type As Type

		''' <summary>�K�p���� Aspect �z��</summary>
		Private _aspects As IList(Of IAspect)

		''' <summary>���ߓI�v���N�V���쐬����C���X�^���X</summary>
		Private _target As Object

#Region " �R���X�g���N�^ "

		''' <summary>
		''' �R���X�g���N�^
		''' </summary>
		''' <param name="target">���ߓI�v���N�V���쐬����C���X�^���X</param>
		''' <remarks></remarks>
		Public Sub New(ByVal target As Object)
			MyBase.New(target.GetType)
			_aspects = New List(Of IAspect)
			_type = target.GetType
			_target = target
		End Sub

		''' <summary>
		''' �R���X�g���N�^
		''' </summary>
		''' <param name="type">���ߓI�v���N�V���쐬����^</param>
		''' <remarks></remarks>
		Public Sub New(ByVal type As Type)
			MyBase.New(type)
			_aspects = New List(Of IAspect)
			_type = type
		End Sub

		''' <summary>
		''' �R���X�g���N�^
		''' </summary>
		''' <param name="type">���ߓI�v���N�V���쐬����^</param>
		''' <param name="aspects">�K�p���� Aspect �z��</param>
		''' <remarks></remarks>
		Public Sub New(ByVal type As Type, ByVal aspects() As IAspect)
			Me.New(type, aspects, Nothing)
		End Sub

		''' <summary>
		''' �R���X�g���N�^
		''' </summary>
		''' <param name="type">���ߓI�v���N�V���쐬����^</param>
		''' <param name="aspects">�K�p���� Aspect �z��</param>
		''' <param name="target">���ߓI�v���N�V���쐬����C���X�^���X</param>
		''' <remarks></remarks>
		Public Sub New(ByVal type As Type, ByVal aspects() As IAspect, ByVal target As Object)
			Me.New(type)
			_aspects = aspects
			_target = target
		End Sub

#End Region

#Region " RealProxy Overrides "

		''' <summary>
		''' IMessage �Ŏw�肳�ꂽ���\�b�h���A���݂̃C���X�^���X���\�������[�g �I�u�W�F�N�g�ŌĂяo���܂��B
		''' </summary>
		''' <param name="msg">���\�b�h�̌Ăяo���Ɋւ�����</param>
		''' <returns>�Ăяo���ꂽ���\�b�h���Ԃ����b�Z�[�W�ŁAout �p�����[�^�܂��� ref �p�����[�^�̂ǂ��炩�Ɩ߂�l���i�[���Ă��郁�b�Z�[�W�B</returns>
		''' <remarks></remarks>
		Public Overrides Function Invoke(ByVal msg As IMessage) As IMessage
			Dim mm As IMethodMessage
			Dim method As MethodInfo
			Dim args() As Object
			Dim ret As Object

			' ������
			ret = Nothing
			mm = DirectCast(msg, IMethodMessage)
			method = DirectCast(mm.MethodBase, MethodInfo)
			args = mm.Args

			' �C���^�t�F�[�X�̈ȊO�Ŏ��Ԃ��������̓C���X�^���X������
			If Not _type.IsInterface Then
				If _target Is Nothing Then
					_target = Activator.CreateInstance(_type)
				End If
			End If
			' ���ߓI�v���N�V���쐬����C���X�^���X�����݂��Ȃ����� Object �^�ŉ��C���X�^���X���쐬
			If _target Is Nothing Then
				_target = New Object()
			End If

			' ���\�b�h���s�I
			If _aspects.Count = 0 Then
				' �U�镑����K�p���Ȃ��Ƃ�
				Try
					ret = method.Invoke(_target, args)
				Catch ex As TargetInvocationException
					CommonException.SaveStackTraceToRemoteStackTraceString(ex.InnerException)
					Throw ex.InnerException
				End Try
			Else
				' �U�镑����K�p����Ƃ�
				Dim topInvocation As IMethodInvocation
				Dim invocation As IMethodInvocation

				topInvocation = New MethodInvocation(_target, method, args)
				invocation = topInvocation

				For Each aspect As IAspect In _aspects
					If Not aspect.Pointcut.IsExecution(method.ToString) Then
						Continue For
					End If
					invocation = New MethodInvocation(aspect.Advice, DirectCast(invocation, MethodInvocation))
				Next
				ret = topInvocation.Proceed()
			End If

			' �߂�l�쐬
			Dim mrm As IMethodReturnMessage
			mrm = New ReturnMessage(ret, args, args.Length, mm.LogicalCallContext, DirectCast(msg, IMethodCallMessage))
			Return mrm
		End Function

#End Region

		''' <summary>
		''' �K�p���� Aspect ��ǉ�����
		''' </summary>
		''' <param name="aspect">Aspect �C���X�^���X</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Function AddAspect(ByVal aspect As IAspect) As IAspect
			_aspects.Add(aspect)
			Return aspect
		End Function

		''' <summary>
		''' �K�p���� Aspect ��ǉ�����
		''' </summary>
		''' <param name="advice">Advice(Interceptor)</param>
		''' <param name="pointcut">Pointcut</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Function AddAspect(ByVal advice As IMethodInterceptor, ByVal pointcut As IPointcut) As IAspect
			Dim item As IAspect

			item = New Aspect(advice, pointcut)
			_aspects.Add(item)

			Return item
		End Function

		''' <summary>
		''' ���ߓI�v���N�V��Ԃ�
		''' </summary>
		''' <returns>���ߓI�v���N�V�̃C���X�^���X</returns>
		''' <remarks></remarks>
		Public Function Create() As Object
			Return GetTransparentProxy()
		End Function

		''' <summary>
		''' ���ߓI�v���N�V��Ԃ�
		''' </summary>
		''' <typeparam name="T">���ߓI�v���N�V�̃C���X�^���X�̌^</typeparam>
		''' <returns>���ߓI�v���N�V�̃C���X�^���X</returns>
		''' <remarks></remarks>
		Public Function Create(Of T)() As T
			Return DirectCast(GetTransparentProxy(), T)
		End Function

	End Class

End Namespace
