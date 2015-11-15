
Namespace Db.CommandWrapper

	''' <summary>
	''' SELECT�������s����ׂ�DBCommand�̃��b�p�[�N���X
	''' </summary>
	''' <remarks></remarks>
	Public Class SelectCommandWrapper
		Inherits SqlCommandWrapper
		Implements IDbCommandSelect

		''' <summary>���ʂ̍s�f�[�^</summary>
		Private _dtEnum As IEnumerator(Of DataRow)
		''' <summary>Select�������s��������</summary>
		Protected ds As DataSet
		''' <summary>Select�������s��������(Reader��)</summary>
		Protected executeResult As ISQLStatementResult

#Region " Constructor/DeConstructor "

		''' <summary>
		''' �R���X�g���N�^
		''' </summary>
		''' <param name="dba">�e�ƂȂ�DBAccess�C���X�^���X</param>
		''' <param name="cmd">���s����DBCommand�C���X�^���X</param>
		''' <remarks>
		''' </remarks>
		Friend Sub New(ByVal dba As IDao, ByVal cmd As IDbCommand)
			MyBase.New(dba, cmd)
		End Sub

		''' <summary>
		''' �j��
		''' </summary>
		''' <param name="disposing"></param>
		''' <remarks></remarks>
		Protected Overrides Sub Dispose(disposing As Boolean)
			MyBase.Dispose(disposing)
			If Not ds Is Nothing Then
				ds.Dispose()
			End If
			If executeResult IsNot Nothing Then
				executeResult.Dispose()
			End If
		End Sub

#End Region

		''' <summary>
		''' SQL���s�I
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Overrides Function Execute() As Integer
			Return dba.Execute(Me)
		End Function

#Region " Implements IDbCommandSelect "

#Region " Property "

		''' <summary>
		''' Select�������s�������ʂ�ݒ�^�Q��
		''' </summary>
		''' <value>Select�������s��������</value>
		''' <remarks>
		''' </remarks>
		Public Property ResultDataSet() As DataSet Implements IDbCommandSelect.ResultDataSet
			Get
				Return ds
			End Get
			Set(ByVal Value As DataSet)
				Dispose()
				ds = Value
				If Result1stTable Is Nothing Then
					_dtEnum = Nothing
				Else
					_dtEnum = DirectCast(Result1stTable.Rows.GetEnumerator, IEnumerator(Of DataRow))
				End If
			End Set
		End Property

		''' <summary>
		''' DataSet���̐擪�e�[�u����Ԃ�
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public ReadOnly Property Result1stTable() As System.Data.DataTable Implements IDbCommandSelect.Result1stTable
			Get
				If ds.Tables.Count = 0 Then
					Return Nothing
				End If
				Return ds.Tables(0)
			End Get
		End Property

		''' <summary>
		''' DataSet���̐擪�e�[�u���ɑ��݂���s�f�[�^��Enumerator��Ԃ�
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public ReadOnly Property Result1stTableRowEnumerator() As IEnumerator(Of DataRow) Implements IDbCommandSelect.Result1stTableRowEnumerator
			Get
				Return _dtEnum
			End Get
		End Property

#End Region

		''' <summary>
		''' �N�G�������s���A�w�肳�ꂽ�G���e�B�e�B�ɕϊ����ĕԂ��܂��B
		''' </summary>
		''' <typeparam name="T">�G���e�B�e�B</typeparam>
		''' <returns>�G���e�B�e�B�̃��X�g</returns>
		''' <remarks>
		''' �����\�b�h�͗\�߃f�[�^�x�[�X���I�[�v�����Ă����K�v������܂����A
		''' �I�[�v������Ă��Ȃ��Ƃ��́A�����ŃI�[�v�����ďI�����ɃN���[�Y���܂��B<br/>
		''' </remarks>
		Public Overridable Overloads Function Execute(Of T)(Optional behavior As CommandBehavior = CommandBehavior.Default) As System.Collections.Generic.IList(Of T) Implements IDbCommandSelect.Execute
			executeResult = dba.Execute(Of T)(Me)
			Return executeResult.Result(Of T)()
		End Function

		''' <summary>
		''' �N�G�������s���A���̃N�G�����Ԃ����ʃZ�b�g�̍ŏ��̍s�ɂ���ŏ��̗��Ԃ��܂��B�]���ȗ�܂��͍s�͖�������܂��B
		''' </summary>
		''' <returns>���ʃZ�b�g�̍ŏ��̍s�ɂ���ŏ��̗�B</returns>
		''' <remarks>
		''' �����\�b�h�͗\�߃f�[�^�x�[�X���I�[�v�����Ă����K�v������܂����A
		''' �I�[�v������Ă��Ȃ��Ƃ��́A�����ŃI�[�v�����ďI�����ɃN���[�Y���܂��B<br/>
		''' �ڍׂ́A<seealso cref="IDbCommand.ExecuteScalar"/> ���Q�Ƃ��Ă��������B
		''' </remarks>
		Public Overridable Function ExecuteScalar() As Object Implements IDbCommandSelect.ExecuteScalar
			Return dba.ExecuteScalar(Me)
		End Function

		''' <summary>
		''' DataSet���̐擪�e�[�u���ɑ��݂���s�f�[�^��Enumerator��Ԃ�
		''' </summary>
		''' <typeparam name="T"></typeparam>
		''' <returns></returns>
		''' <remarks>
		''' ���݂��Ȃ��Ƃ��́A��̔z���Ԃ��B
		''' </remarks>
		<Obsolete("Execute(Of T)() ���g���悤�ɂ��Ă��������B")> _
		Public Function Result1stTableEntitis(Of T)() As T() Implements IDbCommandSelect.Result1stTableEntitis
			Return entityBuilder.Create(Of T)(Me.Result1stTable)
		End Function

		''' <summary>
		''' DataSet���̐擪�e�[�u���̎w�肳�ꂽ�s��Ԃ�
		''' </summary>
		''' <typeparam name="T"></typeparam>
		''' <param name="index"></param>
		''' <returns>�擪�e�[�u���̃f�[�^���w�肳�ꂽEntity���g�p�����z��ɕϊ����ĕԂ�</returns>
		''' <remarks>
		''' ���݂��Ȃ��Ƃ��́ANothing ��Ԃ��B
		''' </remarks>
		Public Function Result1stTableEntity(Of T)(ByVal index As Integer) As T Implements IDbCommandSelect.Result1stTableEntity
			If Me.Result1stTable.Rows.Count = 0 Then
				Return Nothing
			End If
			If Me.Result1stTable.Rows.Count <= index Then
				Return Nothing
			End If
			Dim dr As DataRow
			dr = Me.Result1stTable.DefaultView.Item(index).Row
			Return entityBuilder.Create(Of T)(dr)
		End Function

		Public Function NextResult(Of T)() As System.Collections.Generic.IList(Of T) Implements IDbCommandSelect.NextResult
			Return executeResult.NextResult(Of T)()
		End Function

#End Region

	End Class

End Namespace
