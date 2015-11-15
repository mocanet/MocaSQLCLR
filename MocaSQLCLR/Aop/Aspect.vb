
Imports Moca.Util

Namespace Aop

	''' <summary>
	''' ���f�I�Ȋ֐S�������U�镑���i�����̂��Ɓj�ƁA���U�镑����K�p���邩���֘A�t�����܂��B
	''' Advice(Interceptor)��Pointcut���܂Ƃ߂����̂�Aspect(�A�X�y�N�g)�Ƃ����܂��B
	''' </summary>
	''' <remarks></remarks>
	Public Class Aspect
		Implements IAspect

		''' <summary>Advice(Interceptor)</summary>
		Private _advice As IMethodInterceptor
		''' <summary>Advice(Interceptor)�̌^</summary>
		Private _adviceType As Type

		''' <summary>Pointcut</summary>
		Private _pointcut As IPointcut

#Region " �R���X�g���N�^ "

		''' <summary>
		''' �R���X�g���N�^
		''' </summary>
		''' <param name="advice">Advice(Interceptor)</param>
		''' <param name="pointcut">Pointcut</param>
		''' <remarks></remarks>
		Public Sub New(ByVal advice As IMethodInterceptor, ByVal pointcut As IPointcut)
			_advice = advice
			_pointcut = pointcut
		End Sub

		''' <summary>
		''' �R���X�g���N�^
		''' </summary>
		''' <param name="adviceType">Advice(Interceptor)�̌^</param>
		''' <param name="pointcut">Pointcut</param>
		''' <remarks></remarks>
		Public Sub New(ByVal adviceType As Type, ByVal pointcut As IPointcut)
			If Not ClassUtil.IsInterfaceImpl(adviceType, GetType(IMethodInterceptor)) Then
				Throw New ArgumentException("�w�肳�ꂽAdvice(Interceptor)�̌^�� IMethodInterceptor �������������̂ł͂���܂���B")
			End If
			_advice = Nothing
			_adviceType = adviceType
			_pointcut = pointcut
		End Sub

#End Region

#Region " �v���p�e�B "

		''' <summary>
		''' Advice(Interceptor) �v���p�e�B
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public ReadOnly Property Advice() As IMethodInterceptor Implements IAspect.Advice
			Get
				If _advice IsNot Nothing Then
					Return _advice
				End If
				Return DirectCast(ClassUtil.NewInstance(_adviceType), IMethodInterceptor)
			End Get
		End Property

		''' <summary>
		''' Pointcut �v���p�e�B
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public ReadOnly Property Pointcut() As IPointcut Implements IAspect.Pointcut
			Get
				Return _pointcut
			End Get
		End Property

#End Region

	End Class

End Namespace
