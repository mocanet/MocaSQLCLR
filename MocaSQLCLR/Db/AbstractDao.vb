﻿
Imports System.Data.Common
Imports Moca.Db.CommandWrapper

Namespace Db

	''' <summary>
	''' Data Access Object のインタフェースの実装抽象クラス
	''' </summary>
	''' <remarks>
	''' データベースアクセスする際に最低限必要と思われる機能を提供します。<br/>
	''' 各システムで Data Access Object（DAO）を使用するときは、等クラスを継承します。<br/>
	''' </remarks>
	Public MustInherit Class AbstractDao
		Inherits MarshalByRefObject
		Implements IDao, IDisposable

#Region " Declare "

		''' <summary>DBMS</summary>
		Private _dbms As Dbms
		''' <summary>コネクションオブジェクト</summary>
		Private _conn As IDbConnection
		''' <summary>アダプタオブジェクト</summary>
		Private _adp As IDbDataAdapter
		''' <summary>ヘルパーオブジェクト</summary>
		Private _dbaHelper As IDbAccessHelper
		''' <summary>コマンドラッパーオブジェクト</summary>
		Private _commandWrapper As IDbCommandSql
		''' <summary>コマンド履歴有無</summary>
		Private _executeHistory As Boolean
		''' <summary>更新コマンド履歴有無</summary>
		Private _executeUpdateHistory As Boolean
		''' <summary>コマンド履歴</summary>
		Private _executeHistoryList As IList(Of String)
		''' <summary>ロールバック有無</summary>
		Private _rollbackStatus As Boolean

		''' <summary>トランザクション制御情報</summary>
		Private _txContext As Tx.ITransactionContext

#End Region

#Region " Constructor/DeConstructor "

		''' <summary>
		''' デフォルトコンストラクタ
		''' </summary>
		''' <remarks>
		''' 外部からは利用不可
		''' </remarks>
		Protected Sub New()
			_executeHistory = False
			_executeUpdateHistory = False
		End Sub

		''' <summary>
		''' コンストラクタ
		''' </summary>
		''' <param name="myDbms">接続先のDBMS</param>
		''' <remarks></remarks>
		Public Sub New(ByVal myDbms As Dbms)
			MyClass.New()
			Me._dbms = myDbms
			_conn = myDbms.CreateConnection()
			_adp = myDbms.ProviderFactory.CreateDataAdapter()
		End Sub

#End Region
#Region " IDisposable Support "

		Private disposedValue As Boolean = False		' 重複する呼び出しを検出するには

		' IDisposable
		Protected Overridable Sub Dispose(ByVal disposing As Boolean)
			If Not Me.disposedValue Then
				If disposing Then
					' TODO: 明示的に呼び出されたときにマネージ リソースを解放します
				End If

				' TODO: 共有のアンマネージ リソースを解放します
				Me.Disposing()

				If _dbaHelper IsNot Nothing Then
					_dbaHelper.Dispose()
				End If
				If _conn IsNot Nothing Then
					If _conn.State <> ConnectionState.Closed Then
						_conn.Close()
					End If
					_conn.Dispose()
				End If

			End If
			Me.disposedValue = True
		End Sub

		' このコードは、破棄可能なパターンを正しく実装できるように Visual Basic によって追加されました。
		Public Sub Dispose() Implements IDisposable.Dispose
			' このコードを変更しないでください。クリーンアップ コードを上の Dispose(ByVal disposing As Boolean) に記述します。
			Dispose(True)
			GC.SuppressFinalize(Me)
		End Sub

		Protected Overridable Sub Disposing()

		End Sub

