
Namespace Db

	''' <summary>
	''' SELECT�������s����ׂ�DBCommand�����b�s���O����C���^�t�F�[�X
	''' </summary>
	''' <remarks>
	''' �f�[�^���o�n�̃X�g�A�h���s�ł��g�p�o���܂����A�X�g�A�h�̎���<see cref="IDbCommandStoredProcedure"/>���g�p���Ă��������B<br/>
	''' <example>
	''' <code lang="vb">
	''' Using dba As IDbAccess = New DbAccess("Connection String")
	''' 	Using cmd As IDbCommandSelect = dba.CreateCommandSelect("SELECT * FROM TableName")
	''' 		If cmd.Execute() &lt;= 0 Then
	''' 			Return Nothing
	''' 		End If
	''' 		Return cmd.ResultDataSet
	''' 	End Using
	''' End Using
	''' </code>
	''' </example>
	''' </remarks>
	Public Interface IDbCommandSelect
		Inherits IDbCommandSql

#Region " Property "

		''' <summary>
		''' Select�������s�������ʂ�ݒ�^�Q��
		''' </summary>
		''' <value>Select�������s��������</value>
		''' <remarks></remarks>
		Property ResultDataSet() As DataSet

		''' <summary>
		''' DataSet���̐擪�e�[�u����Ԃ�
		''' </summary>
		''' <value></value>
		''' <returns>�擪�e�[�u��</returns>
		''' <remarks></remarks>
		ReadOnly Property Result1stTable() As DataTable

		''' <summary>
		''' DataSet���̐擪�e�[�u���ɑ��݂���s�f�[�^��Enumerator��Ԃ�
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		ReadOnly Property Result1stTableRowEnumerator() As IEnumerator(Of DataRow)

#End Region

		''' <summary>
		''' DataSet���̐擪�e�[�u����Ԃ�
		''' </summary>
		''' <typeparam name="T"></typeparam>
		''' <returns>�擪�e�[�u���̃f�[�^���w�肳�ꂽEntity���g�p�����z��ɕϊ����ĕԂ�</returns>
		''' <remarks>
		''' Execute ��ɓ����\�b�h�ŃG���e�B�e�B���擾������<see cref="Execute(OF T)"></see>���g�������������ŃX�e�b�v�����点�܂��B
		''' </remarks>
		<Obsolete("Execute(Of T)() ���g���悤�ɂ��Ă��������B")> _
		Function Result1stTableEntitis(Of T)() As T()

		''' <summary>
		''' DataSet���̐擪�e�[�u���̎w�肳�ꂽ�s��Ԃ�
		''' </summary>
		''' <typeparam name="T"></typeparam>
		''' <param name="index"></param>
		''' <returns>�擪�e�[�u���̃f�[�^���w�肳�ꂽEntity���g�p�����z��ɕϊ����ĕԂ�</returns>
		''' <remarks></remarks>
		Function Result1stTableEntity(Of T)(ByVal index As Integer) As T

		''' <summary>
		''' �N�G�������s���iExecuteScalar�j�A���̃N�G�����Ԃ����ʃZ�b�g�̍ŏ��̍s�ɂ���ŏ��̗��Ԃ��܂��B�]���ȗ�܂��͍s�͖�������܂��B
		''' </summary>
		''' <returns>���ʃZ�b�g�̍ŏ��̍s�ɂ���ŏ��̗�B</returns>
		''' <remarks>
		''' �����\�b�h�͗\�߃f�[�^�x�[�X���I�[�v�����Ă����K�v������܂����A
		''' �I�[�v������Ă��Ȃ��Ƃ��́A�����ŃI�[�v�����ďI�����ɃN���[�Y���܂��B<br/>
		''' �ڍׂ́A<seealso cref="IDbCommand.ExecuteScalar"/> ���Q�Ƃ��Ă��������B
		''' </remarks>
		Function ExecuteScalar() As Object

		''' <summary>
		''' �N�G�������s���iExecuteReader�j�A�w�肳�ꂽ�G���e�B�e�B�ɕϊ����ĕԂ��܂��B
		''' </summary>
		''' <typeparam name="T">�G���e�B�e�B</typeparam>
		''' <returns>�G���e�B�e�B�̃��X�g</returns>
		''' <remarks>
		''' �����\�b�h�͗\�߃f�[�^�x�[�X���I�[�v�����Ă����K�v������܂����A
		''' �I�[�v������Ă��Ȃ��Ƃ��́A�����ŃI�[�v�����ďI�����ɃN���[�Y���܂��B<br/>
		''' �ڍׂ́A<seealso cref="IDbCommand.ExecuteReader"/> ���Q�Ƃ��Ă��������B<br/>
		''' <br/>
		''' �Ȃ��A�����\�b�h���g�p�����ꍇ�͌��ʂ��G���e�B�e�B�Ƃ��Ĉ������Ƃ�O��Ƃ��Ă��邽�߁A<see cref="DataSet"></see>��<see cref="DataTable"></see>�Ƃ��Ă͈����܂���B<br/>
		''' �����<see cref="ResultDataSet"></see>, <see cref="Result1stTable"></see>�Ȃǂ̃��\�b�h�͎g�p�ł��܂���B<br/>
		''' �o�b�`SQL�X�e�[�g�����g����<see cref="NextResult"></see>�ɂĎ��̌��ʂ��擾���Ă��������B
		''' </remarks>
		Overloads Function Execute(Of T)(Optional ByVal behavior As CommandBehavior = CommandBehavior.Default) As IList(Of T)

		''' <summary>
		''' ���̌��ʂ�Ԃ�
		''' </summary>
		''' <typeparam name="T">�G���e�B�e�B</typeparam>
		''' <returns>���݂��Ȃ��Ƃ��� Nothing ��������</returns>
		''' <remarks></remarks>
		Function NextResult(Of T)() As IList(Of T)

	End Interface

End Namespace
