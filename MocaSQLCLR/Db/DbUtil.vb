
Imports System.Text
Imports System.Reflection
Imports System.Data

Imports Moca.Db.Attr
Imports Moca.Util

Namespace Db

	Public Enum SortDirectionValue As Integer
		ASC = 0
		DESC = 1
	End Enum

	''' <summary>
	''' �f�[�^�x�[�X�A�N�Z�X����Ŏg�p���郆�[�e�B���e�B���\�b�h�W
	''' </summary>
	''' <remarks></remarks>
	Public Class DbUtil

		''' <summary>SQL �̃\�[�g�\���e���v���[�g</summary>
		Private Const C_ORDER_BY As String = " ORDER BY{0}"

		''' <summary>SQL �̃\�[�g�\���e���v���[�g</summary>
		Private Const C_ORDER_BY_COL As String = " [{0}] {1}"

		''' <summary>
		''' DbCommand�C���X�^���X���I������
		''' </summary>
		''' <param name="cmd"></param>
		''' <remarks></remarks>
		Public Shared Sub CommandDispose(ByVal cmd As IDbCommandSql)
			If cmd Is Nothing Then
				Exit Sub
			End If
			cmd.Dispose()
		End Sub

		'Public Shared Function CNull(ByVal Value As DBNull) As Object
		'	Return Nothing
		'End Function

		''' <summary>
		''' Null�ϊ�
		''' </summary>
		''' <param name="Value">���ؒl</param>
		''' <returns>�ϊ��l</returns>
		''' <remarks>
		''' ��̏ꍇ�́ADBNull.Value ��Ԃ��܂��B
		''' �����񂪋�̏ꍇ�́ADBNull.Value ��Ԃ��܂��B
		''' </remarks>
		Public Shared Function CNull(ByVal value As Object, Optional ByVal emptyDataEqDBNull As Boolean = False) As Object
			If value Is Nothing Then
				Return DBNull.Value
			End If

			If TypeOf value Is String Then
				If CStr(value).Trim().Length = 0 Then
					If emptyDataEqDBNull Then
						Return DBNull.Value
					Else
						Return value
					End If
				Else
					Return value
				End If
			End If

			If IsDBNull(value) Then
				value = Nothing
			End If

			Return value
		End Function

		Public Shared Function CNothing(ByRef value As Object) As Object
			If IsDBNull(value) Then
				value = Nothing
			End If

			If value Is Nothing Then
				Return Nothing
			End If
			If TypeOf value Is String Then
				If CStr(value).Trim().Length = 0 Then
					Return Nothing
				End If
			End If
			Return value
		End Function

		Public Shared Function CNothing(ByRef value As Integer) As Object
			If value = 0 Then
				Return Nothing
			End If
			Return value
		End Function

		''' <summary>
		''' Null�ϊ�
		''' </summary>
		''' <param name="Value">���ؒl</param>
		''' <returns>�ϊ��l</returns>
		''' <remarks>
		''' ��̏ꍇ�́ADBNull.Value ��Ԃ��܂��B
		''' 0�̏ꍇ�́ADBNull.Value ��Ԃ��܂��B
		''' </remarks>
		Public Shared Function CNull(ByVal Value As Integer) As Object
			If Value = 0 Then
				Return DBNull.Value
			Else
				Return Value
			End If
		End Function

		''' <summary>
		''' Null�ϊ�
		''' </summary>
		''' <param name="Value">���ؒl</param>
		''' <returns>�ϊ��l</returns>
		''' <remarks>
		''' �����l�iDate.MinValue�j�̎��́ANothing��Ԃ�
		''' </remarks>
		Public Shared Function CDateValue(ByVal Value As Date) As Object
			If Value.Equals(Date.MinValue) Then
				Return Nothing
			End If
			Return Value
		End Function

		''' <summary>
		''' DB����擾�����f�[�^��Integer�l�̏ꍇ�̕ϊ�
		''' </summary>
		''' <param name="Value">�Ώےl</param>
		''' <returns>�ϊ��l</returns>
		''' <remarks>
		''' Null�̏ꍇ�́A�O�ɕϊ�����
		''' </remarks>
		Public Shared Function CIntValue(ByVal Value As Object) As Integer
			If Value Is DBNull.Value Then
				Return 0
			End If
			If Value.ToString.Length = 0 Then
				Return 0
			End If
			Return CInt(Value)
		End Function

		''' <summary>
		''' DB����擾�����f�[�^��Long�l�̏ꍇ�̕ϊ�
		''' </summary>
		''' <param name="Value">�Ώےl</param>
		''' <returns>�ϊ��l</returns>
		''' <remarks>
		''' Null�̏ꍇ�́A�O�ɕϊ�����
		''' </remarks>
		Public Shared Function CLngValue(ByVal Value As Object) As Long
			If Value Is DBNull.Value Then
				Return 0
			Else
				Return CLng(Value)
			End If
		End Function

		''' <summary>
		''' DB����擾�����f�[�^��Single�l�̏ꍇ�̕ϊ�
		''' </summary>
		''' <param name="Value">�Ώےl</param>
		''' <returns>�ϊ��l</returns>
		''' <remarks>
		''' Null�̏ꍇ�́A�O�ɕϊ�����
		''' </remarks>
		Public Shared Function CSngValue(ByVal Value As Object, Optional ByVal defaultValue As Single = 0) As Single
			If Value Is DBNull.Value Then
				Return defaultValue
			Else
				Return CSng(Value)
			End If
		End Function

		''' <summary>
		''' DB����擾�����f�[�^��String�l�̏ꍇ�̕ϊ�
		''' </summary>
		''' <param name="Value">�Ώےl</param>
		''' <returns>�ϊ��l</returns>
		''' <remarks>
		''' Null�̏ꍇ�́AString.Empty�ɕϊ�����
		''' </remarks>
		Public Shared Function CStrValue(ByVal Value As Object, Optional ByVal defaultValue As String = "") As String
			If Value Is DBNull.Value Then
				Return defaultValue
			Else
				Return CStr(Value)
			End If
		End Function

		''' <summary>
		''' DB����擾�����f�[�^��String�l�̏ꍇ�̕ϊ��iTrim����j
		''' </summary>
		''' <param name="Value">�Ώےl</param>
		''' <returns>�ϊ��l</returns>
		''' <remarks>
		''' Null�̏ꍇ�́AString.Empty�ɕϊ�����
		''' </remarks>
		Public Shared Function CStrTrimValue(ByVal Value As Object) As String
			Return CStrValue(Value).Trim()
		End Function

		''' <summary>
		''' DB����擾�����f�[�^�����z�̏ꍇ�̕ϊ�
		''' </summary>
		''' <param name="Value">�Ώےl</param>
		''' <returns>�ϊ��l</returns>
		''' <remarks>
		''' Null�̏ꍇ�́AString.Empty�ɕϊ�����B
		''' 3���̃J���}��؂蕶����Ƃ��ĕԂ��܂��B�i"###,###,###,###"�j
		''' </remarks>
		Public Shared Function CMoneyValue(ByVal Value As Object) As String
			Return CMoneyValue(Value, "###,###,###,###")
		End Function

		''' <summary>
		''' DB����擾�����f�[�^�����z�̏ꍇ�̕ϊ�
		''' </summary>
		''' <param name="Value">�Ώےl</param>
		''' <param name="formatString"></param>
		''' <returns>�ϊ��l</returns>
		''' <remarks>
		''' Null�̏ꍇ�́AString.Empty�ɕϊ�����B
		''' �w�肳�ꂽ�t�H�[�}�b�g������ɕϊ����ĕԂ��܂��B
		''' </remarks>
		Public Shared Function CMoneyValue(ByVal Value As Object, ByVal formatString As String) As String
			Dim strValue As String

			If Value Is DBNull.Value Then
				Return String.Empty
			End If

			strValue = CStr(Value)
			If strValue.Length = 0 Then
				strValue = "0"
			End If

			formatString = "{0:" & formatString & "}"
			Return String.Format(formatString, CLng(strValue))
		End Function

		''' <summary>
		''' DB����擾�����f�[�^��Date�l�̏ꍇ�ɓ��t�݂̂̕�����֕ϊ�
		''' </summary>
		''' <param name="Value">�Ώےl</param>
		''' <returns>�ϊ��l</returns>
		''' <remarks>
		''' Null�̏ꍇ�́AString.Empty�ɕϊ�����
		''' </remarks>
		Public Shared Function CDateValueToNoTimeString(ByVal Value As Object) As String
			If Value Is DBNull.Value Then
				Return String.Empty
			Else
				Return CDate(Value).ToShortDateString
			End If
		End Function

		''' <summary>
		''' DB����擾�����f�[�^��Date�l�̏ꍇ�ɓ��t�݂̂̕�����֕ϊ�
		''' </summary>
		''' <param name="Value">�Ώےl</param>
		''' <returns>�ϊ��l</returns>
		''' <remarks>
		''' Null�̏ꍇ�́AString.Empty�ɕϊ�����
		''' </remarks>
		Public Shared Function CDateValueToYYYYMM(ByVal Value As Object) As String
			If Value Is DBNull.Value Then
				Return String.Empty
			Else
				Return String.Format("{0:yyyy/MM}", Value)
			End If
		End Function

		''' <summary>
		''' DB����擾�����f�[�^��Integer�l�̏ꍇ�ɓ��t�݂̂̕�����֕ϊ�
		''' </summary>
		''' <param name="Value">�Ώےl</param>
		''' <returns>�ϊ��l</returns>
		''' <remarks>
		''' Null�̏ꍇ�́AString.Empty�ɕϊ�����
		''' </remarks>
		Public Shared Function CIntValueToYYYYMM(ByVal Value As Object) As String
			If Value Is DBNull.Value Then
				Return String.Empty
			Else
				Dim buf As String
				buf = CStr(Value)
				If buf.Length = 6 Then
					buf &= "01"
				End If
				buf = String.Format("{0:0000/00/00}", CLng(buf))
				Return String.Format("{0:yyyy/MM}", CDate(buf))
			End If
		End Function

		''' <summary>
		''' DB����擾�����f�[�^���w�肳�ꂽ�t�H�[�}�b�g�ɕϊ�����
		''' </summary>
		''' <param name="Value">�Ώےl</param>
		''' <param name="formatString">�t�H�[�}�b�g������</param>
		''' <returns>�ϊ��l</returns>
		''' <remarks>
		''' </remarks>
		Public Shared Function CFormat(ByVal Value As Object, ByVal formatString As String) As String
			If Value Is DBNull.Value Then
				Return String.Empty
			Else
				formatString = "{0:" & formatString & "}"
				Return String.Format(formatString, Value)
			End If
		End Function

		''' <summary>
		''' �L�[�ƒl��ێ������R���N�V�������쐬����
		''' </summary>
		''' <param name="rows">�쐬���ƂȂ�f�[�^</param>
		''' <param name="keyColumnName">�L�[�Ƃ���f�[�^�̗�</param>
		''' <param name="valueColumnName">�L�[�ɑ΂��Ēl�Ƃ���f�[�^�̗�</param>
		''' <returns>�R���N�V����</returns>
		''' <remarks>
		''' </remarks>
		Public Shared Function CDictionary(ByVal rows As DataRowCollection, ByVal keyColumnName As String, ByVal valueColumnName As String) As IDictionary
			Dim ht As New Hashtable
			Dim dr As DataRow

			For Each dr In rows
				ht.Add(CStr(dr(keyColumnName)), CStr(dr(valueColumnName)).Trim())
			Next

			Return ht
		End Function

		''' <summary>
		''' SQL���̏�����LIKE�𕡐��쐬���܂��B
		''' </summary>
		''' <param name="columnName">��</param>
		''' <param name="arr"></param>
		''' <param name="op"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Shared Function MakeSQLWhereLikeList(ByRef columnName As String, ByRef arr() As String, ByRef op As String) As String
			Dim buf As ArrayList

			buf = New ArrayList

			For Each item As String In arr
				buf.Add(String.Format("({0} LIKE '%{1}%')", columnName, item))
			Next
			Return "(" & Join(buf.ToArray(), " " & op & " ") & ")"
		End Function

		''' <summary>�f�[�^�x�[�X����擾�����f�[�^�̊i�[��ƂȂ� Entity ���쐬����</summary>
		Private _entityBuilder As New EntityBuilder

		''' <summary>
		''' �w�肳�ꂽ�I�u�W�F�N�g�̃v���p�e�B���y�сA��������񖼂��擾����
		''' </summary>
		''' <param name="typ">�񖼂��擾���������f���̃^�C�v</param>
		''' <returns>�񖼔z��</returns>
		''' <remarks></remarks>
		Public Shared Function GetColumnNames(ByVal typ As Type) As Hashtable
			Dim hash As Hashtable
			Dim pinfo() As PropertyInfo

			hash = New Hashtable

			' �v���p�e�B��`���擾
			pinfo = ClassUtil.GetProperties(typ)
			For Each prop As PropertyInfo In pinfo
				' ���g�p��������
				Dim ignore As ColumnIgnoreAttribute
				ignore = ClassUtil.GetCustomAttribute(Of ColumnIgnoreAttribute)(prop)
				If ignore IsNot Nothing Then
					If ignore.Ignore Then
						Continue For
					End If
				End If

				' �񖼑����擾
				Dim attr As ColumnAttribute
				attr = ClassUtil.GetCustomAttribute(Of ColumnAttribute)(prop)
				If attr IsNot Nothing Then
					hash.Add(attr.ColumnName, prop.Name)
				Else
					hash.Add(prop.Name, prop.Name)
				End If
			Next
			Return hash
		End Function

		Public Shared Function GetColumnName(ByVal prop As PropertyInfo) As String
			' ���g�p��������
			Dim ignore As ColumnIgnoreAttribute
			ignore = ClassUtil.GetCustomAttribute(Of ColumnIgnoreAttribute)(prop)
			If ignore IsNot Nothing Then
				If ignore.Ignore Then
					Return String.Empty
				End If
			End If

			' �񖼑����擾
			Dim attr As ColumnAttribute
			attr = ClassUtil.GetCustomAttribute(Of ColumnAttribute)(prop)
			If attr IsNot Nothing Then
				Return attr.ColumnName
			Else
				Return prop.Name
			End If
		End Function

		''' <summary>
		''' �w�肳�ꂽ�I�u�W�F�N�g�̃v���p�e�B���y�сA��������񖼂��擾����
		''' </summary>
		''' <param name="target">�񖼂��擾���������f��</param>
		''' <returns>�񖼔z��</returns>
		''' <remarks></remarks>
		Public Shared Function GetColumnNames(ByVal target As Object) As Hashtable
			Return GetColumnNames(target.GetType)
		End Function

		''' <summary>
		''' DataTable�̃J�����\�����\�z����
		''' </summary>
		''' <typeparam name="T">�G���e�B�e�B�ƂȂ�N���X</typeparam>
		''' <typeparam name="Order">���ڂ̏����ƂȂ�񋓌^</typeparam>
		''' <param name="captions">��̃L���v�V�����ƂȂ镶����z��</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Function CreateTable(Of T, Order)(Optional ByVal captions() As String = Nothing) As DataTable
			Return _entityBuilder.CreateTable(Of T, Order)(captions)
		End Function

		''' <summary>
		''' ������ DataTable ����w�肳�ꂽ�^�C�v�̃f�[�^�z��֕ϊ����ĕԂ��B
		''' </summary>
		''' <typeparam name="T">�ϊ���̃^�C�v</typeparam>
		''' <param name="tbl">�ϊ����e�[�u���f�[�^</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Function Create(Of T)(ByVal tbl As DataTable) As T()
			Return _entityBuilder.Create(Of T)(tbl)
		End Function

		''' <summary>
		''' ������ DataRow ����w�肳�ꂽ�^�C�v�̃f�[�^�z��֕ϊ����ĕԂ��B
		''' </summary>
		''' <typeparam name="T">�ϊ���̃^�C�v</typeparam>
		''' <param name="row">�ϊ����s�f�[�^</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Function Create(Of T)(ByVal row As DataRow) As T
			Return _entityBuilder.Create(Of T)(row)
		End Function

		''' <summary>
		''' ������ DataRow ����w�肳�ꂽ�^�C�v�̃f�[�^�z��֕ϊ����ĕԂ��B
		''' </summary>
		''' <typeparam name="T">�ϊ���̃^�C�v</typeparam>
		''' <param name="row">�ϊ����s�f�[�^</param>
		''' <param name="version">�ϊ�����f�[�^�̃o�[�W����</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Function Create(Of T)(ByVal row As DataRow, ByVal version As DataRowVersion) As T
			Return _entityBuilder.Create(Of T)(row, version)
		End Function

		''' <summary>
		''' �����̃I�u�W�F�N�g����Table�����̃t�B�[���h�����݂���ꍇ�͗����ݒ肷��
		''' </summary>
		''' <param name="obj">�Ώۂ̃C���X�^���X</param>
		''' <remarks></remarks>
		Public Sub SetColumnInfo(ByVal obj As Object)
			_entityBuilder.SetColumnInfo(obj)
		End Sub

		''' <summary>
		''' �����̃G���e�B�e�B���� DataRow �֕ϊ�
		''' </summary>
		''' <param name="entity">�ϊ���</param>
		''' <param name="row">�ϊ���</param>
		''' <remarks></remarks>
		Public Sub Convert(ByVal entity As Object, ByVal row As DataRow)
			_entityBuilder.Convert(entity, row)
		End Sub

		''' <summary>
		''' SQL �̃\�[�g�����Ŏg�p����J�������쐬
		''' </summary>
		''' <returns>SQL �̃\�[�g������</returns>
		''' <remarks></remarks>
		Public Shared Function GetOrderBy(ByVal columns() As String) As String
			Dim value As String

			value = String.Empty
			If columns.Length = 0 Then
				Return value
			End If
			For Each col As String In columns
				If col.Length = 0 Then
					Continue For
				End If
				value &= CStr(IIf(value.Length = 0, String.Empty, ", ")) & col
			Next
			If value.Length = 0 Then
				Return String.Empty
			End If

			Return String.Format(C_ORDER_BY, value)
		End Function

		''' <summary>
		''' SQL �̃\�[�g�����Ŏg�p����J�������쐬
		''' </summary>
		''' <param name="sortExpression">�\�[�g���ږ�</param>
		''' <param name="sortDirection">�\�[�g��</param>
		''' <returns>SQL �̃\�[�g�Ɏg�p����J����������</returns>
		''' <remarks></remarks>
		Public Shared Function GetOrderByColumn(ByVal sortExpression As String, ByVal sortDirection As SortDirectionValue) As String
			If sortExpression.Length = 0 Then
				Return String.Empty
			End If
			Return String.Format(C_ORDER_BY_COL, sortExpression, sortDirection.ToString)
		End Function

		''' <summary>
		''' DB�p�����[�^���J���}�ŘA�����ĕ�����ɕϊ�����
		''' </summary>
		''' <param name="params">DB�p�����[�^</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Shared Function ToStringParameter(ByVal params As IDataParameterCollection) As String
			Dim sb As New StringBuilder

			For Each param As IDataParameter In params
				sb.Append(IIf(sb.ToString.Length = 0, String.Empty, ","))
				sb.Append(Replace2NullWithSinQt(param.Value))
			Next

			If sb.ToString.Length = 0 Then
				Return String.Empty
			End If

			Return " " & sb.ToString
		End Function

		''' <summary>
		''' Null�ϊ�(�V���O���N�H�[�e�[�V�����t��)
		''' </summary>
		''' <param name="val"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Shared Function Replace2NullWithSinQt(ByVal val As Object) As String
			If val Is Nothing Then
				Return "NULL"
			End If

			If val.ToString.Trim.Length = 0 Then
				Return "NULL"
			End If

			Return "'" & val.ToString & "'"
		End Function

#Region " ���g�p�ł͂��邪�A�T���v���Ƃ��ĂƂ��Ă��������̂ŃR�����g�ɂ��Ďc���Ă܂� "

		'''' <summary>
		'''' ������ DataRow ����w�肳�ꂽ�^�C�v�̃f�[�^�z��֕ϊ����ĕԂ��B
		'''' </summary>
		'''' <typeparam name="T">�ϊ���̃^�C�v</typeparam>
		'''' <returns></returns>
		'''' <remarks></remarks>
		'Public Shared Function Create(Of T)() As T
		'	SyncLock _entityBuilder
		'		Return _entityBuilder.Create(Of T)()
		'	End SyncLock
		'End Function

#End Region

	End Class

End Namespace
