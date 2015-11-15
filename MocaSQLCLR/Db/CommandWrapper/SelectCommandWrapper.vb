
Namespace Db.CommandWrapper

	''' <summary>
	''' SELECT文を実行する為のDBCommandのラッパークラス
	''' </summary>
	''' <remarks></remarks>
	Public Class SelectCommandWrapper
		Inherits SqlCommandWrapper
		Implements IDbCommandSelect

		''' <summary>結果の行データ</summary>
		Private _dtEnum As IEnumerator(Of DataRow)
		''' <summary>Select文を実行した結果</summary>
		Protected ds As DataSet
		''' <summary>Select文を実行した結果(Reader版)</summary>
		Protected executeResult As ISQLStatementResult

#Region " Constructor/DeConstructor "

		''' <summary>
		''' コンストラクタ
		''' </summary>
		''' <param name="dba">親となるDBAccessインスタンス</param>
		''' <param name="cmd">実行するDBCommandインスタンス</param>
		''' <remarks>
		''' </remarks>
		Friend Sub New(ByVal dba As IDao, ByVal cmd As IDbCommand)
			MyBase.New(dba, cmd)
		End Sub

		''' <summary>
		''' 破棄
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
		''' SQL実行！
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Overrides Function Execute() As Integer
			Return dba.Execute(Me)
		End Function

#Region " Implements IDbCommandSelect "

#Region " Property "

		''' <summary>
		''' Select文を実行した結果を設定／参照
		''' </summary>
		''' <value>Select文を実行した結果</value>
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
		''' DataSet内の先頭テーブルを返す
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
		''' DataSet内の先頭テーブルに存在する行データのEnumeratorを返す
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
		''' クエリを実行し、指定されたエンティティに変換して返します。
		''' </summary>
		''' <typeparam name="T">エンティティ</typeparam>
		''' <returns>エンティティのリスト</returns>
		''' <remarks>
		''' 当メソッドは予めデータベースをオープンしておく必要がありますが、
		''' オープンされていないときは、自動でオープンして終了時にクローズします。<br/>
		''' </remarks>
		Public Overridable Overloads Function Execute(Of T)(Optional behavior As CommandBehavior = CommandBehavior.Default) As System.Collections.Generic.IList(Of T) Implements IDbCommandSelect.Execute
			executeResult = dba.Execute(Of T)(Me)
			Return executeResult.Result(Of T)()
		End Function

		''' <summary>
		''' クエリを実行し、そのクエリが返す結果セットの最初の行にある最初の列を返します。余分な列または行は無視されます。
		''' </summary>
		''' <returns>結果セットの最初の行にある最初の列。</returns>
		''' <remarks>
		''' 当メソッドは予めデータベースをオープンしておく必要がありますが、
		''' オープンされていないときは、自動でオープンして終了時にクローズします。<br/>
		''' 詳細は、<seealso cref="IDbCommand.ExecuteScalar"/> を参照してください。
		''' </remarks>
		Public Overridable Function ExecuteScalar() As Object Implements IDbCommandSelect.ExecuteScalar
			Return dba.ExecuteScalar(Me)
		End Function

		''' <summary>
		''' DataSet内の先頭テーブルに存在する行データのEnumeratorを返す
		''' </summary>
		''' <typeparam name="T"></typeparam>
		''' <returns></returns>
		''' <remarks>
		''' 存在しないときは、空の配列を返す。
		''' </remarks>
		<Obsolete("Execute(Of T)() を使うようにしてください。")> _
		Public Function Result1stTableEntitis(Of T)() As T() Implements IDbCommandSelect.Result1stTableEntitis
			Return entityBuilder.Create(Of T)(Me.Result1stTable)
		End Function

		''' <summary>
		''' DataSet内の先頭テーブルの指定された行を返す
		''' </summary>
		''' <typeparam name="T"></typeparam>
		''' <param name="index"></param>
		''' <returns>先頭テーブルのデータを指定されたEntityを使用した配列に変換して返す</returns>
		''' <remarks>
		''' 存在しないときは、Nothing を返す。
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