#End Region
#Region " Property "

		''' <summary>
		''' コネクションオブジェクト
		''' </summary>
		''' <value></value>
		''' <remarks>
		''' </remarks>
		Public Overridable ReadOnly Property Connection() As IDbConnection Implements IDao.Connection
			Get
				Return _conn
			End Get
		End Property

		''' <summary>
		''' アダプタオブジェクト
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public ReadOnly Property Adapter() As System.Data.IDbDataAdapter Implements IDao.Adapter
			Get
				Return _adp
			End Get
		End Property

		''' <summary>
		''' DBMS
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public ReadOnly Property Dbms() As Dbms Implements IDao.Dbms
			Get
				Return _dbms
			End Get
		End Property

		''' <summary>
		''' ヘルパークラスを返す
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public ReadOnly Property Helper() As IDbAccessHelper Implements IDao.Helper
			Get
				If _dbaHelper Is Nothing Then
					_dbaHelper = _dbms.GetHelper(Me)
				End If
				Return _dbaHelper
			End Get
		End Property

		''' <summary>
		''' DBMS
		''' </summary>
		''' <value></value>
		''' <remarks>
		''' </remarks>
		Friend Overridable WriteOnly Property TargetDbms() As Dbms
			Set(ByVal value As Dbms)
				_dbms = value
				_conn = _dbms.ProviderFactory.CreateConnection()
				_conn.ConnectionString = _dbms.ConnectionStringSettings.ConnectionString
				_adp = _dbms.ProviderFactory.CreateDataAdapter()
			End Set
		End Property

		Public ReadOnly Property CommandWrapper() As IDbCommandSql Implements IDao.CommandWrapper
			Get
				Return _commandWrapper
			End Get
		End Property

		Public Property ExecuteHistory() As Boolean Implements IDao.ExecuteHistory
			Get
				Return _executeHistory
			End Get
			Set(ByVal value As Boolean)
				_executeHistory = value
				_executeUpdateHistory = value
				If _executeHistory Then
					_executeHistoryList = New List(Of String)
				Else
					_executeHistoryList = Nothing
				End If
			End Set
		End Property

		Public Property ExecuteUpdateHistory() As Boolean Implements IDao.ExecuteUpdateHistory
			Get
				Return _executeUpdateHistory
			End Get
			Set(ByVal value As Boolean)
				_executeUpdateHistory = value
				If _executeUpdateHistory Then
					_executeHistoryList = New List(Of String)
				Else
					_executeHistoryList = Nothing
				End If
			End Set
		End Property

		Public ReadOnly Property ExecuteHistories() As IList(Of String) Implements IDao.ExecuteHistories
			Get
				Return _executeHistoryList
			End Get
		End Property

		''' <summary>
		''' ロールバック有無
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Property RollbackStatus() As Boolean Implements IDao.RollbackStatus
			Get
				If _txContext IsNot Nothing Then
					Return _txContext.RollbackStatus
				End If
				Return _rollbackStatus
			End Get
			Set(ByVal value As Boolean)
				_rollbackStatus = value
				If _txContext Is Nothing Then
					Return
				End If
				_txContext.RollbackStatus = value
			End Set
		End Property

