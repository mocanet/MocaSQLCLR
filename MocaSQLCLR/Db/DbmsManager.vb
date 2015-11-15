
Imports System.Reflection
Imports Moca.Aop
Imports Moca.Attr
Imports Moca.Util
Imports Moca.Db.Attr

Namespace Db

	''' <summary>
	''' DBMS ���Ǘ����܂��B
	''' </summary>
	''' <remarks>
	''' </remarks>
	Public Class DbmsManager
		Implements IDisposable

		Private Shared ReadOnly _instance As DbmsManager = New DbmsManager()

		''' <summary>DBMS �̃L���b�V��</summary>
		Private _dbms As New Dictionary(Of String, Dbms)

		''' <summary>AopProxy �̃L���b�V��</summary>
		Private _daoProxy As New Dictionary(Of Type, AopProxy)

		''' <summary>DbAccess �̃L���b�V��</summary>
		Private _daos As New List(Of IDao)

#Region " �R���X�g���N�^�^�f�R���X�g���N�^ "

		''' <summary>
		''' �f�t�H���g�R���X�g���N�^
		''' </summary>
		''' <remarks></remarks>
		Private Sub New()
		End Sub

#End Region

#Region " IDisposable Support "

		Private disposedValue As Boolean = False		' �d������Ăяo�������o����ɂ�

		' IDisposable
		Protected Overridable Sub Dispose(ByVal disposing As Boolean)
			If Not Me.disposedValue Then
				If disposing Then
					' TODO: �����I�ɌĂяo���ꂽ�Ƃ��Ƀ}�l�[�W ���\�[�X��������܂�
				End If

				' TODO: ���L�̃A���}�l�[�W ���\�[�X��������܂�
				For Each dba As IDao In _daos
					If dba Is Nothing Then
						Continue For
					End If
					dba.Dispose()
				Next
			End If
			Me.disposedValue = True
		End Sub

		' ���̃R�[�h�́A�j���\�ȃp�^�[���𐳂��������ł���悤�� Visual Basic �ɂ���Ēǉ�����܂����B
		Public Sub Dispose() Implements IDisposable.Dispose
			' ���̃R�[�h��ύX���Ȃ��ł��������B�N���[���A�b�v �R�[�h����� Dispose(ByVal disposing As Boolean) �ɋL�q���܂��B
			Dispose(True)
			GC.SuppressFinalize(Me)
		End Sub

