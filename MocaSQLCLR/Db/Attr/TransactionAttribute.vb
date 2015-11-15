
Imports System.Reflection
Imports System.Transactions
Imports Moca.Aop
Imports Moca.Attr

Namespace Db.Attr

	''' <summary>
	''' �g�����U�N�V��������
	''' </summary>
	''' <remarks>
	''' ���\�b�h���̏������g�����U�N�V�����Ŋ���Ƃ��Ɏw�肷��B
	''' </remarks>
	<AttributeUsage(AttributeTargets.Method, Inherited:=True)> _
	Public Class TransactionAttribute
		Inherits Attribute

		''' <summary>�ǉ��I�v�V����</summary>
		Private _scopeOption As Object

		''' <summary>�������x��</summary>
		Private _isolationLevel As Object

		''' <summary>�g�����U�N�V�����^�C�v</summary>
		Private _transactionType As Config.TransactionType

#Region " �R���X�g���N�^ "

		''' <summary>
		''' �f�t�H���g�R���X�g���N�^
		''' </summary>
		''' <remarks></remarks>
		Public Sub New()
			_scopeOption = Nothing
			_isolationLevel = Nothing
		End Sub

		''' <summary>
		''' �R���X�g���N�^
		''' </summary>
		''' <remarks></remarks>
		Public Sub New(ByVal scopeOption As TransactionScopeOption)
			Me.New()
			_scopeOption = scopeOption
		End Sub

		''' <summary>
		''' �R���X�g���N�^
		''' </summary>
		''' <remarks></remarks>
		Public Sub New(ByVal scopeOption As TransactionScopeOption, ByVal isolationLevel As Transactions.IsolationLevel)
			Me.New()
			_scopeOption = scopeOption
			_isolationLevel = isolationLevel
		End Sub

#End Region

		''' <summary>
		''' �g�����U�N�V�����p�A�X�y�N�g���쐬����
		''' </summary>
		''' <param name="method">���\�b�h</param>
		''' <returns>�g�����U�N�V�����p�A�X�y�N�g</returns>
		''' <remarks></remarks>
		Public Shadows Function CreateAspect(ByVal method As MethodBase) As IAspect
			Dim pointcut As IPointcut
			Dim val As IAspect

			pointcut = New Pointcut(New String() {method.ToString})

			Select Case Config.MocaConfiguration.Section.TransactionType
				Case Config.TransactionType.Local
					val = New Aspect(New Tx.LocalTxInterceptor(_scopeOption, _isolationLevel), pointcut)

				Case Else
					val = New Aspect(New Tx.ScopeTxInterceptor(_scopeOption, _isolationLevel), pointcut)
					'TODO: �X���b�h�����ɂĂ����g�����U�N�V�����̋��������������Ƃ��͂�����ɂ��Ă݂�B
					'val = New Aspect(GetType(Tx.TransactionInterceptor), pointcut)

			End Select

			Return val
		End Function

	End Class

End Namespace
