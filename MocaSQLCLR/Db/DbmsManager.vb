
Imports System.Reflection
Imports Moca.Aop
Imports Moca.Attr
Imports Moca.Util
Imports Moca.Db.Attr

Namespace Db

	''' <summary>
	''' DBMS を管理します。
	''' </summary>
	''' <remarks>
	''' </remarks>
	Public Class DbmsManager
		Implements IDisposable

		Private Shared ReadOnly _instance As DbmsManager = New DbmsManager()

		''' <summary>DBMS のキャッシュ</summary>
		Private _dbms As New Dictionary(Of String, Dbms)

		''' <summary>AopProxy のキャッシュ</summary>
		Private _daoProxy As New Dictionary(Of Type, AopProxy)

		''' <summary>DbAccess のキャッシュ</summary>
		Private _daos As New List(Of IDao)

#Region " コンストラクタ／デコンストラクタ "

		''' <summary>
		''' デフォルトコンストラクタ
		''' </summary>
		''' <remarks></remarks>
		Private Sub New()
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
				For Each dba As IDao In _daos
					If dba Is Nothing Then
						Continue For
					End If
					dba.Dispose()
				Next
			End If
			Me.disposedValue = True
		End Sub

		' このコードは、破棄可能なパターンを正しく実装できるように Visual Basic によって追加されました。
		Public Sub Dispose() Implements IDisposable.Dispose
			' このコードを変更しないでください。クリーンアップ コードを上の Dispose(ByVal disposing As Boolean) に記述します。
			Dispose(True)
			GC.SuppressFinalize(Me)
		End Sub

