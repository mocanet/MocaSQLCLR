Imports System.Configuration
Imports System.Data.Common
Imports System.Reflection
Imports Moca.Db.CommandWrapper
Imports Moca.Util

Namespace Db

	''' <summary>
	''' DB�փA�N�Z�X����ׂ̊�{�I�ȋ@�\��񋟂���
	''' </summary>
	''' <remarks>
	''' 
	''' </remarks>
	Public Class DbAccess
		Inherits AbstractDao
		Implements IDbAccess

		''' <summary>�g�����U�N�V�����I�u�W�F�N�g</summary>
		Private _tx As IDbTransaction
		''' <summary>�g�����U�N�V�����X�R�[�v�I�u�W�F�N�g</summary>
		Private _txs As Transactions.TransactionScope

#Region " IDisposable Support "

		Protected Overrides Sub Dispose(ByVal disposing As Boolean)
			MyBase.Dispose(disposing)
			If _tx IsNot Nothing Then
				_tx.Dispose()
				_tx = Nothing
			End If
			If _txs IsNot Nothing Then
				_txs.Dispose()
				_txs = Nothing
			End If
		End Sub

#End Region
#Region " Constructor/DeConstructor "

		''' <summary>
		''' �f�t�H���g�R���X�g���N�^
		''' </summary>
		''' <remarks>
		''' �O������͗��p�s��
		''' </remarks>
		Protected Sub New()
		End Sub

		''' <summary>
		''' �R���X�g���N�^
		''' </summary>
		''' <param name="myDbms">�ڑ����DBMS</param>
		''' <remarks></remarks>
		Public Sub New(ByVal myDbms As Dbms)
			MyBase.New(myDbms)
		End Sub

#End Region
#Region " Property "

		''' <summary>
		''' �g�����U�N�V�����X�R�[�v�I�u�W�F�N�g
		''' </summary>
		''' <value></value>
		''' <remarks></remarks>
		Public Overridable ReadOnly Property TransactionScope() As System.Transactions.TransactionScope Implements IDbAccess.TransactionScope
			Get
				Return _txs
			End Get
		End Property

		''' <summary>
		''' �g�����U�N�V�����I�u�W�F�N�g
		''' </summary>
		''' <value></value>
		''' <remarks>
		''' </remarks>
		Public Overridable ReadOnly Property Transaction() As IDbTransaction Implements IDbAccess.Transaction
			Get
				Return _tx
			End Get
		End Property

#End Region
#Region " Transaction "

		''' <summary>
		''' �g�����U�N�V�����X�R�[�v���쐬����
		''' </summary>
		''' <returns>�g�����U�N�V�����X�R�[�v</returns>
		''' <remarks></remarks>
		Public Overridable Function NewTransactionScope() As Transactions.TransactionScope Implements IDbAccess.NewTransactionScope
			Try
				If _tx IsNot Nothing Then
					Throw New DbAccessException(Me, "���� TransactionStart ���\�b�h�ɂăg�����U�N�V�������J�n����Ă��܂��B")
				End If
				If _txs IsNot Nothing Then
					Return _txs
				End If
				_txs = New Transactions.TransactionScope()
				Me.Connection.Open()
				Return _txs
			Catch ex As Exception
				Throw New DbAccessException(Me, ex)
			End Try
		End Function

		''' <summary>
		''' �g�����U�N�V�����X�R�[�v����������
		''' </summary>
		''' <remarks></remarks>
		Public Overridable Sub TransactionComplete() Implements IDbAccess.TransactionComplete
			If _tx IsNot Nothing Then
				Throw New DbAccessException(Me, "���� TransactionStart ���\�b�h�ɂăg�����U�N�V�������J�n����Ă��܂��B")
			End If
			If _txs Is Nothing Then
				Exit Sub
			End If
			If Me.Connection.State = ConnectionState.Closed Then
				Exit Sub
			End If

			Try
				Me.Connection.Close()
				_txs.Complete()
				_txs = Nothing
			Catch ex As Exception
				Throw New DbAccessException(Me, ex)
			End Try
		End Sub

		''' <summary>
		''' �g�����U�N�V�������J�n����
		''' </summary>
		''' <remarks>
		''' �g�����U�N�V�������g�p����ꍇ�͎��O��DB�ւ̐ڑ����K�v�ȈׁA������DB�Ƃ̐ڑ����s���܂��B
		''' </remarks>
		''' <exception cref="DbAccessException">
		''' DB�A�N�Z�X�ŃG���[����������
		''' </exception>
		Public Overridable Sub TransactionStart() Implements IDbAccess.TransactionStart
			Try
				If _txs IsNot Nothing Then
					Throw New DbAccessException(Me, "���� TransactionScope ���\�b�h�ɂăg�����U�N�V�������J�n����Ă��܂��B")
				End If
				If _tx IsNot Nothing Then
					Exit Sub
				End If
				Me.Connection.Open()
				_tx = Me.Connection.BeginTransaction
			Catch ex As Exception
				Throw New DbAccessException(Me, ex)
			End Try
		End Sub

		''' <summary>
		''' ����DBAccess�N���X�ƃg�����U�N�V�����𓯂��ɂ���
		''' </summary>
		''' <param name="dba">��������DbAccess�C���X�^���X</param>
		''' <remarks>
		''' �R�l�N�V�����I�u�W�F�N�g�ƃg�����U�N�V�����I�u�W�F�N�g���w�肳�ꂽDbAccess�̃I�u�W�F�N�g�ŏ㏑�����܂��B
		''' </remarks>
		Public Overridable Sub TransactionBinding(ByVal dba As IDbAccess) Implements IDbAccess.TransactionBinding
			Me.ConnectionJoin = dba.Connection
			_tx = dba.Transaction
		End Sub

		''' <summary>
		''' �g�����U�N�V�������I������i�R�~�b�g�j
		''' </summary>
		''' <remarks>
		''' DB�Ƃ̐ڑ���ؒf���܂��B
		''' </remarks>
		''' <exception cref="DbAccessException">
		''' DB�A�N�Z�X�ŃG���[����������
		''' </exception>
		Public Overridable Sub TransactionEnd() Implements IDbAccess.TransactionEnd
			If _tx Is Nothing Then
				Exit Sub
			End If
			If Me.Connection.State = ConnectionState.Closed Then
				Exit Sub
			End If

			Try
				_tx.Commit()
				Me.Connection.Close()
			Catch ex As Exception
				Throw New DbAccessException(Me, ex)
			Finally
				_tx.Dispose()
			End Try
		End Sub

		''' <summary>
		''' �g�����U�N�V���������[���o�b�N����
		''' </summary>
		''' <remarks>
		''' DB�Ƃ̐ڑ���ؒf���܂��B
		''' </remarks>
		''' <exception cref="DbAccessException">
		''' DB�A�N�Z�X�ŃG���[����������
		''' </exception>
		Public Overridable Sub TransactionRollback() Implements IDbAccess.TransactionRollback
			If _tx Is Nothing Then
				Exit Sub
			End If
			If Me.Connection.State = ConnectionState.Closed Then
				Exit Sub
			End If

			Try
				_tx.Rollback()
				Me.Connection.Close()
			Catch ex As Exception
				Throw New DbAccessException(Me, ex)
			Finally
				_tx.Dispose()
			End Try
		End Sub

#End Region
#Region " Create "

		''' <summary>
		''' �w�肳�ꂽ�^�C�v��DbCommand�C���X�^���X�𐶐�����
		''' </summary>
		''' <param name="sqlCommandType">�R�}���h���</param>
		''' <param name="commandText">���s����SQL�����́A�X�g�A�h��</param>
		''' <param name="useConn">�g�p����R�l�N�V����</param>
		''' <returns>�w�肳�ꂽ�^�C�v�̃C���X�^���X</returns>
		''' <remarks>
		''' �R�}���h��ʂɊY������ISqlCommand�̃C���X�^���X�𐶐����܂��B<br/>
		''' <list>
		''' <item><term>SelectText</term><description>ISelectCommand</description></item>
		''' <item><term>Select4Update</term><description>ISelect4UpdateCommand</description></item>
		''' <item><term>UpdateText</term><description>IUpdateCommand</description></item>
		''' <item><term>InsertText</term><description>IInsertCommand</description></item>
		''' <item><term>DeleteText</term><description>IDeleteCommand</description></item>
		''' <item><term>StoredProcedure</term><description>IStoredProcedureCommand</description></item>
		''' <item><term>DDL</term><description>IDDLCommand</description></item>
		''' </list>
		''' </remarks>
		''' <exception cref="DbAccessException">
		''' DB�A�N�Z�X�ŃG���[����������
		''' </exception>
		Protected Friend Overrides Function createCommandWrapper(ByVal sqlCommandType As SQLCommandTypes, ByVal commandText As String, ByVal useConn As System.Data.IDbConnection) As IDbCommandSql
			Dim cmdWrapper As IDbCommandSql
			cmdWrapper = MyBase.createCommandWrapper(sqlCommandType, commandText, useConn)
			If Not _tx Is Nothing Then
				cmdWrapper.Command.Transaction = _tx
			End If
			Return cmdWrapper
		End Function

#End Region
#Region " Execute "

		''' <summary>
		''' INSERT���̎��s
		''' </summary>
		''' <param name="commandWrapper">INSERT�������s����ׂ�DBCommand�̃��b�p�[�C���X�^���X</param>
		''' <returns>�X�V����</returns>
		''' <remarks>
		''' �����\�b�h���g�p����ꍇ�́A�g�����U�N�V�����̊J�n<see cref="DBAccess.TransactionStart"></see>�A�I��<see cref="DBAccess.TransactionEnd"></see>���s���Ă��������B
		''' </remarks>
		''' <exception cref="DbAccessException">
		''' DB�A�N�Z�X�ŃG���[����������
		''' </exception>
		Public Overloads Function ExecuteNonQuery(ByVal commandWrapper As IDbCommandInsert) As Integer Implements IDbAccess.ExecuteNonQuery
			Return ExecuteNonQuery(commandWrapper)
		End Function

		''' <summary>
		''' UPDATE���̎��s
		''' </summary>
		''' <param name="commandWrapper">UPDATE�������s����ׂ�DBCommand�̃��b�p�[�C���X�^���X</param>
		''' <returns>�X�V����</returns>
		''' <remarks>
		''' �����\�b�h���g�p����ꍇ�́A�g�����U�N�V�����̊J�n<see cref="DBAccess.TransactionStart"></see>�A�I��<see cref="DBAccess.TransactionEnd"></see>���s���Ă��������B
		''' </remarks>
		''' <exception cref="DbAccessException">
		''' DB�A�N�Z�X�ŃG���[����������
		''' </exception>
		Public Overloads Function ExecuteNonQuery(ByVal commandWrapper As IDbCommandUpdate) As Integer Implements IDbAccess.ExecuteNonQuery
			Return ExecuteNonQuery(commandWrapper)
		End Function

		''' <summary>
		''' DELETE���̎��s
		''' </summary>
		''' <param name="commandWrapper">DELETE�������s����ׂ�DBCommand�̃��b�p�[�C���X�^���X</param>
		''' <returns>�X�V����</returns>
		''' <remarks>
		''' �����\�b�h���g�p����ꍇ�́A�g�����U�N�V�����̊J�n<see cref="DBAccess.TransactionStart"></see>�A�I��<see cref="DBAccess.TransactionEnd"></see>���s���Ă��������B
		''' </remarks>
		''' <exception cref="DbAccessException">
		''' DB�A�N�Z�X�ŃG���[����������
		''' </exception>
		Public Overloads Function ExecuteNonQuery(ByVal commandWrapper As IDbCommandDelete) As Integer Implements IDbAccess.ExecuteNonQuery
			Return ExecuteNonQuery(commandWrapper)
		End Function

		''' <summary>
		''' �X�g�A�h�̎��s
		''' </summary>
		''' <param name="commandWrapper">�X�g�A�h�����s����ׂ�DBCommand�̃��b�p�[�C���X�^���X</param>
		''' <returns>�X�V����</returns>
		''' <remarks>
		''' �����\�b�h���g�p����ꍇ�́A�g�����U�N�V�����̊J�n<see cref="DBAccess.TransactionStart"></see>�A�I��<see cref="DBAccess.TransactionEnd"></see>���s���Ă��������B
		''' </remarks>
		''' <exception cref="DbAccessException">
		''' DB�A�N�Z�X�ŃG���[����������
		''' </exception>
		Public Overloads Function ExecuteNonQuery(ByVal commandWrapper As IDbCommandStoredProcedure) As Integer Implements IDbAccess.ExecuteNonQuery
			Return ExecuteNonQuery(commandWrapper)
		End Function

		''' <summary>
		''' DDL�̎��s
		''' </summary>
		''' <param name="commandWrapper">DDL�����s����ׂ�DBCommand�̃��b�p�[�C���X�^���X</param>
		''' <returns>�X�V����</returns>
		''' <remarks>
		''' �����\�b�h���g�p����ꍇ�́A�g�����U�N�V�����̊J�n<see cref="DBAccess.TransactionStart"></see>�A�I��<see cref="DBAccess.TransactionEnd"></see>���s���Ă��������B
		''' </remarks>
		''' <exception cref="DbAccessException">
		''' DB�A�N�Z�X�ŃG���[����������
		''' </exception>
		Public Overloads Function ExecuteNonQuery(ByVal commandWrapper As IDbCommandDDL) As Integer Implements IDbAccess.ExecuteNonQuery
			Return ExecuteNonQuery(commandWrapper)
		End Function

		''' <summary>
		''' �f�[�^���X�V
		''' </summary>
		''' <param name="commandWrapper">�X�V�����s����ׂ�DBCommand�̃��b�p�[�C���X�^���X</param>
		''' <returns></returns>
		''' <remarks></remarks>
		''' <exception cref="DbAccessException">
		''' DB�A�N�Z�X�ŃG���[����������
		''' </exception>
		Public Overloads Function ExecuteNonQuery(ByVal commandWrapper As IDbCommandSelect4Update) As Integer Implements IDbAccess.ExecuteNonQuery
			Return UpdateAdapter(commandWrapper.ResultDataSet, commandWrapper.Adapter)
		End Function

#End Region

	End Class

End Namespace
