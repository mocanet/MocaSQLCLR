
Imports System.Reflection
Imports Moca.Exceptions

Namespace Aop

	''' <summary>
	''' Interceptor����C���^�[�Z�v�g����Ă��郁�\�b�h�̏��
	''' </summary>
	''' <remarks></remarks>
	Public Class MethodInvocation
		Implements IMethodInvocation

		''' <summary>���s�Ώۂ̃C���X�^���X</summary>
		Private _this As Object

		''' <summary>���\�b�h��`</summary>
		Private _method As MethodBase

		''' <summary>���\�b�h�̈���</summary>
		Private _args() As Object

		''' <summary>���s����Advice(Interceptor)</summary>
		Private _advice As IMethodInterceptor

		''' <summary>���Ɏ��s����Interceptor����C���^�[�Z�v�g����Ă��郁�\�b�h�̏��</summary>
		Private _nextInvocation As MethodInvocation

#Region " �R���X�g���N�^ "

		''' <summary>
		''' �R���X�g���N�^
		''' </summary>
		''' <param name="this">���s�ΏۂƂȂ�C���X�^���X</param>
		''' <param name="method">���s�ΏۂƂȂ郁�\�b�h��`</param>
		''' <param name="args">���s���郁�\�b�h�̈����z��</param>
		''' <remarks></remarks>
		Public Sub New(ByVal this As Object, ByVal method As MethodBase, ByVal args() As Object)
			_this = this
			_method = method
			_args = args
		End Sub

		''' <summary>
		''' �R���X�g���N�^
		''' </summary>
		''' <param name="interceptor">���s����Advice</param>
		''' <param name="forwardInvocation">��O��Interceptor����C���^�[�Z�v�g����Ă��郁�\�b�h�̏��</param>
		''' <remarks></remarks>
		Public Sub New(ByVal interceptor As IMethodInterceptor, ByVal forwardInvocation As MethodInvocation)
			_this = forwardInvocation.This
			_method = forwardInvocation.Method
			_args = forwardInvocation.Arguments
			_advice = interceptor
			forwardInvocation.NextInvocation = Me
		End Sub

#End Region

#Region " Implements IMethodInvocation "

#Region " �v���p�e�B "

		Public ReadOnly Property Arguments() As Object() Implements IMethodInvocation.Args
			Get
				Return _args
			End Get
		End Property

		Public ReadOnly Property Method() As MethodBase Implements IMethodInvocation.Method
			Get
				Return _method
			End Get
		End Property

		Public ReadOnly Property This() As Object Implements IMethodInvocation.This
			Get
				Return _this
			End Get
		End Property

#End Region

		Public Function Proceed() As Object Implements IMethodInvocation.Proceed
			' ���� Interceptor �����s����
			If _nextInvocation IsNot Nothing Then
				Return _nextInvocation.Advice.Invoke(_nextInvocation)
			End If

			'TODO: ����ł����̂��Ȃ��E�E�E
			'' �^�[�Q�b�g�� Object �^�͌��^�Ȃ��̉��z�I�u�W�F�N�g�Ȃ̂ňȉ��͏����Ȃ�
			'If TypeOf _this Is Object Then
			'	Return Nothing
			'End If
			Try
				Return _method.Invoke(_this, _args)
			Catch ex As TargetInvocationException
				CommonException.SaveStackTraceToRemoteStackTraceString(ex.InnerException)
				Throw ex.InnerException
			End Try
		End Function

#End Region

#Region " �v���p�e�B "

		''' <summary>
		''' ���Ɏ��s����Interceptor����C���^�[�Z�v�g����Ă��郁�\�b�h�̏�� �v���p�e�B
		''' </summary>
		''' <value></value>
		''' <remarks></remarks>
		Public WriteOnly Property NextInvocation() As MethodInvocation
			Set(ByVal value As MethodInvocation)
				_nextInvocation = value
			End Set
		End Property

		''' <summary>
		''' ���s����Advice(Interceptor) �v���p�e�B
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Friend ReadOnly Property Advice() As IMethodInterceptor
			Get
				Return _advice
			End Get
		End Property

#End Region

	End Class

End Namespace