#End Region
#Region " Check "

		''' <summary>
		''' 接続確認の為に一度接続してみる
		''' </summary>
		''' <remarks>
		''' 接続出来たときは切断します。
		''' </remarks>
		''' <exception cref="DbAccessException">
		''' DBアクセスでエラーが発生した
		''' </exception>
		Public Sub CheckConnect() Implements IDao.CheckConnect
			Try
				_conn.Open()
			Catch ex As Exception
				Throw New DbAccessException(Me, ex)
			Finally
				If _conn.State <> ConnectionState.Closed Then
					_conn.Close()
				End If
			End Try
		End Sub

#End Region
#Region " Create "

		''' <summary>
		''' 指定されたタイプのDbCommandインスタンスを生成する
		''' </summary>
		''' <param name="sqlCommandType">コマンド種別</param>
		''' <param name="commandText">実行するSQL文又は、ストアド名</param>
		''' <param name="useConn">使用するコネクション</param>
		''' <returns>指定されたタイプのインスタンス</returns>
		''' <remarks>
		''' コマンド種別に該当するISqlCommandのインスタンスを生成します。<br/>
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
		''' DBアクセスでエラーが発生した
		''' </exception>
		Protected Friend Overridable Function createCommandWrapper(ByVal sqlCommandType As SQLCommandTypes, ByVal commandText As String, ByVal useConn As IDbConnection) As IDbCommandSql
			Dim cmd As IDbCommand
			Dim cmdWrapper As IDbCommandSql

			Try
				' クラスのインスタンスの生成
				cmd = _dbms.ProviderFactory.CreateCommand()

				cmd.Connection = _conn
				If useConn IsNot Nothing Then
					cmd.Connection = useConn
				End If

				cmd.Transaction = _getTransaction()

				cmd.CommandText = commandText

				Select Case sqlCommandType
					Case SQLCommandTypes.SelectText
						cmd.CommandType = CommandType.Text
						cmdWrapper = New SelectCommandWrapper(Me, cmd)
					Case SQLCommandTypes.Select4Update
						cmd.CommandType = CommandType.Text
						cmdWrapper = New Select4UpdateCommandWrapper(Me, cmd)
					Case SQLCommandTypes.UpdateText
						cmd.CommandType = CommandType.Text
						cmdWrapper = New UpdateCommandWrapper(Me, cmd)
					Case SQLCommandTypes.InsertText
						cmd.CommandType = CommandType.Text
						cmdWrapper = New InsertCommandWrapper(Me, cmd)
					Case SQLCommandTypes.DeleteText
						cmd.CommandType = CommandType.Text
						cmdWrapper = New DeleteCommandWrapper(Me, cmd)
					Case SQLCommandTypes.StoredProcedure
						cmd.CommandType = CommandType.StoredProcedure
						cmdWrapper = New StoredProcedureCommandWrapper(Me, cmd)
					Case SQLCommandTypes.DDL
						cmd.CommandType = CommandType.Text
						cmdWrapper = New DDLCommandWrapper(Me, cmd)
					Case Else
						cmdWrapper = Nothing
				End Select

				' 未使用にしてます。
				'' パラメータキャッシュを取得
				'_sqlParameterCacheMgr.GetParameterSet(cmd)

				Return cmdWrapper
			Catch ex As Exception
				Throw New DbAccessException(Me, ex)
			End Try
		End Function

		''' <summary>
		''' SELECT文を実行する為のDBCommandのラッパークラスを生成する。
		''' </summary>
		''' <param name="commandText">SELECT文文字列</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Overridable Function CreateCommandSelect(ByVal commandText As String) As IDbCommandSelect Implements IDao.CreateCommandSelect
			Return DirectCast(createCommandWrapper(SQLCommandTypes.SelectText, commandText, _getTxConnection), IDbCommandSelect)
		End Function

		''' <summary>
		''' SELECT文を実行し、DataSetを使ってUPDATEする為のDBCommandのラッパークラスを生成する。
		''' </summary>
		''' <param name="commandText">SELECT文文字列</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Overridable Function CreateCommandSelect4Update(ByVal commandText As String) As IDbCommandSelect4Update Implements IDao.CreateCommandSelect4Update
			Return DirectCast(createCommandWrapper(SQLCommandTypes.Select4Update, commandText, _getTxConnection), IDbCommandSelect4Update)
		End Function

		''' <summary>
		''' INSERT文を実行する為のDBCommandのラッパークラスを生成する。
		''' </summary>
		''' <param name="commandText">INSERT文文字列</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Overridable Function CreateCommandInsert(ByVal commandText As String) As IDbCommandInsert Implements IDao.CreateCommandInsert
			Return DirectCast(createCommandWrapper(SQLCommandTypes.InsertText, commandText, _getTxConnection), IDbCommandInsert)
		End Function

		''' <summary>
		''' UPDATE文を実行する為のDBCommandのラッパークラスを生成する。
		''' </summary>
		''' <param name="commandText">UPDATE文文字列</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Overridable Function CreateCommandUpdate(ByVal commandText As String) As IDbCommandUpdate Implements IDao.CreateCommandUpdate
			Return DirectCast(createCommandWrapper(SQLCommandTypes.UpdateText, commandText, _getTxConnection), IDbCommandUpdate)
		End Function

		''' <summary>
		''' DELETE文を実行する為のDBCommandのラッパークラスを生成する。
		''' </summary>
		''' <param name="commandText">DELETE文文字列</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Overridable Function CreateCommandDelete(ByVal commandText As String) As IDbCommandDelete Implements IDao.CreateCommandDelete
			Return DirectCast(createCommandWrapper(SQLCommandTypes.DeleteText, commandText, _getTxConnection), IDbCommandDelete)
		End Function

		''' <summary>
		''' StoredProcedureを実行する為のDBCommandのラッパークラスを生成する。
		''' </summary>
		''' <param name="commandText">ストアド名</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Overridable Function CreateCommandStoredProcedure(ByVal commandText As String) As IDbCommandStoredProcedure Implements IDao.CreateCommandStoredProcedure
			Return DirectCast(createCommandWrapper(SQLCommandTypes.StoredProcedure, commandText, _getTxConnection), IDbCommandStoredProcedure)
		End Function

		''' <summary>
		''' DDLを実行する為のDBCommandのラッパークラスを生成する。
		''' </summary>
		''' <param name="commandText">DDL文文字列</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Overridable Function CreateCommandDDL(ByVal commandText As String) As IDbCommandDDL Implements IDao.CreateCommandDDL
			Return DirectCast(createCommandWrapper(SQLCommandTypes.DDL, commandText, _getTxConnection), IDbCommandDDL)
		End Function

