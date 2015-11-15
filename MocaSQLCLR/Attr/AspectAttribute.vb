
Imports System.Reflection
Imports Moca.Aop
Imports Moca.Util

Namespace Attr

	''' <summary>
	''' �A�X�y�N�g����
	''' </summary>
	''' <remarks>
	''' �A�X�y�N�g�������Ƃ��Ɏw�肷��B
	''' ���\�b�h�̂ݎw��\�ł��B
	''' </remarks>
	<AttributeUsage(AttributeTargets.Method, allowmultiple:=True)> _
	Public Class AspectAttribute
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
		''' �A�X�y�N�g���쐬����
		''' </summary>
		''' <param name="method">���\�b�h</param>
		''' <returns>�A�X�y�N�g</returns>
		''' <remarks></remarks>
		Public Function CreateAspect(ByVal method As MethodBase) As IAspect
			Dim pointcut As IPointcut
			Dim val As IAspect

			pointcut = New Pointcut(New String() {method.ToString})
			val = New Aspect(DirectCast(ClassUtil.NewInstance(_type), IMethodInterceptor), pointcut)
			Return val
		End Function

		''' <summary>
		''' �A�X�y�N�g���쐬����
		''' </summary>
		''' <param name="method">���\�b�h</param>
		''' <returns>�A�X�y�N�g</returns>
		''' <remarks></remarks>
		Public Function CreateAspect(ByVal method As EventInfo) As IAspect
			Dim pointcut As IPointcut
			Dim val As IAspect

			pointcut = New Pointcut(New String() {method.ToString})
			val = New Aspect(DirectCast(ClassUtil.NewInstance(_type), IMethodInterceptor), pointcut)
			Return val
		End Function

	End Class

End Namespace