#End Region

		''' <summary>
		''' DBMS���A�v���P�[�V�����\���t�@�C��������擾���܂��B
		''' </summary>
		''' <param name="connectionString">ConnectionString</param>
		''' <returns>DBMS</returns>
		''' <remarks>
		''' ��x�Ǎ��܂ꂽ�ڑ�����͕ۑ�����܂��B
		''' </remarks>
		Public Shared Function GetDbms(ByVal connectionString As String) As Dbms
			Return _instance._getDbms(connectionString)
		End Function

		'''' <summary>
		'''' �w�肳�ꂽ�^�Ńf�[�^�x�[�X�A�N�Z�X�I�u�W�F�N�g���C���X�^���X�����܂��B
		'''' </summary>
		'''' <typeparam name="T"><see cref="AbstractDao"/>���p�������N���X</typeparam>
		'''' <returns></returns>
		'''' <remarks></remarks>
		'Public Shared Function CreateDao(Of T)() As T
		'	Return _my._createDao(Of T)()
		'End Function

		'''' <summary>
		'''' �w�肳�ꂽ�^�Ńf�[�^�x�[�X�A�N�Z�X�I�u�W�F�N�g���C���X�^���X�����܂��B
		'''' </summary>
		'''' <returns></returns>
		'''' <remarks></remarks>
		'Public Shared Function CreateDao(ByVal typ As Type, ByVal attr As DaoAttribute) As IDao
		'	Return _my._createDao(typ, attr)
		'End Function

		''' <summary>
		''' DBMS���A�v���P�[�V�����\���t�@�C��������擾���܂��B
		''' </summary>
		''' <param name="connectionString">ConnectionString</param>
		''' <returns>DBMS</returns>
		''' <remarks>
		''' ��x�Ǎ��܂ꂽ�ڑ�����͕ۑ�����܂��B
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
		''' DBMS����������w�肵�܂��B
		''' </summary>
		''' <param name="name">�ڑ�����</param>
		''' <param name="providerName">�v���p�C�_��</param>
		''' <param name="connectionString">�ڑ�������</param>
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
		'''' �w�肳�ꂽ�^�Ńf�[�^�x�[�X�A�N�Z�X�I�u�W�F�N�g���C���X�^���X�����܂��B
		'''' </summary>
		'''' <typeparam name="T"><see cref="AbstractDao"/>���p�������N���X</typeparam>
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

		'	' DaoAttribute ����������H
		'	Dim daoAttr As DaoAttribute
		'	daoAttr = ClassUtil.GetCustomAttribute(Of DaoAttribute)(implType)
		'	If daoAttr IsNot Nothing Then
		'		Return DirectCast(_createDao(GetType(T), daoAttr), T)
		'	End If

		'	' �v���L�V�����݂��邩�H
		'	If _daoProxy.ContainsKey(implType) Then
		'		proxy = _daoProxy.Item(implType)
		'		dao = proxy.Create(Of T)()
		'		_daos.Add(DirectCast(dao, IDao))
		'		Return dao
		'	End If

		'	' Interface �H
		'	If implType.IsInterface() Then
		'		Throw New ArgumentException(implType.FullName & " �́A" & GetType(DaoAttribute).FullName & " �������w�肵�Ă��������B")
		'	End If

		'	' �^�`�F�b�N
		'	If Not ClassUtil.IsInterfaceImpl(implType, GetType(IDao)) Then
		'		Throw New ArgumentException(implType.FullName & " �́A" & GetType(IDao).FullName & " �����������N���X�ł͂���܂���B")
		'	End If

		'	' DBMS ����
		'	dbmsAttr = ClassUtil.GetCustomAttribute(Of DbmsAttribute)(implType)
		'	If dbmsAttr Is Nothing Then
		'		Throw New ArgumentException(implType.FullName & " �́A" & GetType(DbmsAttribute).FullName & " �������w�肳��Ă��܂���B")
		'	End If
		'	dbmsVal = dbmsAttr.Create()

		'	' ���߃v���L�V�쐬
		'	proxy = New AopProxy(ClassUtil.NewInstance(implType))
		'	dao = proxy.Create(Of T)()
		'	daoi = DirectCast(dao, IDao)
		'	DirectCast(daoi, AbstractDao).TargetDbms = dbmsVal

		'	' Transaction �����̂��郁�\�b�h�� TransactionInterceptor �̒���
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

		'	' �v���L�V�����݂��邩�H
		'	If _daoProxy.ContainsKey(implType) Then
		'		proxy = _daoProxy.Item(implType)
		'		dao = proxy.Create()
		'		daoi = DirectCast(dao, IDao)
		'		_daos.Add(daoi)
		'		Return daoi
		'	End If

		'	' �^�`�F�b�N
		'	If Not ClassUtil.IsInterfaceImpl(implType, GetType(IDao)) Then
		'		Throw New ArgumentException(implType.FullName & " �́A" & GetType(IDao).FullName & " �����������N���X�ł͂���܂���B")
		'	End If

		'	' DBMS ����
		'	dbmsVal = GetDbms(daoAttr.Appkey)

		'	' ���߃v���L�V�쐬
		'	proxy = New AopProxy(ClassUtil.NewInstance(implType))
		'	dao = proxy.Create()
		'	daoi = DirectCast(dao, IDao)
		'	DirectCast(daoi, AbstractDao).TargetDbms = dbmsVal

		'	' Transaction �����̂��郁�\�b�h�� TransactionInterceptor �̒���
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