#End Region
#Region " Execute "

		''' <summary>
		''' クエリを実行し、そのクエリが返す結果セットの最初の行にある最初の列を返します。余分な列または行は無視されます。
		''' </summary>
		''' <param name="commandWrapper"></param>
		''' <returns>結果セットの最初の行にある最初の列。</returns>
		''' <remarks>
		''' 当メソッドは予めデータベースをオープンしておく必要がありますが、
		''' オープンされていないときは、自動でオープンして終了時にクローズします。<br/>
		''' 詳細は、<seealso cref="IDbCommand.ExecuteScalar"/> を参照してください。
		''' </remarks>
		Public Overridable Function ExecuteScalar(ByVal commandWrapper As IDbCommandSelect) As Object Implements IDao.ExecuteScalar
			Dim cmd As IDbCommand = Nothing
			Dim result As Object
			Dim myOpen As Boolean

			Try
				_commandWrapper = commandWrapper

				cmd = commandWrapper.Command

				If cmd.Connection.State <> ConnectionState.Open Then
					cmd.Connection.Open()
					myOpen = True
				End If

				result = cmd.ExecuteScalar()

				' 未使用にしてます。
				'If Not commandWrapper.PreparedStatement Then
				'    If Not _sqlParameterCacheMgr.Contains(cmd) Then
				'        _sqlParameterCacheMgr.PutParameterSet(cmd)
				'    End If
				'End If

				Return result
			Catch ex As DbAccessException
				Throw ex
			Catch ex As Exception
				Throw New DbAccessException(Me, ex)
			Finally
				_addExecuteHistory(cmd, _executeHistory)
				If myOpen Then
					cmd.Connection.Close()
				End If
				Call cmd.Dispose()
				'_commandWrapper = Nothing
			End Try
		End Function

		''' <summary>
		''' SELECT文の実行
		''' </summary>
		''' <param name="commandWrapper">SELECT文を実行する為のDBCommandのラッパーインスタンス</param>
		''' <returns>データ件数</returns>
		''' <remarks>
		''' </remarks>
		''' <exception cref="DbAccessException">
		''' DBアクセスでエラーが発生した
		''' </exception>
		Public Overridable Function Execute(ByVal commandWrapper As IDbCommandSelect) As Integer Implements IDao.Execute
			Dim cmd As IDbCommand = Nothing
			Dim ds As DataSet = Nothing
			Dim result As Integer

			commandWrapper.ResultOutParameter.Clear()

			Try
				_commandWrapper = commandWrapper

				cmd = commandWrapper.Command

				Me.Adapter.SelectCommand = cmd
				result = fill(ds)
				commandWrapper.ResultDataSet = ds

				' 未使用にしてます。
				'If Not commandWrapper.PreparedStatement Then
				'    If Not _sqlParameterCacheMgr.Contains(cmd) Then
				'        _sqlParameterCacheMgr.PutParameterSet(cmd)
				'    End If
				'End If

				Return result
			Catch ex As DbAccessException
				Throw ex
			Catch ex As Exception
				Throw New DbAccessException(Me, ex)
			Finally
				_addExecuteHistory(cmd, _executeHistory)
				cmd.Dispose()
				'_commandWrapper = Nothing
			End Try
		End Function

		''' <summary>
		''' SELECT文の実行(ExecuteReader)
		''' </summary>
		''' <typeparam name="T">エンティティ</typeparam>
		''' <param name="commandWrapper">コマンドラッパー</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Function Execute(Of T)(commandWrapper As IDbCommandSelect, Optional behavior As CommandBehavior = CommandBehavior.Default) As ISQLStatementResult Implements IDao.Execute
			Dim cmd As IDbCommand = Nothing
			Dim ds As DataSet = Nothing
			Dim result As ISQLStatementResult
			Dim myOpen As Boolean

			Try
				_commandWrapper = commandWrapper

				cmd = commandWrapper.Command

				If cmd.Connection.State <> ConnectionState.Open Then
					cmd.Connection.Open()
					myOpen = True
				End If

				Dim reader As IDataReader
				reader = cmd.ExecuteReader(behavior)
				result = New ExecuteReaderResult(cmd, myOpen, reader)

				' 未使用にしてます。
				'If Not commandWrapper.PreparedStatement Then
				'    If Not _sqlParameterCacheMgr.Contains(cmd) Then
				'        _sqlParameterCacheMgr.PutParameterSet(cmd)
				'    End If
				'End If

				Return result
			Catch ex As DbAccessException
				Throw ex
			Catch ex As Exception
				Throw New DbAccessException(Me, ex)
			Finally
				_addExecuteHistory(cmd, _executeHistory)
				'_commandWrapper = Nothing
			End Try
		End Function

		''' <summary>
		''' SELECT文の実行(後にAdapterを利用した更新を行う場合)
		''' </summary>
		''' <param name="commandWrapper">SELECT文を実行する為のDBCommandのラッパーインスタンス</param>
		''' <returns>データ件数</returns>
		''' <remarks>
		''' SELECT実行後のデータ更新をDataSetを使って更新する場合は、こちらを使用してください。<br/>
		''' 予めAdapterとCommandを関連付けます。
		''' </remarks>
		''' <exception cref="DbAccessException">
		''' DBアクセスでエラーが発生した
		''' </exception>
		Public Overridable Function Execute(ByVal commandWrapper As IDbCommandSelect4Update) As Integer Implements IDao.Execute
			Dim cmd As IDbCommand = Nothing
			Dim cmdBuilder As DbCommandBuilder = Nothing
			Dim ds As DataSet = Nothing
			Dim result As Integer

			cmdBuilder = Me.Dbms.ProviderFactory.CreateCommandBuilder()
			cmdBuilder.DataAdapter = DirectCast(commandWrapper.Adapter, DbDataAdapter)

			Try
				_commandWrapper = commandWrapper

				cmd = commandWrapper.Command

				cmdBuilder.DataAdapter.SelectCommand = DirectCast(cmd, DbCommand)

				cmdBuilder.DataAdapter.InsertCommand = cmdBuilder.GetInsertCommand
				cmdBuilder.DataAdapter.UpdateCommand = cmdBuilder.GetUpdateCommand
				cmdBuilder.DataAdapter.DeleteCommand = cmdBuilder.GetDeleteCommand

				result = fill(ds, commandWrapper.Adapter)
				commandWrapper.ResultDataSet = ds

				' 未使用にしてます。
				'If Not commandWrapper.PreparedStatement Then
				'    If Not _sqlParameterCacheMgr.Contains(cmd) Then
				'        _sqlParameterCacheMgr.PutParameterSet(cmd)
				'    End If
				'End If

				Return result
			Catch ex As DbAccessException
				Throw ex
			Catch ex As Exception
				Throw New DbAccessException(Me, ex)
			Finally
				_addExecuteHistory(cmd, _executeUpdateHistory)
				cmd.Dispose()
				'_commandWrapper = Nothing
			End Try
		End Function

		''' <summary>
		''' INSERT,UPDATE,DELETE文の実行
		''' </summary>
		''' <param name="commandWrapper">実行する為のDBCommandのラッパーインスタンス</param>
		''' <returns>更新件数</returns>
		''' <remarks>
		''' </remarks>
		''' <exception cref="DbAccessException">
		''' DBアクセスでエラーが発生した
		''' </exception>
		Public Overridable Function ExecuteNonQuery(ByVal commandWrapper As IDbCommandSql) As Integer Implements IDao.ExecuteNonQuery
			Dim result As Integer
			Dim cmd As IDbCommand

			cmd = Nothing
			commandWrapper.ResultOutParameter.Clear()

			Try
				_commandWrapper = commandWrapper

				cmd = commandWrapper.Command

				result = cmd.ExecuteNonQuery()

				' 未使用にしてます。
				'If Not commandWrapper.PreparedStatement Then
				'    If Not _sqlParameterCacheMgr.Contains(cmd) Then
				'        _sqlParameterCacheMgr.PutParameterSet(cmd)
				'    End If
				'End If

				' 戻り値を取得する
				getResultOutParameter(commandWrapper)

				Return result
			Catch ex As Exception
				Throw New DbAccessException(Me, ex)
			Finally
				_addExecuteHistory(cmd, _executeUpdateHistory)
				If cmd IsNot Nothing Then
					cmd.Dispose()
				End If
				'_commandWrapper = Nothing
			End Try
		End Function

		''' <summary>
		''' データを更新
		''' </summary>
		''' <param name="ds">データセットオブジェクト</param>
		''' <param name="adp">対象となるアダプター</param>
		''' <returns>DataSet で正常に追加または更新された行数</returns>
		''' <remarks>
		''' DataAdapterを使ってDataSet内の変更された情報により更新を行います。
		''' </remarks>
		''' <exception cref="DbAccessException">
		''' DBアクセスでエラーが発生した
		''' </exception>
		Public Function UpdateAdapter(ByVal ds As DataSet, ByVal adp As IDbDataAdapter) As Integer Implements IDao.UpdateAdapter
			Dim result As Integer

			' 更新！
			Try
				result = adp.Update(ds)

				Return result
			Catch ex As Exception
				Throw New DbAccessException(Me, ex)
			End Try
		End Function

#End Region

		''' <summary>
		''' 文字列が空の時はNothingに変換
		''' </summary>
		''' <param name="value"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Protected Function CNothing(ByVal value As String) As String
			If String.IsNullOrEmpty(value) Then
				Return Nothing
			End If
			Return value
		End Function

		''' <summary>
		''' 数値が0（デフォルト）の時はNothingに変換
		''' </summary>
		''' <param name="value"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Protected Function CNothing(ByVal value As Integer, Optional ByVal isValue As Integer = 0) As Integer
			If value.Equals(isValue) Then
				Return Nothing
			End If
			Return value
		End Function

		''' <summary>
		''' データを取得
		''' </summary>
		''' <param name="ds">取得したデータのデータセット</param>
		''' <returns>データ件数</returns>
		''' <remarks>
		''' </remarks>
		''' <exception cref="DbAccessException">
		''' DBアクセスでエラーが発生した
		''' </exception>
		Protected Friend Function fill(ByRef ds As DataSet) As Integer
			Return fill(ds, "Results Data", Nothing)
		End Function

		''' <summary>
		''' データを取得
		''' </summary>
		''' <param name="ds">取得したデータのデータセット</param>
		''' <param name="adp">アダプタ</param>
		''' <returns>データ件数</returns>
		''' <remarks>
		''' アダプタを使用して更新するときに使用する。
		''' </remarks>
		Protected Friend Function fill(ByRef ds As DataSet, ByVal adp As IDbDataAdapter) As Integer
			Return fill(ds, "Results Data", adp)
		End Function

		''' <summary>
		''' データを取得
		''' </summary>
		''' <param name="ds">取得したデータのデータセット</param>
		''' <param name="DataSetName">データセットの名称</param>
		''' <param name="adapter">アダプタ</param>
		''' <returns>データ件数</returns>
		''' <remarks>
		''' </remarks>
		''' <exception cref="DbAccessException">
		''' DBアクセスでエラーが発生した
		''' </exception>
		Protected Friend Function fill(ByRef ds As DataSet, ByVal dataSetName As String, ByVal adapter As IDbDataAdapter) As Integer
			' データ取得
			Try
				Dim result As Integer

				' データセットがインスタンス化されていない時はインスタンス化
				If ds Is Nothing Then
					ds = New DataSet
					ds.DataSetName = dataSetName
				End If

				If adapter Is Nothing Then
					result = Me.Adapter.Fill(ds)
				Else
					result = adapter.Fill(ds)
				End If

				' 戻り値を取得する
				getResultOutParameter(CommandWrapper)

				Return result
			Catch ex As Exception
				Throw New DbAccessException(Me, ex)
			Finally
				If adapter Is Nothing Then
					Me.Adapter.SelectCommand.Dispose()
				Else
					adapter.SelectCommand.Dispose()
				End If
			End Try
		End Function

		''' <summary>
		''' 戻り値を取得する
		''' </summary>
		''' <param name="commandWrapper"></param>
		''' <remarks></remarks>
		Protected Friend Sub getResultOutParameter(ByVal commandWrapper As IDbCommandSql)
			Dim cmd As IDbCommand = Nothing

			cmd = commandWrapper.Command

			' 戻り値が無い時は終了
			If Not commandWrapper.HaveOutParameter() Then
				Exit Sub
			End If

			' 戻り値を取得する
			Dim ee As IEnumerator = cmd.Parameters.GetEnumerator
			While ee.MoveNext
				Dim param As IDbDataParameter
				param = DirectCast(ee.Current, IDbDataParameter)
				If param.Direction = ParameterDirection.InputOutput Or param.Direction = ParameterDirection.Output Or param.Direction = ParameterDirection.ReturnValue Then
					commandWrapper.ResultOutParameter.Add(param.ParameterName, param.Value)
					Dim key As String
					key = param.ParameterName.Replace(Helper.PlaceholderMark, "")
					If Not commandWrapper.ResultOutParameter.ContainsKey(key) Then
						commandWrapper.ResultOutParameter.Add(key, param.Value)
					End If
				End If
			End While
		End Sub

#Region " Transaction "

		''' <summary>
		''' トランザクション制御情報
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Property TransactionContext() As Tx.ITransactionContext
			Get
				Return _txContext
			End Get
			Set(ByVal value As Tx.ITransactionContext)
				_txContext = value
			End Set
		End Property

		''' <summary>
		''' DB接続
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		Private Function _getTxConnection() As IDbConnection
			Dim conn As IDbConnection
			conn = Nothing
			If _txContext IsNot Nothing AndAlso _txContext.Transaction IsNot Nothing Then
				conn = _txContext.Transaction.Connection
			End If
			Return conn
		End Function

		''' <summary>
		''' トランザクション
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		Private Function _getTransaction() As IDbTransaction
			Dim tx As IDbTransaction
			tx = Nothing
			If _txContext IsNot Nothing AndAlso _txContext.Transaction IsNot Nothing Then
				tx = _txContext.Transaction
			End If
			Return tx
		End Function

		'TODO: 修正する必要有り
		Protected Friend WriteOnly Property ConnectionJoin() As IDbConnection
			Set(ByVal value As IDbConnection)
				_conn = value
			End Set
		End Property

#End Region

		''' <summary>
		''' コマンド実行履歴の追加
		''' </summary>
		''' <param name="cmd"></param>
		''' <remarks></remarks>
		Private Sub _addExecuteHistory(ByVal cmd As IDbCommand, ByVal history As Boolean)
			If Not history Then
				Exit Sub
			End If
			If Not _executeHistory And Not _executeUpdateHistory Then
				Return
			End If

			_executeHistoryList.Add(cmd.CommandText & DbUtil.ToStringParameter(cmd.Parameters))
		End Sub

	End Class

End Namespace