#End Region

		''' <summary>
		''' DBMSをアプリケーション構成ファイル内から取得します。
		''' </summary>
		''' <param name="connectionString">ConnectionString</param>
		''' <returns>DBMS</returns>
		''' <remarks>
		''' 一度読込まれた接続先情報は保存されます。
		''' </remarks>
		Public Shared Function GetDbms(ByVal connectionString As String) As Dbms
			Return _instance._getDbms(connectionString)
		End Function

		'''' <summary>
		'''' 指定された型でデータベースアクセスオブジェクトをインスタンス化します。
		'''' </summary>
		'''' <typeparam name="T"><see cref="AbstractDao"/>を継承したクラス</typeparam>
		'''' <returns></returns>
		'''' <remarks></remarks>
		'Public Shared Function CreateDao(Of T)() As T
		'	Return _my._createDao(Of T)()
		'End Function

		'''' <summary>
		'''' 指定された型でデータベースアクセスオブジェクトをインスタンス化します。
		'''' </summary>
		'''' <returns></returns>
		'''' <remarks></remarks>
		'Public Shared Function CreateDao(ByVal typ As Type, ByVal attr As DaoAttribute) As IDao
		'	Return _my._createDao(typ, attr)
		'End Function

		''' <summary>
		''' DBMSをアプリケーション構成ファイル内から取得します。
		''' </summary>
		''' <param name="connectionString">ConnectionString</param>
		''' <returns>DBMS</returns>
		''' <remarks>
		''' 一度読込まれた接続先情報は保存されます。
		''' </remarks>
		Private Function _getDbms(ByVal connectionString As String) As Dbms
			Dim value As Dbms

			SyncLock _dbms
				If _dbms.ContainsKey(connectionString) Then
					Return _dbms.Item(connectionString)
				End If

				value = New Dbms(connectionString)
				_dbms.Add(connectionString, value)
				Return value
			End SyncLock
		End Function

		''' <summary>
		''' DBMSを引数から指定します。
		''' </summary>
		''' <param name="name">接続名称</param>
		''' <param name="providerName">プロパイダ名</param>
		''' <param name="connectionString">接続文字列</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Private Function _getDbms(ByVal name As String, ByVal providerName As String, ByVal connectionString As String) As Dbms
			Dim value As Dbms
			Dim key As String

			SyncLock _dbms
				key = name & providerName & connectionString
				If _dbms.ContainsKey(key) Then
					Return _dbms.Item(key)
				End If

				value = New Dbms(name, providerName, connectionString)
				_dbms.Add(key, value)
				Return value
			End SyncLock
		End Function

		'''' <summary>
		'''' 指定された型でデータベースアクセスオブジェクトをインスタンス化します。
		'''' </summary>
		'''' <typeparam name="T"><see cref="AbstractDao"/>を継承したクラス</typeparam>
		'''' <returns></returns>
		'''' <remarks></remarks>
		'Private Function _createDao(Of T)() As T
		'	Dim implType As Type
		'	Dim dbmsAttr As DbmsAttribute
		'	Dim dbmsVal As Dbms
		'	Dim proxy As AopProxy
		'	Dim daoi As IDao
		'	Dim dao As T

		'	implType = GetType(T)

		'	' DaoAttribute 属性がある？
		'	Dim daoAttr As DaoAttribute
		'	daoAttr = ClassUtil.GetCustomAttribute(Of DaoAttribute)(implType)
		'	If daoAttr IsNot Nothing Then
		'		Return DirectCast(_createDao(GetType(T), daoAttr), T)
		'	End If

		'	' プロキシが存在するか？
		'	If _daoProxy.ContainsKey(implType) Then
		'		proxy = _daoProxy.Item(implType)
		'		dao = proxy.Create(Of T)()
		'		_daos.Add(DirectCast(dao, IDao))
		'		Return dao
		'	End If

		'	' Interface ？
		'	If implType.IsInterface() Then
		'		Throw New ArgumentException(implType.FullName & " は、" & GetType(DaoAttribute).FullName & " 属性を指定してください。")
		'	End If

		'	' 型チェック
		'	If Not ClassUtil.IsInterfaceImpl(implType, GetType(IDao)) Then
		'		Throw New ArgumentException(implType.FullName & " は、" & GetType(IDao).FullName & " を実装したクラスではありません。")
		'	End If

		'	' DBMS 特定
		'	dbmsAttr = ClassUtil.GetCustomAttribute(Of DbmsAttribute)(implType)
		'	If dbmsAttr Is Nothing Then
		'		Throw New ArgumentException(implType.FullName & " は、" & GetType(DbmsAttribute).FullName & " 属性が指定されていません。")
		'	End If
		'	dbmsVal = dbmsAttr.Create()

		'	' 透過プロキシ作成
		'	proxy = New AopProxy(ClassUtil.NewInstance(implType))
		'	dao = proxy.Create(Of T)()
		'	daoi = DirectCast(dao, IDao)
		'	DirectCast(daoi, AbstractDao).TargetDbms = dbmsVal

		'	' Transaction 属性のあるメソッドへ TransactionInterceptor の注入
		'	For Each method As MethodBase In implType.GetMethods
		'		Dim attr As TransactionAttribute
		'		attr = ClassUtil.GetCustomAttribute(Of TransactionAttribute)(method)
		'		If attr Is Nothing Then
		'			Continue For
		'		End If
		'		attr.Aspect(proxy, method, daoi)
		'	Next

		'	_daoProxy.Add(implType, proxy)
		'	_daos.Add(DirectCast(dao, IDao))
		'	Return dao
		'End Function

		'Private Function _createDao(ByVal typ As Type, ByVal daoAttr As DaoAttribute) As IDao
		'	Dim implType As Type
		'	Dim dbmsVal As Dbms
		'	Dim proxy As AopProxy
		'	Dim daoi As IDao
		'	Dim dao As Object

		'	implType = daoAttr.ImplType

		'	' プロキシが存在するか？
		'	If _daoProxy.ContainsKey(implType) Then
		'		proxy = _daoProxy.Item(implType)
		'		dao = proxy.Create()
		'		daoi = DirectCast(dao, IDao)
		'		_daos.Add(daoi)
		'		Return daoi
		'	End If

		'	' 型チェック
		'	If Not ClassUtil.IsInterfaceImpl(implType, GetType(IDao)) Then
		'		Throw New ArgumentException(implType.FullName & " は、" & GetType(IDao).FullName & " を実装したクラスではありません。")
		'	End If

		'	' DBMS 特定
		'	dbmsVal = GetDbms(daoAttr.Appkey)

		'	' 透過プロキシ作成
		'	proxy = New AopProxy(ClassUtil.NewInstance(implType))
		'	dao = proxy.Create()
		'	daoi = DirectCast(dao, IDao)
		'	DirectCast(daoi, AbstractDao).TargetDbms = dbmsVal

		'	' Transaction 属性のあるメソッドへ TransactionInterceptor の注入
		'	For Each method As MethodBase In implType.GetMethods
		'		Dim attr As TransactionAttribute
		'		attr = ClassUtil.GetCustomAttribute(Of TransactionAttribute)(method, True)
		'		If attr Is Nothing Then
		'			Continue For
		'		End If
		'		attr.Aspect(proxy, method, daoi)
		'	Next
		'	For Each method As MethodBase In typ.GetMethods
		'		Dim attr As TransactionAttribute
		'		attr = ClassUtil.GetCustomAttribute(Of TransactionAttribute)(method, True)
		'		If attr Is Nothing Then
		'			Continue For
		'		End If
		'		attr.Aspect(proxy, method, daoi)
		'	Next

		'	_daoProxy.Add(implType, proxy)
		'	_daos.Add(daoi)
		'	Return daoi
		'End Function

	End Class

End Namespace
