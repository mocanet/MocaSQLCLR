
Imports System.Data.Common
Imports System.Data.SqlClient

Namespace Db.Helper

	''' <summary>�r�p�k�G���[�R�[�h</summary>
	<Flags()> _
	Public Enum SqlDbErrorNumbers
		''' <summary>�d���G���[�R�[�h</summary>
		DuplicationPKey = 2627

		''' <summary>�^�C���A�E�g�G���[�R�[�h</summary>
		''' <remarks>
		''' �ڑ�����r�����b�N���̃f�b�h���b�N�Ń^�C���A�E�g����Ƃ��̃G���[<br/>
		''' �ڑ��G���[���́u�^�C���A�E�g�ɒB���܂����B���삪��������O�Ƀ^�C���A�E�g���Ԃ��߂������A�܂��̓T�[�o�[���������Ă��܂���B�v�ƂȂ�A
		''' SQL���s���́u�^�C���A�E�g�ɒB���܂����B���삪��������O�Ƀ^�C���A�E�g���Ԃ��߂������A�܂��̓T�[�o�[���������Ă��܂���B �X�e�[�g�����g�͏I������܂����B�v�ƂȂ�B<br/>
		''' 
		''' �������A�f�b�h���b�N���� <see cref="SqlException.Number"></see> ���ς��\��������B<br/>
		''' lock_timeout �� 0 �ȊO�̎��́A-2 �ƂȂ�A0 �̂Ƃ��́A�^�C���A�E�g�����ɒ����ɃG���[���������A1222 ���Ԃ��Ă���B<br/>
		''' 1222�́u���b�N�v�����^�C���A�E�g���܂����B�v
		''' </remarks>
		LockTimeOut = -2
		''' <summary>�^�C���A�E�g�G���[�R�[�h</summary>
		''' <remarks>
		''' �ڑ�����r�����b�N���̃f�b�h���b�N�Ń^�C���A�E�g����Ƃ��̃G���[<br/>
		''' �ڑ��G���[���́u�^�C���A�E�g�ɒB���܂����B���삪��������O�Ƀ^�C���A�E�g���Ԃ��߂������A�܂��̓T�[�o�[���������Ă��܂���B�v�ƂȂ�A
		''' SQL���s���́u�^�C���A�E�g�ɒB���܂����B���삪��������O�Ƀ^�C���A�E�g���Ԃ��߂������A�܂��̓T�[�o�[���������Ă��܂���B �X�e�[�g�����g�͏I������܂����B�v�ƂȂ�B<br/>
		''' 
		''' �������A�f�b�h���b�N���� <see cref="SqlException.Number"></see> ���ς��\��������B<br/>
		''' lock_timeout �� 0 �ȊO�̎��́A-2 �ƂȂ�A0 �̂Ƃ��́A�^�C���A�E�g�����ɒ����ɃG���[���������A1222 ���Ԃ��Ă���B<br/>
		''' 1222�́u���b�N�v�����^�C���A�E�g���܂����B�v
		''' </remarks>
		LockTimeOut0 = 1222

		''' <summary>
		''' �X�e�[�g�����g�͏I������܂����B
		''' </summary>
		''' <remarks></remarks>
		StatementEnd = 3621
	End Enum

	''' <summary>
	''' SqlClient���g�p����DB�A�N�Z�X
	''' </summary>
	''' <remarks>
	''' �f�[�^�x�[�X�ڑ���SqlClient���g�p����Ƃ��́A���N���X���g�p���܂��B
	''' </remarks>
	Public Class SqlDbAccessHelper
		Inherits DbAccessHelper
		Implements IDbAccessHelper

		''' <summary>�r�p�k�G���[�R�[�h</summary>
		Public Const C_ERRORCODE As Integer = -2146232060

		''' <summary>�r�p�k�R�l�N�V����</summary>
		Private _conn As SqlConnection

#Region " �R���X�g���N�^ "

		''' <summary>
		''' �R���X�g���N�^
		''' </summary>
		''' <param name="dba">�g�p����f�[�^�x�[�X�A�N�Z�X</param>
		''' <remarks></remarks>
		Public Sub New(ByVal dba As IDao)
			MyBase.New(dba)
			_conn = DirectCast(Me.myDba.Connection, SqlConnection)
		End Sub

#End Region

#Region " Implements IDbAccessHelper "

		''' <summary>
		''' SQL�X�e�[�^�X�̃p�����[�^����ϊ�����B
		''' </summary>
		''' <param name="name">�p�����[�^��</param>
		''' <returns></returns>
		''' <remarks>
		''' �p�����[�^���̐擪�������u���v�łȂ��Ƃ��́u���v��t������B
		''' </remarks>
		Public Function CDbParameterName(ByVal name As String) As String Implements IDbAccessHelper.CDbParameterName
			If Not name.StartsWith(PlaceholderMark) Then
				name = PlaceholderMark & name
			End If
			Return name
		End Function

		''' <summary>
		''' �G���[�̌�����Ԃ�
		''' </summary>
		''' <param name="ex">�G���[�������擾��������O</param>
		''' <returns>�G���[����</returns>
		''' <remarks>
		''' </remarks>
		Public Function ErrorCount(ByVal ex As System.Exception) As Integer Implements IDbAccessHelper.ErrorCount
			Dim dbEx As SqlException

			If Not TypeOf ex Is SqlException Then
				Return 0
			End If

			dbEx = DirectCast(ex, SqlException)

			Return dbEx.Errors.Count
		End Function

		''' <summary>
		''' �G���[�ԍ���Ԃ�
		''' </summary>
		''' <param name="ex">�G���[�ԍ����擾��������O</param>
		''' <returns>�G���[�ԍ��z��</returns>
		''' <remarks>
		''' </remarks>
		Public Function ErrorNumbers(ex As System.Exception) As String() Implements IDbAccessHelper.ErrorNumbers
			If ErrorCount(ex) <= 0 Then
				Return Nothing
			End If

			Dim ary As ArrayList
			Dim dbEx As SqlException

			ary = New ArrayList
			dbEx = DirectCast(ex, SqlException)
			For Each err As SqlError In dbEx.Errors
				ary.Add(err.Number.ToString)
			Next

			Return DirectCast(ary.ToArray(GetType(String)), String())
		End Function

		''' <summary>
		''' ������擾����
		''' </summary>
		''' <param name="table">�擾�ΏۂƂȂ�e�[�u�����̃��f��</param>
		''' <returns>�擾�������񃂃f���̃R���N�V����</returns>
		''' <remarks>
		''' �񖼂̎擾�́uCOLUMN_NAME�v��ƂȂ�B<br/>
		''' ���̑��͉��L���Q�Ƃ��Ă��������B<br/>
		''' http://msdn.microsoft.com/ja-jp/library/ms254969(VS.80).aspx <br/>
		''' </remarks>
		''' <exception cref="DbAccessException">
		''' DB�A�N�Z�X�ŃG���[����������
		''' </exception>
		Public Function GetSchemaColumns(ByVal table As DbInfoTable) As DbInfoColumnCollection Implements IDbAccessHelper.GetSchemaColumns
			Dim dt As DataTable
			Dim results As DbInfoColumnCollection
			Dim info As DbInfoColumn
			Dim openFlag As Boolean

			results = New DbInfoColumnCollection()

			Try
				If Me._conn.State <> ConnectionState.Open Then
					Me._conn.Open()
					openFlag = True
				End If
				dt = Me._conn.GetSchema("Columns", New String() {table.Catalog, table.Schema, table.Name, Nothing})
				For ii As Integer = 0 To dt.Rows.Count - 1
					info = New DbInfoColumn( _
					   CStr(dt.Rows(ii).Item("TABLE_CATALOG")) _
					 , CStr(dt.Rows(ii).Item("TABLE_SCHEMA")) _
					 , CStr(dt.Rows(ii).Item("COLUMN_NAME")) _
					 , CStr(dt.Rows(ii).Item("DATA_TYPE")))
					Dim length As Integer
					Dim scale As Integer
					info.MaxLength = getColumnMaxLength(dt.Rows(ii), length, scale)
					info.Precision = length
					info.Scale = scale
					info.UniCode = isUniCode(info.Typ)
					info.ColumnType = getColumnDbType(Of SqlDbType)(dt.Rows(ii))
					results.Add(info.Name, info)
				Next

				Return results
			Catch ex As Exception
				Throw New DbAccessException(Me.targetDba, ex)
			Finally
				If openFlag Then
					Me._conn.Close()
				End If
			End Try
		End Function

		''' <summary>
		''' �֐������擾����
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		''' <exception cref="DbAccessException">
		''' DB�A�N�Z�X�ŃG���[����������
		''' </exception>
		Public Function GetSchemaFunctions() As DbInfoFunctionCollection Implements IDbAccessHelper.GetSchemaFunctions
			Const C_SQL As String = "SELECT sys_o.name AS name, sys_o.type AS type, sys_c.text AS src, sys_c.ctext FROM syscomments sys_c ,sysobjects sys_o WHERE(sys_c.id=sys_o.id)AND(sys_o.xtype in ('FN','TF'))AND(sys_o.schema_ver=0)"
			Dim results As DbInfoFunctionCollection

			results = New DbInfoFunctionCollection

			Try
				Using cmd As IDbCommandSelect = Me.myDba.CreateCommandSelect(C_SQL)
					If Me.myDba.Execute(cmd) <= 0 Then
						Return results
					End If

					Dim dt As DataTable
					dt = cmd.ResultDataSet.Tables(0)
					For ii As Integer = 0 To dt.Rows.Count - 1
						Dim item As DbInfoFunction
						item = New DbInfoFunction( _
						String.Empty _
						 , String.Empty _
						 , CStr(dt.Rows(ii).Item("name")) _
						 , CStr(dt.Rows(ii).Item("type")))
						If results.ContainsKey(item.ToString) Then
							item.Src &= CStr(dt.Rows(ii).Item("src"))
							Continue For
						End If
						item.Src = CStr(dt.Rows(ii).Item("src"))
						results.Add(item.ToString, item)
					Next
				End Using

				Return results
			Catch ex As Exception
				Throw New DbAccessException(Me.targetDba, ex)
			End Try
		End Function

		''' <summary>
		''' �v���V�[�W�������擾����
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		''' <exception cref="DbAccessException">
		''' DB�A�N�Z�X�ŃG���[����������
		''' </exception>
		Public Function GetSchemaProcedures() As DbInfoProcedureCollection Implements IDbAccessHelper.GetSchemaProcedures
			Const C_SQL As String = "SELECT sys_o.name AS name, sys_o.type AS type, sys_c.text AS src, sys_c.ctext FROM syscomments sys_c ,sysobjects sys_o WHERE(sys_c.id=sys_o.id)AND(sys_o.xtype in ('P'))AND(sys_o.schema_ver=0)"
			Dim results As DbInfoProcedureCollection

			results = New DbInfoProcedureCollection

			Try
				Using cmd As IDbCommandSelect = Me.myDba.CreateCommandSelect(C_SQL)
					If Me.myDba.Execute(cmd) <= 0 Then
						Return results
					End If

					Dim dt As DataTable
					dt = cmd.ResultDataSet.Tables(0)
					For ii As Integer = 0 To dt.Rows.Count - 1
						Dim item As DbInfoProcedure
						item = New DbInfoProcedure( _
						String.Empty _
						 , String.Empty _
						 , CStr(dt.Rows(ii).Item("name")) _
						 , CStr(dt.Rows(ii).Item("type")))
						If results.ContainsKey(item.ToString) Then
							item.Src &= CStr(dt.Rows(ii).Item("src"))
							Continue For
						End If
						item.Src = CStr(dt.Rows(ii).Item("src"))
						results.Add(item.ToString, item)
					Next
				End Using

				Return results
			Catch ex As Exception
				Throw New DbAccessException(Me.targetDba, ex)
			End Try
		End Function

		''' <summary>
		''' �e�[�u�������擾����
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		''' <exception cref="DbAccessException">
		''' DB�A�N�Z�X�ŃG���[����������
		''' </exception>
		Public Function GetSchemaTables() As DbInfoTableCollection Implements IDbAccessHelper.GetSchemaTables
			Dim dt As DataTable
			Dim results As DbInfoTableCollection
			Dim info As DbInfoTable
			Dim openFlag As Boolean

			results = New DbInfoTableCollection()

			Try
				If Me._conn.State <> ConnectionState.Open Then
					Me._conn.Open()
					openFlag = True
				End If
				dt = Me._conn.GetSchema("Tables", New String() {Nothing, Nothing, Nothing, Nothing})
				For ii As Integer = 0 To dt.Rows.Count - 1
					info = New DbInfoTable( _
					   CStr(dt.Rows(ii).Item("TABLE_CATALOG")) _
					 , CStr(dt.Rows(ii).Item("TABLE_SCHEMA")) _
					 , CStr(dt.Rows(ii).Item("TABLE_NAME")) _
					 , CStr(dt.Rows(ii).Item("TABLE_TYPE")))
					results.Add(info.ToString, info)

					info.Columns = GetSchemaColumns(info)
				Next

				Return results
			Catch ex As Exception
				Throw New DbAccessException(Me.targetDba, ex)
			Finally
				If openFlag Then
					Me._conn.Close()
				End If
			End Try
		End Function

		''' <summary>
		''' �e�[�u�������擾����
		''' </summary>
		''' <param name="tablename">�e�[�u����</param>
		''' <returns></returns>
		''' <remarks></remarks>
		''' <exception cref="DbAccessException">
		''' DB�A�N�Z�X�ŃG���[����������
		''' </exception>
		Public Function GetSchemaTable(tablename As String) As DbInfoTable Implements IDbAccessHelper.GetSchemaTable
			Dim dt As DataTable
			Dim results As DbInfoTable
			Dim openFlag As Boolean

			results = Nothing

			Try
				If Me._conn.State <> ConnectionState.Open Then
					Me._conn.Open()
					openFlag = True
				End If
				dt = Me._conn.GetSchema("Tables", New String() {Nothing, Nothing, tablename, Nothing})
				For ii As Integer = 0 To dt.Rows.Count - 1
					results = New DbInfoTable( _
					 CStr(dt.Rows(ii).Item("TABLE_CATALOG")) _
					  , CStr(dt.Rows(ii).Item("TABLE_SCHEMA")) _
					  , CStr(dt.Rows(ii).Item("TABLE_NAME")) _
					  , CStr(dt.Rows(ii).Item("TABLE_TYPE")))

					results.Columns = GetSchemaColumns(results)
				Next

				Return results
			Catch ex As Exception
				Throw New DbAccessException(Me.targetDba, ex)
			Finally
				If openFlag Then
					Me._conn.Close()
				End If
			End Try
		End Function

		''' <summary>
		''' �w�肳�ꂽ�G���[�ԍ�������������O�ɑ��݂��邩�Ԃ�
		''' </summary>
		''' <param name="ex">�ΏۂƂȂ��O</param>
		''' <param name="errorNumber">�G���[�ԍ�</param>
		''' <returns>True:���݂���AFalse:���݂��Ȃ�</returns>
		''' <remarks>
		''' </remarks>
		Public Function HasSqlNativeError(ByVal ex As System.Exception, ByVal errorNumber As Long) As Boolean Implements IDbAccessHelper.HasSqlNativeError
			Dim err As SqlError

			If ErrorCount(ex) <= 0 Then
				Return False
			End If

			Dim dbEx As SqlException

			dbEx = DirectCast(ex, SqlException)
			For Each err In dbEx.Errors
				If err.Number = errorNumber Then
					Return True
				End If
			Next

			Return False
		End Function

		''' <summary>
		''' �d���G���[������������O�ɑ��݂��邩�Ԃ�
		''' </summary>
		''' <param name="ex">�ΏۂƂȂ��O</param>
		''' <returns>True:���݂���AFalse:���݂��Ȃ�</returns>
		''' <remarks>
		''' </remarks>
		Public Function HasSqlNativeErrorDuplicationPKey(ByVal ex As System.Exception) As Boolean Implements IDbAccessHelper.HasSqlNativeErrorDuplicationPKey
			Return HasSqlNativeError(ex, SqlDbErrorNumbers.DuplicationPKey)
		End Function

		''' <summary>
		''' �^�C���A�E�g�G���[������������O�ɑ��݂��邩�Ԃ�
		''' </summary>
		''' <param name="ex">�ΏۂƂȂ��O</param>
		''' <returns>True:���݂���AFalse:���݂��Ȃ�</returns>
		''' <remarks>
		''' </remarks>
		Public Function HasSqlNativeErrorTimtout(ByVal ex As System.Exception) As Boolean Implements IDbAccessHelper.HasSqlNativeErrorTimtout
			Return (HasSqlNativeError(ex, SqlDbErrorNumbers.LockTimeOut) OrElse HasSqlNativeError(ex, SqlDbErrorNumbers.LockTimeOut0))
		End Function

		''' <summary>
		''' SQL�v���[�X�t�H���_�̃}�[�N��Ԃ��B
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public ReadOnly Property PlaceholderMark() As String Implements IDbAccessHelper.PlaceholderMark
			Get
				Return "@"
			End Get
		End Property

		''' <summary>
		''' �v���V�[�W���̃p�����[�^�����擾����
		''' </summary>
		''' <param name="cmd"></param>
		''' <remarks></remarks>
		''' <exception cref="DbAccessException">
		''' DB�A�N�Z�X�ŃG���[����������
		''' </exception>
		Public Sub RefreshProcedureParameters(ByVal cmd As System.Data.IDbCommand) Implements IDbAccessHelper.RefreshProcedureParameters
			Try
				Dim openFlg As Boolean = False

				' �R�l�N�V���������Ă�ꍇ�͈�U�ڑ�����
				If cmd.Connection.State = ConnectionState.Closed Then
					cmd.Connection.Open()
					openFlg = True
				End If
				SqlCommandBuilder.DeriveParameters(DirectCast(cmd, SqlCommand))
				' �R�l�N�V���������Ă��ꍇ�͐ڑ���؂�
				If openFlg Then
					cmd.Connection.Close()
				End If

				For Each para As IDataParameter In cmd.Parameters
					para.Value = DBNull.Value
				Next
			Catch ex As Exception
				Throw New DbAccessException(Me.targetDba, ex)
			End Try
		End Sub

#End Region

#Region " Methods "

		''' <summary>
		''' ��̍ő包����Ԃ�
		''' </summary>
		''' <param name="row">�s�f�[�^</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Protected Function getColumnMaxLength(ByVal row As DataRow, ByRef length As Integer, ByRef scale As Integer) As Integer
			Dim wlength As Object = getColumnLength(row)

			' ���̎w�肪���鎞�́A���ݒ肵�ďI��
			' �i�o�C�i�� �f�[�^�A�����f�[�^�A�܂��̓e�L�X�g/�C���[�W �f�[�^�̍ő咷�j
			If Not DBNull.Value.Equals(wlength) Then
				length = 0
				scale = 0
				Return CInt(wlength)
			End If

			' ���̎w�肪�����Ƃ����l�n�̌������擾
			wlength = getColumnPrecision(row)

			' �������݂��Ȃ����͌��w��Ȃ��ŏI��
			If DBNull.Value.Equals(wlength) Then
				length = 0
				scale = 0
				Return length
			End If

			' �T���f�[�^�A�^���f�[�^�A�����f�[�^�A�܂��͒ʉ݂̂Ƃ�
			Dim wscale As Object = getColumnScale(row)

			' �����_���Ȃ���
			If DBNull.Value.Equals(wscale) Then
				length = CInt(wlength)
				scale = 0
				Return length
			End If

			' �����_�����鎞
			'Return CInt(length) + CInt(scale) + 1
			length = CInt(wlength)
			scale = CInt(wscale)
			Return length + 1
		End Function

		''' <summary>
		''' ��̌�����Ԃ��܂��B
		''' </summary>
		''' <param name="row">�s�f�[�^</param>
		''' <returns>�e�[�u�����͗񂪑��݂��Ȃ��Ƃ���A�����̎w�肪�s�v�Ȍ^�̎��� DBNull.Value ��Ԃ��܂��B</returns>
		''' <remarks>
		''' �o�C�i�� �f�[�^�A�����f�[�^�A�܂��̓e�L�X�g/�C���[�W �f�[�^�̍ő咷 (�����P��)�B
		''' ����ȊO�̏ꍇ�́ANULL ���Ԃ���܂��B
		''' �ڍׂɂ��ẮA�wMicrosoft SQL Server 2000 Transact-SQL �v���O���}�[�Y���t�@�����X��x�́u�� 3 �� Transact-SQL �̃f�[�^�^�v���Q�Ƃ��Ă��������B
		''' </remarks>
		Protected Function getColumnLength(ByVal row As DataRow) As Object
			If row Is Nothing Then
				Return DBNull.Value
			End If

			Return row.Item("character_maximum_length")
		End Function

		''' <summary>
		''' ��̌�����Ԃ��܂��B
		''' </summary>
		''' <param name="row">�s�f�[�^</param>
		''' <returns>�e�[�u�����͗񂪑��݂��Ȃ��Ƃ���A�����̎w�肪�s�v�Ȍ^�̎��� DBNull.Value ��Ԃ��܂��B</returns>
		''' <remarks>
		''' �T���f�[�^�A�^���f�[�^�A�����f�[�^�A�܂��͒ʉ݃f�[�^�̗L�������B����ȊO�̏ꍇ�́ANULL ���Ԃ���܂��B
		''' </remarks>
		Protected Function getColumnPrecision(ByVal row As DataRow) As Object
			If row Is Nothing Then
				Return DBNull.Value
			End If

			Return row.Item("NUMERIC_PRECISION")
		End Function

		''' <summary>
		''' ��̌�����Ԃ��܂��B
		''' </summary>
		''' <param name="row">�s�f�[�^</param>
		''' <returns>�e�[�u�����͗񂪑��݂��Ȃ��Ƃ���A�����̎w�肪�s�v�Ȍ^�̎��� DBNull.Value ��Ԃ��܂��B</returns>
		''' <remarks>
		''' �T���f�[�^�A�^���f�[�^�A�����f�[�^�A�܂��͒ʉ݃f�[�^�̌����B����ȊO�̏ꍇ�́ANULL ���Ԃ���܂��B
		''' </remarks>
		Protected Function getColumnScale(ByVal row As DataRow) As Object
			If row Is Nothing Then
				Return DBNull.Value
			End If

			Return row.Item("NUMERIC_SCALE")
		End Function

		''' <summary>
		''' ��̌^���������܂��B
		''' </summary>
		''' <typeparam name="T">�g�p����^��DbType���w�肷��</typeparam>
		''' <param name="row">�s�f�[�^</param>
		''' <returns>�e�[�u�����͗񂪑��݂��Ȃ��Ƃ��� DBNull.Value ��Ԃ��܂��B</returns>
		''' <remarks>
		''' SQLServer �� numeric �� SqlDbType �ɂ͑��݂��Ȃ����� Decimal �Ƀ}�b�v���܂��B
		''' </remarks>
		Protected Function getColumnDbType(Of T)(ByVal row As DataRow) As T
			Dim typ As String

			If row Is Nothing Then
				Return Nothing
			End If

			typ = CStr(row.Item("DATA_TYPE"))

			If GetType(T).Name.Equals("SqlDbType") Then
				' numeric �� SqlDbType �ɂ͑��݂��Ȃ����� Decimal �Ƀ}�b�v����
				If typ.Equals("numeric") Then
					typ = SqlDbType.Decimal.ToString
				End If
			End If

			Return DirectCast([Enum].Parse(GetType(T), typ, True), T)
		End Function

		''' <summary>
		''' �^��UniCode������
		''' </summary>
		''' <param name="typ"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Private Function isUniCode(ByVal typ As String) As Boolean
			Select Case typ
				Case "nvarchar", "nchar"
					Return True
				Case Else
					Return False
			End Select
		End Function

#End Region

#Region " Debug "

		Public Function ConnectionTest() As DataTable
			Dim dt As DataTable
			Dim openFlag As Boolean

			Try
				If Me._conn.State <> ConnectionState.Open Then
					Me._conn.Open()
					openFlag = True
				End If

				dt = Me._conn.GetSchema("MetaDataCollections")

				Return dt
			Finally
				If openFlag Then
					Me._conn.Close()
				End If
			End Try
		End Function

#End Region

	End Class

End Namespace
